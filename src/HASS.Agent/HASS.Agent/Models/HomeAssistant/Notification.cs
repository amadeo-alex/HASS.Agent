using Newtonsoft.Json;

namespace HASS.Agent.Models.HomeAssistant
{
    public class NotificationAction
    {
        public string Action { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
    }

    public class NotificationInput
    {
        public string Id { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class NotificationData
    {
        public const string NoAction = "noAction";
        public const string ImportanceHigh = "high";

        public int Duration { get; set; } = 0;
        public string Image { get; set; } = string.Empty;
        public string ClickAction { get; set; } = NoAction;
        public string Tag { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; } = string.Empty;
        public bool Sticky { get; set; }
        public string Importance { get; set; } = string.Empty;

        public List<NotificationAction> Actions { get; set; } = new List<NotificationAction>();

        public List<NotificationInput> Inputs { get; set; } = new List<NotificationInput>();
    }

    public class Notification
    {
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public NotificationData? Data { get; set; }
    }
}
