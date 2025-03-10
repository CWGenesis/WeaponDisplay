using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;

namespace WeaponDisplay
{
    public class WeaponDisplayPlayer : ModPlayer
    {
        //public List<Vector2> itemOldPositions = new();
        //public float kValue;
        //public bool negativeDir;
        //public float rotationForShadow;//这些应该是都没用了

        //public override void PreUpdate() {
        //    if (Player.itemAnimation == 1) {
        //        negativeDir ^= true;
        //        rotationForShadow = (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
        //        kValue = Main.rand.NextFloat(1, 2);
        //    }
        //}
        public static bool ShowWeapon = false,
            PlayerShowWeapon = false;
        private static Vector2 DrawPlayer_Head_GetSpecialDrawPosition(ref PlayerDrawSet drawinfo, Vector2 helmetOffset, Vector2 hatOffset)
        {
            Vector2 value = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height] * drawinfo.drawPlayer.Directions;
            Vector2 vector = drawinfo.Position - Main.screenPosition + helmetOffset + new Vector2((float)(-(float)drawinfo.drawPlayer.bodyFrame.Width / 2 + drawinfo.drawPlayer.width / 2), (float)(drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4)) + hatOffset * drawinfo.drawPlayer.Directions + value;
            vector = vector.Floor();
            vector += drawinfo.drawPlayer.headPosition + drawinfo.headVect;
            if (drawinfo.drawPlayer.gravDir == -1f)
            {
                vector.Y += 12f;
            }
            vector = vector.Floor();
            return vector;
        }
        /*public override void UpdateLifeRegen()
        {
            Main.NewText($"{Player.direction}");
            base.UpdateLifeRegen();
        }*/
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //HitboxPosition = Vector2.Zero;//重置
            //这个写法可以让绘制的东西在人物旋转后保持原来与人物的相对位置(试做的武器显示)
            if (ShowWeapon)
            {
                if (Main.gameMenu && PlayerShowWeapon)
                {
                    Item firstweapon = null;
                    //Main.NewText(WeaponDisplay.Instance._FirstInventoryItem == null);
                    for (int num2 = 0; num2 <= 58; num2++)
                    {
                        Item weapon = Player.inventory[num2];//num2 == 0 ? WeaponDisplay.Instance._FirstInventoryItem : 
                        if (weapon == null) continue;
                        if (weapon.stack > 0 && (weapon.damage > 0 || weapon.type == 905) && weapon.useAnimation > 0 && weapon.useTime > 0 && !weapon.consumable && weapon.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(weapon, Player) && weapon.holdStyle == 0 && weapon.type != ItemID.FlareGun && weapon.type != ItemID.MagicalHarp && weapon.type != ItemID.NebulaBlaze && weapon.type != ItemID.NebulaArcanum && weapon.type != ItemID.TragicUmbrella && weapon.type != ItemID.CombatWrench && weapon.type != ItemID.FairyQueenMagicItem && weapon.type != ItemID.BouncingShield && weapon.type != ItemID.SparkleGuitar)
                        {
                            firstweapon = weapon;
                            break;
                        }
                    }
                    if (firstweapon != null)
                    {
                        DrawWeapon(Player, firstweapon, drawInfo);
                    }
                    //for (int n = 0; n < player.inventory.Length; n++)
                    //{
                    //    Item item = player.inventory[n];//n == 0 ? WeaponDisplay.Instance._FirstInventoryItem : 
                    //    if (item != null)
                    //    {
                    //        Main.spriteBatch.End();
                    //        Main.spriteBatch.Begin();
                    //        Vector2 vec = new Vector2(360 + 128 * (n % 10), 460 + 64 * (n / 10));
                    //        Main.spriteBatch.Draw(TextureAssets.Item[item.type].Value, vec,
                    //            null, Color.White, 0,
                    //            TextureAssets.Item[item.type].Size() * .5f, 1f, 0, 0);//n * MathHelper.TwoPi / player.inventory.Length
                    //        Main.spriteBatch.DrawString(FontAssets.MouseText.Value, (n, item.type).ToString(), vec + new Vector2(0, 16), Color.White);
                    //        Main.spriteBatch.End();
                    //        Main.spriteBatch.Begin();
                    //    }
                    //}
                    //int n = (int)(Main.GlobalTimeWrappedHourly / 15 % player.inventory.Length);
                    //if (player.inventory[n] != null)
                    //{
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin();
                    //    Main.spriteBatch.Draw(TextureAssets.Item[player.inventory[n].type].Value, new Vector2(960, 560),
                    //        null, Color.White, n * MathHelper.TwoPi / player.inventory.Length,
                    //        TextureAssets.Item[player.inventory[n].type].Size() * .5f, 1f, 0, 0);
                    //    //Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Main.GlobalTimeWrappedHourly.ToString(), new Vector2(960, 560), Color.White);
                    //    Main.spriteBatch.End();
                    //    Main.spriteBatch.Begin();
                    //    drawInfo.DrawDataCache.Add(new DrawData(TextureAssets.Item[player.inventory[n].type].Value, drawInfo.Center, null, Color.White, n * MathHelper.TwoPi / player.inventory.Length, TextureAssets.Item[player.inventory[n].type].Size() * .5f, 1f, 0, 0));
                    //}
                }
                Item holditem = Player.inventory[Player.selectedItem];
                if (Player.active && !Player.dead && holditem.stack > 0 && (holditem.damage > 0 || holditem.type == 905) && holditem.useAnimation > 0 && holditem.useTime > 0 && !holditem.consumable && holditem.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(holditem, Player))
                {
                    if (holditem.holdStyle == 0 && holditem.type != ItemID.FlareGun && holditem.type != ItemID.MagicalHarp && holditem.type != ItemID.NebulaBlaze && holditem.type != ItemID.NebulaArcanum && holditem.type != ItemID.TragicUmbrella && holditem.type != ItemID.CombatWrench && holditem.type != ItemID.FairyQueenMagicItem && holditem.type != ItemID.BouncingShield && holditem.type != ItemID.SparkleGuitar)
                    {
                        DrawWeapon(Player, holditem, drawInfo);
                    }
                }
            }
            base.ModifyDrawInfo(ref drawInfo);
        }
        public static void DrawWeapon(Player Player, Item holditem, PlayerDrawSet drawInfo)
        {
            Texture2D texture = TextureAssets.Item[holditem.type].Value;
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 value5 = DrawPlayer_Head_GetSpecialDrawPosition(ref drawInfo, Vector2.Zero, new Vector2(0f, 8f));
            float rot = MathHelper.Pi;
            if (holditem.DamageType == DamageClass.Ranged || ((holditem.axe > 0 || holditem.pick > 0) && holditem.channel) ||
                (holditem.useStyle == ItemUseStyleID.Shoot && !Item.staff[holditem.type] && holditem.DamageType == DamageClass.Magic)
                || holditem.type == ItemID.KOCannon || holditem.type == ItemID.GolemFist)
            {
                if (Player.gravDir == -1f)
                {
                    rot += MathHelper.PiOver4;
                    if (Player.direction < 0) rot -= MathHelper.PiOver2;
                }
                else
                {
                    rot -= MathHelper.PiOver4;
                    if (Player.direction < 0) rot += MathHelper.PiOver2;
                }
            }
            if (Main.itemAnimations[holditem.type] != null)
            {//动态武器
                rectangle = Main.itemAnimations[holditem.type].GetFrame(texture, -1);
            }
            DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, WeaponDisplay.Config.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
            switch (WeaponDisplay.Config.DyeUsed)
            {
                case DyeSlot.None:
                    item.shader = 0;
                    break;
                case DyeSlot.Head:
                    item.shader = Player.dye[0].dye;
                    break;
                case DyeSlot.Body:
                    item.shader = Player.dye[1].dye;
                    break;
                case DyeSlot.Leg:
                    item.shader = Player.dye[2].dye;
                    break;
                default:
                    break;
            }
            drawInfo.DrawDataCache.Add(item);
            if (WeaponDisplay.Config.ShowGlow && holditem.glowMask >= 0)
            {
                Texture2D glow = TextureAssets.GlowMask[holditem.glowMask].Value;
                DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), drawInfo.bodyGlowColor * WeaponDisplay.Config.GlowLighting, rot, origin, WeaponDisplay.Config.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
                switch (WeaponDisplay.Config.DyeUsed)
                {
                    case DyeSlot.None:
                        itemglow.shader = 0;
                        break;
                    case DyeSlot.Head:
                        item.shader = Player.dye[0].dye;
                        break;
                    case DyeSlot.Body:
                        item.shader = Player.dye[1].dye;
                        break;
                    case DyeSlot.Leg:
                        item.shader = Player.dye[2].dye;
                        break;
                    default:
                        break;
                }
                drawInfo.DrawDataCache.Add(itemglow);
            }
        }
        private static bool CheckItemCanUse(Item sItem, Player player)
        {
            bool flag = true;
            if (sItem.shoot == ProjectileID.EnchantedBoomerang || sItem.shoot == ProjectileID.Flamarang || sItem.shoot == ProjectileID.ThornChakram || sItem.shoot == ProjectileID.WoodenBoomerang || sItem.shoot == ProjectileID.IceBoomerang || sItem.shoot == ProjectileID.BloodyMachete || sItem.shoot == ProjectileID.FruitcakeChakram || sItem.shoot == ProjectileID.Anchor || sItem.shoot == ProjectileID.FlyingKnife || sItem.shoot == ProjectileID.Shroomerang || sItem.shoot == ProjectileID.CombatWrench || sItem.shoot == ProjectileID.BouncingShield)
            {
                if (player.ownedProjectileCounts[sItem.shoot] > 0)
                {
                    flag = false;
                }
            }
            if (sItem.shoot == ProjectileID.LightDisc || sItem.shoot == ProjectileID.Bananarang)
            {
                if (player.ownedProjectileCounts[sItem.shoot] >= sItem.stack)
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 刀光碰撞箱相对玩家的坐标，为了适配联机把原来写的改了一下
        /// </summary>
        //public Vector2 HitboxPosition = Vector2.Zero;
        /// <summary>
        /// 该玩家是否使用斩击特效，为了联机同步写的
        /// </summary>
        //public bool UseSlash;

        /*public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Texture2D texture2D = TextureAssets.MagicPixel.Value;
            Color color = Color.Blue;
            Main.spriteBatch.Draw(texture2D, pos - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(10f, 10f), SpriteEffects.None, 0f);
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }*/
    }
}