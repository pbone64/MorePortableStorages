using System;
using MorePortableStorages.Core.PortableStorages;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace MorePortableStorages.Content.Safe;

internal sealed class FlyingSafe : BasePortableStorageProjectile {
    public static Asset<Texture2D> HoverTexture;

    public override void Load() {
        Main.QueueMainThreadAction(delegate {
            HoverTexture = ModContent.Request<Texture2D>(Texture + "_HoverOutline");
        });
    }

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 5;
        PortableStorageLoader.RegisterInteractiveOpenCloseSound(Type, SoundID.Tink, SoundID.Tink);
    }

    public override void SetDefaults() {
        Projectile.width = 42;
        Projectile.height = 48;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 10800;
    }

    public bool FinishedInitialSlowdown {
        get => Projectile.ai[0] > 50f;
        set => Projectile.ai[0] = value ? 100f : 0f;
    }

    public float HoverTimer {
        get => Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    
    public bool HoveringUp {
        get => Projectile.ai[2] > 50f;
        set => Projectile.ai[2] = value ? 100f : 0f;
    }

    public const float HOVER_SPEED = 0.008f;
    public const float HOVER_TIME = 40f;

    public override void AI() {
        if (++Projectile.frameCounter % (HoveringUp ? 3 : 5) == 0) {
            Projectile.frame++;
            Projectile.frameCounter = 0;

            if (Projectile.frame >= Main.projFrames[Type]) {
                Projectile.frame = 0;
            }
        }

        base.AI();

        if (!FinishedInitialSlowdown) {
            if (Projectile.velocity.Length() < 0.1f) {
                Projectile.velocity.X = 0f;
                Projectile.velocity.Y = 0f;

                HoveringUp = true;
                HoverTimer = HOVER_TIME / 2f;

                FinishedInitialSlowdown = true;
                return;
            }

            Projectile.velocity *= 0.95f;

            if (Projectile.velocity.X < 0f) {
                Projectile.direction = -1;
            } else {
                Projectile.direction = 1;
            }

            Projectile.spriteDirection = Projectile.direction;
            return;
        }
        
        // Face owner
        if (Main.player[Projectile.owner].Center.X > Projectile.Center.X) {
            Projectile.direction = 1;
        } else {
            Projectile.direction = -1;
        }

        Projectile.spriteDirection = Projectile.direction;

        if (HoveringUp) {
            Projectile.velocity.Y -= HOVER_SPEED;
            
            if (HoverTimer++ > HOVER_TIME) {
                HoverTimer = 0f;
                HoveringUp = false;
            }
        } else {
            Projectile.velocity.Y += HOVER_SPEED;

            if (HoverTimer++ > HOVER_TIME) {
                HoverTimer = 0f;
                HoveringUp = true;
            }
        }
    }

    protected override Asset<Texture2D> GetHoverTexture() {
        return HoverTexture;
    }

    public override void PostDraw(Color lightColor) {
        int selectionMode = PortableStorageLoader.TryInteracting<SafePortableStorage>(Main.LocalPlayer, Projectile);
        DrawOutline(selectionMode, lightColor);
    }
}