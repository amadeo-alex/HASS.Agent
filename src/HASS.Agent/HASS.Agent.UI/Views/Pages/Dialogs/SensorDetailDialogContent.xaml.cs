using HASS.Agent.Base.Models;
using HASS.Agent.Base.Models.Entity;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HASS.Agent.UI.Views.Pages.Dialogs;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SensorDetailDialogContent : Page
{
    public ConfiguredEntity Entity { get; }
    public ContentControl? CustomDetails { get; set; }
    public bool CustomDetailsPresent => CustomDetails != null;

    public List<EntityCategory>? SensorsCategories { get; set; }
    public bool SensorsCategoriesPresent => SensorsCategories != null;

    public SensorDetailDialogContent(ConfiguredEntity entity)
    {
        Entity = CloneConfiguredEntity(entity);
        this.InitializeComponent();
    }

    private ConfiguredEntity CloneConfiguredEntity(ConfiguredEntity source) //TODO(Amadeo): think about returning clones from the settings manager
    {
        var jsonValue = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<ConfiguredEntity>(jsonValue) ?? new ConfiguredEntity();
    }
}
