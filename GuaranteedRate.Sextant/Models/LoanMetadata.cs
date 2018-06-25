using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Models
{
    public class LoanMetadata
    {
        public string LoanNumber { get; set; }
        public string Guid { get; set; }
        public string LoanFolder { get; set; }
        public DateTime LastModified { get; set; }
    }
}
