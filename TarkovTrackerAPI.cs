using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TarkovPriceViewer
{
    public class TarkovTrackerAPI
    {
        public class Data
        {
            public List<TasksProgress> tasksProgress { get; set; }
            public List<TaskObjectivesProgress> taskObjectivesProgress { get; set; }
            public List<HideoutModulesProgress> hideoutModulesProgress { get; set; }
            public List<HideoutPartsProgress> hideoutPartsProgress { get; set; }
            public string displayName { get; set; }
            public string userId { get; set; }
            public int? playerLevel { get; set; }
            public int? gameEdition { get; set; }
        }

        public class HideoutModulesProgress
        {
            public string id { get; set; }
            public bool? complete { get; set; }
        }

        public class HideoutPartsProgress
        {
            public string id { get; set; }
            public bool? complete { get; set; }
            public int? count { get; set; }
        }

        public class Meta
        {
            public string self { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
            public Meta meta { get; set; }
        }

        public class TaskObjectivesProgress
        {
            public string id { get; set; }
            public bool? complete { get; set; }
            public int? count { get; set; }
        }

        public class TasksProgress
        {
            public string id { get; set; }
            public bool? complete { get; set; }
        }
    }
}