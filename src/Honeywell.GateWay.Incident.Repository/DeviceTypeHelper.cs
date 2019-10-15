namespace Honeywell.GateWay.Incident.Repository
{
    public class DeviceTypeHelper
    {
        private const string SystemName = "Prowatch";
        private const string UnderScore = "_";

        public static string GetSystemDeviceType(string deviceType)
        {
            return $"{SystemName}{UnderScore}{deviceType}";
        }
    }
}
