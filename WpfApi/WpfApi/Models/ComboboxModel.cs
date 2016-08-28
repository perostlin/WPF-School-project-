using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WpfApi.Models
{
    public class ComboboxModel
    {
        public List<ColorModel> ColorList;
        public List<FuelTypeModel> FuelTypeList;
        public List<VehicleTypeModel> VehicleTypeList;
        public List<ModelYearModel> ModelYearList;

        public ComboboxModel()
        {
            ColorList = new List<ColorModel>();
            FuelTypeList = new List<FuelTypeModel>();
            VehicleTypeList = new List<VehicleTypeModel>();
            ModelYearList = new List<ModelYearModel>();
        }
    }
}