using System;
using System.Collections.Generic;

namespace Dsana.Models.ViewModels
{
    public class DashboardViewModel
    {
        public Company Company { get; set; }
        public List<Project> Projects { get; set; }
        public List<DTask> DTasks { get; set; }
        public List<DSUser> Members { get; set; }


    }
}
