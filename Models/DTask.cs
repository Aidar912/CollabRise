using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Dsana.Models
{
    public class DTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }


        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Task Type")]
        public int DTaskTypeId { get; set; }

        [DisplayName("Task Priority")]
        public int DTaskPriorityId { get; set; }

        [DisplayName("Task Status")]
        public int DTaskStatusId { get; set; }

        [DisplayName("Task Owner")]
        public string OwnerUserId { get; set; }

        [DisplayName("Task Developer")]
        public string DeveloperUserId { get; set; }

        public virtual Project Project { get; set; }

        public virtual DTaskType DTaskType { get; set; }

        public virtual DTaskPriority DTaskPriority { get; set; }

        public virtual DTaskStatus DTaskStatus { get; set; }

        public virtual DSUser OwnerUser { get; set; }

        public virtual DSUser DeveloperUser { get; set; }

        public virtual ICollection<DTaskComment> Comments { get; set; } = new HashSet<DTaskComment>();

        public virtual ICollection<DTaskAttachment> Attachments { get; set; } = new HashSet<DTaskAttachment>();

        public virtual ICollection<DTaskHistory> History { get; set; } = new HashSet<DTaskHistory>();


    }
}