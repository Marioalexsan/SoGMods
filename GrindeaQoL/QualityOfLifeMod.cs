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
using static SoG.SpellVariable;

namespace Marioalexsan.GrindeaQoL
{
    public class QualityOfLifeMod : Mod
    {
        public override string Name => "Marioalexsan.GrindeaQoL";
        public override Version Version => new Version(1, 4, 1);

        public static QualityOfLifeMod Instance { get; private set; }

        public override void Load()
        {
            Logger.LogInformation("My version is {Version}", Version);
            Instance = this;
            Logger.LogInformation("Hello world!");

            BetterSpecialEffectNames.Init();
            BetterLootChance.Init();
            BerserkerStyleQoL.Init();
            SummonPlantQoL.Init();
        }

        public override void Unload()
        {
            SummonPlantQoL.CleanupMethod();
            BerserkerStyleQoL.CleanupMethod();

            Logger.LogInformation("Bye world!");

            Instance = null;
        }
    }
}
