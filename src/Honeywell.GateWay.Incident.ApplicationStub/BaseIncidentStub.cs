using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class BaseIncidentStub
    {
        protected Task<T> StubData<T>()
        {
            var type = typeof(T);
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filePath = Path.Combine(assemblyFolder, "StubData", $"{type.Name}.json");
            using StreamReader r = new StreamReader(filePath);
            var json = r.ReadToEnd();
            T items = JsonConvert.DeserializeObject<T>(json);
            return Task.FromResult(items);
        }
    }
}
