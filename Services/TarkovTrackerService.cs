using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TarkovPriceViewer.Models;
using System.Diagnostics;
using System.Linq;
using static TarkovPriceViewer.Models.TarkovAPI;
using System.Collections.Generic;
using System.IO;

namespace TarkovPriceViewer.Services
{
    public interface ITarkovTrackerService
    {
        TarkovTrackerAPI.Root TrackerData { get; }
        bool IsLoaded { get; }
        Task UpdateTarkovTrackerAPI(bool force = false);

        CurrentTrackedObjective GetCurrentTrackedObjectiveForItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData);
        CurrentTrackedObjective GetCurrentHideoutRequirementForItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData);
        TrackerUpdateResult TryIncrementCurrentObjectiveForCurrentItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData);
        TrackerUpdateResult TryDecrementCurrentObjectiveForCurrentItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData);

        TrackerUpdateResult TryChangeCurrentObjectiveForCurrentItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData, int delta);

        // Apply a change only in memory (local cache) and enqueue the objective for later flush
        TrackerUpdateResult ApplyLocalChangeForCurrentItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData, int delta);

        // Local-only changes for hideout (not sent to the API)
        TrackerUpdateResult ApplyLocalHideoutChangeForCurrentItem(TarkovAPI.Item item, TarkovAPI.Data tarkovData, int delta);

        // Additional local progress for a specific hideout requirement
        int GetLocalHideoutExtraCount(string requirementId);

        Task<TrackerUpdateResult> IncrementObjectiveAndSyncAsync(TarkovAPI.Item item, TarkovAPI.Data tarkovData, CancellationToken cancellationToken = default);
        Task<TrackerUpdateResult> DecrementObjectiveAndSyncAsync(TarkovAPI.Item item, TarkovAPI.Data tarkovData, CancellationToken cancellationToken = default);
        Task<TrackerUpdateResult> ChangeObjectiveAndSyncAsync(TarkovAPI.Item item, TarkovAPI.Data tarkovData, int delta, CancellationToken cancellationToken = default);
    }

    public class TarkovTrackerService : ITarkovTrackerService, IDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISettingsService _settingsService;
        private readonly ITarkovDataService _tarkovDataService;

        private const string LOCAL_TASKS_FILE = "tarkovtracker-tasks.json";
        private const string LOCAL_HIDEOUT_FILE = "tarkovtracker-hideout.json";
        private const string TarkovTrackerBaseUrl = "https://tarkovtracker.org/api/v2";

        public TarkovTrackerAPI.Root TrackerData { get; private set; }
        public bool IsLoaded { get; private set; }
        public DateTime LastUpdated { get; private set; } = DateTime.Now.AddHours(-5);
        private readonly object _lockObject = new object();

        private DateTime _lastTooManyRequests = DateTime.MinValue;
        private static readonly TimeSpan TooManyRequestsCooldown = TimeSpan.FromSeconds(5);

        private readonly Dictionary<string, CurrentTrackedObjective> _pendingTaskObjectives = new Dictionary<string, CurrentTrackedObjective>();
        private readonly Dictionary<string, int> _localHideout = new Dictionary<string, int>();
        private static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(30);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public TarkovTrackerService(IHttpClientFactory httpClientFactory, ISettingsService settingsService, ITarkovDataService tarkovDataService)
        {
            _httpClientFactory = httpClientFactory;
            _settingsService = settingsService;
            _tarkovDataService = tarkovDataService;

            // Load local state (pending objectives and hideout information) from disk if it exists
            LoadLocalTasksState();
            LoadLocalHideoutState();

            // Start a background loop that periodically flushes local changes to the API
            Task.Run(() => FlushLoopAsync(_cts.Token));
        }

        public int GetLocalHideoutExtraCount(string requirementId)
        {
            if (string.IsNullOrEmpty(requirementId))
                return 0;

            lock (_lockObject)
            {
                return _localHideout.TryGetValue(requirementId, out var value) ? value : 0;
            }
        }

        public int GetLocalHideoutExtraCount(string requirementId)
        {
            if (string.IsNullOrEmpty(requirementId))
                return 0;

            lock (_lockObject)
            {
                return _localHideout.TryGetValue(requirementId, out var value) ? value : 0;
            }
        }

        private void ApplyLocalObjectiveUpdate(CurrentTrackedObjective updatedObjective)
        {
            if (updatedObjective == null || updatedObjective.ObjectiveId == null)
            {
                return;
            }

            try
            {
                lock (_lockObject)
                {
                    var trackerData = TrackerData?.data;
                    if (trackerData == null || trackerData.taskObjectivesProgress == null)
                    {
                        return;
                    }

                    var progress = trackerData.taskObjectivesProgress.FirstOrDefault(p => p.id == updatedObjective.ObjectiveId);
                    if (progress == null)
                    {
                        // If the API has not yet returned progress for this objective, create a local entry
                        progress = new TarkovTrackerAPI.TaskObjectivesProgress
                        {
                            id = updatedObjective.ObjectiveId
                        };
                        trackerData.taskObjectivesProgress.Add(progress);
                    }

                    progress.count = updatedObjective.CurrentCount;
                    progress.complete = updatedObjective.CurrentCount >= updatedObjective.RequiredCount;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while applying local objective update: " + ex.Message);
            }
        }

        private async Task FlushLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(FlushInterval, token).ConfigureAwait(false);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    await FlushPendingObjectivesAsync(token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[TarkovTracker] Error in FlushLoopAsync: " + ex.Message);
                }
            }
        }

        private async Task FlushPendingObjectivesAsync(CancellationToken cancellationToken)
        {
            List<CurrentTrackedObjective> snapshot;

            lock (_lockObject)
            {
                if (_pendingTaskObjectives.Count == 0)
                {
                    return;
                }

                snapshot = _pendingTaskObjectives.Values.ToList();
                _pendingTaskObjectives.Clear();
            }

            foreach (var objective in snapshot)
            {
                try
                {
                    if (!await UpdateObjectiveCountAsync(objective, cancellationToken).ConfigureAwait(false))
                    {
                        // If it fails (for example 429), re-enqueue it to retry later
                        lock (_lockObject)
                        {
                            _pendingTaskObjectives[objective.ObjectiveId] = objective;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[TarkovTracker] Error while flushing objective: " + ex.Message);
                    lock (_lockObject)
                    {
                        _pendingTaskObjectives[objective.ObjectiveId] = objective;
                    }
                }
            }

            // Save the new local state of tasks after the flush (in case there are re-enqueued objectives)
            SaveLocalTasksState();

            // Reuse the same cycle to also clean and save the local hideout state
            SaveLocalHideoutState();
        }

        public async Task UpdateTarkovTrackerAPI(bool force = false)
        {
            var settings = _settingsService.Settings;
            string apiKey = settings.TarkovTrackerApiKey;

            if (settings.UseTarkovTrackerApi && !string.Equals(apiKey, "APIKey") && !string.IsNullOrWhiteSpace(apiKey))
            {
                // If Outdated by 30 seconds
                if (force || ((DateTime.Now - LastUpdated).TotalSeconds >= 30))
                {
                    try
                    {
                        Debug.WriteLine("\n--> Updating TarkovTracker API...");

                        var client = _httpClientFactory.CreateClient();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                        var httpResponse = await client.GetAsync($"{TarkovTrackerBaseUrl}/progress");
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            string responseContent = await httpResponse.Content.ReadAsStringAsync();

                            List<CurrentTrackedObjective> pendingSnapshot;

                            lock (_lockObject)
                            {
                                TrackerData = JsonConvert.DeserializeObject<TarkovTrackerAPI.Root>(responseContent);
                                LastUpdated = DateTime.Now;
                                IsLoaded = true;
                                Debug.WriteLine("\n--> TarkovTracker API Updated!");

                                // Tomar un snapshot de los objetivos pendientes para re-aplicarlos después del GET
                                pendingSnapshot = _pendingTaskObjectives.Values.ToList();
                            }

                            // Re-aplicar los cambios locales pendientes sobre los datos recién obtenidos
                            foreach (var obj in pendingSnapshot)
                            {
                                ApplyLocalObjectiveUpdate(obj);
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"[TarkovTracker] Failed to GET /progress: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("--> Error trying to update TarkovTracker API: " + ex.Message);
                    }
                }
                else
                {
                    Debug.WriteLine("--> No need to update TarkovTracker API!");
                }
            }
            else
            {
                Debug.WriteLine("[TarkovTracker] Skipping update: API usage disabled or API key not set.");
            }
        }

        private async Task<bool> UpdateObjectiveCountAsync(CurrentTrackedObjective updatedObjective, CancellationToken cancellationToken)
        {
            var settings = _settingsService.Settings;
            string apiKey = settings.TarkovTrackerApiKey;

            if (!settings.UseTarkovTrackerApi || string.Equals(apiKey, "APIKey") || string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            // Si recientemente recibimos un 429, respetar un pequeño cooldown para no seguir martilleando la API
            if (DateTime.UtcNow - _lastTooManyRequests < TooManyRequestsCooldown)
            {
                Debug.WriteLine("[TarkovTracker] Skipping objective update due to recent 429 (cooldown in effect)");
                return false;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // According to TarkovTracker OpenAPI, both count and state are supported
                var state = updatedObjective.CurrentCount >= updatedObjective.RequiredCount ? "completed" : "uncompleted";
                var payload = new { count = updatedObjective.CurrentCount, state };
                var json = JsonConvert.SerializeObject(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{TarkovTrackerBaseUrl}/progress/task/objective/{updatedObjective.ObjectiveId}";
                Debug.WriteLine($"[TarkovTracker] Updating objective via {url} -> count={updatedObjective.CurrentCount}, state={state}");

                // OpenAPI specifies POST for this endpoint
                using var response = await client.PostAsync(url, content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[TarkovTracker] Failed to update objective {updatedObjective.ObjectiveId}: {(int)response.StatusCode} {response.ReasonPhrase}");

                    if ((int)response.StatusCode == 429)
                    {
                        _lastTooManyRequests = DateTime.UtcNow;
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while updating objective: " + ex.Message);
                return false;
            }
        }

        public CurrentTrackedObjective GetCurrentTrackedObjectiveForItem(Item item, Data tarkovData)
        {
            // To show the current objective on the overlay, use the same directional logic
            // as for an increment: first incomplete objective ordered by ascending minPlayerLevel.
            return SelectTaskObjectiveForDelta(item, tarkovData, +1);
        }

        public CurrentTrackedObjective GetCurrentHideoutRequirementForItem(Item item, Data tarkovData)
        {
            if (item == null || tarkovData == null)
            {
                return null;
            }

            var hideoutStations = tarkovData.hideoutStations;
            if (hideoutStations == null)
            {
                return null;
            }

            CurrentTrackedObjective best = null;
            int bestLevel = int.MinValue;

            foreach (var station in hideoutStations)
            {
                if (station.levels == null)
                    continue;

                foreach (var stationLevel in station.levels)
                {
                    if (stationLevel.itemRequirements == null)
                        continue;

                    foreach (var itemReq in stationLevel.itemRequirements)
                    {
                        if (itemReq.item == null || itemReq.item.id != item.id)
                            continue;

                        int required = itemReq.count ?? 0;
                        int extraLocal = GetLocalHideoutExtraCount(itemReq.id);
                        int current = extraLocal;
                        if (current > required)
                            current = required;

                        int levelValue = stationLevel.level ?? int.MaxValue;
                        if (best == null || levelValue < bestLevel)
                        {
                            bestLevel = levelValue;
                            best = new CurrentTrackedObjective
                            {
                                ObjectiveId = itemReq.id, // here we use the hideout requirement ID
                                ItemId = item.id,
                                RequiredCount = required,
                                CurrentCount = current
                            };
                        }
                    }
                }
            }

            return best;
        }

        public TrackerUpdateResult TryIncrementCurrentObjectiveForCurrentItem(Item item, Data tarkovData)
        {
            return TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, +1);
        }

        public TrackerUpdateResult TryDecrementCurrentObjectiveForCurrentItem(Item item, Data tarkovData)
        {
            return TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, -1);
        }

        public TrackerUpdateResult TryChangeCurrentObjectiveForCurrentItem(Item item, Data tarkovData, int delta)
        {
            if (delta == 0)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            // Directional selection: for + we look for the first incomplete objective,
            // for - the last objective with progress > 0, ordered by ascending minPlayerLevel
            var objective = SelectTaskObjectiveForDelta(item, tarkovData, delta);
            if (objective == null)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            int newCount = objective.CurrentCount + delta;
            if (newCount < 0)
            {
                newCount = 0;
            }
            if (newCount > objective.RequiredCount)
            {
                newCount = objective.RequiredCount;
            }

            if (newCount == objective.CurrentCount)
            {
                if (delta > 0 && objective.CurrentCount >= objective.RequiredCount)
                {
                    // Already completed, nothing more can be added
                    return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.AlreadyCompleted, objective);
                }

                if (delta < 0 && objective.CurrentCount <= 0)
                {
                    // No progress to remove, treat as a silent no-op
                    return TrackerUpdateResult.Ok(objective);
                }
            }

            var updated = new CurrentTrackedObjective
            {
                ObjectiveId = objective.ObjectiveId,
                ItemId = objective.ItemId,
                RequiredCount = objective.RequiredCount,
                CurrentCount = newCount
            };

            Debug.WriteLine($"[TarkovTracker] Change objective {updated.ObjectiveId} for item {updated.ItemId}: {objective.CurrentCount} -> {updated.CurrentCount} (delta={delta})");
            return TrackerUpdateResult.Ok(updated);
        }

        private CurrentTrackedObjective SelectTaskObjectiveForDelta(Item item, Data tarkovData, int delta)
        {
            if (item == null || tarkovData == null || TrackerData == null || TrackerData.data == null)
            {
                return null;
            }

            var trackerData = TrackerData.data;
            var candidates = GetOrderedTaskObjectivesForItem(item, trackerData);
            if (candidates.Count == 0)
            {
                return null;
            }

            if (delta > 0)
            {
                // For +1: first objective that is not yet complete
                foreach (var c in candidates)
                {
                    if (c.current < c.required)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = c.objectiveId,
                            ItemId = item.id,
                            RequiredCount = c.required,
                            CurrentCount = c.current
                        };
                    }
                }
            }
            else // delta < 0
            {
                // For -1: last objective (by ascending level order) with progress > 0
                for (int i = candidates.Count - 1; i >= 0; i--)
                {
                    var c = candidates[i];
                    if (c.current > 0)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = c.objectiveId,
                            ItemId = item.id,
                            RequiredCount = c.required,
                            CurrentCount = c.current
                        };
                    }
                }
            }

            return null;
        }

        private List<(string objectiveId, int required, int current)> GetOrderedTaskObjectivesForItem(Item item, TarkovTrackerAPI.Data trackerData)
        {
            var result = new List<(string objectiveId, int required, int current)>();

            var usedInTasks = item.usedInTasks;
            if (usedInTasks == null || usedInTasks.Count == 0)
            {
                return result;
            }

            // Order tasks by ascending player level (earliest upgrade first)
            var orderedTasks = usedInTasks
                .Where(t => t.objectives != null)
                .OrderBy(t => t.minPlayerLevel ?? int.MaxValue)
                .ToList();

            foreach (var task in orderedTasks)
            {
                if (task.objectives == null)
                    continue;

                foreach (var obj in task.objectives)
                {
                    if (obj.type != "giveItem" || obj.foundInRaid != true || obj.items == null || !obj.items.Any(i => i.id == item.id))
                        continue;

                    int required = obj.count ?? 0;
                    int current = 0;

                    if (trackerData.taskObjectivesProgress != null && obj.id != null)
                    {
                        var progress = trackerData.taskObjectivesProgress.FirstOrDefault(p => p.id == obj.id);
                        if (progress != null)
                        {
                            if (progress.complete == true)
                            {
                                current = required;
                            }
                            else if (progress.count != null)
                            {
                                current = progress.count.Value;
                            }
                        }
                    }

                    result.Add((obj.id, required, current));
                }
            }

            return result;
        }

        public TrackerUpdateResult ApplyLocalChangeForCurrentItem(Item item, Data tarkovData, int delta)
        {
            var validation = TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, delta);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;

            // Actualizar el progreso localmente para que el overlay lo use inmediatamente
            ApplyLocalObjectiveUpdate(updatedObjective);

            // Encolar el objetivo para flush posterior a la API
            lock (_lockObject)
            {
                if (!string.IsNullOrEmpty(updatedObjective.ObjectiveId))
                {
                    _pendingTaskObjectives[updatedObjective.ObjectiveId] = updatedObjective;
                }
            }

            // Persistir cambios locales de tasks
            SaveLocalTasksState();

            return validation;
        }

        public TrackerUpdateResult ApplyLocalHideoutChangeForCurrentItem(Item item, Data tarkovData, int delta)
        {
            if (delta == 0)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            var requirement = SelectHideoutRequirementForDelta(item, tarkovData, delta);
            if (requirement == null || string.IsNullOrEmpty(requirement.ObjectiveId))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            int newCount = requirement.CurrentCount + delta;
            if (newCount < 0)
            {
                newCount = 0;
            }
            if (newCount > requirement.RequiredCount)
            {
                newCount = requirement.RequiredCount;
            }

            if (newCount == requirement.CurrentCount)
            {
                if (delta > 0 && requirement.CurrentCount >= requirement.RequiredCount)
                {
                    return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.AlreadyCompleted, requirement);
                }

                if (delta < 0 && requirement.CurrentCount <= 0)
                {
                    return TrackerUpdateResult.Ok(requirement);
                }
            }

            // In purely local mode, the extra counter is equal to the current value
            int newExtra = newCount;

            lock (_lockObject)
            {
                _localHideout[requirement.ObjectiveId] = newExtra;
            }

            var updated = new CurrentTrackedObjective
            {
                ObjectiveId = requirement.ObjectiveId,
                ItemId = requirement.ItemId,
                RequiredCount = requirement.RequiredCount,
                CurrentCount = newCount
            };

            Debug.WriteLine($"[TarkovTracker] Local hideout change {updated.ObjectiveId} for item {updated.ItemId}: {requirement.CurrentCount} -> {updated.CurrentCount} (delta={delta})");

            // Persist local hideout changes
            SaveLocalHideoutState();
            return TrackerUpdateResult.Ok(updated);
        }

        private class LocalObjectiveState
        {
            public string ObjectiveId { get; set; }
            public string ItemId { get; set; }
            public int RequiredCount { get; set; }
            public int CurrentCount { get; set; }
        }

        private class LocalHideoutRequirementState
        {
            public string RequirementId { get; set; }
            public int Count { get; set; }
        }

        private void SaveLocalTasksState()
        {
            try
            {
                List<LocalObjectiveState> objectives;
                TarkovAPI.Data tarkovDataSnapshot = _tarkovDataService?.Data;

                lock (_lockObject)
                {
                    objectives = _pendingTaskObjectives.Values
                        .Select(o => new LocalObjectiveState
                        {
                            ObjectiveId = o.ObjectiveId,
                            ItemId = o.ItemId,
                            RequiredCount = o.RequiredCount,
                            CurrentCount = o.CurrentCount
                        })
                        .ToList();
                }

                // Diccionarios auxiliares para nombres legibles
                var objectiveInfo = new Dictionary<string, (string TaskName, string ObjectiveDescription)>();
                var itemNames = new Dictionary<string, string>();

                if (tarkovDataSnapshot?.items != null)
                {
                    foreach (var item in tarkovDataSnapshot.items)
                    {
                        if (!string.IsNullOrEmpty(item.id) && !itemNames.ContainsKey(item.id))
                            itemNames[item.id] = item.name;

                        if (item.usedInTasks == null)
                            continue;

                        foreach (var task in item.usedInTasks)
                        {
                            if (task.objectives == null)
                                continue;

                            foreach (var obj in task.objectives)
                            {
                                if (string.IsNullOrEmpty(obj.id))
                                    continue;

                                if (!objectiveInfo.ContainsKey(obj.id))
                                {
                                    objectiveInfo[obj.id] = (task.name, obj.description);
                                }
                            }
                        }
                    }
                }

                // Proyección enriquecida para el JSON (los campos extra se ignoran al cargar)
                var enrichedObjectives = objectives
                    .Select(o =>
                    {
                        objectiveInfo.TryGetValue(o.ObjectiveId, out var info);
                        itemNames.TryGetValue(o.ItemId, out var itemName);

                        return new
                        {
                            o.ObjectiveId,
                            o.ItemId,
                            o.RequiredCount,
                            o.CurrentCount,
                            TaskName = info.TaskName,
                            ObjectiveDescription = info.ObjectiveDescription,
                            ItemName = itemName
                        };
                    })
                    .ToList();

                var json = JsonConvert.SerializeObject(new { Objectives = enrichedObjectives }, Formatting.Indented);
                File.WriteAllText(LOCAL_TASKS_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while saving local tasks state: " + ex.Message);
            }
        }

        private void SaveLocalHideoutState()
        {
            try
            {
                List<LocalHideoutRequirementState> requirements;
                TarkovAPI.Data tarkovDataSnapshot = _tarkovDataService?.Data;

                lock (_lockObject)
                {
                    // Eliminamos de memoria los requirements con valor 0 para no acumular basura
                    var keysToRemove = _localHideout.Where(kvp => kvp.Value <= 0).Select(kvp => kvp.Key).ToList();
                    foreach (var key in keysToRemove)
                    {
                        _localHideout.Remove(key);
                    }

                    requirements = _localHideout
                        .Where(kvp => kvp.Value > 0)
                        .Select(kvp => new LocalHideoutRequirementState
                        {
                            RequirementId = kvp.Key,
                            Count = kvp.Value
                        })
                        .ToList();
                }

                // Diccionario auxiliar para mapear requirementId a datos legibles
                var hideoutInfo = new Dictionary<string, (string StationName, int? StationLevel, string ItemId, string ItemName, int RequiredCount)>();

                if (tarkovDataSnapshot?.hideoutStations != null)
                {
                    foreach (var station in tarkovDataSnapshot.hideoutStations)
                    {
                        if (station.levels == null)
                            continue;

                        foreach (var level in station.levels)
                        {
                            if (level.itemRequirements == null)
                                continue;

                            foreach (var req in level.itemRequirements)
                            {
                                if (string.IsNullOrEmpty(req.id))
                                    continue;

                                string itemId = req.item?.id;
                                string itemName = req.item?.name;
                                int required = req.count ?? 0;

                                hideoutInfo[req.id] = (station.name, level.level, itemId, itemName, required);
                            }
                        }
                    }
                }

                var enrichedRequirements = requirements
                    .Select(r =>
                    {
                        hideoutInfo.TryGetValue(r.RequirementId, out var info);

                        return new
                        {
                            r.RequirementId,
                            Count = r.Count,
                            ItemId = info.ItemId,
                            ItemName = info.ItemName,
                            StationName = info.StationName,
                            StationLevel = info.StationLevel,
                            RequiredCount = info.RequiredCount
                        };
                    })
                    .ToList();

                var json = JsonConvert.SerializeObject(new { Requirements = enrichedRequirements }, Formatting.Indented);
                File.WriteAllText(LOCAL_HIDEOUT_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while saving local hideout state: " + ex.Message);
            }
        }

        private void LoadLocalTasksState()
        {
            try
            {
                if (!File.Exists(LOCAL_TASKS_FILE))
                    return;

                var json = File.ReadAllText(LOCAL_TASKS_FILE);
                var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
                var objectivesToken = wrapper?.Objectives;
                if (objectivesToken == null)
                    return;

                var objectives = objectivesToken.ToObject<List<LocalObjectiveState>>();
                if (objectives == null)
                    return;

                lock (_lockObject)
                {
                    _pendingTaskObjectives.Clear();
                    foreach (var o in objectives)
                    {
                        if (string.IsNullOrEmpty(o.ObjectiveId))
                            continue;

                        _pendingTaskObjectives[o.ObjectiveId] = new CurrentTrackedObjective
                        {
                            ObjectiveId = o.ObjectiveId,
                            ItemId = o.ItemId,
                            RequiredCount = o.RequiredCount,
                            CurrentCount = o.CurrentCount
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while loading local tasks state: " + ex.Message);
            }
        }

        private void LoadLocalHideoutState()
        {
            try
            {
                if (!File.Exists(LOCAL_HIDEOUT_FILE))
                    return;

                var json = File.ReadAllText(LOCAL_HIDEOUT_FILE);
                var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
                var requirementsToken = wrapper?.Requirements;
                if (requirementsToken == null)
                    return;

                var requirements = requirementsToken.ToObject<List<LocalHideoutRequirementState>>();
                if (requirements == null)
                    return;

                lock (_lockObject)
                {
                    _localHideout.Clear();
                    foreach (var r in requirements)
                    {
                        if (string.IsNullOrEmpty(r.RequirementId))
                            continue;

                        if (r.Count <= 0)
                            continue;

                        _localHideout[r.RequirementId] = r.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while loading local hideout state: " + ex.Message);
            }
        }

        private CurrentTrackedObjective SelectHideoutRequirementForDelta(Item item, Data tarkovData, int delta)
        {
            if (item == null || tarkovData == null)
            {
                return null;
            }

            var hideoutStations = tarkovData.hideoutStations;
            if (hideoutStations == null)
            {
                return null;
            }

            // Construimos una lista ordenada por nivel ascendente de todos los requirements de este item
            var requirements = new List<(int level, string requirementId, int required, int current)>();

            foreach (var station in hideoutStations)
            {
                if (station.levels == null)
                    continue;

                foreach (var stationLevel in station.levels)
                {
                    if (stationLevel.itemRequirements == null)
                        continue;

                    foreach (var itemReq in stationLevel.itemRequirements)
                    {
                        if (itemReq.item == null || itemReq.item.id != item.id)
                            continue;

                        int required = itemReq.count ?? 0;
                        int extraLocal = GetLocalHideoutExtraCount(itemReq.id);
                        int current = extraLocal;
                        if (current > required)
                            current = required;

                        int levelValue = stationLevel.level ?? int.MaxValue;
                        requirements.Add((levelValue, itemReq.id, required, current));
                    }
                }
            }

            if (requirements.Count == 0)
            {
                return null;
            }

            var ordered = requirements.OrderBy(r => r.level).ToList();

            if (delta > 0)
            {
                // Para +1: primer requirement con hueco (current < required)
                foreach (var r in ordered)
                {
                    if (r.current < r.required)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = r.requirementId,
                            ItemId = item.id,
                            RequiredCount = r.required,
                            CurrentCount = r.current
                        };
                    }
                }
            }
            else // delta < 0
            {
                // Para -1: último requirement (por nivel asc) con progreso > 0
                for (int i = ordered.Count - 1; i >= 0; i--)
                {
                    var r = ordered[i];
                    if (r.current > 0)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = r.requirementId,
                            ItemId = item.id,
                            RequiredCount = r.required,
                            CurrentCount = r.current
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Increment the current objective for an item and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> IncrementObjectiveAndSyncAsync(Item item, Data tarkovData, CancellationToken cancellationToken = default)
        {
            var validation = TryIncrementCurrentObjectiveForCurrentItem(item, tarkovData);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            // Actualizar TrackerData localmente para que el overlay se refresque sin un GET extra
            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }

        public void Dispose()
        {
            try
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            catch (Exception)
            {
                // Ignore dispose-time exceptions
            }
        }

        /// <summary>
        /// Decrement the current objective for an item and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> DecrementObjectiveAndSyncAsync(Item item, Data tarkovData, CancellationToken cancellationToken = default)
        {
            var validation = TryDecrementCurrentObjectiveForCurrentItem(item, tarkovData);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }

        /// <summary>
        /// Change the current objective for an item by a delta and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> ChangeObjectiveAndSyncAsync(Item item, Data tarkovData, int delta, CancellationToken cancellationToken = default)
        {
            var validation = TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, delta);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }

        private async Task<bool> UpdateObjectiveCountAsync(CurrentTrackedObjective updatedObjective, CancellationToken cancellationToken)
        {
            var settings = _settingsService.Settings;
            string apiKey = settings.TarkovTrackerApiKey;

            if (!settings.UseTarkovTrackerApi || string.Equals(apiKey, "APIKey") || string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            // Si recientemente recibimos un 429, respetar un pequeño cooldown para no seguir martilleando la API
            if (DateTime.UtcNow - _lastTooManyRequests < TooManyRequestsCooldown)
            {
                Debug.WriteLine("[TarkovTracker] Skipping objective update due to recent 429 (cooldown in effect)");
                return false;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // According to TarkovTracker OpenAPI, both count and state are supported
                var state = updatedObjective.CurrentCount >= updatedObjective.RequiredCount ? "completed" : "uncompleted";
                var payload = new { count = updatedObjective.CurrentCount, state };
                var json = JsonConvert.SerializeObject(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"https://tarkovtracker.io/api/v2/progress/task/objective/{updatedObjective.ObjectiveId}";
                Debug.WriteLine($"[TarkovTracker] Updating objective via {url} -> count={updatedObjective.CurrentCount}, state={state}");

                // OpenAPI specifies POST for this endpoint
                using var response = await client.PostAsync(url, content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[TarkovTracker] Failed to update objective {updatedObjective.ObjectiveId}: {(int)response.StatusCode} {response.ReasonPhrase}");

                    if ((int)response.StatusCode == 429)
                    {
                        _lastTooManyRequests = DateTime.UtcNow;
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while updating objective: " + ex.Message);
                return false;
            }
        }

        public CurrentTrackedObjective GetCurrentTrackedObjectiveForItem(Item item, Data tarkovData)
        {
            // To show the current objective on the overlay, use the same directional logic
            // as for an increment: first incomplete objective ordered by ascending minPlayerLevel.
            return SelectTaskObjectiveForDelta(item, tarkovData, +1);
        }

        public CurrentTrackedObjective GetCurrentHideoutRequirementForItem(Item item, Data tarkovData)
        {
            if (item == null || tarkovData == null)
            {
                return null;
            }

            var hideoutStations = tarkovData.hideoutStations;
            if (hideoutStations == null)
            {
                return null;
            }

            CurrentTrackedObjective best = null;
            int bestLevel = int.MinValue;

            foreach (var station in hideoutStations)
            {
                if (station.levels == null)
                    continue;

                foreach (var stationLevel in station.levels)
                {
                    if (stationLevel.itemRequirements == null)
                        continue;

                    foreach (var itemReq in stationLevel.itemRequirements)
                    {
                        if (itemReq.item == null || itemReq.item.id != item.id)
                            continue;

                        int required = itemReq.count ?? 0;
                        int extraLocal = GetLocalHideoutExtraCount(itemReq.id);
                        int current = extraLocal;
                        if (current > required)
                            current = required;

                        int levelValue = stationLevel.level ?? int.MaxValue;
                        if (best == null || levelValue < bestLevel)
                        {
                            bestLevel = levelValue;
                            best = new CurrentTrackedObjective
                            {
                                ObjectiveId = itemReq.id, // here we use the hideout requirement ID
                                ItemId = item.id,
                                RequiredCount = required,
                                CurrentCount = current
                            };
                        }
                    }
                }
            }

            return best;
        }

        public TrackerUpdateResult TryIncrementCurrentObjectiveForCurrentItem(Item item, Data tarkovData)
        {
            return TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, +1);
        }

        public TrackerUpdateResult TryDecrementCurrentObjectiveForCurrentItem(Item item, Data tarkovData)
        {
            return TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, -1);
        }

        public TrackerUpdateResult TryChangeCurrentObjectiveForCurrentItem(Item item, Data tarkovData, int delta)
        {
            if (delta == 0)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            // Directional selection: for + we look for the first incomplete objective,
            // for - the last objective with progress > 0, ordered by ascending minPlayerLevel
            var objective = SelectTaskObjectiveForDelta(item, tarkovData, delta);
            if (objective == null)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            int newCount = objective.CurrentCount + delta;
            if (newCount < 0)
            {
                newCount = 0;
            }
            if (newCount > objective.RequiredCount)
            {
                newCount = objective.RequiredCount;
            }

            if (newCount == objective.CurrentCount)
            {
                if (delta > 0 && objective.CurrentCount >= objective.RequiredCount)
                {
                    // Already completed, nothing more can be added
                    return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.AlreadyCompleted, objective);
                }

                if (delta < 0 && objective.CurrentCount <= 0)
                {
                    // No progress to remove, treat as a silent no-op
                    return TrackerUpdateResult.Ok(objective);
                }
            }

            var updated = new CurrentTrackedObjective
            {
                ObjectiveId = objective.ObjectiveId,
                ItemId = objective.ItemId,
                RequiredCount = objective.RequiredCount,
                CurrentCount = newCount
            };

            Debug.WriteLine($"[TarkovTracker] Change objective {updated.ObjectiveId} for item {updated.ItemId}: {objective.CurrentCount} -> {updated.CurrentCount} (delta={delta})");
            return TrackerUpdateResult.Ok(updated);
        }

        private CurrentTrackedObjective SelectTaskObjectiveForDelta(Item item, Data tarkovData, int delta)
        {
            if (item == null || tarkovData == null || TrackerData == null || TrackerData.data == null)
            {
                return null;
            }

            var trackerData = TrackerData.data;
            var candidates = GetOrderedTaskObjectivesForItem(item, trackerData);
            if (candidates.Count == 0)
            {
                return null;
            }

            if (delta > 0)
            {
                // For +1: first objective that is not yet complete
                foreach (var c in candidates)
                {
                    if (c.current < c.required)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = c.objectiveId,
                            ItemId = item.id,
                            RequiredCount = c.required,
                            CurrentCount = c.current
                        };
                    }
                }
            }
            else // delta < 0
            {
                // For -1: last objective (by ascending level order) with progress > 0
                for (int i = candidates.Count - 1; i >= 0; i--)
                {
                    var c = candidates[i];
                    if (c.current > 0)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = c.objectiveId,
                            ItemId = item.id,
                            RequiredCount = c.required,
                            CurrentCount = c.current
                        };
                    }
                }
            }

            return null;
        }

        private List<(string objectiveId, int required, int current)> GetOrderedTaskObjectivesForItem(Item item, TarkovTrackerAPI.Data trackerData)
        {
            var result = new List<(string objectiveId, int required, int current)>();

            var usedInTasks = item.usedInTasks;
            if (usedInTasks == null || usedInTasks.Count == 0)
            {
                return result;
            }

            // Order tasks by ascending player level (earliest upgrade first)
            var orderedTasks = usedInTasks
                .Where(t => t.objectives != null)
                .OrderBy(t => t.minPlayerLevel ?? int.MaxValue)
                .ToList();

            foreach (var task in orderedTasks)
            {
                if (task.objectives == null)
                    continue;

                foreach (var obj in task.objectives)
                {
                    if (obj.type != "findItem" || obj.foundInRaid != true || obj.items == null || !obj.items.Any(i => i.id == item.id))
                        continue;

                    int required = obj.count ?? 0;
                    int current = 0;

                    if (trackerData.taskObjectivesProgress != null && obj.id != null)
                    {
                        var progress = trackerData.taskObjectivesProgress.FirstOrDefault(p => p.id == obj.id);
                        if (progress != null)
                        {
                            if (progress.complete == true)
                            {
                                current = required;
                            }
                            else if (progress.count != null)
                            {
                                current = progress.count.Value;
                            }
                        }
                    }

                    result.Add((obj.id, required, current));
                }
            }

            return result;
        }

        public TrackerUpdateResult ApplyLocalChangeForCurrentItem(Item item, Data tarkovData, int delta)
        {
            var validation = TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, delta);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;

            // Actualizar el progreso localmente para que el overlay lo use inmediatamente
            ApplyLocalObjectiveUpdate(updatedObjective);

            // Encolar el objetivo para flush posterior a la API
            lock (_lockObject)
            {
                if (!string.IsNullOrEmpty(updatedObjective.ObjectiveId))
                {
                    _pendingTaskObjectives[updatedObjective.ObjectiveId] = updatedObjective;
                }
            }

            // Persistir cambios locales de tasks
            SaveLocalTasksState();

            return validation;
        }

        public TrackerUpdateResult ApplyLocalHideoutChangeForCurrentItem(Item item, Data tarkovData, int delta)
        {
            if (delta == 0)
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            var requirement = SelectHideoutRequirementForDelta(item, tarkovData, delta);
            if (requirement == null || string.IsNullOrEmpty(requirement.ObjectiveId))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.NoObjectiveForItem);
            }

            int newCount = requirement.CurrentCount + delta;
            if (newCount < 0)
            {
                newCount = 0;
            }
            if (newCount > requirement.RequiredCount)
            {
                newCount = requirement.RequiredCount;
            }

            if (newCount == requirement.CurrentCount)
            {
                if (delta > 0 && requirement.CurrentCount >= requirement.RequiredCount)
                {
                    return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.AlreadyCompleted, requirement);
                }

                if (delta < 0 && requirement.CurrentCount <= 0)
                {
                    return TrackerUpdateResult.Ok(requirement);
                }
            }

            // In purely local mode, the extra counter is equal to the current value
            int newExtra = newCount;

            lock (_lockObject)
            {
                _localHideout[requirement.ObjectiveId] = newExtra;
            }

            var updated = new CurrentTrackedObjective
            {
                ObjectiveId = requirement.ObjectiveId,
                ItemId = requirement.ItemId,
                RequiredCount = requirement.RequiredCount,
                CurrentCount = newCount
            };

            Debug.WriteLine($"[TarkovTracker] Local hideout change {updated.ObjectiveId} for item {updated.ItemId}: {requirement.CurrentCount} -> {updated.CurrentCount} (delta={delta})");

            // Persist local hideout changes
            SaveLocalHideoutState();
            return TrackerUpdateResult.Ok(updated);
        }

        private class LocalObjectiveState
        {
            public string ObjectiveId { get; set; }
            public string ItemId { get; set; }
            public int RequiredCount { get; set; }
            public int CurrentCount { get; set; }
        }

        private class LocalHideoutRequirementState
        {
            public string RequirementId { get; set; }
            public int Count { get; set; }
        }

        private void SaveLocalTasksState()
        {
            try
            {
                List<LocalObjectiveState> objectives;
                TarkovAPI.Data tarkovDataSnapshot = _tarkovDataService?.Data;

                lock (_lockObject)
                {
                    objectives = _pendingTaskObjectives.Values
                        .Select(o => new LocalObjectiveState
                        {
                            ObjectiveId = o.ObjectiveId,
                            ItemId = o.ItemId,
                            RequiredCount = o.RequiredCount,
                            CurrentCount = o.CurrentCount
                        })
                        .ToList();
                }

                // Diccionarios auxiliares para nombres legibles
                var objectiveInfo = new Dictionary<string, (string TaskName, string ObjectiveDescription)>();
                var itemNames = new Dictionary<string, string>();

                if (tarkovDataSnapshot?.items != null)
                {
                    foreach (var item in tarkovDataSnapshot.items)
                    {
                        if (!string.IsNullOrEmpty(item.id) && !itemNames.ContainsKey(item.id))
                            itemNames[item.id] = item.name;

                        if (item.usedInTasks == null)
                            continue;

                        foreach (var task in item.usedInTasks)
                        {
                            if (task.objectives == null)
                                continue;

                            foreach (var obj in task.objectives)
                            {
                                if (string.IsNullOrEmpty(obj.id))
                                    continue;

                                if (!objectiveInfo.ContainsKey(obj.id))
                                {
                                    objectiveInfo[obj.id] = (task.name, obj.description);
                                }
                            }
                        }
                    }
                }

                // Proyección enriquecida para el JSON (los campos extra se ignoran al cargar)
                var enrichedObjectives = objectives
                    .Select(o =>
                    {
                        objectiveInfo.TryGetValue(o.ObjectiveId, out var info);
                        itemNames.TryGetValue(o.ItemId, out var itemName);

                        return new
                        {
                            o.ObjectiveId,
                            o.ItemId,
                            o.RequiredCount,
                            o.CurrentCount,
                            TaskName = info.TaskName,
                            ObjectiveDescription = info.ObjectiveDescription,
                            ItemName = itemName
                        };
                    })
                    .ToList();

                var json = JsonConvert.SerializeObject(new { Objectives = enrichedObjectives }, Formatting.Indented);
                File.WriteAllText(LOCAL_TASKS_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while saving local tasks state: " + ex.Message);
            }
        }

        private void SaveLocalHideoutState()
        {
            try
            {
                List<LocalHideoutRequirementState> requirements;
                TarkovAPI.Data tarkovDataSnapshot = _tarkovDataService?.Data;

                lock (_lockObject)
                {
                    // Eliminamos de memoria los requirements con valor 0 para no acumular basura
                    var keysToRemove = _localHideout.Where(kvp => kvp.Value <= 0).Select(kvp => kvp.Key).ToList();
                    foreach (var key in keysToRemove)
                    {
                        _localHideout.Remove(key);
                    }

                    requirements = _localHideout
                        .Where(kvp => kvp.Value > 0)
                        .Select(kvp => new LocalHideoutRequirementState
                        {
                            RequirementId = kvp.Key,
                            Count = kvp.Value
                        })
                        .ToList();
                }

                // Diccionario auxiliar para mapear requirementId a datos legibles
                var hideoutInfo = new Dictionary<string, (string StationName, int? StationLevel, string ItemId, string ItemName, int RequiredCount)>();

                if (tarkovDataSnapshot?.hideoutStations != null)
                {
                    foreach (var station in tarkovDataSnapshot.hideoutStations)
                    {
                        if (station.levels == null)
                            continue;

                        foreach (var level in station.levels)
                        {
                            if (level.itemRequirements == null)
                                continue;

                            foreach (var req in level.itemRequirements)
                            {
                                if (string.IsNullOrEmpty(req.id))
                                    continue;

                                string itemId = req.item?.id;
                                string itemName = req.item?.name;
                                int required = req.count ?? 0;

                                hideoutInfo[req.id] = (station.name, level.level, itemId, itemName, required);
                            }
                        }
                    }
                }

                var enrichedRequirements = requirements
                    .Select(r =>
                    {
                        hideoutInfo.TryGetValue(r.RequirementId, out var info);

                        return new
                        {
                            r.RequirementId,
                            Count = r.Count,
                            ItemId = info.ItemId,
                            ItemName = info.ItemName,
                            StationName = info.StationName,
                            StationLevel = info.StationLevel,
                            RequiredCount = info.RequiredCount
                        };
                    })
                    .ToList();

                var json = JsonConvert.SerializeObject(new { Requirements = enrichedRequirements }, Formatting.Indented);
                File.WriteAllText(LOCAL_HIDEOUT_FILE, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while saving local hideout state: " + ex.Message);
            }
        }

        private void LoadLocalTasksState()
        {
            try
            {
                if (!File.Exists(LOCAL_TASKS_FILE))
                    return;

                var json = File.ReadAllText(LOCAL_TASKS_FILE);
                var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
                var objectivesToken = wrapper?.Objectives;
                if (objectivesToken == null)
                    return;

                var objectives = objectivesToken.ToObject<List<LocalObjectiveState>>();
                if (objectives == null)
                    return;

                lock (_lockObject)
                {
                    _pendingTaskObjectives.Clear();
                    foreach (var o in objectives)
                    {
                        if (string.IsNullOrEmpty(o.ObjectiveId))
                            continue;

                        _pendingTaskObjectives[o.ObjectiveId] = new CurrentTrackedObjective
                        {
                            ObjectiveId = o.ObjectiveId,
                            ItemId = o.ItemId,
                            RequiredCount = o.RequiredCount,
                            CurrentCount = o.CurrentCount
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while loading local tasks state: " + ex.Message);
            }
        }

        private void LoadLocalHideoutState()
        {
            try
            {
                if (!File.Exists(LOCAL_HIDEOUT_FILE))
                    return;

                var json = File.ReadAllText(LOCAL_HIDEOUT_FILE);
                var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
                var requirementsToken = wrapper?.Requirements;
                if (requirementsToken == null)
                    return;

                var requirements = requirementsToken.ToObject<List<LocalHideoutRequirementState>>();
                if (requirements == null)
                    return;

                lock (_lockObject)
                {
                    _localHideout.Clear();
                    foreach (var r in requirements)
                    {
                        if (string.IsNullOrEmpty(r.RequirementId))
                            continue;

                        if (r.Count <= 0)
                            continue;

                        _localHideout[r.RequirementId] = r.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[TarkovTracker] Error while loading local hideout state: " + ex.Message);
            }
        }

        private CurrentTrackedObjective SelectHideoutRequirementForDelta(Item item, Data tarkovData, int delta)
        {
            if (item == null || tarkovData == null)
            {
                return null;
            }

            var hideoutStations = tarkovData.hideoutStations;
            if (hideoutStations == null)
            {
                return null;
            }

            // Construimos una lista ordenada por nivel ascendente de todos los requirements de este item
            var requirements = new List<(int level, string requirementId, int required, int current)>();

            foreach (var station in hideoutStations)
            {
                if (station.levels == null)
                    continue;

                foreach (var stationLevel in station.levels)
                {
                    if (stationLevel.itemRequirements == null)
                        continue;

                    foreach (var itemReq in stationLevel.itemRequirements)
                    {
                        if (itemReq.item == null || itemReq.item.id != item.id)
                            continue;

                        int required = itemReq.count ?? 0;
                        int extraLocal = GetLocalHideoutExtraCount(itemReq.id);
                        int current = extraLocal;
                        if (current > required)
                            current = required;

                        int levelValue = stationLevel.level ?? int.MaxValue;
                        requirements.Add((levelValue, itemReq.id, required, current));
                    }
                }
            }

            if (requirements.Count == 0)
            {
                return null;
            }

            var ordered = requirements.OrderBy(r => r.level).ToList();

            if (delta > 0)
            {
                // Para +1: primer requirement con hueco (current < required)
                foreach (var r in ordered)
                {
                    if (r.current < r.required)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = r.requirementId,
                            ItemId = item.id,
                            RequiredCount = r.required,
                            CurrentCount = r.current
                        };
                    }
                }
            }
            else // delta < 0
            {
                // Para -1: último requirement (por nivel asc) con progreso > 0
                for (int i = ordered.Count - 1; i >= 0; i--)
                {
                    var r = ordered[i];
                    if (r.current > 0)
                    {
                        return new CurrentTrackedObjective
                        {
                            ObjectiveId = r.requirementId,
                            ItemId = item.id,
                            RequiredCount = r.required,
                            CurrentCount = r.current
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Increment the current objective for an item and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> IncrementObjectiveAndSyncAsync(Item item, Data tarkovData, CancellationToken cancellationToken = default)
        {
            var validation = TryIncrementCurrentObjectiveForCurrentItem(item, tarkovData);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            // Actualizar TrackerData localmente para que el overlay se refresque sin un GET extra
            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }

        public void Dispose()
        {
            try
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            catch (Exception)
            {
                // Ignore dispose-time exceptions
            }
        }

        /// <summary>
        /// Decrement the current objective for an item and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> DecrementObjectiveAndSyncAsync(Item item, Data tarkovData, CancellationToken cancellationToken = default)
        {
            var validation = TryDecrementCurrentObjectiveForCurrentItem(item, tarkovData);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }

        /// <summary>
        /// Change the current objective for an item by a delta and send the update immediately to the API.
        /// This bypasses the background flush mechanism and is intended for manual or testing scenarios.
        /// For normal gameplay use the local path (ApplyLocalChangeForCurrentItem + periodic flush).
        /// </summary>
        public async Task<TrackerUpdateResult> ChangeObjectiveAndSyncAsync(Item item, Data tarkovData, int delta, CancellationToken cancellationToken = default)
        {
            var validation = TryChangeCurrentObjectiveForCurrentItem(item, tarkovData, delta);
            if (!validation.Success)
            {
                return validation;
            }

            var updatedObjective = validation.Objective;
            if (!await UpdateObjectiveCountAsync(updatedObjective, cancellationToken).ConfigureAwait(false))
            {
                return TrackerUpdateResult.Fail(TrackerUpdateFailureReason.ApiError, updatedObjective);
            }

            ApplyLocalObjectiveUpdate(updatedObjective);
            return validation;
        }
    }
}
