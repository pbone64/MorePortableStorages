using Terraria;

namespace MorePortableStorages.Core; 

public abstract class ProjectileHoverMovement {
    protected float HoverSpeed { get; }
    protected float HoverTime { get; }

    public ProjectileHoverMovement(float hoverSpeed, float hoverTime) {
        HoverSpeed = hoverSpeed;
        HoverTime = hoverTime;
    }
    
    public abstract ref float FinishedInitialSlowdown(Projectile projectile);
    public abstract ref float HoverTimer(Projectile projectile);
    public abstract ref float HoveringUp(Projectile projectile);

    public void Move(Projectile projectile) {
        ref float finishedInitialSlowdown = ref FinishedInitialSlowdown(projectile);
        ref float hoverTimer = ref HoverTimer(projectile);
        ref float hoveringUp = ref HoveringUp(projectile);
        
        if (finishedInitialSlowdown <= 50f) {
            if (projectile.velocity.Length() < 0.1f) {
                projectile.velocity.X = 0f;
                projectile.velocity.Y = 0f;

                hoveringUp = 100f;
                hoverTimer = HoverTime / 2f;

                finishedInitialSlowdown = 100f;
                return;
            }

            projectile.velocity *= 0.95f;

            if (projectile.velocity.X < 0f) {
                projectile.direction = -1;
            } else {
                projectile.direction = 1;
            }

            projectile.spriteDirection = projectile.direction;
            return;
        }
        
        // Face owner
        if (Main.player[projectile.owner].Center.X > projectile.Center.X) {
            projectile.direction = 1;
        } else {
            projectile.direction = -1;
        }

        projectile.spriteDirection = projectile.direction;

        if (hoveringUp >= 50f) {
            projectile.velocity.Y -= HoverSpeed;
            
            if (hoverTimer++ > HoverTime) {
                hoverTimer = 0f;
                hoveringUp = 0f;
            }
        } else {
            projectile.velocity.Y += HoverSpeed;

            if (hoverTimer++ > HoverTime) {
                hoverTimer = 0f;
                hoveringUp = 100f;
            }
        }
    }
}