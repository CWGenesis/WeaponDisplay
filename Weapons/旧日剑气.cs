using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;

namespace WeaponDisplay.Weapons
{
    public class 旧日剑气 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 500;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 5;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)//转换碰撞箱方向
        {
            float num9 = 0f;
            Vector2 value = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(-1.5707963705062866, default(Vector2)) * Projectile.scale;
            Vector2 unit = Vector2.Normalize(Projectile.velocity);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + unit * 20f - value * 69f, Projectile.Center + unit * 20f + value * 69f, 66f * Projectile.scale, ref num9);
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 40)
            {
                Projectile.alpha += 5;
                Projectile.alpha += Projectile.alpha / 10;
                if (Projectile.alpha > Projectile.penetrate * 51)
                {
                    Projectile.alpha = Projectile.penetrate * 51;
                    Projectile.ai[0] = 40;
                }
            }
            else
            {
                Projectile.alpha = Projectile.penetrate * 51;
            }
            if (Projectile.alpha > 255) Projectile.alpha = 255;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;//改弹幕方向
            if (Projectile.alpha > 50)
            {
                Vector2 value = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(-1.5707963705062866, default(Vector2)) * Projectile.scale;
                Vector2 unit = Vector2.Normalize(Projectile.velocity);
                if (Main.rand.Next(2) == 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center - unit * 12f - value * 66f, 1, 1, 229, 0f, 0f, 100, default(Color), 0.8f);
                    dust.noGravity = true;
                }
                if (Main.rand.Next(2) == 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center - unit * 12f + value * 66f, 1, 1, 229, 0f, 0f, 100, default(Color), 0.8f);
                    dust.noGravity = true;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            int num276 = Main.rand.Next(15, 25);
            int num3;
            for (int num277 = 0; num277 < num276; num277 = num3 + 1)
            {
                int num278 = Dust.NewDust(Projectile.Center, 0, 0, 229, 0f, 0f, 100, default(Color), 0.75f);
                Dust dust = Main.dust[num278];
                dust.velocity *= 5f * (0.3f + 0.7f * Main.rand.NextFloat());
                Main.dust[num278].fadeIn = 1.3f + Main.rand.NextFloat() * 0.2f;
                Main.dust[num278].noGravity = true;
                dust = Main.dust[num278];
                dust.position += Main.dust[num278].velocity * 2f;
                num3 = num277;
            }
            base.Kill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D t = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = t.Height / Main.projFrames[Projectile.type];
            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(t.Width / 2, frameHeight / 2);
            Color color = Color.White * ((float)Projectile.alpha / 255);
            color.A = 0;
            spriteBatch.Draw(t, Projectile.Center - Main.screenPosition,
            new Rectangle(0, frameHeight * Projectile.frame, t.Width, frameHeight),
            color, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}
