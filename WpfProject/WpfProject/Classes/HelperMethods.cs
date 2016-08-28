using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using WpfProject.Model;

namespace WpfProject.Classes
{
    public class HelperMethods
    {
        public static HttpClient GetClient(UserModel user)
        {
            string username = user.Username;
            string password = user.Password;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiAddress"]);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //Setting authorization header
            string credentials = username + ":" + password;
            var credentialBytes = Encoding.UTF8.GetBytes(credentials);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentialBytes));

            return client;
        }
    }
}
