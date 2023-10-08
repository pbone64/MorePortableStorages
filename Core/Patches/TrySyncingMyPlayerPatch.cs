using MonoMod.Cil;
using MorePortableStorages.Core.PortableStorages;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

internal sealed class TrySyncingMyPlayerPatch : BasePatch {
    internal override void Patch(Mod mod) {
        IL_Main.TrySyncingMyPlayer += IL_Main_TrySyncingMyPlayer;
    }

    private void IL_Main_TrySyncingMyPlayer(ILContext il) {
        ILCursor c = new(il);

        if (!c.TryGotoNext(instr => instr.MatchLdfld<Player>("voidLensChest"))) {
            throw new Exception(
                "Error applying patch \"PortableStorageNetSyncEdit\": Unable to match ldfld instruction."
            );
        }

        c.EmitDelegate(
            () => {
                if (PortableStorageLoader.NeedsSyncing(Main.LocalPlayer, Main.clientPlayer)) {
                    NetMessage.SendData(MessageID.SyncProjectileTrackers, number: Main.myPlayer);
                }
            }
        );
    }
}