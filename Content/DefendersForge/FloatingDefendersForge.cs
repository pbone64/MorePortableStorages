using MorePortableStorages.Core;
using MorePortableStorages.Core.PortableStorages;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.DefendersForge; 

public class FloatingDefendersForge : BasePortableStorageProjectile<DefendersForgePortableStorage> {
    public static ProjectileHoverMovement Movement = new ProjectileMovement();

    public override int CursorIcon => ModContent.ItemType<EterniaCrystalShard>();

    public override void SetStaticDefaults() {
        PortableStorageLoader.RegisterInteractiveOpenCloseSound(Type, SoundID.Tink, SoundID.Tink);
    }

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 36;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 10800;
    }
    
    public const float HOVER_SPEED = 0.00715f;
    public const float HOVER_TIME = 50f;
    
    public override void AI() {
        Movement.Move(Projectile);
        
        base.AI();
    }

    public class ProjectileMovement : ProjectileHoverMovement {
        public ProjectileMovement() : base(HOVER_SPEED, HOVER_TIME) { }

        public override ref float FinishedInitialSlowdown(Projectile projectile) {
            return ref projectile.ai[0];
        }

        public override ref float HoverTimer(Projectile projectile) {
            return ref projectile.ai[1];
        }

        public override ref float HoveringUp(Projectile projectile) {
            return ref projectile.ai[2];
        }
    }
}