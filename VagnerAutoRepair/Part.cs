using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VagnerAutoRepair
{
    [Serializable]
    public class Part
    {
        public string PartName { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
    }
}
