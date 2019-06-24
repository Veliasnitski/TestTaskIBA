using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class ExtendedUser
    {
        public int IdProject { get; set; }
        public int Role { get; set; }
        public User User { get; set; }
    }
}
