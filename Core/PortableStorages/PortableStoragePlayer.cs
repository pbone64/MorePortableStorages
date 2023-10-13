using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.PortableStorages;

public class PortableStoragePlayer : ModPlayer {
    public TrackedProjectileReference[] PortableStorageTrackers;

    public override void Initialize() {
        PortableStorageTrackers = new TrackedProjectileReference[PortableStorageLoader.RequiredProjectileTrackers];

        for (int i = 0; i < PortableStorageTrackers.Length; i++) {
            PortableStorageTrackers[i] = new TrackedProjectileReference();
            PortableStorageTrackers[i].Clear();
        }
    }

    public override void CopyClientState(ModPlayer targetCopy) {
        if (targetCopy is PortableStoragePlayer psp) {
            psp.PortableStorageTrackers = PortableStorageTrackers;
        }
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        if (clientPlayer is not PortableStoragePlayer psp) {
            return;
        }

        for (int i = 0; i < PortableStorageTrackers.Length; i++) {
            if (!PortableStorageTrackers[i].Equals(psp.PortableStorageTrackers[i])) {
                // We hijack this in PortableStorageNetSyncing to sync our trackers as well.
                NetMessage.SendData(MessageID.SyncProjectileTrackers, number: Player.whoAmI);
                break;
            }
        }
    }
}