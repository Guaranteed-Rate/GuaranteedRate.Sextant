using System;
using System.Collections.Generic;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Reporting;

namespace GuaranteedRate.Sextant.CustomFieldComparer
{
    /// <summary>
    /// Merge FieldDescriptor and IReportingFieldDescriptor.
    /// A field can have 0->many reporting fields
    /// </summary>
    public class FullFieldDescription
    {
        ///FieldDescriptor instead of IFieldDescriptor because not all fields 
        /// are in the interface
        public FieldDescriptor fieldDescriptor { get; set; }
        public IList<IReportingFieldDescriptor> reportingFields { get; set; }
    }
}
