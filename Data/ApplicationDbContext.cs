using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dsana.Models;

namespace Dsana.Data
{
    public class ApplicationDbContext : IdentityDbContext<DSUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<DTask> DTasks { get; set; }
        public DbSet<ProjectPriority> ProjectPriorities { get; set; }
        public DbSet<DTaskAttachment> DTaskAttachments { get; set; }
        public DbSet<DTaskComment> DTaskComments { get; set; }
        public DbSet<DTaskHistory> DTaskHistories { get; set; }
        public DbSet<DTaskPriority> DTaskPriorities { get; set; }
        public DbSet<DTaskStatus> DTaskStatuses { get; set; }
        public DbSet<DTaskType> DTaskTypes { get; set; }
    }   
}
