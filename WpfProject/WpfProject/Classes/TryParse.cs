using System;

namespace WpfProject.Classes
{
    public class TryParse
    {
        public bool IsValidDateTime(string value)
        {
            DateTime valueToCheck;
            DateTime.TryParse(value, out valueToCheck);

            if (valueToCheck != null)
            {
                return true;
            }
            return false;
        }
        public bool IsValidInteger(string value)
        {
            int valueToReturn = 0;
            if(int.TryParse(value, out valueToReturn))
            {
                return true;
            }

            return false;
        }

        public bool IsValidDecimal(string value)
        {
            decimal valueToReturn = 0;
            if (decimal.TryParse(value, out valueToReturn))
            {
                return true;
            }

            return false;
        }
    }
}
