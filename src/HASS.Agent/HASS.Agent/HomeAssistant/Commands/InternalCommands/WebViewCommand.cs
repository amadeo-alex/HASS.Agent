using HASS.Agent.Functions;
using HASS.Agent.Managers;
using HASS.Agent.Models.Internal;
using HASS.Agent.Shared.Enums;
using HASS.Agent.Shared.HomeAssistant.Commands;
using Newtonsoft.Json;
using Serilog;

namespace HASS.Agent.HomeAssistant.Commands.InternalCommands
{
    internal class WebViewCommand : InternalCommand
    {
        private const string DefaultName = "webview";
        private readonly WebViewInfo _webViewInfo = new();

        internal WebViewCommand(string entityName = DefaultName, string? name = DefaultName, string webViewInfo = "", CommandEntityType entityType = CommandEntityType.Switch, string? id = null) : base(entityName ?? DefaultName, name, webViewInfo, entityType, id)
        {
            CommandConfig = webViewInfo;
            State = "OFF";

            if (string.IsNullOrEmpty(webViewInfo)) return;
            var webViewPackage = JsonConvert.DeserializeObject<WebViewInfo>(webViewInfo);
            if (webViewPackage == null)
            {
                return;
            }

            webViewPackage.Id = entityName ?? DefaultName;
            _webViewInfo = webViewPackage;
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(_webViewInfo.Url))
            {
                Log.Error("[WEBVIEW] [{name}] Unable to launch webview, it's configured as action-only", EntityName);

                State = "OFF";
                return;
            }

            WebViewManager.LaunchWebView(_webViewInfo);
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(action) && string.IsNullOrWhiteSpace(_webViewInfo.Url))
            {
                Log.Error("[WEBVIEW] [{name}] Unable to launch URL, it's configured as action-only but no URL is provided", EntityName);

                State = "OFF";
                return;
            }

            // prepare url
            var url = string.IsNullOrWhiteSpace(_webViewInfo.Url) ? action : $"{_webViewInfo.Url} {action}";

            WebViewManager.LaunchWebView(_webViewInfo, url);
        }

        public override void TurnOff()
        {
            if (State == "OFF")
            {
                return;
            }
            
            WebViewManager.CloseWebView(_webViewInfo);
            State = "OFF";
        }
    }
}
