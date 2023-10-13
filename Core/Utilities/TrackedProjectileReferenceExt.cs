using Terraria;
using Terraria.DataStructures;

namespace MorePortableStorages.Core.Utilities;

public static class TrackedProjectileReferenceExt {
    public static Projectile GetTrackedProjectile(this TrackedProjectileReference trackedProjectileReference) {
        return Main.projectile[trackedProjectileReference.ProjectileLocalIndex];
    }
}