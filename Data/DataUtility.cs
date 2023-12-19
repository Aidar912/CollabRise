using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsana.Models;
using Dsana.Models.Enums;

namespace Dsana.Data
{
    public static class DataUtility
    {
        //Company Ids
        private static int company1Id;
        private static int company2Id;
        private static int company3Id;
        private static int company4Id;
        private static int company5Id;

        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost host)
        {
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;
            //Service: An instance of RoleManager
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service: An instance of RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service: An instance of the UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<DSUser>>();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();


            //Custom  Bug Tracker Seed Methods
            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc);
            await SeedDemoUsersAsync(userManagerSvc);
            await SeedDefaultDTaskTypeAsync(dbContextSvc);
            await SeedDefaultDTaskStatusAsync(dbContextSvc);
            await SeedDefaultDTaskPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefautProjectsAsync(dbContextSvc);
            await SeedDefautDTasksAsync(dbContextSvc);
        }


        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        public static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultcompanies = new List<Company>() {
                    new Company() { Name = "Demo Company", Description="This is the Demo Company" },
                    new Company() { Name = "Apple", Description="This is Apple" },
                    new Company() { Name = "Google", Description="This is Google" },
                    new Company() { Name = "Microsoft", Description="This is Microsoft" },
                    new Company() { Name = "IBM", Description="This is IBM" }
                };

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(defaultcompanies.Where(c => !dbCompanies.Contains(c.Name)));
                await context.SaveChangesAsync();

                //Get company Ids
                company1Id = context.Companies.FirstOrDefault(p => p.Name == "Demo Company").Id;
                company2Id = context.Companies.FirstOrDefault(p => p.Name == "Apple").Id;
                company3Id = context.Companies.FirstOrDefault(p => p.Name == "Google").Id;
                company4Id = context.Companies.FirstOrDefault(p => p.Name == "Microsoft").Id;
                company5Id = context.Companies.FirstOrDefault(p => p.Name == "IBM").Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Companies.");
                throw;
            }
        }

        public static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Models.ProjectPriority> projectPriorities = new List<ProjectPriority>() {
                                                    new ProjectPriority() { Name = DSProjectPriority.Low.ToString() },
                                                    new ProjectPriority() { Name = DSProjectPriority.Medium.ToString() },
                                                    new ProjectPriority() { Name = DSProjectPriority.High.ToString() },
                                                    new ProjectPriority() { Name = DSProjectPriority.Urgent.ToString() },
                };

                var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
                await context.ProjectPriorities.AddRangeAsync(projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Project Priorities.");
                throw;
            }
        }

        public static async Task SeedDefautProjectsAsync(ApplicationDbContext context)
        {

            //Get project priority Ids
            int priorityLow = context.ProjectPriorities.FirstOrDefault(p => p.Name == DSProjectPriority.Low.ToString()).Id;
            int priorityMedium = context.ProjectPriorities.FirstOrDefault(p => p.Name == DSProjectPriority.Medium.ToString()).Id;
            int priorityHigh = context.ProjectPriorities.FirstOrDefault(p => p.Name == DSProjectPriority.High.ToString()).Id;
            int priorityUrgent = context.ProjectPriorities.FirstOrDefault(p => p.Name == DSProjectPriority.Urgent.ToString()).Id;

            try
            {
                IList<Project> projects = new List<Project>() {
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Rewrite customer web applications",
                         Description="Description: Rewrite customer web applications" ,
                         StartDate = new DateTime(2022,2,20),
                         EndDate = new DateTime(2022,8,20),
                         ProjectPriorityId = priorityLow
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Update bank customer's API",
                         Description="Description: Update bank customer's API",
                         StartDate = new DateTime(2021,12,20),
                         EndDate = new DateTime(2022,8,2),
                         ProjectPriorityId = priorityMedium
                     },
                     new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Audit customer's application security",
                         Description="Description: Audit customer's application security",
                         StartDate = new DateTime(2022,2,20),
                         EndDate = new DateTime(2022,9,20),
                         ProjectPriorityId = priorityHigh
                     },
                     new Project()
                     {
                         CompanyId = company2Id,
                         Name = "Build a project management tool",
                         Description="Need to build project management tool",
                         StartDate = new DateTime(2022,1,20),
                         EndDate = new DateTime(2022,11,20),
                         ProjectPriorityId = priorityLow
                     },
                    new Project()
                     {
                         CompanyId = company1Id,
                         Name = "Migrate database for distribution center",
                         Description="We need database migrated for distribution center",
                         StartDate = new DateTime(2022,1,2),
                         EndDate = new DateTime(2022,5,20),
                         ProjectPriorityId = priorityHigh
                     }
                };

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(projects.Where(c => !dbProjects.Contains(c.Name)));
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Projects.");
                throw;
            }
        }



        public static async Task SeedDefaultUsersAsync(UserManager<DSUser> userManager)
        {
            //Seed Default Admin User
            var defaultUser = new DSUser
            {
                UserName = "admin1@dsana.com",
                Email = "danieljsalo@gmail.com",
                FirstName = "Daniel",
                LastName = "Salo",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Admin User.");
            }

            //Seed Default Admin User
            defaultUser = new DSUser
            {
                UserName = "admin2@dsana.com",
                Email = "djsalo1221@gmail.com",
                FirstName = "Daniel",
                LastName = "Salo2",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Admin User.");
                throw;
            }


            //Seed Default ProjectManager1 User
            defaultUser = new DSUser
            {
                UserName = "ProjectManager1@dsana.com",
                Email = "ProjectManager1@dsana.com",
                FirstName = "John",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default ProjectManager1 User.");
                throw;
            }


            //Seed Default ProjectManager2 User
            defaultUser = new DSUser
            {
                UserName = "ProjectManager2@dsana.com",
                Email = "ProjectManager2@dsana.com",
                FirstName = "Jane",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default ProjectManager2 User.");
                throw;
            }


            //Seed Default Developer1 User
            defaultUser = new DSUser
            {
                UserName = "Developer1@dsana.com",
                Email = "Developer1@dsana.com",
                FirstName = "Elon",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer1 User.");
                throw;
            }


            //Seed Default Developer2 User
            defaultUser = new DSUser
            {
                UserName = "Developer2@dsana.com",
                Email = "Developer2@dsana.com",
                FirstName = "James",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer2 User.");
                throw;
            }


            //Seed Default Developer3 User
            defaultUser = new DSUser
            {
                UserName = "Developer3@dsana.com",
                Email = "Developer3@dsana.com",
                FirstName = "Natasha",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer3 User.");
                throw;
            }


            //Seed Default Developer4 User
            defaultUser = new DSUser
            {
                UserName = "Developer4@dsana.com",
                Email = "Developer4@dsana.com",
                FirstName = "Carol",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer4 User.");
                throw;
            }


            //Seed Default Developer5 User
            defaultUser = new DSUser
            {
                UserName = "Developer5@dsana.com",
                Email = "Developer5@dsana.com",
                FirstName = "Tony",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer5 User.");
                throw;
            }

            //Seed Default Developer6 User
            defaultUser = new DSUser
            {
                UserName = "Developer6@dsana.com",
                Email = "Developer6@dsana.com",
                FirstName = "Bruce",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Developer5 User.");
                throw;
            }

            //Seed Default Submitter1 User
            defaultUser = new DSUser
            {
                UserName = "Submitter1@dsana.com",
                Email = "Submitter1@dsana.com",
                FirstName = "Scott",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                throw;
            }


            //Seed Default Submitter2 User
            defaultUser = new DSUser
            {
                UserName = "Submitter2@dsana.com",
                Email = "Submitter2@dsana.com",
                FirstName = "Sue",
                LastName = "Appuser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Default Submitter2 User.");
                throw;
            }

        }

        public static async Task SeedDemoUsersAsync(UserManager<DSUser> userManager)
        {
            //Seed Demo Admin User
            var defaultUser = new DSUser
            {
                UserName = "demoadmin@dsana.com",
                Email = "demoadmin@dsana.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyID = company1Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Demo Admin User.");
                throw;
            }


            //Seed Demo ProjectManager User
            defaultUser = new DSUser
            {
                UserName = "demopm@dsana.com",
                Email = "demopm@dsana.com",
                FirstName = "Demo",
                LastName = "ProjectManager",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Demo ProjectManager1 User.");
                throw;
            }


            //Seed Demo Developer User
            defaultUser = new DSUser
            {
                UserName = "demodev@dsana.com",
                Email = "demodev@dsana.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Demo Developer1 User.");
                throw;
            }


            //Seed Demo Submitter User
            defaultUser = new DSUser
            {
                UserName = "demosub@dsana.com",
                Email = "demosub@dsana.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Demo Submitter User.");
                throw;
            }


            //Seed Demo New User
            defaultUser = new DSUser
            {
                UserName = "demonew@dsana.com",
                Email = "demonew@dsana.com",
                FirstName = "Demo",
                LastName = "NewUser",
                EmailConfirmed = true,
                CompanyID = company2Id
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abc&123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Demo New User.");
                throw;
            }
        }



        public static async Task SeedDefaultDTaskTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<DTaskType> dtaskTypes = new List<DTaskType>() {
                     new DTaskType() { Name = DSDTaskType.NewDevelopment.ToString() },      
                     new DTaskType() { Name = DSDTaskType.WorkTask.ToString() },            
                     new DTaskType() { Name = DSDTaskType.Defect.ToString()},               
                     new DTaskType() { Name = DSDTaskType.ChangeRequest.ToString() },       
                     new DTaskType() { Name = DSDTaskType.Enhancement.ToString() },         
                     new DTaskType() { Name = DSDTaskType.GeneralTask.ToString() }          
                };

                var dbDTaskTypes = context.DTaskTypes.Select(c => c.Name).ToList();
                await context.DTaskTypes.AddRangeAsync(dtaskTypes.Where(c => !dbDTaskTypes.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding DTask Types.");
                throw;
            }
        }

        public static async Task SeedDefaultDTaskStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<DTaskStatus> dtaskStatuses = new List<DTaskStatus>() {
                    new DTaskStatus() { Name = DSDTaskStatus.New.ToString() },                 
                    new DTaskStatus() { Name = DSDTaskStatus.Development.ToString() },         
                    new DTaskStatus() { Name = DSDTaskStatus.Testing.ToString()  },            
                    new DTaskStatus() { Name = DSDTaskStatus.Resolved.ToString()  },
                    new DTaskStatus() { Name = DSDTaskStatus.Complete.ToString()  },           
                };

                var dbDTaskStatuses = context.DTaskStatuses.Select(c => c.Name).ToList();
                await context.DTaskStatuses.AddRangeAsync(dtaskStatuses.Where(c => !dbDTaskStatuses.Contains(c.Name)));
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding DTask Statuses.");
                throw;
            }
        }

        public static async Task SeedDefaultDTaskPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<DTaskPriority> dtaskPriorities = new List<DTaskPriority>() {
                                                    new DTaskPriority() { Name = DSDTaskPriority.Low.ToString()  },
                                                    new DTaskPriority() { Name = DSDTaskPriority.Medium.ToString() },
                                                    new DTaskPriority() { Name = DSDTaskPriority.High.ToString()},
                                                    new DTaskPriority() { Name = DSDTaskPriority.Urgent.ToString()},
                };

                var dbDTaskPriorities = context.DTaskPriorities.Select(c => c.Name).ToList();
                await context.DTaskPriorities.AddRangeAsync(dtaskPriorities.Where(c => !dbDTaskPriorities.Contains(c.Name)));
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding DTask Priorities.");
                throw;
            }
        }



        public static async Task SeedDefautDTasksAsync(ApplicationDbContext context)
        {
            //Get project Ids
            int webId = context.Projects.FirstOrDefault(p => p.Name == "Rewrite customer web applications").Id;
            int apiId = context.Projects.FirstOrDefault(p => p.Name == "Update bank customer's API").Id;
            int securityId = context.Projects.FirstOrDefault(p => p.Name == "Audit customer's application security").Id;
            int projmanId = context.Projects.FirstOrDefault(p => p.Name == "Build a project management tool").Id;

            //Get dtask type Ids
            int typeNewDev = context.DTaskTypes.FirstOrDefault(p => p.Name == DSDTaskType.NewDevelopment.ToString()).Id;
            int typeWorkTask = context.DTaskTypes.FirstOrDefault(p => p.Name == DSDTaskType.WorkTask.ToString()).Id;
            int typeDefect = context.DTaskTypes.FirstOrDefault(p => p.Name == DSDTaskType.Defect.ToString()).Id;
            int typeEnhancement = context.DTaskTypes.FirstOrDefault(p => p.Name == DSDTaskType.Enhancement.ToString()).Id;
            int typeChangeRequest = context.DTaskTypes.FirstOrDefault(p => p.Name == DSDTaskType.ChangeRequest.ToString()).Id;

            //Get dtask priority Ids
            int priorityLow = context.DTaskPriorities.FirstOrDefault(p => p.Name == DSDTaskPriority.Low.ToString()).Id;
            int priorityMedium = context.DTaskPriorities.FirstOrDefault(p => p.Name == DSDTaskPriority.Medium.ToString()).Id;
            int priorityHigh = context.DTaskPriorities.FirstOrDefault(p => p.Name == DSDTaskPriority.High.ToString()).Id;
            int priorityUrgent = context.DTaskPriorities.FirstOrDefault(p => p.Name == DSDTaskPriority.Urgent.ToString()).Id;

            //Get dtask status Ids
            int statusNew = context.DTaskStatuses.FirstOrDefault(p => p.Name == DSDTaskStatus.New.ToString()).Id;
            int statusDev = context.DTaskStatuses.FirstOrDefault(p => p.Name == DSDTaskStatus.Development.ToString()).Id;
            int statusTest = context.DTaskStatuses.FirstOrDefault(p => p.Name == DSDTaskStatus.Testing.ToString()).Id;
            int statusResolved = context.DTaskStatuses.FirstOrDefault(p => p.Name == DSDTaskStatus.Resolved.ToString()).Id;


            try
            {
                IList<DTask> dtasks = new List<DTask>() {
                                //Web
                                new DTask() {Title = "Web Task 1", Description = "Task details for Web task 1", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Web Task 2", Description = "Task details for Web task 2", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusNew, DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "Web Task 3", Description = "Task details for Web task 3", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusDev, DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "Web Task 4", Description = "Task details for Web task 4", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusTest, DTaskTypeId = typeDefect},
                                new DTask() {Title = "Web Task 5", Description = "Task details for Web task 5", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Web Task 6", Description = "Task details for Web task 6", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusNew, DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "Web Task 7", Description = "Task details for Web task 7", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusDev, DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "Web Task 8", Description = "Task details for Web task 8", Created = DateTimeOffset.Now, ProjectId = webId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusTest, DTaskTypeId = typeDefect},
                                //API
                                new DTask() {Title = "API Task 1", Description = "Task details for API dtask 1", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew, DTaskTypeId = typeDefect},
                                new DTask() {Title = "API Task 2", Description = "Task details for API dtask 2", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusDev, DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "API Task 3", Description = "Task details for API dtask 3", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "API Task 4", Description = "Task details for API dtask 4", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "API Task 5", Description = "Task details for API dtask 5", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityLow, DTaskStatusId = statusDev,  DTaskTypeId = typeDefect},
                                new DTask() {Title = "API Task 6", Description = "Task details for API dtask 6", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusNew,  DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "API Task 7", Description = "Task details for API dtask 7", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "API Task 8", Description = "Task details for API dtask 8", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusDev,  DTaskTypeId = typeNewDev},
                                new DTask() {Title = "API Task 9", Description = "Task details for API dtask 9", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew,  DTaskTypeId = typeDefect},
                                new DTask() {Title = "API Task 10", Description = "Task details for API dtask 10", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusNew, DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "API Task 11", Description = "Task details for API dtask 11", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusDev,  DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "API Task 12", Description = "Task details for API dtask 12", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusNew,  DTaskTypeId = typeNewDev},
                                new DTask() {Title = "API Task 13", Description = "Task details for API dtask 13", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew, DTaskTypeId = typeDefect},
                                new DTask() {Title = "API Task 14", Description = "Task details for API dtask 14", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusDev,  DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "API Task 15", Description = "Task details for API dtask 15", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew,  DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "API Task 16", Description = "Task details for API dtask 16", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "API Task 17", Description = "Task details for API dtask 17", Created = DateTimeOffset.Now, ProjectId = apiId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusDev,  DTaskTypeId = typeNewDev},
                                //dsana                                                                                                                         
                                new DTask() {Title = "Security Task 1", Description = "Task details for Security Task 1", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 2", Description = "Task details for Security Task 2", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 3", Description = "Task details for Security Task 3", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 4", Description = "Task details for Security Task 4", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 5", Description = "Task details for Security Task 5", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 6", Description = "Task details for Security Task 6", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 7", Description = "Task details for Security Task 7", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 8", Description = "Task details for Security Task 8", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 9", Description = "Task details for Security Task 9", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Security Task 10", Description = "Task details for Security 10", Created = DateTimeOffset.Now, ProjectId = securityId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                //MOVIE
                                new DTask() {Title = "Project Management Tool Task 1", Description = "Task details for Project Management Tool Task 1", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityLow, DTaskStatusId = statusNew, DTaskTypeId = typeDefect},
                                new DTask() {Title = "Project Management Tool Task 2", Description = "Task details for Project Management Tool Task 2", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusDev, DTaskTypeId = typeEnhancement},
                                new DTask() {Title = "Project Management Tool Task 3", Description = "Task details for Project Management Tool Task 3", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityHigh, DTaskStatusId = statusNew, DTaskTypeId = typeChangeRequest},
                                new DTask() {Title = "Project Management Tool Task 4", Description = "Task details for Project Management Tool Task 4", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityUrgent, DTaskStatusId = statusNew, DTaskTypeId = typeNewDev},
                                new DTask() {Title = "Project Management Tool Task 5", Description = "Task details for Project Management Tool Task 5", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityLow, DTaskStatusId = statusDev,  DTaskTypeId = typeDefect},
                                new DTask() {Title = "Project Management Tool Task 6", Description = "Task details for Project Management Tool Task 6", Created = DateTimeOffset.Now, ProjectId = projmanId, DTaskPriorityId = priorityMedium, DTaskStatusId = statusNew,  DTaskTypeId = typeEnhancement},

                };


                var dbDTasks = context.DTasks.Select(c => c.Title).ToList();
                await context.DTasks.AddRangeAsync(dtasks.Where(c => !dbDTasks.Contains(c.Title)));
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Seeding Tasks.");
                throw;
            }
        }
    }
}



//postgres://qrtxbduqzzkddk:3af0288475b008d9039cf8db104570bdb05d183ef14fadf9ca12411fe465d990@ec2-34-239-33-57.compute-1.amazonaws.com:5432/d18qpjs9dake96