namespace WeaponDisplay.Weapons
{
    public class 真旧日支配者 : ModItem
    {
        private Item item
        {
            get
            {
                return Item;
            }
        }
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("真▪旧日支配者");
        }
        public override void SetDefaults()
        {
            item.damage = 139;
            item.DamageType = DamageClass.Melee;
            item.width = 103;
            item.height = 103;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.DD2_SonicBoomBladeSlash;
            item.autoReuse = true;
            item.useTurn = true;
            item.shoot = ModContent.ProjectileType<旧日剑气>();
            item.shootSpeed = 12f;
            item.scale = 1.1f;
            item.crit = +12;

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 plrToMouse = Main.MouseWorld - player.Center;
            for (int i = -4; i <= 4; i++)
            {
                Vector2 shootVec = (plrToMouse.ToRotation() + i * MathHelper.Pi / 18.0f).ToRotationVector2() * 14f;
                Projectile.NewProjectileDirect(source, position, shootVec, type, damage, knockback, player.whoAmI, 0, 0);
            }
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<旧日支配者>(), 1);
            recipe.AddIngredient(676, 1);
            recipe.AddIngredient(723, 1);
            recipe.AddIngredient(1166, 1);
            recipe.AddIngredient(3211, 1);
            recipe.AddIngredient(4144, 1);
            recipe.AddIngredient(1570, 1);
            recipe.AddIngredient(ItemID.LunarOre, 999);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
}
