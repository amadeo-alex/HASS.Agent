using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Agent.Base.Enums;
public enum HassDomain
{
    [EnumMember(Value = "automation")]
    Automation,

    [EnumMember(Value = "climate")]
    Climate,

    [EnumMember(Value = "cover")]
    Cover,

    [EnumMember(Value = "input_boolean")]
    InputBoolean,

    [EnumMember(Value = "input_select")]
    InputSelect,

    [EnumMember(Value = "light")]
    Light,

    [EnumMember(Value = "media_player")]
    MediaPlayer,

    [EnumMember(Value = "scene")]
    Scene,

    [EnumMember(Value = "script")]
    Script,

    [EnumMember(Value = "switch")]
    Switch,

    [EnumMember(Value = "sensor")]
    Sensor,

    [EnumMember(Value = "button")]
    Button,

    [EnumMember(Value = "select")]
    Select,

    [EnumMember(Value = "fan")]
    Fan
}
