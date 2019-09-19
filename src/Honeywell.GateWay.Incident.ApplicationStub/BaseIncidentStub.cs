using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class BaseIncidentStub
    {
        protected Task<T> StubData<T>()
        {
            var name = nameof(T);
            using StreamReader r = new StreamReader($"{name}.json");
            var json = r.ReadToEnd();
            T items = JsonConvert.DeserializeObject<T>(json);
            return Task.FromResult(items);
        }

    }
}
