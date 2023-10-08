using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.PortableStorages;

internal sealed class PortableStorageNetSyncing : ModSystem {
    public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text,
        int number, float number2, float number3, float number4, int number5, int number6, int number7) {
        if (msgType == MessageID.SyncProjectileTrackers) {
            ModContent.GetInstance<MorePortableStoragesMod>().SyncPortableStorageTrackers(Main.LocalPlayer);
        }

        return base.HijackSendData(
            whoAmI, msgType, remoteClient, ignoreClient, text, number, number2, number3, number4,
            number5, number6, number7
        );
    }
}