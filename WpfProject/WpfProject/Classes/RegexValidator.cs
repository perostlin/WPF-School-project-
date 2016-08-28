using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfProject.Classes
{
    class RegexValidator
    {
        public bool IsValidLocalHost(string uriAddress)
        {
            var localHostPattern = new Regex(@"^http(s?)\:\/\/(([a-zA-Z0-9\-\._]+(\.[a-zA-Z0-9\-\._]+)+)|localhost):\d+(\/?)$", RegexOptions.Compiled);
            return localHostPattern.IsMatch(ConfigurationManager.AppSettings["ApiAddress"]);
        }

        public bool IsValidAzure(string uriAddress)
        {
            var azurePattern = new Regex(@"^http(s?)\:\/\/(([a-zA-Z0-9\-\._]+(\.[a-zA-Z0-9\-\._]+)+)|azurewebsites.net)(\/?)$", RegexOptions.Compiled);
            return azurePattern.IsMatch(ConfigurationManager.AppSettings["ApiAddress"]);
        }
    }
}
