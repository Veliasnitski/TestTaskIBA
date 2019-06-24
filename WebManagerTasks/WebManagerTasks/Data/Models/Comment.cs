using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebManagerTasks.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int IdTask { get; set; }
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreate { get; set; }
        [JsonIgnore]
        public MyTask IdTaskNavigation { get; set; }
    }
}
