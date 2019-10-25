using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Honeywell.Infra.Core.Common.Exceptions;
using Newtonsoft.Json;

namespace Honeywell.GateWay.Incident.ApplicationStub
{
    public class BaseIncidentStub
    {
        protected T StubData<T>()
        {
            var type = typeof(T);
            if (type.IsGenericType)
            {
                type = type.GenericTypeArguments[0];
            }
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (type != null)
            {
                var filePath = Path.Combine(assemblyFolder, "StubData", $"{type.Name}.json");
                using var reader = new StreamReader(filePath);
                var json = reader.ReadToEnd();
                var items = JsonConvert.DeserializeObject<T>(json);
                return items;
            }
            throw new HoneywellException($"can not identity the type {typeof(T).FullName}");
        }

        protected Task<T> StubDataTask<T>()
        {
            var result = StubData<T>();
            return Task.FromResult(result);
        }
    }
}
