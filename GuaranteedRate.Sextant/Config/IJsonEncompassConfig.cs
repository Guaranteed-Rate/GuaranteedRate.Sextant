
namespace GuaranteedRate.Sextant.Config
{
    public interface IJsonEncompassConfig : IEncompassConfig
    {
        T GetValue<T>(string key, T defaultValue = default(T));
    }
}
