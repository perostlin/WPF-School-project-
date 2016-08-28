using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WpfApi.Models
{
    public class UserModel
    {
        public Guid ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsAdmin { get; set; }
    }
}