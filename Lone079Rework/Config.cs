using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace Lone079Rework;

public class Config : IConfig
{
    [Description("Whether the plugin is enabled or not.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Whether debug messages should be shown in the server console.")]
    public bool Debug { get; set; } = false;

    [Description("Whether SCP-049-2 (zombies) should be counted as SCPs when checking if SCP-079 is the last SCP alive.")]
    public bool CountZombies { get; set; } = false;

    [Description("Whether SCP-079's respawn health should scale with its level. If true, health increases by 5% per level.")]
    public bool ScaleWithLevel { get; set; } = false;

    [Description("The base health percentage SCP-079 will respawn with. Default is 50%.")]
    public int HealthPercent { get; set; } = 50;

    [Description("The broadcast message shown to SCP-079 when it is respawned as another SCP.")]
    public string BroadcastMessage { get; set; } =
        "<i>You have been respawned as a random SCP with half health because all other SCPs have died.</i>";

    [Description("The duration (in seconds) of the broadcast message shown to SCP-079 when it is respawned.")]
    public ushort BroadcastDuration { get; set; } = 10;

    [Description("The delay (in seconds) before SCP-079 is respawned after being the last SCP alive.")]
    public float RespawnDelay { get; set; } = 1f;
    
    [Description("Whether SCP-079 should transform into another SCP when recontainment is blocked.")]
    public bool TransformOnRecontain { get; set; } = false;
    
    [Description("List of SCP roles that SCP-079 can transform into.")]
    public List<RoleTypeId> Scp079AvailableRoles { get; set; } =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939
    ];
}