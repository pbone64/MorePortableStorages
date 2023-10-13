using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MorePortableStorages.Content.Safe;

public class GrotesqueStatuette : ModItem {
    public override void SetDefaults() {
        Item.width = 26;
        Item.height = 24;
        Item.UseSound = SoundID.Item59;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(0, 2);

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 28;
        Item.useTime = 28;
        Item.shootSpeed = 4f;
        Item.shoot = ModContent.ProjectileType<FlyingSafe>();
    }
}