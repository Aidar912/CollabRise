using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dsana.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project Project { get; set; }
        public SelectList PMList { get; set; }
        public string PmId { get; set; }
        public SelectList PriorityList { get; set; }


        //public Project Project { get; set; }
        //public SelectList PMList { get; set; }
        //public string PmId { get; set; }
    }
}
