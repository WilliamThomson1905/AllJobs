using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AllJobs.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        // A DbSet represents the collection of all entities in the context, or that can be queried from the database, of a given type.
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<JobHistory> PreviousJobs { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<CV> Cvs { get; set; }
        //public DbSet<InterviewStatus> InterviewStatus { get; set; }

        public DbSet<LocationCategory> Locations { get; set; }
        public DbSet<JobSectorCategory> JobSectors { get; set; }
        public DbSet<JobTypeCategory> JobTypes { get; set; }

      


        public ApplicationDbContext() : base("AllJobsDb", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());



        }

        public static ApplicationDbContext Create()
        {
            
            return new ApplicationDbContext();
        }

    }//end of class ApplicationDbContext

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // If there isn't a context for users then seed then seed new users.
            // Will create admin, staff, recruiter and applicant accounts. 
            // Each of which have varying privileges.
            if (!context.Users.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var userStore = new UserStore<ApplicationUser>(context);

                //_____________________________________________________________________
                //populating the Role table - Administrator
                if (!roleManager.RoleExists(RoleNames.ROLE_ADMINISTRATOR))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_ADMINISTRATOR));
                }


                if (!roleManager.RoleExists(RoleNames.ROLE_SUSPENDED))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_SUSPENDED));
                }


                //populating the Role table - AllJobs Staff - Manager, HR, IT, Recruitment Consultant
                if (!roleManager.RoleExists(RoleNames.ROLE_HR))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_HR));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_IT))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_IT));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_MANAGER))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_MANAGER));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_RECRUITMENT_CONSULTANT))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_RECRUITMENT_CONSULTANT));
                }





                //populating the Role table - Recruiter and Applicant
                if (!roleManager.RoleExists(RoleNames.ROLE_RECRUITER))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_RECRUITER));
                }

                if (!roleManager.RoleExists(RoleNames.ROLE_APPLICANT))
                {
                    var roleresult = roleManager.Create(new IdentityRole(RoleNames.ROLE_APPLICANT));
                }







                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////
                /// ------ Creating Initial Job Sector Category Instances for Seeded Advertisements ----- ///

                // All Advertisements will need 1 or More Job Sector Categories: Education, Accountancy, Editorial, etc...
                // It is also best to seed all these Categories when the DB is initiallt created, and then the option for Admin to create new Sector options.
                context.JobTypes.AddRange(new List<JobTypeCategory>()
                {
                    new JobTypeCategory() { JobTypeName = "Part-Time" },
                    new JobTypeCategory() { JobTypeName = "Full-Time" },
                    new JobTypeCategory() { JobTypeName = "Apprenticeship" },
                    new JobTypeCategory() { JobTypeName = "Internship" },
                    new JobTypeCategory() { JobTypeName = "Commission" },
                    new JobTypeCategory() { JobTypeName = "Volunteer" },
                    new JobTypeCategory() { JobTypeName = "Temporary" },
                    new JobTypeCategory() { JobTypeName = "Contract" },
                    new JobTypeCategory() { JobTypeName = "Permanent" }



                });

                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////
                /// ------ Creating Initial Job Sector Category Instances for Seeded Advertisements ----- ///

                // All Advertisements will need 1 or More Job Sector Categories: Education, Accountancy, Editorial, etc...
                // It is also best to seed all these Categories when the DB is initiallt created, and then the option for Admin to create new Sector options.
                context.JobSectors.AddRange(new List<JobSectorCategory>()
                {
                    new JobSectorCategory() { SectorName = "Education" }, // 1
                    new JobSectorCategory() { SectorName = "Accountancy" },  // 2
                    new JobSectorCategory() { SectorName = "Health and Medicine" },
                    new JobSectorCategory() { SectorName = "Recruitment Consultancy" },
                    new JobSectorCategory() { SectorName = "Arts" },
                    new JobSectorCategory() { SectorName = "IT" },
                    new JobSectorCategory() { SectorName = "Sales" },
                    new JobSectorCategory() { SectorName = "Marketing" },
                    new JobSectorCategory() { SectorName = "Engineering" },
                    new JobSectorCategory() { SectorName = "Gaming" }, // 10
                    new JobSectorCategory() { SectorName = "Editorial" },
                    new JobSectorCategory() { SectorName = "Retail" },
                    new JobSectorCategory() { SectorName = "Media" },
                    new JobSectorCategory() { SectorName = "Motoring" },
                    new JobSectorCategory() { SectorName = "Graphic Design" }, // 15
                    new JobSectorCategory() { SectorName = "Animation" },
                    new JobSectorCategory() { SectorName = "Fashion" },
                    new JobSectorCategory() { SectorName = "Publishing" },
                    new JobSectorCategory() { SectorName = "Photography" },
                    new JobSectorCategory() { SectorName = "Journalism" },
                    new JobSectorCategory() { SectorName = "Administration" },
                    new JobSectorCategory() { SectorName = "Agriculture" },
                    new JobSectorCategory() { SectorName = "Architecture" },
                    new JobSectorCategory() { SectorName = "Banking" },
                    new JobSectorCategory() { SectorName = "Communications" },
                    new JobSectorCategory() { SectorName = "Community" },
                    new JobSectorCategory() { SectorName = "Hospitality" },
                    new JobSectorCategory() { SectorName = "Insurance" },
                    new JobSectorCategory() { SectorName = "Sport" },
                    new JobSectorCategory() { SectorName = "Transport" },
                    new JobSectorCategory() { SectorName = "Mining" },
                    new JobSectorCategory() { SectorName = "Web Design" },

                    new JobSectorCategory() { SectorName = "Other" }


                });


                ////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////
                /// ------ Creating Initial Location Category Instances for Seeded Advertisements ----- ///

                // All Advertisements will need a Location Category: Glasgow, Fyfe, Edinburgh, etc...
                // It is also best to seed all these Categories when the DB is initiallt created, and then the option for Admin to create new Location options.
                context.Locations.AddRange(new List<LocationCategory>()
                {
                    new LocationCategory() { LocationName = "Glasgow" },
                    new LocationCategory() { LocationName = "Fyfe" },
                    new LocationCategory() { LocationName = "Edinburgh" },
                    new LocationCategory() { LocationName = "Aberdeen" },
                    new LocationCategory() { LocationName = "London" },
                    new LocationCategory() { LocationName = "England" },
                    new LocationCategory() { LocationName = "Hampshire" },
                    new LocationCategory() { LocationName = "Leeds" },
                    new LocationCategory() { LocationName = "Liverpool" },
                    new LocationCategory() { LocationName = "Lanarkshire" },
                    new LocationCategory() { LocationName = "Livingston" },
                    new LocationCategory() { LocationName = "Scotland" },
                    new LocationCategory() { LocationName = "Angus" },
                    new LocationCategory() { LocationName = "Mersey" },
                    new LocationCategory() { LocationName = "Midlothian" }

                });



             








                /////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////




                //_________________________________________________________________
                //_________________________________________________________________

                string userName = "admin@admin.com";
                string password = "Password#1";

                //  var passwordHash = new PasswordHasher();
                //  password = passwordHash.HashPassword(password);

                //create Admin user and role

                var user = userManager.FindByName(userName);
                if (user == null)
                {
                    var newUser = new Staff()
                    {
                        Forename = "Robert",
                        Surname = "Lilt",
                        DOB = new DateTime(1964, 7, 9),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920828",
                        Mobile = "07418920828",
                        UserName = userName,
                        Email = userName,
                        EmailConfirmed = true



                    };


                    //userManager.Create(newUser, password);
                    //userManager.AddToRole(newUser, RoleNames.ROLE_ADMINISTRATOR);
                    userManager.Create(newUser, password);
                    //userManager.SetLockoutEnabled(newUser.Id, true);
                    userManager.AddToRole(newUser.Id, RoleNames.ROLE_ADMINISTRATOR);
                }
                //___________________________________________________________________________________
                // Create Second Admin Role

                string userNameTwo = "admin2@admin.com";
                string passwordTwo = "Password#1";

                //  var passwordHash = new PasswordHasher();
                //  password = passwordHash.HashPassword(password);

                //create Admin user and role

                var userTwo = userManager.FindByName(userNameTwo);
                if (userTwo == null)
                {

                    var newUserTwo = new Staff()
                    {
                        Forename = "Violet",
                        Surname = "Malcolm",
                        DOB = new DateTime(1959, 3, 23),
                        Title = "Mrs",
                        Gender = 'F',
                        PhoneNumber = "01418920828",
                        Mobile = "07418920828",
                        UserName = userNameTwo,
                        Email = userNameTwo,
                        EmailConfirmed = true,
                        

                    };

                    context.Advertisements.Add(new Advertisement()
                    { //AdvertisementId = "3", 

                        //AdvertisementId = 3,
                        JobTitle = "Web Designer",
                        AvailableFrom = DateTime.Now,
                        ClosingDate = DateTime.Now.AddHours(3),
                        ShortDescription = "We are seeking a talented UI/UX Front End Web Developer to design, develop, support web app software." +
                        "UI/UX Front-End Web Developer...",
                        Description = "Making websites is like making bacon.",
                        JobResponsibilites = "Develop new user-facing features Build reusable code and libraries for future use\n" +
                                            "Ensure the technical feasibility of UI / UX designs\n" +
                                            "Optimize application for maximum speed and scalability\n" +
                                            "Assure that all user input is validated before submitting to back - end\n" +
                                            "Collaborate with other team members and stakeholders\n" +
                                            "Responds to, researches, and resolves on - going client inquiries.\n" +
                                            "Writes functional requirements for systems in standard template format.\n" +
                                            "Interacts with project sponsor(s), internal team members, client, and vendor project team members to develop and maintain project management documents\n" +
                                            "Strives to manage stakeholder expectations via planning and work outcomes.\n" +
                                            "Other duties as assigned.\n",

                        QualificationRequired = "Proficient understanding of web markup, including HTML5, CSS3 \n" +
                                                "Basic understanding of server-side CSS pre-processing platforms, such as LESS and SASS\n" +
                                                "Proficient understanding of client-side scripting and JavaScript frameworks, including jQuery\n" +
                                                "Proficient understanding of cross-browser compatibility issues and ways to work around them.\n" +
                                                "Proficient understanding of code versioning tools, such as {{Git / Mercurial / SVN}}\n",

                        TechnicalExperience = "Bachelor's degree or higher in a related discipline (preferred but not required\n" +
                                                "Minimum 2 + years in a front - end development position\n",
                        JobTypeCategoryId = 1,
                        JobSectorCategoryId = 1,
                        CompanyName = "Snippets Design",
                        LocationCategoryId = 3,
                        MinSalary = 18000,
                        MaxSalary = 28000,
                        //CVRequired = true,
                        CoverLetterRequired = true,
                        EmployerId = newUserTwo.Id,
                        Employer = newUserTwo


                    });


                    //userManager.Create(newUser, password);
                    //userManager.AddToRole(newUser, RoleNames.ROLE_ADMINISTRATOR);
                    userManager.Create(newUserTwo, passwordTwo);
                    //userManager.SetLockoutEnabled(newUser.Id, false);
                    userManager.AddToRole(newUserTwo.Id, RoleNames.ROLE_ADMINISTRATOR);
                }

                //__________________________________________________________________

                // Create a new Recruiter User

                string userName1 = "RBetfred@gmail.com";
                string password1 = "Password_1";

                //create Staff user and role
                var user1 = userManager.FindByName(userName1);
                if (user1 == null)
                {
                    var newUser1 = new Recruiter()
                    {
                        Forename = "Robert",
                        Surname = "Pitch",
                        DOB = new DateTime(1981, 12, 3),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920001",
                        Mobile = "07418920001",
                        UserName = userName1,
                        Email = userName1,
                        EmailConfirmed = true,
                        CompanyName = "Google"
                    };

                    
                    context.Advertisements.AddRange(new List<Advertisement>()
                    {
                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Sales Associate",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(7),
                                JobTypeCategoryId = 3,
                                CompanyName = "THOMAS SABO",
                                LocationCategoryId = 1,
                                JobSectorCategoryId = 1,
                                ShortDescription = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, " +
                                "selling and distributing lifestyle products for women an...",


                                Description = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, selling and distributing lifestyle products for women and men.Our driving forces are a love of fashion and a fascination for creating innovative, highly-expressive accessories.We are looking for a passionate, sales-driven brand ambassador for our HOF Glasgow Concession.",

                                JobResponsibilites = "Minimum of one year premium retail experience," +
                                                     "Professional selling skills and exceptional interpersonal skills," +
                                                     "Prior LUXURY retail experience preferred," +
                                                     "Proactive ability to multi-task and prioritise," +
                                                     "Works well in a team environment,\n",

                                QualificationRequired = "Premium Retail: 1 year\n",

                                TechnicalExperience = "If you love retail, you are enthusiastic about being on the shop floor and you enjoy the thrill you get from selling, please apply today." +
                                                      "We especially want to hear from you if you have previous PREMIUM retail experience and a proven sales track record and the demonstrated ability to meet and exceed your personal sales KPI's." +
                                                      "Our generous reward package is designed to value those who want to make a real difference to the business they are in.\n",



                                MinSalary = 11000,
                                MaxSalary = 18000,
                                //CVRequired = true,
                                EmployerId = newUser1.Id,
                                Employer = newUser1

                            },
                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Core driller/ concrete repair specialist ",
                                AvailableFrom = DateTime.Now.AddYears(-1),
                                ClosingDate = DateTime.Now.AddDays(17),
                                JobTypeCategoryId = 4,
                                CompanyName = "Architectural Coatings limited",
                                LocationCategoryId = 1,
                                JobSectorCategoryId = 17,

                                ShortDescription = "FPSG's client is a well established Scottish Accountancy & Finance organisation who is looking to grow its payroll team....",

                                Description = "Experienced core driller required for contracts in Glasgow and throughout the central belt.",

                                TechnicalExperience = "Experience with Brokks, wire saws and wall saw preferable. Ipaf, Pasma and full uk driving licence essential.",

                                OtherDetails  = "Please no time wasters, if you feel you cant turn up to the site induction without the Managing director holding your hand then this is not the job for you.",

                                MinSalary = 8000,
                                MaxSalary = 12000,
                                EmployerId = newUser1.Id,
                                Employer = newUser1

                            },new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Associate",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(7),
                                JobTypeCategoryId = 3,
                                CompanyName = "THOMAS SABO",
                                LocationCategoryId = 1,
                                JobSectorCategoryId = 1,
                                ShortDescription = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, " +
                                "selling and distributing lifestyle products for women an...",


                                Description = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, selling and distributing lifestyle products for women and men.Our driving forces are a love of fashion and a fascination for creating innovative, highly-expressive accessories.We are looking for a passionate, sales-driven brand ambassador for our HOF Glasgow Concession.",

                                JobResponsibilites = "Minimum of one year premium retail experience," +
                                                     "Professional selling skills and exceptional interpersonal skills," +
                                                     "Prior LUXURY retail experience preferred," +
                                                     "Proactive ability to multi-task and prioritise," +
                                                     "Works well in a team environment,\n",

                                TechnicalExperience = "If you love retail, you are enthusiastic about being on the shop floor and you enjoy the thrill you get from selling, please apply today." +
                                                      "We especially want to hear from you if you have previous PREMIUM retail experience and a proven sales track record and the demonstrated ability to meet and exceed your personal sales KPI's." +
                                                      "Our generous reward package is designed to value those who want to make a real difference to the business they are in.\n",



                                MinSalary = 11000,
                                MaxSalary = 40000,
                                //CVRequired = true,
                                EmployerId = newUser1.Id,
                                Employer = newUser1

                            }
                            ,

                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Banana King",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(7),
                                JobTypeCategoryId = 3,
                                CompanyName = "Tooty Fruity",
                                LocationCategoryId = 8,
                                JobSectorCategoryId = 15,
                                ShortDescription = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, " +
                                "selling and distributing lifestyle products for women an...",


                                Description = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, selling and distributing lifestyle products for women and men.Our driving forces are a love of fashion and a fascination for creating innovative, highly-expressive accessories.We are looking for a passionate, sales-driven brand ambassador for our HOF Glasgow Concession.",

                                JobResponsibilites = "Minimum of one year premium retail experience," +
                                                     "Professional selling skills and exceptional interpersonal skills," +
                                                     "Prior LUXURY retail experience preferred," +
                                                     "Proactive ability to multi-task and prioritise," +
                                                     "Works well in a team environment,\n",

                                TechnicalExperience = "If you love retail, you are enthusiastic about being on the shop floor and you enjoy the thrill you get from selling, please apply today." +
                                                      "We especially want to hear from you if you have previous PREMIUM retail experience and a proven sales track record and the demonstrated ability to meet and exceed your personal sales KPI's." +
                                                      "Our generous reward package is designed to value those who want to make a real difference to the business they are in.\n",



                                MinSalary = 61000,
                                MaxSalary = 80000,
                                //CVRequired = true,
                                EmployerId = newUser1.Id,
                                Employer = newUser1

                            }
                            ,new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Blah",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(7),
                                JobTypeCategoryId = 6,
                                CompanyName = "THOMAS SABO",
                                LocationCategoryId = 11,
                                JobSectorCategoryId = 12,
                                ShortDescription = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, " +
                                "selling and distributing lifestyle products for women an...",


                                Description = "THOMAS SABO is one of the globally-leading jewellery, watches and beauty companies, designing, selling and distributing lifestyle products for women and men.Our driving forces are a love of fashion and a fascination for creating innovative, highly-expressive accessories.We are looking for a passionate, sales-driven brand ambassador for our HOF Glasgow Concession.",

                                JobResponsibilites = "Minimum of one year premium retail experience," +
                                                     "Professional selling skills and exceptional interpersonal skills," +
                                                     "Prior LUXURY retail experience preferred," +
                                                     "Proactive ability to multi-task and prioritise," +
                                                     "Works well in a team environment,\n",

                                TechnicalExperience = "If you love retail, you are enthusiastic about being on the shop floor and you enjoy the thrill you get from selling, please apply today." +
                                                      "We especially want to hear from you if you have previous PREMIUM retail experience and a proven sales track record and the demonstrated ability to meet and exceed your personal sales KPI's." +
                                                      "Our generous reward package is designed to value those who want to make a real difference to the business they are in.\n",



                                MinSalary = 21000,
                                MaxSalary = 40000,
                                //CVRequired = true,
                                EmployerId = newUser1.Id,
                                Employer = newUser1

                            },
                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Law Accountant ",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(3),
                                JobTypeCategoryId = 3,
                                CompanyName = "HBJ Claim Solutions",
                                LocationCategoryId = 1,
                                JobSectorCategoryId = 1,
                                ShortDescription = "HBJ Claim Solutions LLP is one of Scotland’s foremost personal injury firms processing several thousand... ",


                                Description = "HBJ Claim Solutions LLP is one of Scotland’s foremost personal injury firms processing several thousand claims a year. We pride ourselves on a first class, cost-effective claims handling legal service for motor insurers, legal expenses insurers, insurance brokers and individual clients.",

                                JobResponsibilites = "Minimum of one year premium retail experience," +
                                                     "Professional selling skills and exceptional interpersonal skills," +
                                                     "Prior LUXURY retail experience preferred," +
                                                     "Proactive ability to multi-task and prioritise," +
                                                     "Works well in a team environment,\n",

                                QualificationRequired = "Strong Negotiating skills" +
                                                        "Ability to challenge current ways of working" +
                                                        "Ability to work under pressure" +
                                                        "Self-motivation" +
                                                        "Ability to prioritise workload and to use initiative" +
                                                        "Willingness to adapt approach where required" +
                                                        "Attention to detail and accuracy" +
                                                        "Excellent interpersonal skills" +
                                                        "Law Accountancy: 2 years",

                                TechnicalExperience = "Experienced Law Accountant, ideally SOLAS qualified" +
                                                        "Computer literacy including proficiency in using MS Office particularly with Excel and Word",
                                                     

                                MinSalary = 12000,
                                MaxSalary = 18000,

                                EmployerId = newUser1.Id,
                                Employer = newUser1,
                                CoverLetterRequired = true

                            },
                            new Advertisement
                            {
                                JobTitle = "Payroll Administrator",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(4),
                                JobTypeCategoryId = 2,
                                JobSectorCategoryId = 2,
                                IsPublic = true,
                                CompanyName = "FPSG",
                                LocationCategoryId = 2,

                                ShortDescription = "FPSG's client is a well established Scottish Accountancy & Finance organisation who is looking to grow its payroll team....",


                                Description = "FPSG's client is a well established Scottish Accountancy & Finance organisation who is looking to grow its payroll team. This is due to restructure, and provides a genuine opportunity to join a successful team within an organisation that genuinely values staff contribution to its continued success.",

                                JobResponsibilites ="Processing multiple payrolls for a variety of clients - This includes weekly, fortnightly, 4 weekly, monthly and annually" +
                                                    "Be first point of contact for any client queries, providing information and technical support" +
                                                    "Accurately managing high volumes of work within time deadlines" +
                                                    "Liaising confidently with clients and senior internal stakeholders" +
                                                    "Make positive contributions to the wider team\n",


                                TechnicalExperience = "Experience gained within a similar role, preferably within a CA firm or Bureau" +
                                                      "Knowledge of PAYE, NIC, SSP, SMP, Pension Schemes and CIS returns" +
                                                      "Working knowledge of Auto Enrolment" +
                                                      "Advanced knowledge of SAGE Payroll and Excel - Essential" +
                                                      "Knowledge of RTI and Tax Year End processes" +
                                                      "Ability to deal effectively with payroll enquires from clients and liaise confidently with HMRC" +
                                                      "The flexibility to carry our ad hoc Payroll administration duties" +
                                                      "A positive, proactive attitude with first class attention to detail\n",




                                MinSalary = 26000,
                                MaxSalary = 32000,
                                EmployerId = newUser1.Id,
                                Employer = newUser1,
                                CoverLetterRequired = true
                            }

                    });

                    userManager.Create(newUser1, password1);
                    userManager.AddToRole(newUser1.Id, RoleNames.ROLE_RECRUITER);
                }
                //______________________________________________________

                //////////////////////////
                // Create a new Recruiter User

                string userName2 = "JGreenLeaf@test.com";
                string password2 = "Password_1";

                //create Staff user and role
                var user2 = userManager.FindByName(userName2);
                if (user2 == null)
                {
                    var newUser2 = new Recruiter()
                    {

                        Forename = "John",
                        Surname = "GreenLeaf",
                        DOB = new DateTime(1983, 9, 23),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920002",
                        Mobile = "07418920002",
                        UserName = userName2,
                        Email = userName2,
                        EmailConfirmed = true,
                        CompanyName = "FTSP"
                    };
                    userManager.Create(newUser2, password2);
                    userManager.AddToRole(newUser2.Id, RoleNames.ROLE_RECRUITER);
                }

                //////////////////////////
                // Create a new Recruiter User

                string userName3 = "HTachi@test.com";
                string password3 = "Password_1";

                //create Staff user and role
                var user3 = userManager.FindByName(userName3);
                if (user3 == null)
                {
                    var newUser3 = new Recruiter()
                    {

                        Forename = "Helen",
                        Surname = "Tachi",
                        DOB = new DateTime(1984, 8, 20),
                        Title = "Ms",
                        Gender = 'F',
                        PhoneNumber = "01418920003",
                        Mobile = "07418920003",
                        UserName = userName3,
                        Email = userName3,
                        EmailConfirmed = true,
                        CompanyName = "Apple"
                    };




                    userManager.Create(newUser3, password3);
                    userManager.AddToRole(newUser3.Id, RoleNames.ROLE_RECRUITER);
                }

                //////////////////////////
                // Create a new Manager User

                string userName4 = "GGates@test.com";
                string password4 = "Password_1";

                //create DogWalker user and role
                var user4 = userManager.FindByName(userName4);
                if (user4 == null)
                {
                    var newUser4 = new Staff()
                    {

                        Forename = "Garry",
                        Surname = "Gates",
                        DOB = new DateTime(1989, 2, 17),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920004",
                        Mobile = "071418920004",
                        UserName = userName4,
                        Email = userName4,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser4, password4);
                    userManager.AddToRole(newUser4.Id, RoleNames.ROLE_MANAGER);
                }



                //////////////////////////
                // Create a new RECRUITMENT CONSULTANT User

                string userName5 = "DButcher@test.com";
                string password5 = "Password_1";

                //create DogWalker user and role
                var user5 = userManager.FindByName(userName5);
                if (user5 == null)
                {
                    var newUser5 = new Staff()
                    {
                        Forename = "Douglas",
                        Surname = "Butcher",
                        DOB = new DateTime(1991, 3, 26),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920005",
                        Mobile = "071418920005",
                        UserName = userName5,
                        Email = userName5,
                        EmailConfirmed = true,

                    };
                    userManager.Create(newUser5, password5);
                    userManager.AddToRole(newUser5.Id, RoleNames.ROLE_RECRUITMENT_CONSULTANT);
                }


                //////////////////////////
                // Create a new IT staff User

                string userName6 = "JWalker@test.com";
                string password6 = "Password_1";

                //create Staff user and role
                var user6 = userManager.FindByName(userName6);
                if (user6 == null)
                {
                    var newUser6 = new Staff()
                    {


                        Forename = "James",
                        Surname = "Walker",
                        DOB = new DateTime(1993, 4, 23),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920006",
                        Mobile = "071418920006",
                        UserName = userName6,
                        Email = userName6,
                        EmailConfirmed = true,

                    };
                    userManager.Create(newUser6, password6);
                    userManager.AddToRole(newUser6.Id, RoleNames.ROLE_IT);
                }


                //////////////////////////
                // Create a new Applicant User

                string userName7 = "WWallace@test.com";
                string password7 = "Password_1";

                //create User user and role
                var user7 = userManager.FindByName(userName7);
                if (user7 == null)
                {
                    var newUser7 = new Applicant()
                    {
                        Forename = "William",
                        Surname = "Wallace",
                        DOB = new DateTime(1994, 9, 24),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01418920007",
                        Mobile = "071418920007",
                        Address = "7 Happy Street",
                        City = "Glasgow",
                        PostCode = "G53 7QW",
                        UserName = userName7,
                        Email = userName7,
                        EmailConfirmed = true,
                       
                    };




                    context.Qualifications.AddRange(new List<Qualification>()
                    {
                        new Qualification
                        {
                            InstitutionName = "Glasgow Clyde College",
                            QualificationName = "HND",
                            Grade = Grade.B,
                            Level = "bb",
                            StartDate = DateTime.Now.AddYears(-5).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-4).AddMonths(-4),
                            Id = newUser7.Id,
                            Applicant = newUser7
                        },
                        new Qualification
                        {
                            InstitutionName = "City of Glasgow College",
                            QualificationName = "Degree",
                            Grade = Grade.C,
                            Level = "bb",
                            StartDate = DateTime.Now.AddYears(-7).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-6).AddMonths(-4),
                            Id = newUser7.Id,
                            Applicant = newUser7
                        },
                        new Qualification
                        {
                            InstitutionName = "Langside College",
                            QualificationName = "HND",
                            Grade = Grade.A,
                            Level = "bb",
                            StartDate = DateTime.Now.AddYears(-10).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-9).AddMonths(-4),
                            Id = newUser7.Id,
                            Applicant = newUser7
                        },
                        new Qualification
                        {
                            InstitutionName = "Glasgow Caledonian University",
                            QualificationName = "Degree",
                            Level = "bb",
                            Grade = Grade.A,
                            StartDate = DateTime.Now.AddYears(-2).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-1).AddMonths(-4),
                            Id = newUser7.Id,
                            Applicant = newUser7,

                        }


                   });


                    userManager.Create(newUser7, password7);
                    userManager.AddToRole(newUser7.Id, RoleNames.ROLE_APPLICANT);
                }


                // Create a new Applicant User and Suspended

                string userName8 = "EReid@test.com";
                string password8 = "Password_1";

                //create User user and role
                var user8 = userManager.FindByName(userName8);
                if (user8 == null)
                {
                    var newUser8 = new Applicant()
                    {
                        Forename = "Elizabeth",
                        Surname = "Reid",
                        DOB = new DateTime(1995, 6, 9),
                        Address = "8 Happy Street",
                        Title = "Ms",
                        Gender = 'F',
                        //City = "Glasgow",
                        PostCode = "G53 8QW",
                        PhoneNumber = "01419920008",
                        Mobile = "07419920008",
                        UserName = userName8,
                        Email = userName8,
                        EmailConfirmed = true,
                  

                    };
                    userManager.Create(newUser8, password8);

                    userManager.AddToRole(newUser8.Id, RoleNames.ROLE_APPLICANT);
                    userManager.AddToRole(newUser8.Id, RoleNames.ROLE_SUSPENDED);

                }




                // Create a new Applicant User and Suspended

                string userName9 = "AEastcroft@test.com";
                string password9 = "Password_1";

                //create User user and role
                var user9 = userManager.FindByName(userName9);
                if (user9 == null)
                {
                    var newUser9 = new Applicant()
                    {
                        Forename = "Alex",
                        Surname = "Eastcroft",
                        DOB = new DateTime(1964, 1, 26),
                        Address = "9 Happy Street",
                        Title = "Mr",
                        Gender = 'M',
                        City = "Glasgow",
                        PostCode = "G53 9QW",
                        PhoneNumber = "01419920009",
                        Mobile = "07419920009",
                        UserName = userName9,
                        Email = userName9,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser9, password9);

                    userManager.AddToRole(newUser9.Id, RoleNames.ROLE_APPLICANT);
                }



                // Create a new Applicant User and Suspended

                string userName10 = "GWilson@admin.com";
                string password10 = "Password_1";

                //create User user and role
                var user10 = userManager.FindByName(userName10);
                if (user10 == null)
                {
                    var newUser10 = new Staff()
                    {
                        Forename = "Gavin",
                        Surname = "Wilson",
                        DOB = new DateTime(1984, 3, 26),
                        //Address = "10 Happy Street",
                        Title = "Mr",
                        Gender = 'M',
                        //City = "Glasgow",
                        //PostCode = "G51 0QW",
                        PhoneNumber = "01419920010",
                        Mobile = "07419920010",
                        UserName = userName10,
                        Email = userName10,
                        EmailConfirmed = true,

                    };



                    context.Advertisements.AddRange(new List<Advertisement>()
                    {
                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Vehicle Technician",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(7),
                                JobTypeCategoryId = 7,
                                CompanyName = "Kwik Fit",
                                LocationCategoryId = 1,
                                JobSectorCategoryId = 4,
                                ShortDescription = "We are currently looking for an experienced Car Mechanic / Vehicle Technician / MOT tester to join our busy " +
                                "Clydebank Autocentre on a full-time basis...",


                                Description = "We are currently looking for an experienced Car Mechanic / Vehicle Technician / MOT tester to join our busy Clydebank Autocentre on a full-time basis." +
                                              "The desired candidate will possess the skills and experience to carry out all aspects of light vehicle maintenance including light and heavier engine work.",

                                JobResponsibilites = "The ideal Car Mechanic / Vehicle Technician / MOT Tester will possess: " +
                                                     "A Full UK driving licence with no more than 9 points(you will be subject to licence checks)" +
                                                     "Experience in a professional workshop environment" +
                                                     "A good approach to customer service" +
                                                     "Flexibility towards working hours(40-48 hours per week)" +
                                                     "Halfords Autocentres strive to be there for all of life's journeys, in order to deliver this some of our Autocentres are open 6/7 days a week.\n",

                                //QualificationRequired = "Premium Retail: 1 year\n",

                                TechnicalExperience = "NVQ Level 3 or Equivalent / Time Served Experience/n" +
                                                      "A valid class 4 and 7 MOT testing licence",



                                MinSalary = 18000,
                                MaxSalary = 23000,
                                EmployerId = newUser10.Id,
                                Employer = newUser10

                            },
                            new Advertisement
                            {
                                //AdvertisementId = 1,
                                JobTitle = "Cleaner",
                                AvailableFrom = DateTime.Now,
                                ClosingDate = DateTime.Now.AddDays(17),
                                JobTypeCategoryId = 4,
                                CompanyName = "Kwik Fit",
                                LocationCategoryId = 6,
                                JobSectorCategoryId = 6,
                                ShortDescription = "We are currently looking for an experienced Car Mechanic / Vehicle Technician / MOT tester to join our busy " +
                                "Clydebank Autocentre on a full-time basis...",


                                Description = "We are currently looking for an experienced Car Mechanic / Vehicle Technician / MOT tester to join our busy Clydebank Autocentre on a full-time basis." +
                                              "The desired candidate will possess the skills and experience to carry out all aspects of light vehicle maintenance including light and heavier engine work.",

                                JobResponsibilites = "The ideal Car Mechanic / Vehicle Technician / MOT Tester will possess: " +
                                                     "A Full UK driving licence with no more than 9 points(you will be subject to licence checks)" +
                                                     "Experience in a professional workshop environment" +
                                                     "A good approach to customer service" +
                                                     "Flexibility towards working hours(40-48 hours per week)" +
                                                     "Halfords Autocentres strive to be there for all of life's journeys, in order to deliver this some of our Autocentres are open 6/7 days a week.\n",


                                TechnicalExperience = "NVQ Level 3 or Equivalent / Time Served Experience/n" +
                                                      "A valid class 4 and 7 MOT testing licence",



                                MinSalary = 12000,
                                MaxSalary = 15000,
                                IsPositionFilled = true,
                                EmployerId = newUser10.Id,
                                Employer = newUser10

                            }
                        });



                

                    userManager.Create(newUser10, password10);
                    userManager.AddToRole(newUser10.Id, RoleNames.ROLE_ADMINISTRATOR);

                }


                // Create a new Applicant User and Suspended

                string userName11 = "CIvy@test.com";
                string password11 = "Password_1";

                //create User user and role
                var user11 = userManager.FindByName(userName11);
                if (user11 == null)
                {
                    var newUser11 = new Staff()
                    {
                        Forename = "Catriona",
                        Surname = "Ivy",
                        DOB = new DateTime(1984, 8, 26),
                        //Address = "10 Happy Street",
                        Title = "Mr",
                        Gender = 'M',
                        //City = "Glasgow",
                        //PostCode = "G51 1QW",
                        PhoneNumber = "01419920011",
                        Mobile = "07419920011",
                        UserName = userName11,
                        Email = userName11,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser11, password11);
                    userManager.AddToRole(newUser11.Id, RoleNames.ROLE_HR);
                }



                // Create a new Applicant User and Suspended

                string userName12 = "SMacDonald@test.com";
                string password12 = "Password_1";

                //create User user and role
                var user12 = userManager.FindByName(userName12);
                if (user12 == null)
                {
                    var newUser12 = new Staff()
                    {
                        Forename = "Steven",
                        Surname = "MacDonald",
                        DOB = new DateTime(1984, 8, 26),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01419920012",
                        Mobile = "07419920012",
                        UserName = userName12,
                        Email = userName12,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser12, password12);
                    userManager.AddToRole(newUser12.Id, RoleNames.ROLE_MANAGER);
                    userManager.AddToRole(newUser12.Id, RoleNames.ROLE_SUSPENDED);

                }




                // Create a new Manager

                string userName13 = "MLarkins@test.com";
                string password13 = "Password_1";

                //create User user and role
                var user13 = userManager.FindByName(userName13);
                if (user13 == null)
                {
                    var newUser13 = new Staff()
                    {
                        Forename = "Meghan",
                        Surname = "Larkins",
                        DOB = new DateTime(1993, 6, 20),
                        Title = "Miss",
                        Gender = 'M',
                        PhoneNumber = "01419920013",
                        Mobile = "07419920013",
                        UserName = userName13,
                        Email = userName13,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser13, password13);
                    userManager.AddToRole(newUser13.Id, RoleNames.ROLE_MANAGER);
                }



                // Create a new Applicant User and Suspended

                string userName14 = "XEastcroft@test.com";
                string password14 = "Password_1";

                //create User user and role
                var user14 = userManager.FindByName(userName14);
                if (user14 == null)
                {
                    var newUser14 = new Applicant()
                    {
                        Forename = "Xander",
                        Surname = "Eastcroft",
                        DOB = new DateTime(1993, 12, 23),
                        Address = "14 Happy Street",
                        Title = "Mr",
                        Gender = 'M',
                        City = "Glasgow",
                        PostCode = "G51 4QW",
                        PhoneNumber = "01419920014",
                        Mobile = "07419920014",
                        UserName = userName14,
                        Email = userName14,
                        EmailConfirmed = true,




                    };
                    context.Qualifications.AddRange(new List<Qualification>()
                    {
                        new Qualification
                        {
                            InstitutionName = "Glasgow Clyde College",
                            QualificationName = "Graphic Design",
                            Grade = Grade.B,
                            Level = "HND",
                            StartDate = DateTime.Now.AddYears(-5).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-4).AddMonths(-4),
                            Id = newUser14.Id,
                            Applicant = newUser14,

                        },
                        new Qualification
                        {
                            InstitutionName = "City of Glasgow College",
                            QualificationName = "Graphic Design",
                            Grade = Grade.C,
                            Level = "HNC",
                            StartDate = DateTime.Now.AddYears(-7).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-6).AddMonths(-4),
                            Id = newUser14.Id,
                            Applicant = newUser14,

                        },
                        new Qualification
                        {
                            InstitutionName = "Langside College",
                            QualificationName = "Software Development",
                            Grade = Grade.A,
                            Level = "HND",
                            StartDate = DateTime.Now.AddYears(-10).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-9).AddMonths(-4),
                            Id = newUser14.Id,
                            Applicant = newUser14,

                        },
                        new Qualification
                        {
                            InstitutionName = "Glasgow Caledonian University",
                            QualificationName = "Graphic Design",
                            Level = "Degree",
                            Grade = Grade.A,
                            StartDate = DateTime.Now.AddYears(-2).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-1).AddMonths(-4),
                            Id = newUser14.Id,
                            Applicant = newUser14,

                        }


                   });


                    context.PreviousJobs.AddRange(new List<JobHistory>()
                    {
                        new JobHistory
                        {
                            JobTitle = "Manager",
                            CompanyName = "Invarado Mortons",
                            StartDate = DateTime.Now.AddYears(-5).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-4).AddMonths(-4),
                            Description = "Being an awesome Manager, doing manager stuff.", 
                            Applicant_Id = newUser14.Id,
                            Applicant = newUser14
                        },
                        new JobHistory
                        {
                            JobTitle = "Button Salesman",
                            CompanyName = "Basell's Buttons",
                            StartDate = DateTime.Now.AddYears(-3).AddMonths(-2),
                            EndDate = DateTime.Now.AddYears(-1).AddMonths(-1),
                            Description = "We sell buttons because no one else will.",
                            Applicant_Id = newUser14.Id,
                            Applicant = newUser14
                        },
                        new JobHistory
                        {
                            JobTitle = "Web Developer",
                            CompanyName = "A1 Sites",
                            StartDate = DateTime.Now.AddYears(-5).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-4).AddMonths(-4),
                            Description = "We develop websites.",
                            Applicant_Id = newUser14.Id,
                            Applicant = newUser14
                        },
                        new JobHistory
                        {
                            JobTitle = "Cat Babysitter",
                            CompanyName = "Meows Inc.",
                            StartDate = DateTime.Now.AddYears(-5).AddMonths(-4),
                            EndDate = DateTime.Now.AddYears(-4).AddMonths(-4),
                            Description = "Being an awesome Manager, doing manager stuff.",
                            Applicant_Id = newUser14.Id,
                            Applicant = newUser14
                        },

                   });




                    userManager.Create(newUser14, password14);
                    userManager.AddToRole(newUser14.Id, RoleNames.ROLE_APPLICANT);
                }





                // Create a new Applicant User and Suspended

                string userName15 = "SMacDonald@test.com";
                string password15 = "Password_1";

                //create User user and role
                var user15 = userManager.FindByName(userName15);
                if (user15 == null)
                {
                    var newUser15 = new Staff()
                    {
                        Forename = "Steven",
                        Surname = "MacDonald",
                        DOB = new DateTime(1984, 8, 26),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01419920015",
                        Mobile = "07419920015",
                        UserName = userName15,
                        Email = userName15,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser15, password15);
                    userManager.AddToRole(newUser15.Id, RoleNames.ROLE_MANAGER);
                }




                // Create a new Applicant User and Suspended

                string userName16 = "JPuckett@test.com";
                string password16 = "Password_1";

                //create User user and role
                var user16 = userManager.FindByName(userName16);
                if (user16 == null)
                {
                    var newUser16 = new Staff()
                    {
                        Forename = "Joseph",
                        Surname = "Puckett",
                        DOB = new DateTime(1994, 4, 6),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01419920016",
                        Mobile = "07419920016",
                        UserName = userName16,
                        Email = userName16,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser16, password16);
                    userManager.AddToRole(newUser16.Id, RoleNames.ROLE_HR);
                }

                // Create a new Applicant User and Suspended

                string userName17 = "AKetchum@test.com";
                string password17 = "Password_1";

                //create User user and role
                var user17 = userManager.FindByName(userName17);
                if (user17 == null)
                {
                    var newUser17 = new Staff()
                    {
                        Forename = "Ash",
                        Surname = "Ketchum",
                        DOB = new DateTime(1990, 1, 1),
                        Title = "Mr",
                        Gender = 'M',
                        PhoneNumber = "01419920017",
                        Mobile = "07419920017",
                        UserName = userName17,
                        Email = userName17,
                        EmailConfirmed = true,


                    };
                    userManager.Create(newUser17, password17);
                    userManager.AddToRole(newUser17.Id, RoleNames.ROLE_RECRUITMENT_CONSULTANT);
                    userManager.AddToRole(newUser17.Id, RoleNames.ROLE_SUSPENDED);
                }

                // Create a new Applicant User and Suspended

                string userName18 = "MTulio@test.com";
                string password18 = "Password_1";

                //create User user and role
                var user18 = userManager.FindByName(userName18);
                if (user18 == null)
                {
                    var newUser18 = new Applicant()
                    {
                        Forename = "Miguel",
                        Surname = "Tulio",
                        DOB = new DateTime(1994, 1, 16),
                        Address = "2 Eldorado Lane",
                        Title = "Mr",
                        Gender = 'M',
                        City = "Glasgow",
                        PostCode = "G51 1QW",
                        PhoneNumber = "01419920018",
                        Mobile = "07419920018",
                        UserName = userName18,
                        Email = userName18,
                        EmailConfirmed = true



                    };
                    


                    userManager.Create(newUser18, password18);
                    userManager.AddToRole(newUser18.Id, RoleNames.ROLE_APPLICANT);

                }

             

            }

             
                            
                            

                        

            base.Seed(context);
            context.SaveChanges();
        }

    }
}
