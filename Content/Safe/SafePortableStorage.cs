using MorePortableStorages.Core.PortableStorages;
using MorePortableStorages.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.Safe;

public class SafePortableStorage : PortableStorage {
    public override int Bank => BankID.Safe;

    public override bool IsPortableStorageProjectile(Projectile projectile) {
        return projectile.type == ModContent.ProjectileType<FlyingSafe>();
    }
}