using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace AllJobs.Models
{

    /// <summary>
    /// Once the user navigates to a single advertisement individual page this ViewModel will be implemented to display all of the relavant 
    /// information for any given advertisement.
    /// </summary>
    public class SingleAdvertisementViewModel
    {
        public int AdvertisementId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public string Description { get; set; }
        public string JobResponsibilites { get; set; }
        public string QualificationRequired { get; set; }
        public string TechnicalExperience { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string Website { get; set; }
        public bool? IsPositionFilled { get; set; }

        public bool CoverLetterRequired { get; set; }

        public string EmployerId { get; set; }
        public ApplicationUser Employer { get; set; }

        public int LocationCategoryId { get; set; }
        public LocationCategory Location { get; set; }

        public int JobSectorCategoryId { get; set; }
        public JobSectorCategory Sector { get; set; }  // Health & Safety, Education etc...

        public int JobTypeCategoryId { get; set; }
        public JobTypeCategory JobType { get; set; } // Full-time, Part-Time, etc...


        public static Expression<Func<Advertisement, SingleAdvertisementViewModel>> ViewModel
        {
            get
            {
                return e => new SingleAdvertisementViewModel()
                {
                    AdvertisementId = e.AdvertisementId,
                    JobTitle = e.JobTitle,
                    CompanyName = e.CompanyName,

                    JobTypeCategoryId = e.JobTypeCategoryId,
                    JobType = e.JobType,

                    JobSectorCategoryId = e.JobSectorCategoryId,
                    Sector = e.Sector,

                    MinSalary = e.MinSalary,
                    MaxSalary = e.MaxSalary,
                    Description = e.Description,
                    JobResponsibilites = e.JobResponsibilites,
                    QualificationRequired = e.QualificationRequired,
                    TechnicalExperience = e.TechnicalExperience,
                    AvailableFrom = e.AvailableFrom,
                    ClosingDate = e.ClosingDate,
                    Website = e.Website,

                    LocationCategoryId = e.LocationCategoryId,
                    Location = e.Location,

                    EmployerId = e.EmployerId,
                    Employer = e.Employer,
              
                    CoverLetterRequired = e.CoverLetterRequired,
                    IsPositionFilled = e.IsPositionFilled

                };
            }
        }

    }




    /// <summary>
    /// This ViewModel is used to retrieve data that is required for an Advertisement on the home page. 
    /// On the home page only some of the data is required. 
    /// </summary>
    public class AdvertisementViewModel
    {
        public int AdvertisementId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public string ShortDescription { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool IsPublic { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsPositionFilled { get; set; }

        public ApplicationUser Employer { get; set; }

        public ICollection<Applicant> Applicants { get; set; }

        public int LocationCategoryId { get; set; }
        public LocationCategory Location { get; set; }

        public int JobSectorCategoryId { get; set; }
        public JobSectorCategory Sector { get; set; }  // Health & Safety, Education etc...

        public int JobTypeCategoryId { get; set; }
        public JobTypeCategory JobType { get; set; } // Full-time, Part-Time, etc...


        public static Expression<Func<Advertisement, AdvertisementViewModel>> ViewModel
        {
            get
            {
                return e => new AdvertisementViewModel()
                {
                    AdvertisementId = e.AdvertisementId,
                    JobTitle = e.JobTitle,
                    CompanyName = e.CompanyName,
                    JobTypeCategoryId = e.JobTypeCategoryId,
                    JobType = e.JobType,
                    MinSalary = e.MinSalary,
                    MaxSalary = e.MaxSalary,
                    ShortDescription = e.ShortDescription,
                    AvailableFrom = e.AvailableFrom,
                    ClosingDate = e.ClosingDate,
                    LocationCategoryId = e.LocationCategoryId,
                    Location = e.Location,
                    JobSectorCategoryId = e.JobSectorCategoryId,
                    Sector = e.Sector,
                    DateCreated = e.DateCreated,
                    //CVRequired = e.CVRequired,
                    IsPublic = e.IsPublic,
                    Applicants = e.Applicants,
                    Employer = e.Employer,
                    IsPositionFilled = e.IsPositionFilled
                };
            }
        }
    }


    /// <summary>
    /// Used to Create a Collection of Advertisements. This ViewModel will be used on home/index page to display all advertisemenmts that are public. 
    /// </summary>
    public class AdvertisementListViewModel
    {

        public IEnumerable<AdvertisementViewModel> AllAdvertisements { get; set; }


    }



    /// <summary>
    /// This View Model will be used when an Applicant User attempts to Apply to a Job. It will be used on the ApplyToJob View. 
    /// It will retrieve values from two Models: Advertisement and Applicant.
    /// </summary>
    public class ApplicationViewModel
    {
        // Advertisement Variable that will be displayed on Application Page
        public int AdvertisementId { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public double MinSalary { get; set; }
        public double MaxSalary { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool IsPublic { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CoverLetterRequired { get; set; }


        public int LocationCategoryId { get; set; }
        public LocationCategory Location { get; set; }

        public int JobSectorCategoryId { get; set; }
        public JobSectorCategory Sector { get; set; }  // Health & Safety, Education etc...

        public int JobTypeCategoryId { get; set; }
        public JobTypeCategory JobType { get; set; } // Full-time, Part-Time, etc...



        public string Skills_Interests { get; set; }
        public IEnumerable<CV> CVs { get; set; }


        // Storing a CV for Applicant
        public string CV_FileUpload { get; set; }




        // User Emails
        public string systemEmail { get; set; }
        public string recruiterEmail { get; set; }

        public virtual Applicant applicant { get; set; }


        public string coverLetter { get; set; }



        public static Expression<Func<AllJobs.Models.Advertisement, ApplicationViewModel>> ViewModel
        {

            get
            {
                return e => new ApplicationViewModel()
                {

                    // Advertisement Variable that will be displayed on Application Page
                    AdvertisementId = e.AdvertisementId,
                    JobTitle = e.JobTitle,
                    CompanyName = e.CompanyName,
                    JobTypeCategoryId = e.JobTypeCategoryId,
                    JobType = e.JobType,
                    MinSalary = e.MinSalary,
                    MaxSalary = e.MaxSalary,
                    AvailableFrom = e.AvailableFrom,
                    ClosingDate = e.ClosingDate,
                    LocationCategoryId = e.LocationCategoryId,
                    Location = e.Location,
                    JobSectorCategoryId = e.JobSectorCategoryId,
                    Sector = e.Sector,
                    DateCreated = e.DateCreated,
                    Description = e.Description,
                    IsPublic = e.IsPublic,
                    CoverLetterRequired = e.CoverLetterRequired,
                    CVs = new List<CV>(),
                    systemEmail = "williamthomsondesign@gmail.com",
                    recruiterEmail = e.Employer.Email,
                    coverLetter = ""







                };
            }
        }





        public static Expression<Func<Applicant, ApplicationViewModel>> Applicant_ViewModel
        {
            get
            {
                return f => new ApplicationViewModel()
                {

                    CV_FileUpload = f.CV_FileUpload,
                   
                };
            }
        }




        //public static Expression<Func<List<CV>, ApplicationViewModel>> CV_ViewModel
        //{
        //    get
        //    {
        //        return e => new ApplicationViewModel()
        //        {

        //            //     CV_FileUpload =

        //            // Values from CV model




        //        };
        //    }
        //}




    }



    // This ViewModel will be used to retrieve all the applicant that have applied for a specified job.
    // The following data will be stored.
    public class ApplicantsForAdvertisementViewModel
    {

        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }

        public string JobTitle { get; set; }
        public DateTime DateApplied { get; set; }
        public IEnumerable<Applicant> Applicants { get; set; }

        public IEnumerable<Application> application { get; set; }



    }

}