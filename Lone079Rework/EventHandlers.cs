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

    private static readonly List<RoleTypeId> Scp079Respawns =
    [
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp939
    ];
    
    private static IEnumerator<float> OnCheck079(float delay = 0.5f)
    {
        yield return Timing.WaitForSeconds(delay);

        if (_canChange)
        {
            var scpPlayers = Player.List.Where(x => x.Role.Team == Team.SCPs);

            if (!Lone079.Instance.Config.CountZombies)
                scpPlayers = scpPlayers.Where(x => x.Role != RoleTypeId.Scp0492);

            var scpList = scpPlayers.ToList();

            if (scpList.Count == 1 && scpList[0].Role == RoleTypeId.Scp079)
            {
                var player = scpList[0];
                var level = player.Role.As<Scp079Role>().Level;

                var existingScps = Player.List
                    .Where(p => p.Role.Team == Team.SCPs && p.Role != RoleTypeId.Scp079 && p.Role != RoleTypeId.Scp0492)
                    .Select(p => p.Role.Type)
                    .ToList();

                var availableRoles = Scp079Respawns.Except(existingScps).ToList();

                if (availableRoles.Count > 0)
                {
                    var role = availableRoles[Rand.Next(availableRoles.Count)];
                    Log.Debug(
                        $"Transforming SCP-079 into {role} with {Lone079.Instance.Config.HealthPercent}% health.");
                    player.Role.Set(role);

                    var healthMultiplier = Lone079.Instance.Config.ScaleWithLevel
                        ? (Lone079.Instance.Config.HealthPercent + (level - 1) * 5) / 100f
                        : Lone079.Instance.Config.HealthPercent / 100f;

                    player.Health = player.MaxHealth * healthMultiplier;

                    Log.Debug($"Broadcasting message to SCP-079: {Lone079.Instance.Config.BroadcastMessage}");
                    player.Broadcast(Lone079.Instance.Config.BroadcastDuration,
                        Lone079.Instance.Config.BroadcastMessage);
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
        else
        {
            Log.Debug("Conditions for SCP-079 respawn are not met (_canChange is false).");
        }
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

    public static void OnPlayerDied(DiedEventArgs ev)
    {
        Log.Debug($"Player {ev.Player.Nickname} died. Checking if SCP-079 needs to be transformed.");
        Timing.RunCoroutine(OnCheck079(Lone079.Instance.Config.RespawnDelay));
    }

    public static void OnCassie(SendingCassieMessageEventArgs ev)
    {
        if (!ev.Words.Contains("allgeneratorsengaged")) return;
        Log.Debug("Blocking CASSIE message: 'allgeneratorsengaged'.");
        ev.IsAllowed = false;
    }

    public static void OnRecontaining(RecontainingEventArgs ev)
    {
        Log.Debug("Blocking SCP-079 recontainment.");
        ev.IsAllowed = false;
        if (!_canChange) return;
        if (!Lone079.Instance.Config.TransformOnRecontain) return;
        var player = ev.Player;
        if (player == null || player.Role != RoleTypeId.Scp079) return;
        var existingScps = Player.List
            .Where(p => p.Role.Team == Team.SCPs && p.Role != RoleTypeId.Scp079 && p.Role != RoleTypeId.Scp0492)
            .Select(p => p.Role.Type)
            .ToList();

        var availableRoles = Scp079Respawns.Except(existingScps).ToList();

        if (availableRoles.Count > 0)
        {
            var role = availableRoles[Rand.Next(availableRoles.Count)];

            Log.Debug($"Transforming SCP-079 into {role} after recontainment.");
            player.Role.Set(role);

            var healthMultiplier = Lone079.Instance.Config.HealthPercent / 100f;
            player.Health = player.MaxHealth * healthMultiplier;

            Log.Debug($"Broadcasting message to SCP-079: {Lone079.Instance.Config.BroadcastMessage}");
            player.Broadcast(Lone079.Instance.Config.BroadcastDuration,
                Lone079.Instance.Config.BroadcastMessage);
        }
        else
        {
            Log.Debug("No available SCP roles to transform SCP-079 into after recontainment.");
        }
    }
}