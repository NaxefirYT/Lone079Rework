using System;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using JetBrains.Annotations;
using Cassie = Exiled.Events.Handlers.Cassie;
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
    public override Version Version { get; } = new(1, 1, 1);

    public override void OnEnabled()
    {
        Instance = this;
        Server.RoundStarted += EventHandlers.OnRoundStart;
        Player.Dying += EventHandlers.OnPlayerDying;
        Player.Left += EventHandlers.OnPlayerLeave;
        Warhead.Detonated += EventHandlers.OnDetonated;
        Cassie.SendingCassieMessage += EventHandlers.OnCassie;
        Scp079.Recontaining += EventHandlers.OnRecontaining;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Server.RoundStarted -= EventHandlers.OnRoundStart;
        Player.Dying -= EventHandlers.OnPlayerDying;
        Player.Left -= EventHandlers.OnPlayerLeave;
        Warhead.Detonated -= EventHandlers.OnDetonated;
        Cassie.SendingCassieMessage -= EventHandlers.OnCassie;
        Scp079.Recontaining -= EventHandlers.OnRecontaining;
        base.OnDisabled();
    }
}