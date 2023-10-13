using MorePortableStorages.Core.PortableStorages;
using MorePortableStorages.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.DefendersForge; 

public class DefendersForgePortableStorage : PortableStorage {
    public override int Bank => BankID.DefendersForge;
    public override bool IsPortableStorageProjectile(Projectile projectile) {
        return projectile.type == ModContent.ProjectileType<FloatingDefendersForge>();
    }
}