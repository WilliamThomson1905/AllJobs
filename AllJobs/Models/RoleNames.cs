using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllJobs.Models
{
    public class RoleNames
    {
        // ADMIN ACCOUNTS
        public const string ROLE_ADMINISTRATOR = "Administrator";

        // STAFF ACCOUNTS
        public const string ROLE_MANAGER = "Manager";
        public const string ROLE_IT = "IT";
        public const string ROLE_HR = "HR";
        public const string ROLE_RECRUITMENT_CONSULTANT = "Recruitment Consultant";


        // Job Seekers and Recruiter Accounts
        public const string ROLE_RECRUITER = "Recruiter";
        public const string ROLE_APPLICANT = "Applicant";

        // If user is to be Locked Out/suspended then assign to this Role
        public const string ROLE_SUSPENDED = "Suspended";

    }
}

/*
 
Q&A with Graham Determined these User Types


Employee accounts – Manager, IT staff, HR staff, Recruitment Consultants

    Managing Data:
        - Employers data – Manager, IT staff, HR staff
        - Applicants data - Recruitment Consultants, Manager


    
*/
