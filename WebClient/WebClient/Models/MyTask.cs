using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebClient.Models
{
    public class MyTask
    {
        public MyTask()
        {
            Comments = new HashSet<Comment>();
            ProjectsTasks = new HashSet<ProjectTask>();
        }

        public int IdTask { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; }
        [JsonIgnore]
        public ICollection<ProjectTask> ProjectsTasks { get; set; }
    }
}
