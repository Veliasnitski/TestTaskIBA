using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebManagerTasks.Data.Models;

namespace WebManagerTasks.ViewModels
{
    public class ExtendedProject
    {
        public int AdminId { get; set; }
        public Project Project { get; set; }
    }
}
