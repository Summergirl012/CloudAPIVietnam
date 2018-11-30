using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CloudApiVietnam.Models;
using CloudApiVietnam.Providers;
using CloudApiVietnam.Results;
using System.Linq;
using System.Net;
using System.Collections;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public HttpResponseMessage ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The user's password could not be changed.");
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //GET /api/Account
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            try
            {
                // Check of er filtering is
                Dictionary<string, string> filter = Request.GetQueryNameValuePairs().ToDictionary(x => x.Key, x => x.Value);
                
                //Haal de users op uit de database
                List<UserInfo> usersInfo = new List<UserInfo>();
                List<User> users = new List<User>();
              
                if (filter.Count > 0)
                {
                    foreach (KeyValuePair<string, string> entry in filter)
                    {
                        DateTime date;
                        try
                        {
                            date = new DateTime(Convert.ToInt32(entry.Value), 1, 1);
                        } catch (Exception ex)
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "The year you've filled in is not a valid year");
                        }
                        
                        switch (entry.Key)
                        {
                            case "bornAfter":
                                users = db.Users.Where(x => x.BirthDate > date).ToList();
                                break;
                            case "bornBefore":
                                users = db.Users.Where(x => x.BirthDate < date).ToList();
                                break;
                            default:
                                users = db.Users.ToList();
                                break;
                        }
                    }
                    if(users.Count == 0)
                        return Request.CreateErrorResponse(HttpStatusCode.NoContent, "There are no users in the database with this filtering.");
                } else
                {
                    users = db.Users.ToList();
                    // Foute if. Users.count = null, not users.
                    //if (users == null)
                    if(users.Count == 0)
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "There are no users in the database.");
                }

                //Zet alle users van de database om naar users die getoond kunnen worden.
                foreach (User user in users)
                {
                    UserInfo info = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = user.Roles,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DateOfBirth = user.BirthDate
                    };
                    usersInfo.Add(info);
                }
                return Request.CreateResponse(HttpStatusCode.OK, usersInfo);
            } catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }

        //GET /api/Account/{id}
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                //Haal de user uit de database op
                User user = db.Users.Where(u => u.Id == id).FirstOrDefault();

                if(user == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found with id: " + id);

                //Zet de user uit de database om naar een user die getoond moet worden
                UserInfo info = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = user.Roles,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.BirthDate
                };
                return Request.CreateResponse(HttpStatusCode.OK, info);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }

        //POST /api/Account
        //Voor nu even AllowAnonymous voor het eenvoudig testen[]
        //[Authorize(Roles ="Admin")]
        [AllowAnonymous]
        public HttpResponseMessage Post(RegisterBindingModel model)
        {
            try
            {
                //Check of het meegestuurde model valide is, stuur anders een custom message mee
                if (!ModelState.IsValid)
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = Convert.ToDateTime(model.DateOfBirth)
                };

                IdentityResult result = new IdentityResult();
                try
                {
                    result = UserManager.Create(user, model.Password);
                }
                catch( Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, e.Message);
                }

                if (!result.Succeeded)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, result.Errors.ToString());

                try
                {
                    UserManager.AddToRole(user.Id, model.UserRole);
                }
                catch
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "The user role could not be added.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, user);
            } catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
            }
        }
        
        //DELETE /api/Account
        [AllowAnonymous]
        public HttpResponseMessage Delete(string id)
        {
            //Haal de user op
            User user = db.Users.Where(f => f.Id == id).FirstOrDefault();

            if (user == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            else
            {
                //Probeer de user te verwijderen
                try
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
                }
            }
        }

        //PUT /api/Account/{id}
        [AllowAnonymous]
        public HttpResponseMessage Put(string id, [FromBody]RegisterBindingModel model)
        {
            //User en role opvragen.
            User user = db.Users.Where(f => f.Id == id).FirstOrDefault();
            IdentityRole role = db.Roles.Where(r => r.Name == model.UserRole).FirstOrDefault();
            
            
            if (user == null) //Checken of er een user is gevonden met het id
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No user found with id: " + id.ToString());
            else
            {
                try
                {
                    if (role == null) //Checken of er roles zijn gevonden bij de user
                        throw new System.ArgumentException("There is no userrole named: " + role.Name);
                    else
                    {
                        //Vervang de usre role als deze anders is
                        if (user.Roles.FirstOrDefault().RoleId != role.Id)
                        {
                            UserManager.RemoveFromRole(user.Id, role.Name);
                            UserManager.AddToRole(user.Id, model.UserRole);
                        }

                        //Vervang de user Email
                        user.Email = model.Email;
                        // Vervang username (=email), voornaam, achternaam & geboortedatum
                        user.UserName = model.Email;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.BirthDate = Convert.ToDateTime(model.DateOfBirth);
                    }

                    //Sla de changes op
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong. Exception: " + ex);
                }
            }
        }

        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

      
        #endregion
    }
}
