namespace GuaranteedRate.Sextant.WebClients
{
    public interface IEventReporter
    {
        bool ReportEvent(string formattedData);
        void Shutdown();
    }
}