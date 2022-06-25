using System;
using System.IO;

namespace WeaponDisplay
{
    internal class HandleNetwork
    {
        internal enum MessageType
        {
            BasicStats,
            Hitbox
        }

        internal static void HandlePacket(BinaryReader reader, int whoAmI) {
            MessageType msgType = (MessageType)reader.ReadByte();
            if (Main.netMode == NetmodeID.Server) {
                switch (msgType) {
                    case MessageType.BasicStats: {
                            bool negativeDir = reader.ReadBoolean();
                            float rotationForShadow = reader.ReadSingle();
                            float kValue = reader.ReadSingle();
                            bool UseSlash = reader.ReadBoolean();
                            WeaponDisplayPlayer modPlayer = Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.negativeDir = negativeDir;
                            modPlayer.rotationForShadow = rotationForShadow;
                            modPlayer.kValue = kValue;
                            modPlayer.UseSlash = UseSlash;

                            ModPacket packet = WeaponDisplay.Instance.GetPacket();
                            packet.Write((byte)MessageType.BasicStats);
                            packet.Write(negativeDir);
                            packet.Write(rotationForShadow);
                            packet.Write(kValue);
                            packet.Write(UseSlash);
                            packet.Write((byte)whoAmI);
                            packet.Send(-1, whoAmI);
                            return;
                        }
                    case MessageType.Hitbox: {
                            var HitboxPosition = reader.ReadPackedVector2();
                            WeaponDisplayPlayer modPlayer = Main.player[whoAmI].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.HitboxPosition = HitboxPosition;

                            ModPacket packet = WeaponDisplay.Instance.GetPacket();
                            packet.Write((byte)MessageType.Hitbox);
                            packet.WritePackedVector2(HitboxPosition);
                            packet.Write((byte)whoAmI);
                            packet.Send(-1, whoAmI);

                            return;
                        }
                }
                WeaponDisplay.Instance.Logger.Debug($"Unknown Message type: {msgType}, Please contact the mod developers");
                return;
            }
            else {
                switch (msgType) {
                    case MessageType.BasicStats: {
                            bool negativeDir = reader.ReadBoolean();
                            float rotationForShadow = reader.ReadSingle();
                            float kValue = reader.ReadSingle();
                            bool UseSlash = reader.ReadBoolean();
                            int playerIndex = reader.ReadByte();

                            WeaponDisplayPlayer modPlayer = Main.player[playerIndex].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.negativeDir = negativeDir;
                            modPlayer.rotationForShadow = rotationForShadow;
                            modPlayer.kValue = kValue;
                            modPlayer.UseSlash = UseSlash;
                            return;
                        }
                    case MessageType.Hitbox: {
                            var HitboxPosition = reader.ReadPackedVector2();
                            int playerIndex = reader.ReadByte();

                            WeaponDisplayPlayer modPlayer = Main.player[playerIndex].GetModPlayer<WeaponDisplayPlayer>();
                            modPlayer.HitboxPosition = HitboxPosition;
                            return;
                        }
                }
                WeaponDisplay.Instance.Logger.Debug($"Unknown Message type: {msgType}, Please contact the mod developers");
                return;
            }
        }
    }
}
