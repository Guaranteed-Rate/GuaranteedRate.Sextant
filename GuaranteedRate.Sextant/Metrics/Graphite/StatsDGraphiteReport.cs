using System;
using System.Collections.Generic;
using System.Linq;
using Metrics.Graphite;

namespace GuaranteedRate.Sextant.Metrics.Graphite
{
    public class StatsDGraphiteReport : GraphiteReport
    {
        private string _rootNamespace;

        public StatsDGraphiteReport(GraphiteSender sender, string rootNamespace) : base(sender)
        {
            _rootNamespace = rootNamespace;
        }

        protected override void Send(string name, string value)
        {
            var sendName = string.IsNullOrEmpty(_rootNamespace)
                ? name
                : $"{_rootNamespace}.{name}";

            base.Send(sendName, value);
        }
    }
}
