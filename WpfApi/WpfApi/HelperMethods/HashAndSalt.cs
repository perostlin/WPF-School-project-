using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace WpfApi.HelperMethods
{
    public class HashAndSalt
    {
        public string GetHashedPassword(string password)
        {
            HashAlgorithm hash = new SHA256Managed();
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);

            //in this string you got the encrypted password
            string hashValue = Convert.ToBase64String(hashBytes);

            return hashValue;
        }

        public string GetSaltedPassword()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}