using System;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using JetBrains.Annotations;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using Warhead = Exiled.Events.Handlers.Warhead;

namespace Lone079Rework;

[UsedImplicitly]
public class Lone079 : Plugin<Config>
{
    public static Lone079 Instance;

    public override string Name => "Lone079Rework";
    public override string Author => "Naxefir";
    public override Version Version { get; } = new(2, 0, 0);

    public override void OnEnabled()
    {
        if (Config.Scp079AvailableRoles == null || Config.Scp079AvailableRoles.Count == 0)
        {
            Log.Error("Scp079AvailableRoles is empty or null. Plugin will not work correctly.");
        }
        Instance = this;
        Server.RoundStarted += EventHandlers.OnRoundStarted;
        Player.Dying += EventHandlers.OnPlayerDeath;
        Warhead.Detonated += EventHandlers.OnWarheadDetonated;
        Scp079.Recontaining += EventHandlers.HandleRecontainment;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Server.RoundStarted -= EventHandlers.OnRoundStarted;
        Player.Dying -= EventHandlers.OnPlayerDeath;
        Warhead.Detonated -= EventHandlers.OnWarheadDetonated;
        Scp079.Recontaining -= EventHandlers.HandleRecontainment;
        base.OnDisabled();
    }
}