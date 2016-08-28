using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Model
{
    public class VehicleModel
    {
        public Guid ID { get; set; }
        public string Description { get; set; }
        public string RegNo { get; set; }
        public int OriginalMilage { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleType { get; set; }
        public int FuelTypeID { get; set; }
        public string FuelType { get; set; }
        public int ModelYearID { get; set; }
        public int ModelYear { get; set; }
        public int ColorID { get; set; }
        public string Color { get; set; }
    }
}
