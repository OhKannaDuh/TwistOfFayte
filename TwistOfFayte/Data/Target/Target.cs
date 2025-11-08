using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using ECommons.GameFunctions;
using ECommons.ObjectLifeTracker;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using Ocelot.Extensions;
using Ocelot.Services.ClientState;
using Ocelot.Services.Data;

namespace TwistOfFayte.Data;

public class Target(IBattleNpc gameObject, IClient client, IDataRepository<ClassJob> data, float range = 3.5f)
{
    public readonly IBattleNpc GameObject = gameObject;

    public uint NameId
    {
        get => GameObject.NameId;
    }

    public unsafe uint NamePlateIconId
    {
        get => GameObject.Struct()->NamePlateIconId;
    }

    public bool IsHostile
    {
        get => GameObject.IsHostile();
    }

    public bool IsTargetable
    {
        get => GameObject.IsTargetable;
    }

    public bool IsDead
    {
        get => GameObject.IsDead;
    }

    public Vector3 Position
    {
        get => GameObject.Position;
    }

    public float Range
    {
        get => range;
    }

    public bool IsMelee
    {
        get => Range <= 3.5f;
    }

    public bool IsRanged
    {
        get => !IsMelee;
    }

    public Vector3 GetApproachPosition(Vector3 from, float range = 3f)
    {
        var distance = from.Distance(Position);
        if (distance <= range)
        {
            return from;
        }

        var direction = Position - from;

        if (direction.LengthSquared() < 0.0001f)
        {
            return Position;
        }

        direction /= distance;
        return Position - direction * range;
    }

    public bool HasTarget()
    {
        return GameObject.TargetObject != null;
    }

    public bool IsTargetingLocalPlayer()
    {
        var target = GetTargetedPlayer();
        return target != null && target.IsLocalPlayer();
    }

    public bool IsTargetingAnyPlayer()
    {
        return GetTargetedPlayer() != null;
    }

    public float GetWanderRange()
    {
        return 40f;
    }

    public unsafe Vector3 GetSpawnPosition()
    {
        var obj = (GameObject*)GameObject.Address;
        if (obj == null)
        {
            return Vector3.Zero;
        }

        var pos = obj->DefaultPosition;
        return new Vector3(pos.X, pos.Y, pos.Z);
    }

    public unsafe TargetedPlayer? GetTargetedPlayer()
    {
        if (client.Player == null || !HasTarget())
        {
            return null;
        }

        var target = (BattleChara*)GameObject.TargetObject!.Address;
        var isPlayer = GameObject.TargetObject?.Address == client.Player.Address;

        return new TargetedPlayer(target, isPlayer, data);
    }

    public float GetLifeTimeSeconds()
    {
        return GameObject.GetLifeTimeSeconds();
    }

    public unsafe void Highlight(ObjectHighlightColor color)
    {
        var obj = (GameObject*)GameObject.Address;
        if (obj != null)
        {
            obj->Highlight(color);
        }
    }
}
