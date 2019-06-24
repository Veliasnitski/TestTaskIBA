using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebManagerTasks.Data.Models
{
    public class UserProject
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdProject { get; set; }
        public int Role { get; set; }
        [JsonIgnore]
        public Project IdProjectNavigation { get; set; }
        [JsonIgnore]
        public User IdUserNavigation { get; set; }
    }
}
