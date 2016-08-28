using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WpfApi.Models
{
    public class OtherCostModel
    {
        public DateTime Date { get; set; }
        public string Cost { get; set; }
        public string Comment { get; set; }
        public Guid VehicleID { get; set; }
        public int TypeOfCostID { get; set; }
    }
}