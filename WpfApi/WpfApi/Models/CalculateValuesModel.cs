using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WpfApi.Models
{
    public class CalculateValuesModel
    {
        public Guid VehicleID { get; set; }
        public Guid ChauffeurID { get; set; }
        public int VehicleTypeID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal AverageFuelValue { get; set; }
        public decimal TotalFuelValue { get; set; }
        public decimal TotalMonthlyVehicleCost { get; set; }
        public decimal TotalYearlyVehicleCost { get; set; }
        public decimal AverageFuelConsumption { get; set; }
        public decimal AverageFuelCost { get; set; }
    }
}