namespace WeaponDisplay.Weapons
{
    public class 旧日支配者 : ModItem
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
            //DisplayName.SetDefault("旧日支配者");
            //Tooltip.SetDefault("\"这股力量还未完全苏醒\"");
        }
        public override void SetDefaults()
        {
            item.damage = 99;
            item.DamageType = DamageClass.Melee;
            item.width = 97;
            item.height = 97;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.knockBack = 6f;
            item.value = Item.sellPrice(0, 0, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.DD2_SonicBoomBladeSlash;
            item.autoReuse = true;
            item.crit = +6;
            item.useTurn = true;
            item.shoot = ProjectileID.DD2SquireSonicBoom;
            item.shootSpeed = 12f;
            item.scale = 1.1f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 plrToMouse = Main.MouseWorld - player.Center;
            for (int i = -3; i <= 3; i++)
            { //上下各发出多少弹幕
                Vector2 shootVec = (plrToMouse.ToRotation() + i * MathHelper.Pi / 24).ToRotationVector2() * 12f;
                Projectile.NewProjectile(source, position, shootVec, type, damage, knockback, player.whoAmI, 0, 0);
            }
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(3823, 1);
            recipe.AddIngredient(3827, 1);
            recipe.AddIngredient(3835, 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.ReplaceResult(this);
            recipe.Register();
        }
    }
}
