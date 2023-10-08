using MorePortableStorages.Core.PortableStorages;
using Terraria;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

internal sealed class PlayInteractiveOpenCloseSoundPatch : BasePatch {
    internal override void Patch(Mod mod) {
        On_Main.PlayInteractiveProjectileOpenCloseSound += On_Main_PlayInteractiveProjectileOpenCloseSound;
    }

    private void On_Main_PlayInteractiveProjectileOpenCloseSound(
        On_Main.orig_PlayInteractiveProjectileOpenCloseSound orig, int projType, bool open) {
        if (!PortableStorageLoader.TryPlayInteractiveOpenCloseSound(projType, open)) {
            orig(projType, open);
        }
    }
}