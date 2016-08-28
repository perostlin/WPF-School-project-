using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfProject.Model
{
    public class ReportDriverJournalModel
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public int Milage { get; set; }
        public decimal FuelAmount { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid ChauffeurID { get; set; }
        public Guid VehicleID { get; set; }
        public int FuelTypeID { get; set; }
    }
}
