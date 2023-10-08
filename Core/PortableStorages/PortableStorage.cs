using Microsoft.Xna.Framework;
using MorePortableStorages.Core.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace MorePortableStorages.Core.PortableStorages;

internal abstract class PortableStorage : ModType {
    internal abstract int Bank { get; }

    internal int ID;

    internal virtual bool AllocateStockProjectileTracker() {
        return true;
    }

    internal abstract bool IsPortableStorageProjectile(Projectile projectile);

    internal virtual void TryOpen(Player player, Projectile projectileInstance) {
        if (!IsPortableStorageProjectile(projectileInstance)) {
            return;
        }

        player.chest = Bank;

        for (int i = 0; i < 40; i++) {
            ItemSlot.SetGlow(i, -1f, true);
        }

        GetProjectileTracker(player).Set(projectileInstance);
        Point chestPosition = projectileInstance.Center.ToTileCoordinates();
        player.chestX = chestPosition.X;
        player.chestY = chestPosition.Y;

        player.SetTalkNPC(-1);
        Main.SetNPCShopIndex(0);
        Main.playerInventory = true;
        Main.PlayInteractiveProjectileOpenCloseSound(projectileInstance.type, true);
        Recipe.FindRecipes();
    }

    internal virtual void Close(Player player) {
        Main.PlayInteractiveProjectileOpenCloseSound(GetProjectileTracker(player).ProjectileType, false);
        player.chest = BankID.None;
        Recipe.FindRecipes();
    }

    internal virtual ref TrackedProjectileReference GetProjectileTracker(Player player) {
        if (!AllocateStockProjectileTracker()) {
            throw new Exception(
                $"{nameof(AllocateStockProjectileTracker)} returned false but the base {nameof(GetProjectileTracker)} implementation was called. A custom implementation is required for portable storages that don't use provided trackers."
            );
        }

        return ref PortableStorageLoader.GetStockProjectileTracker(player, ID);
    }

    protected sealed override void Register() {
        ModTypeLookup<PortableStorage>.Register(this);
    }

    public sealed override void SetupContent() {
        PortableStorageLoader.RegisterPortableStorage(this);
    }

    public sealed override void SetStaticDefaults() { }

    protected sealed override void ValidateType() {
        base.ValidateType();

        if (Bank == BankID.None) {
            throw new Exception("BankID cannot be -1");
        }
    }

    protected sealed override void InitTemplateInstance() { }
}