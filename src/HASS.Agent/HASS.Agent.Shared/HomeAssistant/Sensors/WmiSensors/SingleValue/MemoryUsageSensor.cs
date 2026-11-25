using System.Globalization;
using System.Management;
using HASS.Agent.Shared.Models.HomeAssistant;

namespace HASS.Agent.Shared.HomeAssistant.Sensors.WmiSensors.SingleValue
{
    /// <summary>
    /// Sensor containing the amount of used memory in MB
    /// </summary>
    public class MemoryUsageSensor : WmiQuerySensor
    {
        private const string DefaultName = "memoryusage";

        public MemoryUsageSensor(int? updateInterval = null, string entityName = DefaultName, string? name = DefaultName, string? id = null, bool applyRounding = false, int? round = null, string? advancedSettings = null) : base("SELECT FreePhysicalMemory,TotalVisibleMemorySize FROM Win32_OperatingSystem", string.Empty, applyRounding, round, updateInterval ?? 30, entityName ?? DefaultName, name, id, advancedSettings: advancedSettings) { }

        public override DiscoveryConfigModel? GetAutoDiscoveryConfig()
        {
            if (Variables.MqttManager == null) return null;

            var deviceConfig = Variables.MqttManager.GetDeviceConfigModel();
            if (deviceConfig == null) return null;

            return AutoDiscoveryConfigModel ?? SetAutoDiscoveryConfigModel(new SensorDiscoveryConfigModel(Domain)
            {
                EntityName = EntityName,
                Name = Name,
                Unique_id = Id,
                Device = deviceConfig,
                State_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/{ObjectId}/state",
                State_class = "measurement",
                Icon = "mdi:memory",
                Unit_of_measurement = "%",
                Availability_topic = $"{Variables.MqttManager.MqttDiscoveryPrefix()}/{Domain}/{deviceConfig.Name}/availability"
            });
        }

        public override string GetState()
        {
            using var collection = Searcher.Get();
            ulong totalMemory = 0;
            ulong freeMemory = 0;

            foreach (var o in collection)
            {
                var mo = (ManagementObject)o;
                try
                {
                    totalMemory = (ulong)(mo.Properties["TotalVisibleMemorySize"].Value ?? 0);
                    freeMemory = (ulong)(mo.Properties["FreePhysicalMemory"].Value ?? 0);
                }
                finally
                {
                    mo?.Dispose();
                }
            }

            if (totalMemory == 0 || freeMemory == 0)
            {
                return string.Empty;
            }

            decimal totalMemoryDec = totalMemory; //NOTE(Amadeo): no idea why, check at some point if this is even required
            decimal freeMemoryDec = freeMemory;
            
            var percentageUsed = 100 - (freeMemoryDec / totalMemoryDec) * 100;
            return percentageUsed.ToString("#.##", CultureInfo.InvariantCulture);
        }
    }
}
