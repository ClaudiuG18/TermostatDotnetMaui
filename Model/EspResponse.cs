using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermostatAndroid.Model
{
   public class EspResponse
    {
        public Dictionary<string, EspData> RawDataFromESP { get; set; } = new();
    }
}
