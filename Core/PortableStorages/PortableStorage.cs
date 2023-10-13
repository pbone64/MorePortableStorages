using Microsoft.Xna.Framework;
using MorePortableStorages.Core.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;

namespace MorePortableStorages.Core.PortableStorages;

public abstract class PortableStorage : ModType {
    public abstract int Bank { get; }

    public int Id { get; private set; } = -1;

    public void AssignId(int id) {
        if (Id >= 0) {
            throw new InvalidOperationException($"{nameof(PortableStorage)} already has an assigned ID.");
        }

        Id = id;
    }

    public virtual bool AllocateStockProjectileTracker() {
        return true;
    }

    public abstract bool IsPortableStorageProjectile(Projectile projectile);

    public virtual void TryOpen(Player player, Projectile projectileInstance) {
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

    public virtual void Close(Player player) {
        Main.PlayInteractiveProjectileOpenCloseSound(GetProjectileTracker(player).ProjectileType, false);
        player.chest = BankID.None;
        Recipe.FindRecipes();
    }

    public virtual ref TrackedProjectileReference GetProjectileTracker(Player player) {
        if (!AllocateStockProjectileTracker()) {
            throw new Exception(
                $"{nameof(AllocateStockProjectileTracker)} returned false but the base {nameof(GetProjectileTracker)} implementation was called. A custom implementation is required for portable storages that don't use provided trackers."
            );
        }

        return ref PortableStorageLoader.GetStockProjectileTracker(player, Id);
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