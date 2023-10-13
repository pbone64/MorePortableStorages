using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.DefendersForge;

public class EterniaCrystalShard : ModItem {
    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 22;
        Item.UseSound = SoundID.Item8;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(0, 2);

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 28;
        Item.useTime = 28;
        Item.shootSpeed = 4f;
        Item.shoot = ModContent.ProjectileType<FloatingDefendersForge>();
    }
}