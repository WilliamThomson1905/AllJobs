// Name: William Thomson
// Date: 27/03/2017
// Description: 

using AllJobs.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AllJobs.Controllers
{
    public class AdminController : BaseController
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        // Controllers for managing administrators site functionality
        


        // GET: /Admin/Index
        /// <summary>
        /// Will display all the user of the system. And will filter them out to seperate pages by implementing pagination
        /// </summary>
        /// <param name="searchStringUserNameOrEmail"></param>
        /// <param name="currentFilter">search value</param>
        /// <param name="page">Represents the numeric value of what page the user is currently at on the pagination</param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Index(string searchStringUserNameOrEmail)
        public ActionResult Index(string searchStringUserNameOrEmail, string currentFilter, int? page)
        {
            try
            {
                int intPage = 1;
                int intPageSize = 6; // Will only display six users per page
                int intTotalPageCount = 0;  

                if (searchStringUserNameOrEmail != null)
                {
                    intPage = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchStringUserNameOrEmail = currentFilter;
                        intPage = page ?? 1; // If page doesn't have a value, default to 1
                    }
                    else
                    {
                        searchStringUserNameOrEmail = ""; 
                        intPage = page ?? 1;
                    }
                }

                ViewBag.CurrentFilter = searchStringUserNameOrEmail;


                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();


                int intSkip = (intPage - 1) * intPageSize;

                // Retrieving count of all the users that email and/or username match the user search value. 
                intTotalPageCount = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .Count();

                // Retrieving a list of users that match the search criteria and the ordering by username
                var result = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .OrderBy(x => x.UserName)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();


                // Looping through all users and getting their requested values for output. 
                foreach (var item in result)
                {

                    ExpandedUserDTO objUserDTO = new ExpandedUserDTO();

                    objUserDTO.UserName = item.UserName;
                    objUserDTO.Email = item.Email;

                    objUserDTO.PhoneNumber = item.PhoneNumber;
                    objUserDTO.LockoutEndDateUtc = item.LockoutEndDateUtc;


                    // Get all rolenames that correspond to each user. 
                    // This will be used to display their current accout status: suspended or active.
                    var user = UserManager.FindByName(searchStringUserNameOrEmail);
                    ICollection<UserRolesDTO> colUserRoleDTO = (from objRole in UserManager.GetRoles(item.Id)
                        select new UserRolesDTO
                        {
                            RoleName = objRole

                        }).ToList();
                        

                    objUserDTO.Roles = colUserRoleDTO;
                    col_UserDTO.Add(objUserDTO);
                }

                // Set the number of pages
                var _UserDTOAsIPagedList = new StaticPagedList<ExpandedUserDTO>( col_UserDTO, intPage, intPageSize, intTotalPageCount );

                return View(_UserDTOAsIPagedList);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUserDTO> col_UserDTO = new List<ExpandedUserDTO>();

                return View(col_UserDTO.ToPagedList(1, 25));
            }
        }
        #endregion

        // Users *****************************

        // GET: /Admin/Edit/Create 
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Create()
        public ActionResult Create()
        {
            ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();
            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(objExpandedUserDTO);
        }
        #endregion

        // PUT: /Admin/Create
        /// <summary>
        /// Will be used to create a new user
        /// </summary>
        /// <param name="paramExpandedUserDTO">A model of values that correspond to the creation of a new user </param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        public ActionResult Create(ExpandedUserDTO paramExpandedUserDTO)
        {
            try
            {
                // Validating that user has inputted appropriate values
                if (paramExpandedUserDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Trim: Removes white space
                var Email = paramExpandedUserDTO.Email.Trim();
                var UserName = paramExpandedUserDTO.Email.Trim();
                var Password = paramExpandedUserDTO.Password.Trim();

                if (Email == "")
                {
                    throw new Exception("No Email");
                }

                if (Password == "")
                {
                    throw new Exception("No Password");
                }

                // UserName is LowerCase of the Email
                UserName = Email.ToLower();

                // Create user
                var objNewAdminUser = new ApplicationUser { UserName = UserName, Email = Email };
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in selected role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    // If unsuccessful
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty, "Error: Failed to create the user. Check password requirements.");
                    return View(paramExpandedUserDTO);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("Create");
            }
        }
        #endregion

        // GET: /Admin/Edit/EditUser 
        [Authorize(Roles = "Administrator")]
        #region public ActionResult EditUser(string UserName)
        public ActionResult EditUser(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

            if (objExpandedUserDTO == null)
            {
                return HttpNotFound();
            }
            return View(objExpandedUserDTO);
        }
        #endregion

        // PUT: /Admin/EditUser
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
        public ActionResult EditUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            try
            {
                if (paramExpandedUserDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                // Making a call to UpdateDTOUser and passing the values of the selected user
                ExpandedUserDTO objExpandedUserDTO = UpdateDTOUser(paramExpandedUserDTO);
                
                                
                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(paramExpandedUserDTO.UserName));
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUser
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteUser(string UserName)
        public ActionResult DeleteUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // if the username is the same as the cuurent user username return ToString view
                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot delete the current user");

                    return View("EditUser");
                }


                // Get the the values of the current user by calling GetUser()
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                // If it doesn't exist
                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    DeleteUser(objExpandedUserDTO);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }
        #endregion



        // Suspend: /Admin/SuspendUser
        [Authorize(Roles = "Administrator")]
        #region public ActionResult SuspendUser(string UserName)
        public ActionResult SuspendUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot Suspend the current user");

                    return View("EditUser");
                }

                // Getting current user, using their email/username
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    // Make a call to suspend user
                    SuspendUser(objExpandedUserDTO);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }
        #endregion


        // Unsuspend: /Admin/UnsuspendUser
        [Authorize(Roles = "Administrator")]
        #region public ActionResult UnsuspendUser(string UserName)
        public ActionResult UnsuspendUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Cannot unsuspend your own account
                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot Unsuspend the current user");

                    return View("EditUser");
                }

                // Getting selected user to unsuspend
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    // The selected user was suspended - make a call to unsuspend
                    UnsuspendUser(objExpandedUserDTO);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }
        #endregion






        // GET: /Admin/EditRoles/TestUser 
        [Authorize(Roles = "Administrator")]
        #region ActionResult EditRoles(string UserName)
        public ActionResult EditRoles(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user of the system
            ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

            if (objExpandedUserDTO == null)
            {
                return HttpNotFound();
            }

            // Retrieving a user and all the roles relating to that user
            UserAndRolesDTO objUserAndRolesDTO = GetUserAndRoles(UserName);

            return View(objUserAndRolesDTO);
        }
        #endregion

        // PUT: /Admin/EditRoles/TestUser 
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        public ActionResult EditRoles(UserAndRolesDTO paramUserAndRolesDTO)
        {
            try
            {
                if (paramUserAndRolesDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRolesDTO.UserName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    ApplicationUser user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View(objUserAndRolesDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteRole?UserName="TestUser&RoleName=Administrator
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteRole(string UserName, string RoleName)
        public ActionResult DeleteRole(string UserName, string RoleName)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();

                // Check that we have an actual user
                ExpandedUserDTO objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);


                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRolesDTO);
            }
        }
        #endregion

        // Roles *****************************



        // GET: /Admin/ViewAllRoles
        [Authorize(Roles = "Administrator")]
        #region public ActionResult ViewAllRoles()
        public ActionResult ViewAllRoles()
        {
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );

                    List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                        select new RoleDTO
                                        {
                                            Id = objRole.Id,
                                            RoleName = objRole.Name
                                        }).ToList();

            return View(colRoleDTO);
        }
        #endregion

        // GET: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        #region public ActionResult AddRole()
        public ActionResult AddRole()
        {
            RoleDTO objRoleDTO = new RoleDTO();
            return View(objRoleDTO);
        }
        #endregion

        // PUT: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult AddRole(RoleDTO paramRoleDTO)
        public ActionResult AddRole(RoleDTO paramRoleDTO)
        {
            try
            {
                // Ensure paramRoleDTO has a value
                if (paramRoleDTO == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Remove white space
                var RoleName = paramRoleDTO.RoleName.Trim();


                // If administratot didn't input a value for rolename
                if (RoleName == "")
                {
                    throw new Exception("No RoleName");
                }

                // Create Role
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                // If role doesn't already exist
                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }

                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }
        #endregion

        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole
        /// <summary>
        /// Will be called when deleting a role from the system.
        /// </summary>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteUserRole(string RoleName)
        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                // Cannot delete administrator role from system
                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(String.Format("Cannot delete {0} Role.", RoleName));
                }

                // Instanciate a new roleManager
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                // Get a count of all the users in the selected role for deletion
                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();

                // If role has users
                if (UsersInRole > 0)
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role because it still has users.",
                            RoleName)
                            );
                }

                // check db for role the adminstrator wishes to remove
                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();


                // If exists
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role does not exist.",
                            RoleName)
                            );
                }

                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                List<RoleDTO> colRoleDTO = (from objRole in roleManager.Roles
                                            select new RoleDTO
                                            {
                                                Id = objRole.Id,
                                                RoleName = objRole.Name
                                            }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
        }
        #endregion


        // Utility

        #region public ApplicationUserManager UserManager
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();  // if _userManager desn't have a value then call getUserManager
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        #region public ApplicationRoleManager RoleManager
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>(); // if _roleManager desn't have a value then call getUserManager
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region private List<SelectListItem> GetAllRolesAsSelectList()
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems =
                new List<SelectListItem>();

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
        }
        #endregion


        /// <summary>
        /// This method is called whenever I want to search for a specific instance of a user 
        /// </summary>
        /// <param name="paramUserName">usernames must be unique, search for user using username  </param>
        #region private ExpandedUserDTO GetUser(string paramUserName)
        private ExpandedUserDTO GetUser(string paramUserName)
        {
            // Instanciate a new objExpandedUserDTO
            ExpandedUserDTO objExpandedUserDTO = new ExpandedUserDTO();

            // Defined as var since there are many user types.
            var result = UserManager.FindByName(paramUserName);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the User");

            objExpandedUserDTO.UserName = result.UserName;


            // Getting all the roles the searched for user has
            ICollection<UserRolesDTO> colUserRoleDTO = (from objRole in UserManager.GetRoles(result.Id)
                select new UserRolesDTO
                {
                    RoleName = objRole

                }).ToList();



            objExpandedUserDTO.Roles = colUserRoleDTO;
            objExpandedUserDTO.Email = result.Email;
            objExpandedUserDTO.LockoutEndDateUtc = result.LockoutEndDateUtc;
            objExpandedUserDTO.AccessFailedCount = result.AccessFailedCount;
            objExpandedUserDTO.PhoneNumber = result.PhoneNumber;

            return objExpandedUserDTO;
        }
        #endregion

        #region private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO objExpandedUserDTO)
        private ExpandedUserDTO UpdateDTOUser(ExpandedUserDTO paramExpandedUserDTO)
        {

            ApplicationUser result = UserManager.FindByName(paramExpandedUserDTO.UserName);


            // If we could not find the user, throw an exception
            if (result == null)
            {
                throw new Exception("Could not find the User");
            }

            result.Email = paramExpandedUserDTO.Email;

            // Lets check if the account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }

            UserManager.Update(result);

            // Was a password sent across?
            if (!string.IsNullOrEmpty(paramExpandedUserDTO.Password))
            {
                // Remove current password
                var removePassword = UserManager.RemovePassword(result.Id);
                if (removePassword.Succeeded)
                {
                    // Add new password
                    var AddPassword = UserManager.AddPassword( result.Id, paramExpandedUserDTO.Password);

                    if (AddPassword.Errors.Count() > 0)
                    {
                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                    }
                }
            }

            return paramExpandedUserDTO;
        }
        #endregion

        #region private void SuspendUser(ExpandedUserDTO paramExpandedUserDTO)
        private void SuspendUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            ApplicationUser user = UserManager.FindByName(paramExpandedUserDTO.UserName);

            
            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            UserManager.AddToRole(user.Id, RoleNames.ROLE_SUSPENDED);
        }
        #endregion

        #region private void UnsuspendUser(ExpandedUserDTO paramExpandedUserDTO)
        private void UnsuspendUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            ApplicationUser user = UserManager.FindByName(paramExpandedUserDTO.UserName);


            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            UserManager.RemoveFromRole(user.Id, RoleNames.ROLE_SUSPENDED);
        }
        #endregion

        /// <summary>
        /// Will be used t delete all user types. Except for the user doing deleting
        /// </summary>
        /// <param name="paramExpandedUserDTO"></param>
        #region private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
        private void DeleteUser(ExpandedUserDTO paramExpandedUserDTO)
        {
            // Get the curretn user
            ApplicationUser user = UserManager.FindByName(paramExpandedUserDTO.UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }


            // Find all qualifications relating to user 
            var quals = db.Qualifications.Where(e => e.Id.Equals(user.Id)).ToList();

            // Remove all qualifications that the user we wish to delete has
            foreach (Qualification qual in quals)
            {
                db.Qualifications.Remove(qual);
                db.SaveChanges();

            }

            // Find and remove all Previous Jobs that the user we wish to delete has
            var jh = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(user.Id)).ToList();
            foreach (JobHistory jobHistory in jh)
            {
                db.PreviousJobs.Remove(jobHistory);
                db.SaveChanges();

            }

            // Find and remove all CVs the selected user has
            var cvs = db.Cvs.Where(e => e.Id.Equals(user.Id)).ToList();
            foreach (CV cv in cvs)
            {
                db.Cvs.Remove(cv);
                db.SaveChanges();

            }

            // Find and remove all advertisements the current user owns
            var advertisements = db.Advertisements.Where(e => e.EmployerId.Equals(user.Id)).ToList();
            foreach (Advertisement ad in advertisements)
            {
                db.Advertisements.Remove(ad);
                db.SaveChanges();

            }

            // Then remove the user from all their roles they are in
            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }
        #endregion

        #region private UserAndRolesDTO GetUserAndRoles(string UserName)
        private UserAndRolesDTO GetUserAndRoles(string UserName)
        {
            // Go get the User
            ApplicationUser user = UserManager.FindByName(UserName);

            List<UserRoleDTO> colUserRoleDTO = (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleDTO
                 {
                     RoleName = objRole,
                     UserName = UserName

                 }).ToList();

            if (colUserRoleDTO.Count() == 0)
            {
                colUserRoleDTO.Add(new UserRoleDTO { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesDTO objUserAndRolesDTO = new UserAndRolesDTO();
            objUserAndRolesDTO.UserName = UserName;
            objUserAndRolesDTO.colUserRoleDTO = colUserRoleDTO;
            return objUserAndRolesDTO;
        }
        #endregion

        #region private List<string> RolesUserIsNotIn(string UserName)
        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            ApplicationUser user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();
            var colRolesUserInNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserInNotIn.Count() == 0)
            {
                colRolesUserInNotIn.Add("No Roles Found");
            }

            return colRolesUserInNotIn;
        }
        #endregion
    }
}