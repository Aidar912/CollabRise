using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Dsana.Models
{
    public class DTaskHistory
    {
        public int Id{ get; set; }

        [DisplayName("Task")]
        public int DTaskId { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of Change")]
        public string Description { get; set; }

        [DisplayName("Team Member")]
        public string UserId { get; set; }

        public virtual DTask DTask { get; set; }
        public virtual DSUser User { get; set; }

    }
}
