using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using WeaponDisplay.Weapons;

namespace WeaponDisplay
{
    [Label("$Mods.WeaponDisplay.Config.Label")]
    public class Configuration : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.WeaponDisplay.Config.head1")]
        [Label("$Mods.WeaponDisplay.Config.1")]
        [Tooltip("$Mods.WeaponDisplay.Config.2")]
        [DefaultValue(DyeSlot.None)]
        [DrawTicks]
        public DyeSlot DyeUsed;

        [DefaultValue(false)]
        [Label("$Mods.WeaponDisplay.Config.3")]
        [Tooltip("$Mods.WeaponDisplay.Config.4")]
        public bool ShowGlow;

        [Increment(0.05f)]
        [Range(0f, 1f)]
        [DefaultValue(0.8f)]
        [Label("$Mods.WeaponDisplay.Config.5")]
        [Tooltip("$Mods.WeaponDisplay.Config.6")]
        [Slider]
        public float GlowLighting;

        [Increment(0.05f)]
        [Range(0.5f, 2f)]
        [DefaultValue(1f)]
        [Label("$Mods.WeaponDisplay.Config.11")]
        [Tooltip("$Mods.WeaponDisplay.Config.12")]
        [Slider]
        public float WeaponScale;

        [Header("$Mods.WeaponDisplay.Config.head2")]
        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.Config.9")]
        [Tooltip("$Mods.WeaponDisplay.Config.10")]
        public bool LightItem;

        [Increment(1)]
        [Range(0, 2)]
        [DefaultValue(0)]
        [Label("$Mods.WeaponDisplay.Config.23")]
        [DrawTicks]
        public int LightItemNum;

        [Header("$Mods.WeaponDisplay.Config.head3")]
        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.Config.7")]
        [Tooltip("$Mods.WeaponDisplay.Config.8")]
        public bool CoolerSwooshActive;

        //[Increment(0.05f)]
        //[Range(0f, 1f)]
        //[DefaultValue(0.5f)]
        //[Label("$Mods.WeaponDisplay.Config.13")]
        //[Tooltip("$Mods.WeaponDisplay.Config.14")]
        //[Slider]
        //public float IsLighterDecider;

        //[DefaultValue(true)]
        //[Label("$Mods.WeaponDisplay.Config.15")]
        //[Tooltip("$Mods.WeaponDisplay.Config.16")]
        //public bool UseItemTexForSwoosh;

        [DefaultValue(false)]
        [Label("$Mods.WeaponDisplay.Config.17")]
        [Tooltip("$Mods.WeaponDisplay.Config.18")]
        //[BackgroundColor(0,0,0,0)]
        //[ColorHSLSlider(true)]
        public bool ItemAdditive;

        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.Config.19")]
        [Tooltip("$Mods.WeaponDisplay.Config.20")]
        public bool ToolsNoUseNewSwooshEffect;

        [Increment(0.05f)]
        [Range(0.5f, 5f)]
        [DefaultValue(1f)]
        [Label("$Mods.WeaponDisplay.Config.New1")]
        [Tooltip("$Mods.WeaponDisplay.Config.New2")]
        [Slider]
        public float Scale;//尺寸

        [Increment(0.05f)]
        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        [Label("$Mods.WeaponDisplay.Config.New3")]
        [Tooltip("$Mods.WeaponDisplay.Config.New4")]
        [Slider]
        public float Rapid;//刀光削减速度，越小刀光拖得越长

        [Increment(0.05f)]
        [Range(-1f, 1f)]
        [DefaultValue(0f)]
        [Label("$Mods.WeaponDisplay.Config.New5")]
        [Tooltip("$Mods.WeaponDisplay.Config.New6")]
        [Slider]
        public float Saturation;//饱和度偏移

        [Increment(0.05f)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        [Label("$Mods.WeaponDisplay.Config.New7")]
        [Tooltip("$Mods.WeaponDisplay.Config.New8")]
        [Slider]
        public float Alpha;//透明度

        [Range(1, 5)]
        [Increment(1)]
        [DefaultValue(1)]
        [Label("$Mods.WeaponDisplay.Config.New9")]
        [Tooltip("$Mods.WeaponDisplay.Config.New10")]
        [Slider]
        public int SlashEffectType;

        public override void OnChanged() 
        {
            //PlayerSlash.IsLighterDecider = IsLighterDecider;
            //PlayerSlash.UseItemTexForSwoosh = UseItemTexForSwoosh;
            PlayerSlash.LightWeaponAndEffect = ItemAdditive;
            PlayerSlash.ToolsNoUse = ToolsNoUseNewSwooshEffect;
            PlayerSlash.UseNewSlashEffect = CoolerSwooshActive;
            PlayerSlash.Rapid = Rapid;
            PlayerSlash.ItemScale = Scale;
            PlayerSlash.Saturation = Saturation;
            PlayerSlash.Alpha = Alpha;
            PlayerSlash.EffectType = SlashEffectType;
        }
    }

    [Label("$Mods.WeaponDisplay.ConfigurationServer.Label")]
    public class ConfigurationServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.WeaponDisplay.ConfigurationServer.SlashSettings")]

        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.HitboxName")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.HitboxTooltip")]
        public bool UseHitbox;

        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.New5")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.New6")]
        public bool FollowTheMouse;//跟随鼠标（耍刀花）

        [Increment(MathHelper.Pi / 10)]
        [Range(MathHelper.PiOver2, MathHelper.TwoPi)]
        [DefaultValue(MathHelper.Pi)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.New1")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.New2")]
        [Slider]
        public float SlashMaxRot;//挥砍角度

        [Increment(MathHelper.PiOver4 / 20)]
        [Range(0, MathHelper.PiOver4)]
        [DefaultValue(0)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.New3")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.New4")]
        [Slider]
        public float SlashMaxOffset;//最大偏移

        [Range(1, 20)]
        [Increment(1)]
        [DefaultValue(4)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.AttackablesName")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.AttackablesTooltip")]
        [Slider]
        public int ItemAttackCD;

        [Header("$Mods.WeaponDisplay.ConfigurationServer.Parry")]

        [DefaultValue(true)]
        [Label("$Mods.WeaponDisplay.ConfigurationServer.ParryName")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.ParryTooltip")]
        public bool CanParry;

        [Label("$Mods.WeaponDisplay.ConfigurationServer.ParryListName")]
        [Tooltip("$Mods.WeaponDisplay.ConfigurationServer.ParryListTooltip")]
        public List<ItemDefinition> ParryItems = new List<ItemDefinition> {
            new(ModContent.ItemType<旧日支配者>()),
            new(ModContent.ItemType<真旧日支配者>())
        }; // 默认值直接在这里写
        public override void OnChanged()
        {
            SlashItem.FollowTheMouse = FollowTheMouse;
            SlashItem.SlashMaxRot = SlashMaxRot;
            PlayerSlash.UseHitbox = UseHitbox;
            base.OnChanged();
        }
    }

    public enum DyeSlot
    {
        None,
        Head,
        Body,
        Leg
    }
}