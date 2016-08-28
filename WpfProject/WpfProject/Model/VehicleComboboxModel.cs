using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Model
{
    public class VehicleComboboxModel
    {
        public List<ColorModel> ColorList;
        public List<FuelTypeModel> FuelTypeList;
        public List<VehicleTypeModel> VehicleTypeList;
        public List<ModelYearModel> ModelYearList;

        public VehicleComboboxModel()
        {
            ColorList = new List<ColorModel>();
            FuelTypeList = new List<FuelTypeModel>();
            VehicleTypeList = new List<VehicleTypeModel> ();
            ModelYearList = new List<ModelYearModel>();
        }
    }
}
