using System;
using Serilog.Core;
using Serilog.Events;

public class HostNameEnricher : ILogEventEnricher
{
    LogEventProperty _cachedProperty;

    /// <summary>
    /// The property name added to enriched log events.
    /// </summary>
    public const string MachineNamePropertyName = "hostname";

    /// <summary>
    /// Enrich the log event.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {

        var machineName = Environment.GetEnvironmentVariable("COMPUTERNAME");
        if (string.IsNullOrWhiteSpace(machineName))
            machineName = Environment.GetEnvironmentVariable("HOSTNAME");

        _cachedProperty = _cachedProperty ?? propertyFactory.CreateProperty(MachineNamePropertyName, machineName);
        logEvent.AddPropertyIfAbsent(_cachedProperty);
    }
}