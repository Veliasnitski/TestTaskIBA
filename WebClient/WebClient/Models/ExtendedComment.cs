using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Models
{
    public class ExtendedComment
    {
        public Comment Comment { get; set; }
        public User User { get; set; }
    }
}
