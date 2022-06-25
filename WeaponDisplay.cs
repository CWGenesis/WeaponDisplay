global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.Audio;
global using Terraria.DataStructures;
global using Terraria.GameInput;
global using Terraria.ID;
global using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent;

namespace WeaponDisplay
{
    public class WeaponDisplay : Mod
    {

        internal static Configuration Config;
        internal static ConfigurationServer ConfigGameplay;
        internal static WeaponDisplay Instance;

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            HandleNetwork.HandlePacket(reader, whoAmI);
            base.HandlePacket(reader, whoAmI);
        }

        public static Effect DefaultEffect;
        public override void PostSetupContent()
        {
            DefaultEffect = Assets.Request<Effect>("Effects/Content/Trail").Value;
            base.PostSetupContent();
        }
        public override void Load()
        {
            Config = ModContent.GetInstance<Configuration>();
            ConfigGameplay = ModContent.GetInstance<ConfigurationServer>();
            Instance = this;
            DefaultEffect = Assets.Request<Effect>("Effects/Content/Trail").Value;
            base.Load();
        }
        public override void Unload()
        {
            Config = null;
            ConfigGameplay = null;
            Instance = null;
            DefaultEffect = null;
            base.Unload();
        }
    }
}