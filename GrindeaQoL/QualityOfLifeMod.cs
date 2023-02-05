using Behaviours;
using Grindless;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static SoG.Inventory;
using static SoG.Inventory.PreviewPair;

namespace Marioalexsan.GrindeaQoL
{
    [HarmonyPatch]
    public class QualityOfLifeMod : Mod
    {
        public override string Name => "Marioalexsan.GrindeaQoL";
        public override Version Version => new Version(1, 0, 0, 0);

        private readonly Harmony _patcher = new Harmony(typeof(QualityOfLifeMod).FullName);
        private Dictionary<EquipmentInfo.SpecialEffect, string> _displays;

        public static QualityOfLifeMod Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            Logger.LogInformation("Hello world!");
            _patcher.PatchAll(typeof(QualityOfLifeMod).Assembly);
            
            // 18-19 characters is the maximum length that can be displayed correctly
            
            _displays = new Dictionary<EquipmentInfo.SpecialEffect, string>()
            {
                [EquipmentInfo.SpecialEffect._Unique_AncientPendant_IncreasedChargeSpeed] = "Quick Charging",
                [EquipmentInfo.SpecialEffect._Unique_ArcherApple] = "More Bow Damage",
                [EquipmentInfo.SpecialEffect._Unique_BarrelHat_BarrelShieldSynergy] = "Barrel Combo",
                [EquipmentInfo.SpecialEffect._Unique_BladeOfEchoes_Cursed] = "Cursed!",
                [EquipmentInfo.SpecialEffect._Unique_BootsOfBloodthirst_HPDrainASPDBuff] = "Drains Health",
                [EquipmentInfo.SpecialEffect._Unique_BugNet_BugCatcher] = "Catch Bugs",
                [EquipmentInfo.SpecialEffect._Unique_CactusClub_SpawnThorns] = "Thorns on Melee Cast",
                [EquipmentInfo.SpecialEffect._Unique_CameraLens_CritIncreaseOnPG] = "Crits on PGuard",
                [EquipmentInfo.SpecialEffect._Unique_CameraShield_StunOnPG] = "Stun on PGuard",
                [EquipmentInfo.SpecialEffect._Unique_CaptainBonesHead_Summon] = "Summon Cpt.Bones",
                [EquipmentInfo.SpecialEffect._Unique_CogShield_EPRegenOnGuard] = "EP on PGuard",
                [EquipmentInfo.SpecialEffect._Unique_CrystalPumps_MoveSlowWhileChargingMagic] = "Slow Charge Movment",
                [EquipmentInfo.SpecialEffect._Unique_CrystalShield_PGDamageReduce01] = "Durable PGuards",
                [EquipmentInfo.SpecialEffect._Unique_CrystalShield_PGDamageReduce02] = "Unbreakable PGuards",
                [EquipmentInfo.SpecialEffect._Unique_EmptyBottle_QuickerPotionRecharge] = "Faster Potions",
                [EquipmentInfo.SpecialEffect._Unique_EvilEye_EyeLaser] = "?????",
                [EquipmentInfo.SpecialEffect._Unique_ExtraDamagePerCard] = "ATK per Card",
                [EquipmentInfo.SpecialEffect._Unique_GasMask_PoisonResistance] = "Poison Resist",
                [EquipmentInfo.SpecialEffect._Unique_GiantIcicle_ChillingTouch] = "Chill on Hit",
                [EquipmentInfo.SpecialEffect._Unique_GoblinShoes_LessSlippery] = "Less Ice Slipping",
                [EquipmentInfo.SpecialEffect._Unique_GoldenEarrings_IncreasedGoldDrops] = "More Gold",
                [EquipmentInfo.SpecialEffect._Unique_IcePendant_StrongerIceSpells] = "Strong Ice Magic",
                [EquipmentInfo.SpecialEffect._Unique_KobesTag_SummonCollar] = "Cheaper Summons",
                [EquipmentInfo.SpecialEffect._Unique_LightningGlove_StaticTouch] = "Zap on Hit",
                [EquipmentInfo.SpecialEffect._Unique_LuckySeven_GuaranteedCrits] = "7th Hit Crits",
                [EquipmentInfo.SpecialEffect._Unique_MagicBattery_EPRegOnCrit] = "EP on Hit",
                [EquipmentInfo.SpecialEffect._Unique_MissileControlUnit] = "?????",
                [EquipmentInfo.SpecialEffect._Unique_MushroomShield_SpawnShrooms] = "Shrooms on Block",
                [EquipmentInfo.SpecialEffect._Unique_MushroomSlippers_EPRegWhileBlinded] = "EP while Blind",
                [EquipmentInfo.SpecialEffect._Unique_MysteryCube_RandomEffectOnBasicAttackHit] = "Random Effects",
                [EquipmentInfo.SpecialEffect._Unique_Pan_PotionRechargeIncrease] = "Fast Potions",
                [EquipmentInfo.SpecialEffect._Unique_Pickaxe_InstantBreakEnvironment] = "Break Things",
                [EquipmentInfo.SpecialEffect._Unique_PlantBlade_IncreasedSeedDrops] = "More Seed Drops",
                [EquipmentInfo.SpecialEffect._Unique_RecipeBook_LoosePages] = "Drops Pages on Hit",
                [EquipmentInfo.SpecialEffect._Unique_RedFlowerWhip_LongerPlantDurations] = "More Plant Time",
                [EquipmentInfo.SpecialEffect._Unique_RestlessSpirit_RandomMovementWhenIdle] = "Move when AFK",
                [EquipmentInfo.SpecialEffect._Unique_RollerBlades_FasterChargeMovement] = "Faster Charge Movement",
                [EquipmentInfo.SpecialEffect._Unique_SailorHat_FasterChargeMovement] = "Fast Charge Movement",
                [EquipmentInfo.SpecialEffect._Unique_Shiidu] = "Pacifist?",
                [EquipmentInfo.SpecialEffect._Unique_SlimeFriend] = "Summon Slimes",
                [EquipmentInfo.SpecialEffect._Unique_SmashLight_BurningTouch] = "Burn on Hit",
                [EquipmentInfo.SpecialEffect._Unique_SolemShield_LaserBlast] = "Laser on PGuard",
                [EquipmentInfo.SpecialEffect._Unique_SpectralBlindfold_TradeHPForEP] = "Drain HP into EP",
                [EquipmentInfo.SpecialEffect._Unique_StingerBonuses] = "Fast Dash Charge",
                [EquipmentInfo.SpecialEffect._Unique_SunShield_SpinningSun] = "Orbits while Blocking",
                [EquipmentInfo.SpecialEffect._Unique_ThornMane_ThornWormTrail] = "Thorn Walk",
                [EquipmentInfo.SpecialEffect._Unique_ThornWorm_ReturnDamage] = "Retaliate",
                [EquipmentInfo.SpecialEffect._Unique_WinterShield_ColdGuard] = "Chill on Block",
                [EquipmentInfo.SpecialEffect._Unique_WispShield_BetterProjectileReflect] = "Strong Reflects"
            };
        }

