using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Content; 

public abstract class BasePortableStorageProjectile : ModProjectile {
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

    protected abstract Asset<Texture2D> GetHoverTexture();

    protected void DrawOutline(int selectionMode, Color lightColor, Vector2? offset = null) {
        int averageLight = (lightColor.R + lightColor.G + lightColor.B) / 3;
        if (selectionMode <= 0) {
            return;
        }

        Asset<Texture2D> hoverTexture = GetHoverTexture();
        Color selectionGlowColor = Colors.GetSelectionGlowColor(selectionMode == 2, averageLight);
        Rectangle frame = hoverTexture.Frame(1, 5, 0, Projectile.frame);
        Main.spriteBatch.Draw(
            hoverTexture.Value,
            Projectile.Center - Main.screenPosition + (offset ?? Vector2.Zero),
            frame,
            selectionGlowColor,
            0f,
            frame.Size() * 0.5f,
            1f,
            SpriteEffects.None,
            0
        );
    }
}