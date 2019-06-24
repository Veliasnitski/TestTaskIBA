using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebManagerTasks.Data.Models
{
    public class Project
    {
        public Project()
        {
            ProjectsTasks = new HashSet<ProjectTask>();
            UsersProjects = new HashSet<UserProject>();
        }

        public int IdProject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }

        [JsonIgnore]
        public ICollection<ProjectTask> ProjectsTasks { get; set; }
        [JsonIgnore]
        public ICollection<UserProject> UsersProjects { get; set; }
    }
}
