using System.Collections.Generic;
using Terraria.ModLoader.Config;
using Terraria.ID;

namespace WeaponDisplay
{
    public class WeaponParry : ModSystem
    {
        public override void Load() {
            On.Terraria.Player.ItemCheck_ManageRightClickFeatures_ShieldRaise += ParryPatch;
        }

        private static void ParryPatch(On.Terraria.Player.orig_ItemCheck_ManageRightClickFeatures_ShieldRaise orig, Player player, bool theGeneralCheck) {
            if (!WeaponDisplay.ConfigGameplay.CanParry) {
                orig.Invoke(player, theGeneralCheck);
                return;
            }

            bool mouseRight = PlayerInput.Triggers.JustPressed.MouseRight;
            if (player.whoAmI != Main.myPlayer) {
                mouseRight = player.shieldRaised;
                theGeneralCheck = player.shieldRaised;
            }
            bool shouldGuard = false;
            ItemDefinition itemd = new ItemDefinition(player.inventory[player.selectedItem].type);
            bool flag = player.inventory[player.selectedItem].type == ItemID.DD2SquireDemonSword || player.inventory[player.selectedItem].type == ItemID.BouncingShield || WeaponDisplay.ConfigGameplay.ParryItems.Contains(itemd);
            if (theGeneralCheck && flag && player.hasRaisableShield && !player.mount.Active && (player.itemAnimation == 0 || mouseRight)) {
                shouldGuard = true;
            }
            if (player.shield_parry_cooldown > 0) {
                player.shield_parry_cooldown--;
                if (player.shield_parry_cooldown == 0) {
                    SoundEngine.PlaySound(SoundID.MaxMana, player.Center);
                    for (int i = 0; i < 10; i++) {
                        int num = Dust.NewDust(player.Center + new Vector2((float)(player.direction * 6 + ((player.direction == -1) ? -10 : 0)), -14f), 10, 16, 45, 0f, 0f, 255, new Color(255, 100, 0, 127), (float)Main.rand.Next(10, 16) * 0.1f);
                        Main.dust[num].noLight = true;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].velocity *= 0.5f;
                    }
                }
            }
            if (player.shieldParryTimeLeft > 0) {
                int num2 = player.shieldParryTimeLeft + 1;
                player.shieldParryTimeLeft = num2;
                if (num2 > 20) {
                    player.shieldParryTimeLeft = 0;
                }
            }
            player.TryTogglingShield(shouldGuard);
        }
    }
}