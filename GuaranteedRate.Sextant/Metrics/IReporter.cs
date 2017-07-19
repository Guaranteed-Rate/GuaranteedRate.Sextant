
namespace GuaranteedRate.Sextant.Metrics
{
    public interface IReporter
    {
        void AddCounter(string name, long value);
        void AddGauge(string name, long value);
        void AddMeter(string name, long value);
    }
}
