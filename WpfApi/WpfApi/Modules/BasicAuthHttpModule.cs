﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using WpfApi.HelperMethods;

namespace WpfApi.Modules
{
    public class BasicAuthHttpModule : IHttpModule
    {
        private const string Realm = "WpfApi";
        
        public void Init(HttpApplication context)
        {
            // Register event handlers
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        // TODO: Here is where you would validate the username and password.
        private static bool CheckPassword(string username, string password)
        {
            using (var db = new WpfProjectDatabaseEntities())
            {
                string salt = db.User.Where(x => x.Username == username).Select(x => x.Salt).SingleOrDefault();

                HashAndSalt hasher = new HashAndSalt();

                string passwordAndSalt = password + salt;
                string hashedPassword = hasher.GetHashedPassword(passwordAndSalt);

                if (db.User.Any(x => x.Username == username && x.Password == hashedPassword))
                {
                    return true;
                }
                return false;
            }
        }

        private static void AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                int separator = credentials.IndexOf(':');
                string name = credentials.Substring(0, separator);
                string password = credentials.Substring(separator + 1);

                if (CheckPassword(name, password))
                {
                    using (var db = new WpfProjectDatabaseEntities())
                    {
                        string salt = db.User.Where(x => x.Username == name).Select(x => x.Salt).SingleOrDefault();

                        HashAndSalt hasher = new HashAndSalt();

                        string passwordAndSalt = password + salt;
                        string hashedPassword = hasher.GetHashedPassword(passwordAndSalt);

                        var currentUserIsAdmin =
                            db.User.Where(x => x.Username == name && x.Password == hashedPassword)
                                .Select(x => x.IsAdmin)
                                .SingleOrDefault();

                        var identity = new GenericIdentity(name);
                        if (currentUserIsAdmin)
                        {
                            SetPrincipal(new GenericPrincipal(identity, new[] { "Admin", "User" }));
                        }
                        else
                        {
                            SetPrincipal(new GenericPrincipal(identity, new[] { "User" }));
                        }
                    }
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header 
        // to the response.
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",
                    string.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        public void Dispose()
        {
        }
    }
}