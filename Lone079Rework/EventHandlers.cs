using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp079;
using MEC;
using PlayerRoles;
using Random = System.Random;

namespace Lone079Rework;

public static class EventHandlers
{
    private static readonly Random Random = new();
    private static bool _isTransformationAllowed;
    private static CoroutineHandle _checkCoroutine;

    private static IEnumerator<float> CheckForScp079Transformation(float delay = 0.5f)
    {
        yield return Timing.WaitForSeconds(delay);

        if (!_isTransformationAllowed)
        {
            Log.Debug("[SCP-079] Transformation conditions not met.");
            yield break;
        }

        var scpTeam = Player.Get(Team.SCPs)
            .Where(p => Lone079.Instance.Config.CountZombies || p.Role != RoleTypeId.Scp0492)
            .Where(p => p.Role != RoleTypeId.Scp079)
            .ToList();

        if (scpTeam.Count > 0)
        {
            Log.Debug("[SCP-079] Other SCPs still alive.");
            yield break;
        }

        var scp079 = Player.Get(Team.SCPs).FirstOrDefault(p => p.IsScp079());
        if (scp079 == null)
        {
            Log.Debug("[SCP-079] SCP-079 not found.");
            yield break;
        }

        if (!TryGetRandomScpRole(out var newRole))
        {
            Log.Debug("[SCP-079] No available SCP roles configured.");
            yield break;
        }

        PerformScpTransformation(scp079, newRole);
    }

    private static void PerformScpTransformation(Player player, RoleTypeId newRole)
    {
        if (!player.IsScp079())
        {
            Log.Debug($"[SCP-079] Player {player.Nickname} is not SCP-079.");
            return;
        }

        var scp079Role = player.Role.As<Scp079Role>();
        Log.Debug($"[SCP-079] Transforming {player.Nickname} to {newRole}");

        player.Role.Set(newRole);
        ApplyHealthModifications(player, scp079Role.Level);
        player.ShowTransformationBroadcast();
    }

    private static void ApplyHealthModifications(Player player, int scp079Level)
    {
        var config = Lone079.Instance.Config;
        var healthMultiplier = config.ScaleWithLevel
            ? (config.HealthPercent + (scp079Level - 1) * 5) / 100f
            : config.HealthPercent / 100f;

        player.Health = player.MaxHealth * healthMultiplier;
    }

    public static void OnPlayerDeath(DyingEventArgs ev)
    {
        if (ev.Player.Role.Team != Team.SCPs) return;

        Log.Debug($"[SCP-079] SCP death detected: {ev.Player.Nickname}");

        if (_checkCoroutine.IsRunning)
            Timing.KillCoroutines(_checkCoroutine);

        _checkCoroutine = Timing.RunCoroutine(
            CheckForScp079Transformation(Lone079.Instance.Config.RespawnDelay)
        );
    }

    public static void OnWarheadDetonated()
    {
        Log.Debug("[SCP-079] Warhead detonated - disabling transformations");
        _isTransformationAllowed = false;
    }

    public static void OnRoundStarted()
    {
        Log.Debug("[SCP-079] New round started - resetting settings");
        _isTransformationAllowed = true;
    }

    public static void HandleRecontainment(RecontainingEventArgs ev)
    {
        var activeScps = Player.Get(Team.SCPs)
            .Where(p => p != ev.Player)
            .Where(p => Lone079.Instance.Config.CountZombies || p.Role != RoleTypeId.Scp0492)
            .ToList();

        var shouldTransform = activeScps.Count == 0 || Lone079.Instance.Config.TransformOnRecontain;

        ev.IsAllowed = !shouldTransform;

        if (!shouldTransform) return;

        if (!TryGetRandomScpRole(out var newRole))
        {
            Log.Debug("[SCP-079] No valid SCP roles available");
            return;
        }

        PerformScpTransformation(ev.Player, newRole);
    }

    private static bool TryGetRandomScpRole(out RoleTypeId role)
    {
        role = RoleTypeId.None;
        var availableRoles = Lone079.Instance.Config.Scp079AvailableRoles;

        if (availableRoles == null || availableRoles.Count == 0)
            return false;

        role = availableRoles[Random.Next(availableRoles.Count)];
        return true;
    }
}

public static class PlayerExtensions
{
    public static bool IsScp079(this Player player)
    {
        return player != null &&
               player.Role == RoleTypeId.Scp079;
    }

    public static void ShowTransformationBroadcast(this Player player)
    {
        player.Broadcast(
            Lone079.Instance.Config.BroadcastDuration,
            Lone079.Instance.Config.BroadcastMessage
        );
    }
}