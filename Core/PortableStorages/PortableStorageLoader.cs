using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MorePortableStorages.Content.Safe;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.PortableStorages;

public static class PortableStorageLoader {
    public static IReadOnlyList<PortableStorage> PortableStorages => _portableStorages.AsReadOnly();
    private static readonly List<PortableStorage> _portableStorages = new();
    private static readonly Dictionary<int, (SoundStyle open, SoundStyle close)> _interactiveOpenCloseSounds = new();
    private static readonly Dictionary<int, int> _portableStorageProjectileTrackerIds = new();

    public static int PortableStorageCount => _portableStorages.Count;
    public static int RequiredProjectileTrackers => _portableStorageProjectileTrackerIds.Count;

    public static void RegisterPortableStorage(PortableStorage ps) {
        ps.AssignId(PortableStorageCount);
        _portableStorages.Add(ps);
        if (ps.AllocateStockProjectileTracker()) {
            _portableStorageProjectileTrackerIds.Add(RequiredProjectileTrackers, ps.Id);
        }
    }

    public static void RegisterInteractiveOpenCloseSound(int projectileType, SoundStyle open, SoundStyle close) {
        _interactiveOpenCloseSounds.Add(projectileType, (open, close));
    }

    public static bool TryPlayInteractiveOpenCloseSound(int projectileType, bool open) {
        if (!_interactiveOpenCloseSounds.TryGetValue(projectileType, out (SoundStyle open, SoundStyle close) sound)) {
            return false;
        }

        SoundEngine.PlaySound(open ? sound.open : sound.close);
        return true;

    }

    public static void TryOpen<T>(Player player, Projectile projectileInstance) where T : PortableStorage {
        ModContent.GetInstance<T>().TryOpen(player, projectileInstance);
    }

    public static void Close<T>(Player player, Projectile projectile) where T : PortableStorage {
        ModContent.GetInstance<T>().Close(player);
    }

    public static ref TrackedProjectileReference GetProjectileTracker<T>(Player player) where T : PortableStorage {
        return ref ModContent.GetInstance<T>().GetProjectileTracker(player);
    }

    public static ref TrackedProjectileReference GetProjectileTracker(Player player, int id) {
        return ref _portableStorages[id].GetProjectileTracker(player);
    }

    public static ref TrackedProjectileReference GetStockProjectileTracker(Player player, int id) {
        return ref player.GetModPlayer<PortableStoragePlayer>()
            .PortableStorageTrackers[_portableStorageProjectileTrackerIds[id]];
    }

    public static void ClearTrackers(Player player) {
        for (int i = 0; i < PortableStorageCount; i++) {
            PortableStorages[i].GetProjectileTracker(player).Clear();
        }
    }

    public static bool NeedsSyncing(Player localPlayer, Player clientPlayer) {
        for (int i = 0; i < PortableStorageCount; i++) {
            if (PortableStorages[i].GetProjectileTracker(localPlayer) !=
                PortableStorages[i].GetProjectileTracker(clientPlayer)) {
                return true;
            }
        }

        return false;
    }

    public static int TryInteracting<T>(Player player, Projectile projectile, int cursorIcon) where T : PortableStorage {
        if (Main.gamePaused || Main.gameMenu) {
            return 0;
        }

        Vector2 compareSpot = player.Center;

        bool showOutline = Main.SmartCursorIsUsed || PlayerInput.UsingGamepad;
        if (!IsInteractableAndInRange<T>(player, projectile, compareSpot)) {
            return 0;
        }

        Vector2 logicalMousePos = Main.ReverseGravitySupport(Main.MouseScreen) + Main.screenPosition;
        bool containsMouse = projectile.Hitbox.Contains(logicalMousePos.ToPoint());

        if (!((containsMouse || Main.SmartInteractProj == projectile.whoAmI)
              & !player.mouseInterface)
           ) {
            return showOutline ? 1 : 0;
        }

        Main.HasInteractibleObjectThatIsNotATile = true;

        if (containsMouse) {
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = cursorIcon;
        }

        if (PlayerInput.UsingGamepad) {
            player.GamepadEnableGrappleCooldown();
        }

        if (!Main.mouseRight || !Main.mouseRightRelease || Player.BlockInteractionWithProjectiles != 0) {
            return showOutline ? 2 : 0;
        }

        Main.mouseRightRelease = true;
        player.tileInteractAttempted = true;
        player.tileInteractionHappened = true;
        player.releaseUseTile = false;

        if (player.chest == ModContent.GetInstance<T>().Bank) {
            Close<T>(player, projectile);
        } else {
            TryOpen<T>(player, projectile);
        }

        return showOutline ? 2 : 0;
    }

    // We can't use the vanilla IsProjectileInteractableAndInRange method because Projectile.IsInteractable is small enough to get inlined
    public static bool IsInteractableAndInRange<T>(Player player, Projectile projectile, Vector2 compareSpot)
        where T : PortableStorage {
        if (!projectile.active || !ModContent.GetInstance<T>().IsPortableStorageProjectile(projectile)) {
            return false;
        }

        Point point = projectile.Hitbox.ClosestPointInRect(compareSpot).ToTileCoordinates();
        return player.IsInTileInteractionRange(point.X, point.Y, TileReachCheckSettings.Simple);
    }
}