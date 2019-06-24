using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebManagerTasks.Data.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public int IdProject { get; set; }
        public int IdTask { get; set; }
        public int IdUser { get; set; }
        public DateTime? DateStart { get; set; }
        [JsonIgnore]
        public Project IdProjectNavigation { get; set; }
        [JsonIgnore]
        public MyTask IdTaskNavigation { get; set; }
    }
}
