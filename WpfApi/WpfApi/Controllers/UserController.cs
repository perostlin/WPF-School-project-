using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WpfApi.Models;
using WpfApi.HelperMethods;

namespace WpfApi.Controllers
{
    public class UserController : ApiController
    {
        // Hash och Salt klass.
        HashAndSalt hash = new HashAndSalt();

        #region Login
        [HttpPost]
        [ActionName("Login")]
        [ResponseType(typeof(UserModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage Login([FromBody]UserModel userToLogin)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Hämtar användaren samt hashar och saltar lösenordet.
                        if (!db.User.Any(x => x.Username == userToLogin.Username))
                        {
                            // Error, användaren fanns inte.
                            return Request.CreateResponse(HttpStatusCode.NotFound);
                        }
                        User user = db.User.Where(x => x.Username == userToLogin.Username).SingleOrDefault();
                        string hashedPassword = hash.GetHashedPassword(userToLogin.Password + user.Salt);

                        // Kollar om det finns någon i databasen som matchar Username och hashade Password:et.
                        if (db.User.Any(x => x.Username == user.Username && x.Password == hashedPassword))
                        {
                            UserModel userToReturn = new UserModel
                            {
                                ID = user.ID,
                                Username = user.Username,
                                IsAdmin = user.IsAdmin,
                                Password = userToLogin.Password
                            };
                            return Request.CreateResponse(HttpStatusCode.OK, userToReturn);
                        }

                        // Error, användaren fanns inte.
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion Login

        #region Register
        [HttpPost]
        [ActionName("Register")]
        [ResponseType(typeof(UserModel))]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Register([FromBody]UserModel userToRegister)
        {
            if (ClaimsPrincipal.Current.IsInRole("Admin"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Kollar om det finns någon i databasen som matchar Username.
                        if (!db.User.Any(x => x.Username == userToRegister.Username))
                        {
                            string salt = hash.GetSaltedPassword();
                            User userToAdd = new User
                            {
                                ID = Guid.NewGuid(),
                                Username = userToRegister.Username,
                                IsAdmin = userToRegister.IsAdmin,

                                // Sätter salt och hashar lösenordet som angavs.
                                Salt = salt,
                                Password = hash.GetHashedPassword(userToRegister.Password + salt)
                            };

                            // Lägger till och sparar ny användare.
                            db.User.Add(userToAdd);
                            db.SaveChanges();

                            // Returnerar OK om det går igenom.
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }

                        // Error, användaren fanns redan.
                        return Request.CreateResponse(HttpStatusCode.Found);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion Register

        #region GetAllUsers
        [HttpGet]
        [ActionName("GetAllUsers")]
        [ResponseType(typeof(List<UserModel>))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage GetAllUsers()
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Skapar och fyller en lista med Users.
                        List<User> userList = db.User.ToList();

                        // Skapar en lista som skall fyllas och returneras.
                        List<UserModel> userListToReturn = new List<UserModel>();

                        // Loopar igenom varje användare och lägger till dessa i listan ovan.
                        foreach (var user in userList)
                        {
                            UserModel userToAdd = new UserModel
                            {
                                ID = user.ID,
                                Username = user.Username,
                                IsAdmin = user.IsAdmin
                            };

                            userListToReturn.Add(userToAdd);
                        }

                        // Returnerar listan med användare och OK, om det går igenom.
                        return Request.CreateResponse(HttpStatusCode.OK, userListToReturn);
                    }
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetAllUsers

        #region GetSelectedUser
        [HttpPost]
        [ActionName("GetSelectedUser")]
        [ResponseType(typeof(UserModel))]
        [Authorize(Roles = "User")]
        public HttpResponseMessage GetSelectedUser([FromBody]UserModel user)
        {
            if (ClaimsPrincipal.Current.IsInRole("User"))
            {
                try
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        // Hämtar vald user.
                        var selectedUser = db.User.Where(x => x.Username == user.Username).SingleOrDefault();

                        if (selectedUser != null)
                        {
                            UserModel userToReturn = new UserModel
                            {
                                ID = selectedUser.ID,
                                Username = selectedUser.Username,
                                IsAdmin = selectedUser.IsAdmin,
                                Salt = selectedUser.Salt
                            };

                            return Request.CreateResponse(HttpStatusCode.OK, userToReturn);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
                catch (Exception e)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
        #endregion GetSelectedUser
    }
}