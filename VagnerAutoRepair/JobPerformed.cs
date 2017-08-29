using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VagnerAutoRepair
{
    [Serializable]
    public class JobPerformed
    {
        public string Job { get; set; }
        public double Cost { get; set; }
    }
}
