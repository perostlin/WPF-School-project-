﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WpfApi.Models
{
    public class DeeperAnalysisModel
    {
        public Guid VehicleID { get; set; }
        public string RegNo { get; set; }
        public string VehicleType { get; set; }
        public int MilageLatestMonth { get; set; }
        public decimal FuelConsumptionLatestMonth { get; set; }
        public decimal FuelCostLatestMonth { get; set; }
        public string FuelType { get; set; }
    }
}