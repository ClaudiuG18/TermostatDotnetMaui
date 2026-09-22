using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermostatAndroid.Model
{
    public class EspData
    {
        public required string id { get; set; }
        public double temp { get; set; }
        public double hum { get; set; }
        public double setpoint { get; set; }
        public double calibTemp { get; set; }
        public uint relay { get; set; }
    }
}
