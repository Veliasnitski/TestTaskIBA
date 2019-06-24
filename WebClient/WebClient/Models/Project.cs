using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebClient.Models
{
    public class Project
    {
        public Project()
        {
            ProjectsTasks = new HashSet<ProjectTask>();
            UsersProjects = new HashSet<UserProject>();
        }

        public int IdProject { get; set; }
        [Required(ErrorMessage = "Enter name project")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Choice status")]
        public int? Status { get; set; }

        [JsonIgnore]
        public ICollection<ProjectTask> ProjectsTasks { get; set; }
        [JsonIgnore]
        public ICollection<UserProject> UsersProjects { get; set; }
    }
}
