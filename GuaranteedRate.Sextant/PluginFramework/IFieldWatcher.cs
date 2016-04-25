using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.PluginFramework
{
    /// <summary>
    /// Simple interface for a class that wants to monitor 1 or more fields
    /// </summary>
    public interface IFieldWatcher
    {
        /// <summary>
        /// A collection of the fields that the class wants to watch for
        /// </summary>
        /// <returns>A collection of field ids</returns>
        ICollection<string> WatchedFields();

        /// <summary>
        /// When the Dispatcher detects a change that the class is watching for,
        /// this method is called
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e">The Field that has changed.</param>
        void OnFieldChange(object source, EllieMae.Encompass.BusinessObjects.Loans.FieldChangeEventArgs e);
    }
}
