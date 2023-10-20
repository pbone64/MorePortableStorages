using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MorePortableStorages.Core.PortableStorages;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Content; 

public abstract class BasePortableStorageProjectile<TPortableStorage> : ModProjectile where TPortableStorage : PortableStorage {
    public abstract int CursorIcon { get; }
    public virtual string HoverTexturePath => _hoverTexturePath ??= Texture + "_HoverOutline";
    private string _hoverTexturePath;
    
    protected static Asset<Texture2D> HoverTexture { get; set; }
    
    public override void Load() {
        base.Load();
        Main.QueueMainThreadAction(delegate {
            HoverTexture ??= ModContent.Request<Texture2D>(HoverTexturePath);
        });
    }

    public override void Unload() {
        base.Unload();
        Main.QueueMainThreadAction(delegate {
            HoverTexture?.Dispose();
        });
    }
    
    public override void AI() {
        Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = true;

        if (Projectile.owner != Main.myPlayer) {
            return;
        }

        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].whoAmI == Projectile.whoAmI || !Main.projectile[i].active ||
                Main.projectile[i].owner != Projectile.owner || Main.projectile[i].type != Projectile.type) {
                continue;
            }

            if (Projectile.timeLeft >= Main.projectile[i].timeLeft) {
                Main.projectile[i].Kill();
            } else {
                Projectile.Kill();
            }
        }
    }
    
    public override void PostDraw(Color lightColor) {
        int selectionMode = PortableStorageLoader.TryInteracting<TPortableStorage>(Main.LocalPlayer, Projectile, CursorIcon);
        DrawOutline(selectionMode, lightColor);
    }
    
    protected void DrawOutline(int selectionMode, Color lightColor, Vector2? offset = null) {
        int averageLight = (lightColor.R + lightColor.G + lightColor.B) / 3;
        if (selectionMode <= 0) {
            return;
        }

        Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionMode == 2, averageLight);
        Rectangle frame = HoverTexture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        Main.spriteBatch.Draw(
            HoverTexture.Value,
            Projectile.Center - Main.screenPosition + (offset ?? Vector2.Zero),
            frame,
            selectionGlowColor,
            0f,
            frame.Size() * 0.5f,
            1f,
            Projectile.spriteDirection == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            0
        );
    }
}