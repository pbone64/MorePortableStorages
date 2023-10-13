using Mono.Cecil.Cil;
using MonoMod.Cil;
using MorePortableStorages.Core.PortableStorages;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

public class TileInteractionUsePatch : BasePatch {
    protected override void Patch(Mod mod) {
        IL_Player.TileInteractionsUse += IL_Player_TileInteractionsUse;
    }

    private void IL_Player_TileInteractionsUse(ILContext il) {
        ILCursor c = new(il);

        if (!c.TryGotoNext(
                MoveType.After,
                opcode => opcode.MatchCall<TrackedProjectileReference>(nameof(TrackedProjectileReference.Clear))
            )) {
            throw new Exception("Failed while patching TileInteractionsUse: could not match call");
        }

        c.Emit(OpCodes.Ldarg_0);
        c.Emit(
            OpCodes.Call,
            typeof(PortableStorageLoader).GetMethod(
                nameof(PortableStorageLoader.ClearTrackers),
                BindingFlags.Static | BindingFlags.NonPublic
            )!
        );
    }
}