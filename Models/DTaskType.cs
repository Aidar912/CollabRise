using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Dsana.Models
{
    public class DTaskType
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; }

    }
}
