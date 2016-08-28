using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBilKlient
{
    public class Bil
    {
        public int Id { get; set; }
        public string Registreringsnummer { get; set; }
        public int Arsmodell { get; set; }
        public string Modellbeteckning { get; set; }
        public string Marke { get; set; }
        public string Farg { get; set; }
        public bool Metalliclack { get; set; }
    }
}
