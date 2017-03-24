using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Util.Rules
{
    /// <summary>
    /// PONO for rule details
    /// </summary>
    public class RuleDetails
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public int id { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime LastModified { get; set; }
        public string Status { get; set; }
        public string RuleXml { get; set; }
    }
}
