using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dsana.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public DTask DTask { get; set; }

        public SelectList Developers { get; set; }

        public string DeveloperId { get; set; }

    }
}
