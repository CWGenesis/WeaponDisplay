using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WeaponDisplay
{
    public class PlayerSlash : ModPlayer
    {
        public static bool
            UseNewSlashEffect = false,
            ToolsNoUse = false,
            UseHitbox = false,
            LightWeaponAndEffect = false;
        public static float
            Rapid = 0.5f,//刀光衰减速度，需要小于1f，按理说等于1f没有刀光
            ItemScale = 1f,//尺寸
            Saturation = 0f,//饱和度偏移
            Alpha = 1f;//透明度
        public static int EffectType = 1;//样式

        public Vector2 DrawPos = Vector2.Zero;
        public float MouseToPlayerRot = 0f;
        public bool Reverse = false;//是否反向挥舞
        public float SlashOffset = 0f;//挥砍偏移
        public override void Load()
        {
            On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_27_HeldItem += PlayerDrawLayers_DrawPlayer_27_HeldItem;
            base.Load();
        }
        private void PlayerDrawLayers_DrawPlayer_27_HeldItem(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        {
            if (drawinfo.drawPlayer.JustDroppedAnItem)
            {
                return;
            }
            if (drawinfo.drawPlayer.heldProj >= 0 && drawinfo.shadow == 0f && !drawinfo.heldProjOverHand)
            {
                drawinfo.projectileDrawPosition = drawinfo.DrawDataCache.Count;
            }
            Item heldItem = drawinfo.heldItem;
            Player drawPlayer = drawinfo.drawPlayer;
            PlayerSlash modplayer = drawPlayer.GetModPlayer<PlayerSlash>();
            int num = heldItem.type;
            bool flag = drawPlayer.itemAnimation > 0 && heldItem.useStyle != ItemUseStyleID.None;
            bool flag2 = heldItem.holdStyle != 0 && !drawPlayer.pulley;
            if (!drawPlayer.CanVisuallyHoldItem(heldItem))
            {
                flag2 = false;
            }
            bool shouldNotDrawItem = drawinfo.shadow != 0f || drawinfo.drawPlayer.frozen || (!flag && !flag2) || num <= 0 || drawinfo.drawPlayer.dead || heldItem.noUseGraphic || (drawinfo.drawPlayer.wet && heldItem.noWet) || (drawinfo.drawPlayer.happyFunTorchTime && drawinfo.drawPlayer.inventory[drawinfo.drawPlayer.selectedItem].createTile == 4 && drawinfo.drawPlayer.itemAnimation == 0);

            if (shouldNotDrawItem || !UseNewSlashEffect)
            {
                orig.Invoke(ref drawinfo);
                return;
            }
            bool New = UseNewSlashEffect && heldItem.useStyle == ItemUseStyleID.Swing && heldItem.DamageType == DamageClass.Melee && !heldItem.noUseGraphic && heldItem.damage > 0 && drawPlayer.itemAnimation > 0;
            if (ToolsNoUse)
            {
                New = New && drawPlayer.HeldItem.axe == 0 && drawPlayer.HeldItem.hammer == 0 && drawPlayer.HeldItem.pick == 0;
            }
            if (New)
            {
                //Main.NewText("New");
                try
                {
                    Texture2D itemTex = TextureAssets.Item[num].Value;
                    int width = itemTex.Width;
                    int height = itemTex.Height;
                    float length = (float)Math.Sqrt(width * width + height * height);//剑（刀）长
                    //length *= Main.GameViewMatrix.TransformationMatrix.M11;
                    length *= ItemScale * drawPlayer.GetAdjustedItemScale(heldItem);
                    var drawCen = drawPlayer.Center;

                    var factor = (float)(drawPlayer.itemAnimation - 1) / (drawPlayer.itemAnimationMax - 1);//物品挥动程度的插值，这里应该是从1到0
                    if (modplayer.MouseToPlayerRot.ToRotationVector2().X * drawPlayer.direction < 0)
                    {
                        modplayer.MouseToPlayerRot = new Vector2(-modplayer.MouseToPlayerRot.ToRotationVector2().X, modplayer.MouseToPlayerRot.ToRotationVector2().Y).ToRotation();
                    }
                    float rot = modplayer.MouseToPlayerRot + (modplayer.Reverse ? -1 : 1) * SlashItem.SlashMaxRot * (0.5f - factor) * drawPlayer.direction + modplayer.SlashOffset;
                    float startrot = modplayer.MouseToPlayerRot + (modplayer.Reverse ? -1 : 1) * SlashItem.SlashMaxRot * -0.5f * drawPlayer.direction + modplayer.SlashOffset;
                    float rot0 = rot - startrot;
                    rot0 *= Rapid;
                    rot0 = startrot + rot0;
                    float rot1 = rot - rot0;
                    // 为了联机下缩放看到别的玩家挥舞武器，武器显示在正常的地方
                    //var screenCenterPos = Main.screenPosition + Main.ScreenSize.ToVector2() / 2f;
                    //var centerToPlayerVec = drawCen - screenCenterPos; // 玩家坐标减去屏幕中心坐标得到向量
                    //float centerToPlayerLength = centerToPlayerVec.Length() * Main.GameViewMatrix.TransformationMatrix.M11; // 原距离乘屏幕缩放得到视觉距离
                    //var playerVisualPos = screenCenterPos + centerToPlayerVec;//Vector2.Normalize(centerToPlayerVec) * centerToPlayerLength;

                    //int sec = drawPlayer.itemAnimationMax - drawPlayer.itemAnimation;//从0到drawPlayer.itemAnimationMax-1

                    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
                    int mount = 5 + (30 - drawPlayer.itemAnimationMax / 2) / 2;
                    rot1 /= mount;
                    for (int i = 0; i < mount; i++)
                    {
                        var factor0 = i / (float)mount;
                        var color = Color.Lerp(Color.White, Color.Black, factor0);
                        var w = MathHelper.Lerp(1f, 0.05f, factor);
                        float colorlight = (float)(mount - i) / mount;
                        bars.Add(new CustomVertexInfo(drawCen + (rot - rot1 * i).ToRotationVector2() * length, color * colorlight, new Vector3((float)Math.Sqrt(factor0), 1, w)));
                        bars.Add(new CustomVertexInfo(drawCen, color * colorlight, new Vector3((float)Math.Sqrt(factor0), 0, w)));
                    }
                    modplayer.DrawPos = rot.ToRotationVector2() * length;
                    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();

                    if (bars.Count > 2)
                    {
                        for (int i = 0; i < bars.Count - 2; i += 2)
                        {
                            triangleList.Add(bars[i]);
                            triangleList.Add(bars[i + 2]);
                            triangleList.Add(bars[i + 1]);
                            triangleList.Add(bars[i + 1]);
                            triangleList.Add(bars[i + 2]);
                            triangleList.Add(bars[i + 3]);
                        }

                        SpriteBatch spriteBatch = Main.spriteBatch;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                        RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                        // 干掉注释掉就可以只显示三角形栅格
                        //RasterizerState rasterizerState = new RasterizerState();
                        //rasterizerState.CullMode = CullMode.None;
                        //rasterizerState.FillMode = FillMode.WireFrame;
                        //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                        var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                        var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

                        Color lightcolor = Color.White;
                        if (!LightWeaponAndEffect)
                        {
                            lightcolor = Lighting.GetColor((drawCen / 16).ToPoint().X, (drawCen / 16).ToPoint().Y);
                        }
                        Color[] color0 = new Color[width * height];
                        itemTex.GetData(0, new Rectangle(0, 0, width, height), color0, 0, width * height);
                        Color[] color1 = new Color[9] {color0[width / 2 + width * height / 2], color0[width / 5 + width * height / 5 * 4],color0[width / 10*9 + width * height / 10],
                            color0[width / 10*9 + width * height / 2], color0[width / 2 + width * height / 10],color0[width / 2 + width * height / 20*13],
                            color0[width / 20*13 + width * height / 2], color0[width / 20*7 + width * height / 2], color0[width / 2 + width * height / 20*7]};
                        int k = 9;
                        Vector4 allcolor = new Vector4(0, 0, 0, 0);
                        for (int n = 0; n < 9; n++)
                        {
                            if (color1[n].A == 0)
                                k--;
                            else
                                allcolor = new Vector4(allcolor.X + color1[n].R, allcolor.Y + color1[n].G, allcolor.Z + color1[n].B, allcolor.W + color1[n].A);
                        }
                        //Main.NewText($"{allcolor0.R}, {allcolor0.G}, {allcolor0.B}, {allcolor0.A}");255加满了不会继续加了，所以要换成Vector4
                        allcolor = new Vector4(allcolor.X / k, allcolor.Y / k, allcolor.Z / k, allcolor.W / k);
                        allcolor = new Vector4(allcolor.X / 255, allcolor.Y / 255, allcolor.Z / 255, allcolor.W / 255);
                        //Main.NewText($"{allcolor.X}, {allcolor.Y}, {allcolor.Z}, {allcolor.W}");
                        int parameter = 0;
                        float saturation = 0f;
                        if (allcolor.X < allcolor.Y)
                        {
                            parameter = 2;
                            if (allcolor.Y < allcolor.Z)
                            {
                                parameter = 3;
                            }
                        }
                        else
                        {
                            parameter = 1;
                            if (allcolor.X < allcolor.Z)
                            {
                                parameter = 3;
                            }
                        }
                        if (parameter == 1)
                        {
                            if (allcolor.Y < allcolor.Z)
                            {
                                saturation = CalculateSaturation(allcolor.X, allcolor.Y);
                                float median = (1 - allcolor.Y / allcolor.X) * allcolor.Z;
                                float kk = 1 - saturation;
                                allcolor = new Vector4(allcolor.X, allcolor.X * kk, (allcolor.X - median) * kk + median, allcolor.W);
                            }
                            else
                            {
                                saturation = CalculateSaturation(allcolor.X, allcolor.Z);
                                float median = (1 - allcolor.Z / allcolor.X) * allcolor.Y;
                                float kk = 1 - saturation;
                                allcolor = new Vector4(allcolor.X, (allcolor.X - median) * kk + median, allcolor.X * kk, allcolor.W);
                            }
                        }
                        else if (parameter == 2)
                        {
                            if (allcolor.X < allcolor.Z)
                            {
                                saturation = CalculateSaturation(allcolor.Y, allcolor.X);
                                float median = (1 - allcolor.X / allcolor.Y) * allcolor.Z;
                                allcolor = new Vector4(allcolor.Y * (1 - saturation), allcolor.Y, (allcolor.Y - median) * (1 - saturation) + median, allcolor.W);
                            }
                            else
                            {
                                saturation = CalculateSaturation(allcolor.Y, allcolor.Z);
                                float median = (1 - allcolor.Z / allcolor.Y) * allcolor.X;
                                allcolor = new Vector4((allcolor.Y - median) * (1 - saturation) + median, allcolor.Y, allcolor.Y * (1 - saturation), allcolor.W);
                            }
                        }
                        else if (parameter == 3)
                        {
                            if (allcolor.X < allcolor.Y)
                            {
                                saturation = CalculateSaturation(allcolor.Z, allcolor.X);
                                float median = (1 - allcolor.X / allcolor.Z) * allcolor.Y;
                                allcolor = new Vector4(allcolor.Z * (1 - saturation), (allcolor.Z - median) * (1 - saturation) + median, allcolor.Z, allcolor.W);
                            }
                            else
                            {
                                saturation = CalculateSaturation(allcolor.Z, allcolor.Y);
                                float median = (1 - allcolor.Y / allcolor.Z) * allcolor.X;
                                allcolor = new Vector4((allcolor.Z - median) * (1 - saturation) + median, allcolor.Z * (1 - saturation), allcolor.Z, allcolor.W);
                            }
                        }
                        allcolor.W = Alpha;
                        //Main.NewText($"{allcolor.X}, {allcolor.Y}, {allcolor.Z}, {allcolor.W}");
                        // 把变换和所需信息丢给shader
                        WeaponDisplay.DefaultEffect.Parameters["uTransform"].SetValue(model * projection);
                        WeaponDisplay.DefaultEffect.Parameters["uTime"].SetValue(-(float)Main.time * 0.03f);
                        WeaponDisplay.DefaultEffect.Parameters["allcolor"].SetValue(allcolor * lightcolor.ToVector4());
                        Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("WeaponDisplay/Images/Blackmap").Value;//itemTex;
                        Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("WeaponDisplay/Images/" + $"{EffectType}").Value;//2变窄加厚
                        Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("WeaponDisplay/Images/Extra_193").Value;
                        //Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("SlashEffect/Images/Blackmap").Value;
                        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
                        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
                        //Main.graphics.GraphicsDevice.Textures[0] = Main.magicPixel;
                        //Main.graphics.GraphicsDevice.Textures[1] = Main.magicPixel;
                        //Main.graphics.GraphicsDevice.Textures[2] = Main.magicPixel;

                        WeaponDisplay.DefaultEffect.CurrentTechnique.Passes[0].Apply();

                        //Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

                        //Main.graphics.GraphicsDevice.RasterizerState = originalState;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                        Vector2 unit = Vector2.Normalize(drawCen + modplayer.DrawPos - drawPlayer.Center);
                        bool flag0 = (modplayer.Reverse && drawPlayer.direction > 0) || (!modplayer.Reverse && drawPlayer.direction < 0);
                        length = (float)Math.Sqrt(width * width + height * height);
                        float Rot = MathHelper.PiOver4 - (float)Math.Asin(width / length);
                        Rot = unit.ToRotation() + (flag0 ? 3 : 1) * MathHelper.PiOver4 + (flag0 ? -1 : 1) * Rot;
                        Main.EntitySpriteDraw(itemTex, drawCen + modplayer.DrawPos - Main.screenPosition, null, LightWeaponAndEffect?Color.White:lightcolor,
                                       Rot, new Vector2(flag0 ? 0 : width, 0), drawPlayer.GetAdjustedItemScale(heldItem) * ItemScale, flag0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        if (heldItem.glowMask != -1)
                        {
                            Texture2D glow = TextureAssets.GlowMask[heldItem.glowMask].Value;
                            Main.EntitySpriteDraw(glow, drawCen + modplayer.DrawPos - Main.screenPosition, null, Color.White,
                                           Rot, new Vector2(flag0 ? 0 : width, 0), drawPlayer.GetAdjustedItemScale(heldItem) * ItemScale, flag0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        }
                        else if (heldItem.type == 3823)
                        {
                            Texture2D glow = ModContent.Request<Texture2D>("WeaponDisplay/Images/ItemFlame_3823").Value;
                            Main.EntitySpriteDraw(glow, drawCen + modplayer.DrawPos - Main.screenPosition, null, Color.White,
                                           Rot, new Vector2(flag0 ? 0 : width, 0), drawPlayer.GetAdjustedItemScale(heldItem) * ItemScale, flag0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        }
                        if (factor == 0)
                        {
                            modplayer.Reverse = !modplayer.Reverse;
                            modplayer.SlashOffset = Main.rand.NextFloat(-SlashItem.SlashMaxOffset, SlashItem.SlashMaxOffset);
                        }
                        /*Rectangle hitbox;
                        Vector2 hitboxpos = rot.ToRotationVector2() * length;
                        Vector2 hitboxSize = new Vector2(Math.Abs(hitboxpos.X), Math.Abs(hitboxpos.Y));
                        hitbox.Width = (int)hitboxSize.X;
                        hitbox.Height = (int)hitboxSize.Y;
                        hitbox.X = (int)drawPlayer.Center.X;
                        if (hitboxpos.Y < -4)
                        {
                            hitbox.Y = (int)drawPlayer.Center.Y - (int)hitboxSize.Y;
                            if (drawPlayer.gravDir == -1f)
                            {
                                hitbox.Y += (int)hitboxSize.Y;
                            }
                        }
                        else if (hitboxpos.Y > 4)
                        {
                            hitbox.Y = (int)drawPlayer.Center.Y;
                            if (drawPlayer.gravDir == -1f)
                            {
                                hitbox.Y -= (int)hitboxSize.Y;
                            }
                        }
                        else
                        {
                            hitbox.Y = (int)drawPlayer.Center.Y - 4;
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
                            hitbox.X = (int)drawPlayer.Center.X - 4;
                            hitbox.Width = 8;
                        }
                        Texture2D texture2D = TextureAssets.MagicPixel.Value;
                        Color color = Color.Blue;
                        Vector2 Pos = new Vector2(hitbox.X, hitbox.Y);
                        Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width * 0.5f, 0) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(hitbox.Width, 2f), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width, hitbox.Height * 0.5f) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(2f, hitbox.Height), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(texture2D, Pos + new Vector2(hitbox.Width * 0.5f, hitbox.Height) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(hitbox.Width, 2f), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(texture2D, Pos + new Vector2(0, hitbox.Height * 0.5f) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 1, 1)), color, 0, new Vector2(0.5f, 0.5f), new Vector2(2f, hitbox.Height), SpriteEffects.None, 0f);
                        // 这些是写在刀光绘制那里用来查看碰撞箱的*/

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                orig.Invoke(ref drawinfo);
            }
        }
        private static float CalculateSaturation(float Max,float Min)
        {
            float saturation;
            saturation = (Max - Min) / Max;//当前饱和度
            saturation += Saturation;
            if (saturation < 0)
            {
                saturation = 0;
            }
            else if (saturation > 1) 
            {
                saturation = 1; 
            }
            return saturation;
        }
        private struct CustomVertexInfo : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
                new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;

            public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
            {
                this.Position = position;
                this.Color = color;
                this.TexCoord = texCoord;
            }

            public VertexDeclaration VertexDeclaration
            {
                get
                {
                    return _vertexDeclaration;
                }
            }
        }
    }
}