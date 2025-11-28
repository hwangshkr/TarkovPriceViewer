using System;

namespace TarkovPriceViewer.Models
{
    public class CurrentTrackedObjective
    {
        public string ObjectiveId { get; set; }
        public string ItemId { get; set; }
        public int RequiredCount { get; set; }
        public int CurrentCount { get; set; }
        public int Remaining => Math.Max(0, RequiredCount - CurrentCount);
    }

    public enum TrackerUpdateFailureReason
    {
        None = 0,
        NoObjectiveForItem,
        AlreadyCompleted,
        NoProgressToRemove,
        ApiError,
    }

    public class TrackerUpdateResult
    {
        public bool Success { get; set; }
        public TrackerUpdateFailureReason FailureReason { get; set; } = TrackerUpdateFailureReason.None;
        public CurrentTrackedObjective Objective { get; set; }

        public static TrackerUpdateResult Ok(CurrentTrackedObjective objective) => new TrackerUpdateResult
        {
            Success = true,
            Objective = objective
        };

        public static TrackerUpdateResult Fail(TrackerUpdateFailureReason reason, CurrentTrackedObjective objective = null) => new TrackerUpdateResult
        {
            Success = false,
            FailureReason = reason,
            Objective = objective
        };
    }
}
