using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebManagerTasks.Data.Models
{
    public class User
    {
        public User()
        {
            UsersProjects = new HashSet<UserProject>();
        }

        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        [JsonIgnore]
        public ICollection<UserProject> UsersProjects { get; set; }
    }
}
