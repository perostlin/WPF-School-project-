using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = string.Empty;
            string hashedPassword = string.Empty;
            string salt = string.Empty;

            Console.WriteLine("Type in the wished password:");
            password = Console.ReadLine();

            // Hash and salt.

            if (password != null)
            {
                // Set the salt.
                salt = GetSaltedPassword();

                string passwordWithSalt = password + salt;
                hashedPassword = GetHashedPassword(passwordWithSalt);

                Console.WriteLine("You´re salt: " + salt);
                Console.WriteLine("You´re hashed and salted password: " + hashedPassword);

                SaveFile(hashedPassword, salt);
            }

            Console.Read();
        }

        public static string GetHashedPassword(string password)
        {
            HashAlgorithm hash = new SHA256Managed();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);

            //in this string you got the encrypted password
            string hashValue = Convert.ToBase64String(hashBytes);

            return hashValue;
        }

        public static string GetSaltedPassword()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            var salt = new byte[16];
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public static void SaveFile(string password, string salt)
        {
            // Specify a name for your top-level folder. 
            //string folderName = @"C:\Users\Per\Desktop\WPFProjekt\ConsoleApplication1\ConsoleApplication1\Password";

            //// To create a string that specifies the path to a subfolder under your  
            //// top-level folder, add a name for the subfolder to folderName. 
            //string pathString = System.IO.Path.Combine(folderName, "SubFolder");

            //System.IO.Directory.CreateDirectory(pathString);
            
            //string fileName = "password.txt";
            
            //pathString = System.IO.Path.Combine(pathString, fileName);
            
            //Console.WriteLine("Path to my file: {0}\n", pathString);

            //if (!System.IO.File.Exists(pathString))
            //{
                using (StreamWriter sw = new StreamWriter(@"C:\Users\Per\Desktop\WPFProjekt\ConsoleApplication1\ConsoleApplication1\path.txt", true, UTF8Encoding.UTF8))
                {
                    sw.WriteLine(password);
                    sw.WriteLine(salt);
                }
            //}
            //else
            //{
            //    Console.WriteLine("File \"{0}\" already exists.", fileName);
            //    return;
            //}

            //// Read and display the data from your file. 
            //try
            //{
            //    byte[] readBuffer = System.IO.File.ReadAllBytes(pathString);
            //    foreach (byte b in readBuffer)
            //    {
            //        Console.Write(b + " ");
            //    }
            //    Console.WriteLine();
            //}
            //catch (System.IO.IOException e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            // Keep the console window open in debug mode.
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}


