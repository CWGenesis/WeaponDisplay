using MonoMod.Cil;
using System;

namespace WeaponDisplay
{
    public class SlashItem : GlobalItem
    {
        public static bool FollowTheMouse = false;
        public override bool CanUseItem(Item item, Player player)
        {
            PlayerSlash modPlayer = player.GetModPlayer<PlayerSlash>();
            bool New = PlayerSlash.UseNewSlashEffect && item.useStyle == ItemUseStyleID.Swing && item.DamageType == DamageClass.Melee && !item.noUseGraphic && item.damage > 0;// && player.itemAnimation > 0;
            if (PlayerSlash.ToolsNoUse)
            {
                New = New && player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0;
            }
            if (Main.myPlayer == player.whoAmI && New)
            {
                var vec = Main.MouseWorld - player.Center;
                //vec.Y *= player.gravDir;
                //player.direction = Math.Sign(vec.X);
                //modPlayer.negativeDir ^= true;
                modPlayer.MouseToPlayerRot = vec.ToRotation();
                //Main.NewText($"{modPlayer.MouseToPlayerRot}");
                modPlayer.SlashOffset = Main.rand.NextFloat(-SlashMaxOffset, SlashMaxOffset);
                //modPlayer.kValue = Main.rand.NextFloat(1, 2);
                //modPlayer.UseSlash = WeaponDisplay.Config.CoolerSwooshActive;
                /*if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)HandleNetwork.MessageType.BasicStats);
                    packet.Write(modPlayer.negativeDir);
                    packet.Write(modPlayer.MouseToPlayerRot);
                    packet.Write(modPlayer.kValue);
                    packet.Write(modPlayer.UseSlash);
                    packet.Send(-1, -1); // 发包到服务器上 再由服务器转发到其他客户端
                    NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // 同步direction
                }*/
            }
            return true;
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (FollowTheMouse)
            {
                PlayerSlash modPlayer = player.GetModPlayer<PlayerSlash>();
                bool New = PlayerSlash.UseNewSlashEffect && item.useStyle == ItemUseStyleID.Swing && item.DamageType == DamageClass.Melee && !item.noUseGraphic && item.damage > 0 && player.itemAnimation > 0;
                if (PlayerSlash.ToolsNoUse)
                {
                    New = New && player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0;
                }
                if (Main.myPlayer == player.whoAmI && New)
                {
                    var vec = Main.MouseWorld - player.Center;
                    modPlayer.MouseToPlayerRot = vec.ToRotation();
                    modPlayer.SlashOffset = Main.rand.NextFloat(-SlashMaxOffset, SlashMaxOffset);
                }
            }
            return base.UseItem(item, player);
        }
        public static float SlashMaxRot = MathHelper.Pi;//挥舞最大角度
        public static float SlashMaxOffset = 0f;//最大偏移
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            PlayerSlash modplayer = player.GetModPlayer<PlayerSlash>();
            bool New = PlayerSlash.UseNewSlashEffect && item.useStyle == ItemUseStyleID.Swing && item.DamageType == DamageClass.Melee && !item.noUseGraphic && item.damage > 0 && player.itemAnimation > 0;
            if (PlayerSlash.ToolsNoUse)
            {
                New = New && player.HeldItem.axe == 0 && player.HeldItem.hammer == 0 && player.HeldItem.pick == 0;
            }
            if (PlayerSlash.UseNewSlashEffect && New && modplayer.DrawPos != Vector2.Zero)
            {
                var factor = (float)(player.itemAnimation - 1) / (player.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
                //if (factor == 0) {modPlayer.Reverse = !modPlayer.Reverse;modPlayer.SlashOffset = Main.rand.NextFloat(-SlashMaxOffset, SlashMaxOffset); }这一段应该在绘制之后
                if (modplayer.MouseToPlayerRot.ToRotationVector2().X * player.direction < 0)
                {
                    modplayer.MouseToPlayerRot = new Vector2(-modplayer.MouseToPlayerRot.ToRotationVector2().X, (player.gravDir < 0 ? -1 : 1) * modplayer.MouseToPlayerRot.ToRotationVector2().Y).ToRotation();
                }
                modplayer.MouseToPlayerRot = new Vector2(modplayer.MouseToPlayerRot.ToRotationVector2().X, (player.gravDir < 0 ? -1 : 1) * modplayer.MouseToPlayerRot.ToRotationVector2().Y).ToRotation();
                player.itemRotation = modplayer.MouseToPlayerRot + (player.gravDir < 0 ? -1 : 1) * (modplayer.Reverse ? -1 : 1) * SlashMaxRot * (0.5f - factor) * player.direction + modplayer.SlashOffset;
                player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, player.itemRotation - MathHelper.PiOver2);
                //我只需要在这里根据玩家重力方向和玩家朝向算出武器角度即可，其中武器角度会根据玩家朝向做对称变化
            }
        }
        public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            PlayerSlash modPlayer = player.GetModPlayer<PlayerSlash>();
            if (!PlayerSlash.UseHitbox)
                return;
            Vector2 hitboxpos = modPlayer.DrawPos;
            if (hitboxpos != Vector2.Zero)
            {
                Vector2 hitboxSize = new Vector2(Math.Abs(hitboxpos.X), Math.Abs(hitboxpos.Y));
                hitbox.Width = (int)hitboxSize.X;
                hitbox.Height = (int)hitboxSize.Y;
                hitbox.X = (int)player.Center.X;
                if (hitboxpos.Y < -4)
                {
                    hitbox.Y = (int)player.Center.Y - (int)hitboxSize.Y;
                    if (player.gravDir == -1f)
                    {
                        hitbox.Y += (int)hitboxSize.Y;
                    }
                }
                else if (hitboxpos.Y > 4)
                {
                    hitbox.Y = (int)player.Center.Y;
                    if (player.gravDir == -1f)
                    {
                        hitbox.Y -= (int)hitboxSize.Y;
                    }
                }
                else
                {
                    hitbox.Y = (int)player.Center.Y - 4;
                    hitbox.Height = 8;
                }
                if (hitboxpos.X < -4)
                {
                    hitbox.X -= (int)hitboxSize.X;
                }
                else if (hitboxpos.X > 4)
                {
                }
                else
                {
                    hitbox.X = (int)player.Center.X - 4;
                    hitbox.Width = 8;
                }
                //Texture2D texture2D = TextureAssets.MagicPixel.Value;
                //Color color = Color.Blue;
                //Vector2 Pos = new Vector2(hitbox.X, hitbox.Y);
                //Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width * 0.5f, 0) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(hitbox.Width, 2f), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width, hitbox.Height * 0.5f) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(2f, hitbox.Height), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width * 0.5f, hitbox.Height) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(hitbox.Width, 2f), SpriteEffects.None, 0f);
                //Main.spriteBatch.Draw(texture2D, Pos + new Vector2(0, hitbox.Height * 0.5f) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(2f, hitbox.Height), SpriteEffects.None, 0f);
                // 这些是写在刀光绘制那里用来查看碰撞箱的
            }
            base.UseItemHitbox(item, player, ref hitbox, ref noHitbox);
        }
        public override void Load()
        {
            IL.Terraria.Player.ItemCheck_MeleeHitNPCs += Player_ItemCheck_MeleeHitNPCs;
        }
        private void Player_ItemCheck_MeleeHitNPCs(ILContext il)
        {
            var c = new ILCursor(il);
            while (c.TryGotoNext(MoveType.After, i => i.MatchLdcR8(0.33)))
            {
                c.EmitDelegate<Func<double, double>>((_) =>
                {
                    return 1.0 / WeaponDisplay.ConfigGameplay.ItemAttackCD;
                });
            }
        }
    }
}