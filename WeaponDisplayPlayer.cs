using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;

namespace WeaponDisplay
{
    public class WeaponDisplayPlayer : ModPlayer
    {
        public List<Vector2> itemOldPositions = new();
        public float kValue;
        public bool negativeDir;
        public float rotationForShadow;

        //public override void PreUpdate() {
        //    if (Player.itemAnimation == 1) {
        //        negativeDir ^= true;
        //        rotationForShadow = (Main.MouseWorld - Player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 6, MathHelper.Pi / 6);
        //        kValue = Main.rand.NextFloat(1, 2);
        //    }
        //}

        private static Vector2 DrawPlayer_Head_GetSpecialDrawPosition(ref PlayerDrawSet drawinfo, Vector2 helmetOffset, Vector2 hatOffset) {
            Vector2 value = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height] * drawinfo.drawPlayer.Directions;
            Vector2 vector = drawinfo.Position - Main.screenPosition + helmetOffset + new Vector2((float)(-(float)drawinfo.drawPlayer.bodyFrame.Width / 2 + drawinfo.drawPlayer.width / 2), (float)(drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4)) + hatOffset * drawinfo.drawPlayer.Directions + value;
            vector = vector.Floor();
            vector += drawinfo.drawPlayer.headPosition + drawinfo.headVect;
            if (drawinfo.drawPlayer.gravDir == -1f) {
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
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
            HitboxPosition = Vector2.Zero;//����
            //���д�������û��ƵĶ�����������ת�󱣳�ԭ������������λ��(������������ʾ)
            Item holditem = Player.inventory[Player.selectedItem];
            if (Player.active && !Player.dead && holditem.stack > 0 && holditem.damage > 0 && holditem.useAnimation > 0 && holditem.useTime > 0 && !holditem.consumable && holditem.ammo == 0 && Player.itemAnimation == 0 && Player.ItemTimeIsZero && CheckItemCanUse(holditem, Player)) {
                if (holditem.holdStyle == 0 && holditem.type != ItemID.FlareGun && holditem.type != ItemID.MagicalHarp && holditem.type != ItemID.NebulaBlaze && holditem.type != ItemID.NebulaArcanum && holditem.type != ItemID.TragicUmbrella && holditem.type != ItemID.CombatWrench && holditem.type != ItemID.FairyQueenMagicItem && holditem.type != ItemID.BouncingShield && holditem.type != ItemID.SparkleGuitar) {
                    Texture2D texture = TextureAssets.Item[holditem.type].Value;
                    Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                    Vector2 origin = rectangle.Size() / 2f;
                    Vector2 value5 = DrawPlayer_Head_GetSpecialDrawPosition(ref drawInfo, Vector2.Zero, new Vector2(0f, 8f));
                    float rot = MathHelper.Pi;
                    if (holditem.DamageType == DamageClass.Ranged || ((holditem.axe > 0 || holditem.pick > 0) && holditem.channel) ||
                        (holditem.useStyle == ItemUseStyleID.Shoot && !Item.staff[holditem.type] && holditem.DamageType == DamageClass.Magic)
                        || holditem.type == ItemID.KOCannon || holditem.type == ItemID.GolemFist) {
                        if (Player.gravDir == -1f) {
                            rot += MathHelper.PiOver4;
                            if (Player.direction < 0) rot -= MathHelper.PiOver2;
                        }
                        else {
                            rot -= MathHelper.PiOver4;
                            if (Player.direction < 0) rot += MathHelper.PiOver2;
                        }
                    }
                    if (Main.itemAnimations[holditem.type] != null) {//��̬����
                        rectangle = Main.itemAnimations[holditem.type].GetFrame(texture, -1);
                    }
                    DrawData item = new DrawData(texture, value5, new Rectangle?(rectangle), drawInfo.colorArmorBody, rot, origin, WeaponDisplay.Config.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
                    switch (WeaponDisplay.Config.DyeUsed) {
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
                    if (WeaponDisplay.Config.ShowGlow && holditem.glowMask >= 0) {
                        Texture2D glow = TextureAssets.GlowMask[holditem.glowMask].Value;
                        DrawData itemglow = new DrawData(glow, value5, new Rectangle?(rectangle), drawInfo.bodyGlowColor * WeaponDisplay.Config.GlowLighting, rot, origin, WeaponDisplay.Config.WeaponScale * holditem.scale, drawInfo.playerEffect, 0);
                        switch (WeaponDisplay.Config.DyeUsed) {
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
            }
            base.ModifyDrawInfo(ref drawInfo);
        }

        private static bool CheckItemCanUse(Item sItem, Player player) {
            bool flag = true;
            if (sItem.shoot == ProjectileID.EnchantedBoomerang || sItem.shoot == ProjectileID.Flamarang || sItem.shoot == ProjectileID.ThornChakram || sItem.shoot == ProjectileID.WoodenBoomerang || sItem.shoot == ProjectileID.IceBoomerang || sItem.shoot == ProjectileID.BloodyMachete || sItem.shoot == ProjectileID.FruitcakeChakram || sItem.shoot == ProjectileID.Anchor || sItem.shoot == ProjectileID.FlyingKnife || sItem.shoot == ProjectileID.Shroomerang || sItem.shoot == ProjectileID.CombatWrench || sItem.shoot == ProjectileID.BouncingShield) {
                if (player.ownedProjectileCounts[sItem.shoot] > 0) {
                    flag = false;
                }
            }
            if (sItem.shoot == ProjectileID.LightDisc || sItem.shoot == ProjectileID.Bananarang) {
                if (player.ownedProjectileCounts[sItem.shoot] >= sItem.stack) {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// ������ײ�������ҵ����꣬Ϊ������������ԭ��д�ĸ���һ��
        /// </summary>
        public Vector2 HitboxPosition = Vector2.Zero;
        /// <summary>
        /// ������Ƿ�ʹ��ն����Ч��Ϊ������ͬ��д��
        /// </summary>
        public bool UseSlash;

        /*public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Texture2D texture2D = TextureAssets.MagicPixel.Value;
            Color color = Color.Blue;
            Main.spriteBatch.Draw(texture2D, pos - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(10f, 10f), SpriteEffects.None, 0f);
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
        }*/
    }
}