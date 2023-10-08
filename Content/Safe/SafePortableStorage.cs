using MorePortableStorages.Core.PortableStorages;
using MorePortableStorages.Core.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.Safe;

internal sealed class SafePortableStorage : PortableStorage {
    internal override int Bank => BankID.Safe;

    internal override bool IsPortableStorageProjectile(Projectile projectile) {
        return projectile.type == ModContent.ProjectileType<FlyingSafe>();
    }
}