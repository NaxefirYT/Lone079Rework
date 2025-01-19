using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Cassie;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using MEC;
using PlayerRoles;
using Random = System.Random;

namespace Lone079Rework;

public static class EventHandlers
{
    private static readonly Random Rand = new();
    private static bool _canChange;

    private static readonly HashSet<RoleTypeId> Scp079Respawns =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939
    ];

    private static IEnumerator<float> OnCheck079(float delay = 0.5f)
    {
        yield return Timing.WaitForSeconds(delay);

        if (!_canChange)
        {
            Log.Debug("Conditions for SCP-079 respawn are not met (_canChange is false).");
            yield break;
        }
        
        var scpPlayers = Player.Get(Team.SCPs).Where(p => p.Role != RoleTypeId.Scp079);

        if (!Lone079.Instance.Config.CountZombies)
            scpPlayers = scpPlayers.Where(p => p.Role != RoleTypeId.Scp0492);

        var scpList = scpPlayers.ToList();
        
        if (scpList.Count == 0)
        {
            var player = Player.Get(Team.SCPs).FirstOrDefault(p => p.Role == RoleTypeId.Scp079);
            if (player == null)
            {
                Log.Debug("SCP-079 not found.");
                yield break;
            }
            
            if (Scp079Respawns.Count > 0)
            {
                var role = Scp079Respawns.ElementAt(Rand.Next(Scp079Respawns.Count));
                TransformScp079(player, role);
            }
            else
            {
                Log.Debug("No available SCP roles to transform SCP-079 into.");
            }
        }
        else
        {
            Log.Debug("SCP-079 is not the last SCP or conditions for respawn are not met.");
        }
    }

    private static void TransformScp079(Player player, RoleTypeId role)
    {
        var level = player.Role.As<Scp079Role>().Level;

        Log.Debug($"Transforming SCP-079 into {role} with {Lone079.Instance.Config.HealthPercent}% health.");
        player.Role.Set(role);

        var healthMultiplier = Lone079.Instance.Config.ScaleWithLevel
            ? (Lone079.Instance.Config.HealthPercent + (level - 1) * 5) / 100f
            : Lone079.Instance.Config.HealthPercent / 100f;

        player.Health = player.MaxHealth * healthMultiplier;

        Log.Debug($"Broadcasting message to SCP-079: {Lone079.Instance.Config.BroadcastMessage}");
        player.Broadcast(Lone079.Instance.Config.BroadcastDuration, Lone079.Instance.Config.BroadcastMessage);
    }

    public static void OnPlayerDying(DyingEventArgs ev)
    {
        if (ev.Player.Role.Team != Team.SCPs) return;
        Log.Debug($"Player {ev.Player.Nickname} (SCP) died. Checking if SCP-079 needs to be transformed.");
        Timing.RunCoroutine(OnCheck079(Lone079.Instance.Config.RespawnDelay));
    }

    public static void OnPlayerLeave(LeftEventArgs ev)
    {
        if (ev.Player.Role.Team != Team.SCPs) return;
        Log.Debug($"Player {ev.Player.Nickname} left. Checking if SCP-079 needs to be transformed.");
        Timing.RunCoroutine(OnCheck079(Lone079.Instance.Config.RespawnDelay));
    }

    public static void OnDetonated()
    {
        Log.Debug("Warhead detonated. Disabling SCP-079 transformation.");
        _canChange = false;
    }

    public static void OnRoundStart()
    {
        Log.Debug("Round started. Initializing SCP-079 transformation settings.");
        _canChange = true;
    }

    public static void OnCassie(SendingCassieMessageEventArgs ev)
    {
        if (!ev.Words.Contains("allgeneratorsengaged")) return;
        Log.Debug("Blocking CASSIE message: 'allgeneratorsengaged'.");
        ev.IsAllowed = false;
    }

    public static void OnRecontaining(RecontainingEventArgs ev)
    {
        if (!_canChange || !Lone079.Instance.Config.TransformOnRecontain) return;
        Log.Debug("Blocking SCP-079 recontainment.");
        ev.IsAllowed = false;
        
        var player = ev.Player;
        if (player == null || player.Role != RoleTypeId.Scp079) return;

        if (Scp079Respawns.Count > 0)
        {
            var role = Scp079Respawns.ElementAt(Rand.Next(Scp079Respawns.Count));
            TransformScp079(player, role);
        }
        else
        {
            Log.Debug("No available SCP roles to transform SCP-079 into after recontainment.");
        }
    }
}