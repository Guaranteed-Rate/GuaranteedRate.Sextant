using EllieMae.Encompass.Automation;
using GuaranteedRate.Sextant.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.PluginFramework
{
    /// <summary>
    /// The FieldChangeDispatcher monitors for field changes and dispatches events
    /// to interested listeners.
    /// 
    /// This is a (hopefully) more efficient system than having all listeners registered
    /// for all field change events and then discarding events that they are not interested in.
    /// </summary>
    public class FieldChangeDispatcher
    {
        private readonly IDictionary<string, ICollection<IFieldWatcher>> _listeners;

        public FieldChangeDispatcher()
        {
            _listeners = new Dictionary<string, ICollection<IFieldWatcher>>();

            //Register for callbacks whenever a loan is opened or closed
            EncompassApplication.LoanOpened += EncompassApplication_LoanOpened;
            EncompassApplication.LoanClosing += EncompassApplication_LoanClosing;
        }

        /// <summary>
        /// This is the meat of the dispatcher and allows a class to register for a callback.
        /// A class can register to watch 1 or more field.
        /// </summary>
        /// <param name="watcher"></param>
        public void Register(IFieldWatcher watcher)
        {
            if (watcher==null || watcher.WatchedFields() == null) {
                Loggly.Error("FieldChangeDispatcher","Error, attempt to register watcher with nothing to watch");
            }
            foreach (string field in watcher.WatchedFields())
            {
                ICollection<IFieldWatcher> watchers;
                if (_listeners.ContainsKey(field))
                {
                    watchers = _listeners[field];
                }
                else
                {
                    watchers = new List<IFieldWatcher>();
                    _listeners.Add(field, watchers);
                }
                watchers.Add(watcher);
            }
        }

        public void EncompassApplication_LoanOpened(object sender, EventArgs e)
        {
            try
            {
                EncompassApplication.CurrentLoan.FieldChange += CurrentLoan_FieldChange;
            }
            catch (Exception ex)
            {
                Loggly.Error("FieldChangeDispatcher", "Exception in EncompassApplication_LoanOpened " + ex);
            }
        }

        public void EncompassApplication_LoanClosing(object sender, EventArgs e)
        {
            try
            {
                EncompassApplication.CurrentLoan.FieldChange -= CurrentLoan_FieldChange;
            }
            catch (Exception ex)
            {
                Loggly.Error("FieldChangeDispatcher", "Exception in EncompassApplication_LoanClosing " + ex);
            }
        }

        /// <summary>
        /// This method does the actual dispatching to inteterested listeners.
        /// A single callback from Encompass, with a single hash lookup should perform much better than
        /// 30-50 callbacks, each with a distinct 'if' check to see if the changed field was the one the class is
        /// interested in.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CurrentLoan_FieldChange(object source, EllieMae.Encompass.BusinessObjects.Loans.FieldChangeEventArgs e)
        {
            try
            {
                if (_listeners.ContainsKey(e.FieldID))
                {
                    foreach (IFieldWatcher watcher in _listeners[e.FieldID])
                    {
                        watcher.OnFieldChange(source, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Loggly.Error("FieldChangeDispatcher", "Exception in CurrentLoan_FieldChange " + ex);
            }
        }
    }
}
