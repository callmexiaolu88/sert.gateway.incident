namespace Honeywell.GateWay.Incident.Repository.Device
{
    public class DevicesEntity
    {
        public DeviceEntity[] Config { get; set; }
    }

    public class DeviceEntity
    {
        public IdentifiersEntity Identifiers { get; set; }

        public RelationEntity[] Relation { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }
    }

    public class IdentifiersEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tag { get; set; }
    }

    public class RelationEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EntityId { get; set; }
    }
}
