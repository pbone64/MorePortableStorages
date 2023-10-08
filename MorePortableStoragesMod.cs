using System.IO;
using MorePortableStorages.Core.PortableStorages;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages;

public class MorePortableStoragesMod : Mod {
    public void SyncPortableStorageTrackers(Player player) {
        ModPacket packet = GetPacket();
        packet.Write(player.whoAmI);

        PortableStoragePlayer modPlayer = player.GetModPlayer<PortableStoragePlayer>();
        for (int i = 0; i < modPlayer.PortableStorageTrackers.Length; i++) {
            modPlayer.PortableStorageTrackers[i].Write(packet);
        }
    }
    
    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        Player player = Main.player[reader.ReadInt32()];
        PortableStoragePlayer modPlayer = player.GetModPlayer<PortableStoragePlayer>();
        for (int i = 0; i < modPlayer.PortableStorageTrackers.Length; i++) {
            modPlayer.PortableStorageTrackers[i].TryReading(reader);
        }
    }
}