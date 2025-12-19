using System;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ECommons.GameFunctions;
using ECommons.ObjectLifeTracker;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Ocelot.Extensions;

namespace TwistOfFayte.Data;

public readonly unsafe struct Target(IBattleNpc gameObject, float range, IObjectTable objects) : IEquatable<Target>
{
    public readonly ulong ObjectId = gameObject.GameObjectId;

    public readonly uint NameId = gameObject.NameId;

    public readonly uint NamePlateIconId = gameObject.Struct()->NamePlateIconId;

    public readonly bool IsHostile = gameObject.IsHostile();

    public readonly bool IsTargetable = gameObject.IsTargetable;

    public readonly bool IsDead = gameObject.IsDead;

    public readonly Vector3 Position = gameObject.Position;

    public readonly float Range = range;

    public readonly bool IsMelee = range <= 3.5f;

    public readonly bool IsRanged = range > 3.5f;

    public readonly float LifeTimeSeconds = gameObject.GetLifeTimeSeconds();

    public readonly float WanderRange = 40f;

    public readonly float HitboxRadius = gameObject.HitboxRadius;

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

    public delegate void BattleTargetAction(scoped in BattleTarget target);

    public bool TryUse(BattleTargetAction action)
    {
        var entity = objects.SearchById(ObjectId);
        if (entity is not IBattleNpc npc)
        {
            return false;
        }

        var target = new BattleTarget(npc, objects);
        action(in target);
        return true;
    }

    public delegate T BattleTargetFunc<T>(scoped in BattleTarget target);

    public bool TryUse<T>(BattleTargetFunc<T> action, out T result)
    {
        var entity = objects.SearchById(ObjectId);
        if (entity is not IBattleNpc npc)
        {
            result = default!;
            return false;
        }

        var target = new BattleTarget(npc, objects);
        result = action(in target);
        return true;
    }

    public bool Equals(Target other)
    {
        return ObjectId == other.ObjectId;
    }

    public override bool Equals(object? obj)
    {
        return obj is Target other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ObjectId.GetHashCode();
    }

    public static bool operator ==(Target left, Target right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Target left, Target right)
    {
        return !left.Equals(right);
    }
}

public readonly ref struct BattleTarget(IBattleNpc gameObject, IObjectTable objects)
{
    public readonly IntPtr Address = gameObject.Address;

    public readonly IGameObject GameObject = gameObject;

    public bool HasTarget()
    {
        return gameObject.TargetObject != null;
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

    public unsafe TargetedPlayer? GetTargetedPlayer()
    {
        if (objects.LocalPlayer is not { } player || !HasTarget())
        {
            return null;
        }


        var target = (BattleChara*)gameObject.TargetObject!.Address;
        var isPlayer = gameObject.TargetObject?.Address == player.Address;

        return new TargetedPlayer(target, isPlayer);
    }
    
    

    public unsafe Vector3 GetSpawnPosition()
    {
        var obj = (GameObject*)gameObject.Address;
        if (obj == null)
        {
            return Vector3.Zero;
        }

        var pos = obj->DefaultPosition;
        return new Vector3(pos.X, pos.Y, pos.Z);
    }

    public unsafe void Highlight(ObjectHighlightColor color)
    {
        var obj = (GameObject*)gameObject.Address;
        if (obj != null)
        {
            obj->Highlight(color);
        }
    }
}
