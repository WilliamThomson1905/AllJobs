using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AllJobs.ModelValidation;

namespace AllJobs.Models
{
    // You can add profile data for the user by adding more cv to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string Title { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        public char Gender { get; set; }
        public string Mobile { get; set; }

        // for generating recent account registrations - if time
        //public DateTime? DateRegistered { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            EmailConfirmed = false;
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        
    }  // End of ApplicationUser




    /*
     The Staff class will be used to store data for the following user: Administrators, HR, IT Rcruitment Consultants. 
     This is because all these users do not need alot of values stored for them. 
    */
    [Table("Staff")]
    public class Staff : ApplicationUser
    {
        public Staff(){}
    }



    /// <summary>
    /// The advertisement class will hold all values that relate to an instance of a job vacancy
    /// </summary>
    [Table("Advertisement")]
    public class Advertisement
    {

        [Key]
        public int AdvertisementId { get; set; }

        [Required(ErrorMessage = "Please enter a Job Title")]
        [StringLength(50)]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Please enter a Company Name")]
        [StringLength(50)]
        public string CompanyName { get; set; }

 
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please Enter a Positive Number")]
        public double MinSalary { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please Enter a Positive Number")]
        public double MaxSalary { get; set; }

        [StringLength(250)]
        public string ShortDescription { get; set; }

        [Required]
        [StringLength(1400)]
        public string Description { get; set; }

        [StringLength(1400)]
        public string JobResponsibilites { get; set; }

        [StringLength(1400)]
        public string QualificationRequired { get; set; }

        [StringLength(1400)]
        public string TechnicalExperience { get; set; }

        [StringLength(1400)]
        public string OtherDetails { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AvailableFrom { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DateGreaterThanNow(ErrorMessage = "Closing Date cannot be in the Past")]
        public DateTime ClosingDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Website { get; set; }

        public bool IsPublic { get; set; }
        public bool CoverLetterRequired { get; set; }

        public bool IsPositionFilled { get; set; } 

        public string EmployerId { get; set; }
        public ApplicationUser Employer { get; set; }

        public virtual ICollection<Applicant> Applicants { get; set; }


        // Advertisement belongs to a single Location: Glasgow, Edinburgh, Fyfe, etc...
        [Required(ErrorMessage = "Please Specifiy a Location")]
        [Display(Name = "Location")]
        public int LocationCategoryId { get; set; }
        public virtual LocationCategory Location { get; set; }

        [Display(Name = "Job Sector")]
        public int JobSectorCategoryId { get; set; }
        public virtual JobSectorCategory Sector { get; set; }

        [Display(Name = "Job Type")]
        public int JobTypeCategoryId { get; set; }
        public virtual JobTypeCategory JobType { get; set; }     // Full-time, Part-Time, etc...






        public Advertisement()
        {
            IsPositionFilled = false;
            IsPublic = true;
            DateCreated = DateTime.Now;
            Applicants = new List<Applicant>();
        }

    }



    /// <summary>
    /// The job type class will hold all values that relate to an instance of a job type - full time, part time, volunteer etc.
    /// </summary>
    [Table("JobType")]
    public class JobTypeCategory
    {
        [Key]
        [Display(Name = "Job Type")]
        public int JobTypeCategoryId { get; set; }

 
        [StringLength(50)]
        public string JobTypeName { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }

        //Constructor: 0 Parameter
        public JobTypeCategory()
        {
            this.Advertisements = new List<Advertisement>();
        }
    }



    /// <summary>
    /// The job sector class will hold all values that relate to an instance of a job sector - education, history, health etc.
    /// </summary>
    [Table("JobSector")]
    public class JobSectorCategory
    {
        [Key]
        public int JobSectorCategoryId { get; set; }

        [StringLength(50)]
        public string SectorName { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }

        //Constructor: 0 Parameter
        public JobSectorCategory()
        {
            this.Advertisements = new List<Advertisement>();
        }
    }


    /// <summary>
    /// An advertisement can have a single location
    /// </summary>
    [Table("Location")]
    public class LocationCategory
    {
        [Key]
        public int LocationCategoryId { get; set; }

        [Display(Name = "Location")]
        [StringLength(50)]
        public string LocationName { get; set; }

        public ICollection<Advertisement> Advertisements { get; set; }

        //Constructor: 0 Parameter
        public LocationCategory()
        {
            this.Advertisements = new List<Advertisement>();
        }
    }


    /// <summary>
    /// The recruiter class will hold all values that relate to an instance of a recruiter
    /// Recruiter inherits most of its values from applicationUser - except companyName
    /// </summary>
    [Table("Recruiter")]
    public class Recruiter : ApplicationUser
    {
        [StringLength(50)]
        public string CompanyName { get; set; }

    }


    /// <summary>
    /// The applicant class will hold all values that relate to an instance of a applicant
    /// applicant inherits some of its values from the ApplicationUser calss - this is where emails, passwords and emailConfirmed is declared, to name a few. 
    /// </summary>
    [Table("Applicant")]
    public class Applicant : ApplicationUser
    {
       
        public string Address { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        // Storing a CV - file use a filePath
        public string CV_FileUpload { get; set; }
        public string PostCode { get; set; }



        // Navigational Properties
        public virtual ICollection<Advertisement> Advertisements { get; set; }

        // Navigational Properties - lazy loading Nav. Properties
        public ICollection<JobHistory> PreviousJobs { get; set; }
        public virtual ICollection<Qualification> Qualifications { get; set; }

        // A Applicant can have multiple Applications, for multiple advertisements 
        public virtual ICollection<Application> Applications { get; set; }


        // Applicant can create multiple CV's
        public ICollection<CV> CVs { get; set; }

        public Applicant()
        {
            EmailConfirmed = false;
            CVs = new List<CV>();
            CV_FileUpload = "";
            Advertisements = new List<Advertisement>();
            Applications = new List<Application>();
       
        }

    }


    /// <summary>
    /// When an applocant applies to a job vacancy they instanciate this class. 
    /// </summary>
    [Table("Application")]
    public class Application
    {
        [Key]
        public int Application_Id { get; set; }

        [StringLength(1400, ErrorMessage = "Cannot exceed 1400 characters.")]
        public string CoverLetter { get; set; }

        public int? CV_Id { get; set; }

        public string CV_Location { get; set; }

        // this value will be set when the new instance of Applicsnt is created.
        public DateTime DateApplied { get; set; }    

        [ForeignKey("Advertisement")]
        public int advertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }


        [ForeignKey("Applicant")]
        public string Id { get; set; }
        public virtual Applicant Applicant { get; set; }

    }


    /// <summary>
    /// An applicant can have many cvs. Each cv can hold the following values
    /// </summary>
    [Table("CV")]
    public class CV
    {


        public CV()
        {
            Qualifications = new List<Qualification>();
            PreviousJobs = new List<JobHistory>();
            Applications = new List<Application>();
        }

        [Key]
        public int CV_Id { get; set; }

        // A title will be given to each CV so User can determine which one to use at a glance.
        [Required]
        public string Title { get; set; }




        [Required]
        [StringLength(2500, ErrorMessage ="Cannot exceed 2500 characters.")]
        public string Profile { get; set; }

        // Each CV has a Collection of Qualifications
        public virtual ICollection<Qualification> Qualifications { get; set; }


        // Each CV has a Collection of JobHistory
        public virtual ICollection<JobHistory> PreviousJobs { get; set; }


        public virtual ICollection<Application> Applications { get; set; }


        [StringLength(2500, ErrorMessage = "Cannot exceed 2500 characters.")]
        public string Skills_Interests { get; set; }

        [Required]
        public string Description { get; set; }


        // Navigational properties to Applicant table
        [ForeignKey("Applicant")]
        public virtual string Id { get; set; }
        public virtual Applicant Applicant { get; set; }


    }


    // Enumeration which will used by the qualifications call. Each qualification can have one of these values
    public enum Grade
    {
        A, B, C, D, F, Pass, Fail, Pending
    }


    /// <summary>
    /// An applicant can create as many qualifications and they'd like and add them to a CV when creating one
    /// </summary>
    [Table("Qualification")]
    public class Qualification
    {
        [Key]
        public int Qualification_Id { get; set; }

        [Required]
        public string QualificationName { get; set; }

        [Required]
        public string Level { get; set; }

        [DateBeforeNow(ErrorMessage = "Start Date cannot be in the future")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        public Grade Grade { get; set; }

        [Required]
        public string InstitutionName { get; set; }



        public virtual ICollection<CV> CVs { get; set; }


        //Navigational Properties
        [ForeignKey("Applicant")]
        public string Id { get; set; }
        public virtual Applicant Applicant { get; set; }

        public Qualification()
        {
            CVs = new List<CV>();
        }
    }




    /// <summary>
    /// An applicant can create as much job history and they'd like and add them to a CV when creating one
    /// </summary>
    [Table("JobHistory")]
    public class JobHistory
    {
        [Key]
        public int Id { get; set; }

       
        [DateBeforeNow]  // Check that date doesn't exceed todays date - custom validation
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required]
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual ICollection<CV> CVs { get; set; }


        //Navigational Properties
        [ForeignKey("Applicant")]
        public virtual string Applicant_Id { get; set; }
        public virtual Applicant Applicant { get; set; }

        public JobHistory()
        {
            CVs = new List<CV>();
        }



    }

    


}