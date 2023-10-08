using MorePortableStorages.Core.PortableStorages;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

// DEPRECATED: IsInteractable gets inlined
internal sealed class ProjectileIsInteractablePatch : BasePatch {
    internal override void Patch(Mod mod) {
        On_Projectile.IsInteractible += On_Projectile_IsInteractible;
    }

    private bool On_Projectile_IsInteractible(On_Projectile.orig_IsInteractible orig, Projectile self) {
        if (PortableStorageLoader.PortableStorages.Any(x => x.IsPortableStorageProjectile(self))) {
            return true;
        }

        return orig(self);
    }
}