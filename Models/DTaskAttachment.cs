using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Dsana.Extensions;

namespace Dsana.Models
{
    public class DTaskAttachment
    {
        public int Id { get; set; }

        [DisplayName("Task")]
        public int DTaskId { get; set; }

        [DisplayName("File Date")]
        public DateTimeOffset Created { get; set; }


        [DisplayName("Team Member")]
        public string UserId { get; set; }

        [DisplayName("File Description")]
        public string Description { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [DisplayName("Select a file")]
        [MaxFileSize(1024 * 1024 )]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile FormFile { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Data")]
        public byte[] FileData { get; set; }

        [DisplayName("File Extension")]
        public string FileContentType { get; set; }



        public virtual DTask DTask { get; set; }
        public virtual DSUser User { get; set; }
    }
}
