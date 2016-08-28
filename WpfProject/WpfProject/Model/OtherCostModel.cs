using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Model
{
    class OtherCostModel
    {
        public DateTime Date { get; set; }
        public string Cost { get; set; }
        public string Comment { get; set; }
        public Guid VehicleID { get; set; }
        public int TypeOfCostID { get; set; }
    }
}
