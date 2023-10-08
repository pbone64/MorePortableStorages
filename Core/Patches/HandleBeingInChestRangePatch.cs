using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MorePortableStorages.Core.PortableStorages;
using MorePortableStorages.Core.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MorePortableStorages.Core.Patches;

internal sealed class HandleBeingInChestRangePatch : BasePatch {
    internal override void Patch(Mod mod) {
        IL_Player.HandleBeingInChestRange += IL_Player_HandleBeingInChestRange;
        On_Player.HandleBeingInChestRange += On_Player_HandleBeingInChestRange;
    }

    private void IL_Player_HandleBeingInChestRange(ILContext il) {
        ILCursor c = new(il);

        if (!c.TryGotoNext(MoveType.After, instr => instr.MatchLdloc(0))) {
            throw new Exception(
                "Error applying patch \"PortableStorageHandleBeingInChestRangeEdit\": Unable to match ldloc instruction."
            );
        }

        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Func<bool, Player, bool>>(
            (vanillaInRange, self) => {
                bool anyTracking = vanillaInRange;

                for (int i = 0; i < PortableStorageLoader.PortableStorageCount; i++) {
                    PortableStorage ps = PortableStorageLoader.PortableStorages[i];
                    ref TrackedProjectileReference projRef = ref ps.GetProjectileTracker(self);

                    if (projRef.IsTrackingSomething) {
                        anyTracking = true;

                        Projectile proj = projRef.GetTrackedProjectile();
                        if (!proj.active || !ps.IsPortableStorageProjectile(proj)) {
                            ps.Close(self);
                        } else {
                            Point chestCoords = proj.Hitbox.ClosestPointInRect(self.Center).ToTileCoordinates();
                            self.chestX = chestCoords.X;
                            self.chestY = chestCoords.Y;

                            Point playerTileCenter = self.Center.ToTileCoordinates();
                            if (NotInRange(
                                    playerTileCenter.X, self.chestX - Player.tileRangeX,
                                    self.chestX + Player.tileRangeX + 1
                                )
                                || NotInRange(
                                    playerTileCenter.Y, self.chestY - Player.tileRangeY,
                                    self.chestY + Player.tileRangeY + 1
                                )) {
                                if (self.chest != BankID.None) {
                                    ps.Close(self);
                                }
                            }
                        }
                    }
                }

                return anyTracking;
            }
        );
    }

    private void On_Player_HandleBeingInChestRange(On_Player.orig_HandleBeingInChestRange orig, Player self) {
        if (self.chest != BankID.None) {
            for (int i = 0; i < PortableStorageLoader.PortableStorageCount; i++) {
                PortableStorage ps = PortableStorageLoader.PortableStorages[i];
                if (self.chest != ps.Bank) {
                    PortableStorageLoader.PortableStorages[i].GetProjectileTracker(self).Clear();
                }
            }
        }

        orig(self);
    }

    private static bool NotInRange(int value, int lower, int upper) {
        return value < lower || value > upper;
    }
}