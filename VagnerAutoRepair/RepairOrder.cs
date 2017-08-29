using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VagnerAutoRepair
{
    [Serializable]
    public class RepairOrder
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }              
        public string State { get; set; }
        public string ZIPCode { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }        
        public int Year { get; set; }
        public string ProblemDescription { get; set; }
        public double TotalParts { get; set; }
        public double TotalLabor { get; set; }
        public double TaxRate { get; set; }
        public double TaxAmount { get; set; }
        public double RepairTotal { get; set; }
        public string Recommendations { get; set; }

        public List<Part> Parts = new List<Part>();

        public List<JobPerformed> Jobs = new List<JobPerformed>();


    }
}
