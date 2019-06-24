using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManagerTasks.Data.Models;

namespace WebManagerTasks.ViewModels
{
    public class ExtendedComment
    {
        public Comment Comment { get; set; }
        public User User { get; set; }
    }
}
