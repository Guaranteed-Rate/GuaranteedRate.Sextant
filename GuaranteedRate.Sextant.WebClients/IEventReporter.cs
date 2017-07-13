namespace GuaranteedRate.Sextant.WebClients
{
    public interface IEventReporter
    {
        bool ReportEvent(object formattedData);
        void Shutdown();
    }
}