        public string GetEffectText(EquipmentInfo.SpecialEffect effect) => _displays.TryGetValue(effect, out string value) ? value : "Special Effect";

        public override void Unload()
        {
            Logger.LogInformation("Bye world!");
            _patcher.UnpatchAll(_patcher.Id);
            Instance = null;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PreviewPair), "SetInfo")]
        private static IEnumerable<CodeInstruction> 
            BetterSpecialEffectDescription(IEnumerable<CodeInstruction> code, ILGenerator gen)
        {
            // It's much easier to just jump to the end of the method and overwrite previous changes

            List<CodeInstruction> codeList = code.ToList();

            var ret = codeList.Last();
            codeList.RemoveAt(codeList.Count - 1);

            int insertStart = codeList.Count;

            codeList.AddRange(new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(QualityOfLifeMod), nameof(QualityOfLifeMod.Instance))).WithLabels(ret.labels),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(QualityOfLifeMod), nameof(QualityOfLifeMod.LoadStatChanges))),
                new CodeInstruction(OpCodes.Ret)
            });

            return codeList;
        }

        private void LoadStatChanges(PreviewPair view, EquipmentInfo equip, EquipmentInfo discard)
        {
            string text = CAS.GetLibraryText("Menus", "InGameMenu_LeftInfo_SpecialEffect");

            view.lxStatChanges = view.lxStatChanges.Where(x => x.sString != text).ToList();

            List<EquipmentInfo.SpecialEffect> gained = equip.lenSpecialEffects;
            List<EquipmentInfo.SpecialEffect> lost = discard?.lenSpecialEffects ?? new List<EquipmentInfo.SpecialEffect>();
            
            for (int j = 0; j < gained.Count; j++)
            {
                if (lost.Contains(gained[j]))
                {
                    view.lxStatChanges.Add(new ColoredString(GetEffectText(gained[j]), Color.White));
                }
                else
                {
                    view.lxStatChanges.Add(new ColoredString(GetEffectText(gained[j]), new Color(141, 255, 110)));
                }
            }

            for (int i = 0; i < lost.Count; i++)
            {
                if (!gained.Contains(lost[i]))
                {
                    view.lxStatChanges.Add(new ColoredString(GetEffectText(lost[i]), new Color(255, 119, 119)));
                }
            }

            if (equip.enItemType == discard.enItemType || equip == discard)
                view.lxEquipStats.AddRange(equip.lenSpecialEffects.Select(x => new ColoredString(GetEffectText(x), Color.White)));

            view.bKeepEffect = false;
        }
    }
}
