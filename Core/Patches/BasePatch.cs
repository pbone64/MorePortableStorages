using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

public abstract class BasePatch : ILoadable {
    protected abstract void Patch(Mod mod);
    protected virtual void Unpatch() { }

    void ILoadable.Load(Mod mod) {
        Patch(mod);
    }

    void ILoadable.Unload() {
        Unpatch();
    }
}