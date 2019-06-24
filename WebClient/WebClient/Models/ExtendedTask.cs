using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class ExtendedTask
    {
        public int IdUser { get; set; }
        public int IdProject { get; set; }
        public MyTask Task { get; set; }
    }
}
