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
    private static CoroutineHandle _checkCoroutine;

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

            if (Lone079.Instance.Config.Scp079AvailableRoles.Count > 0)
            {
                var role = Lone079.Instance.Config.Scp079AvailableRoles[
                    Rand.Next(Lone079.Instance.Config.Scp079AvailableRoles.Count)];
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
            ? ((Lone079.Instance.Config.HealthPercent + ((level - 1) * 5)) / 100f)
            : (Lone079.Instance.Config.HealthPercent / 100f);

        player.Health = player.MaxHealth * healthMultiplier;
        
        player.Broadcast(Lone079.Instance.Config.BroadcastDuration, Lone079.Instance.Config.BroadcastMessage);
    }

    public static void OnPlayerDying(DyingEventArgs ev)
    {
        if (ev.Player.Role.Team != Team.SCPs) return;
        Log.Debug($"Player {ev.Player.Nickname} (SCP) died. Checking if SCP-079 needs to be transformed.");
        if (_checkCoroutine.IsRunning) Timing.KillCoroutines(_checkCoroutine);
        _checkCoroutine = Timing.RunCoroutine(OnCheck079(Lone079.Instance.Config.RespawnDelay));
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
        var player = ev.Player;
        if (player == null) return;
        
        var scpPlayers = Player.Get(Team.SCPs).Where(p => p.Role != RoleTypeId.Scp079);

        if (!Lone079.Instance.Config.CountZombies)
            scpPlayers = scpPlayers.Where(p => p.Role != RoleTypeId.Scp0492);

        var scpList = scpPlayers.ToList();
        
        if (scpList.Count == 0)
        {
            Log.Debug("SCP-079 is the last SCP. Blocking recontainment and transforming.");
            ev.IsAllowed = false;

            if (Lone079.Instance.Config.Scp079AvailableRoles.Count > 0)
            {
                var role = Lone079.Instance.Config.Scp079AvailableRoles[Rand.Next(Lone079.Instance.Config.Scp079AvailableRoles.Count)];
                TransformScp079(player, role);
            }
            else
            {
                Log.Debug("No available SCP roles to transform SCP-079 into after recontainment.");
            }
        }
        else if (Lone079.Instance.Config.TransformOnRecontain)
        {
            Log.Debug("TransformOnRecontain is enabled. Blocking recontainment and transforming.");
            ev.IsAllowed = false;

            if (Lone079.Instance.Config.Scp079AvailableRoles.Count > 0)
            {
                var role = Lone079.Instance.Config.Scp079AvailableRoles[Rand.Next(Lone079.Instance.Config.Scp079AvailableRoles.Count)];
                TransformScp079(player, role);
            }
            else
            {
                Log.Debug("No available SCP roles to transform SCP-079 into after recontainment.");
            }
        }
        else
        {
            Log.Debug("TransformOnRecontain is disabled. Allowing recontainment.");
            ev.IsAllowed = true;
        }
    }
}