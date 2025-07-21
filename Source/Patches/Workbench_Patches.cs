using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse.AI;
using Verse;

using HarmonyLib;

using Mastery.Core.Data.Level_Framework.Comps;
using Mastery.Core.Data.Level_Framework.Extensions;

using Mastery.Workbench.Data;
using Mastery.Workbench.Settings;

using Mastery.Workbench.Patches.Vanilla;

namespace Mastery.Workbench.Patches
{
    public static class Workbench_Patches
    {
        public static void Patch()
        {
            var harmony = Mod_Workbench_Mastery.harmony;

            var defs = DefDatabase<ThingDef>.AllDefsListForReading.Where(def =>
            typeof(Building_WorkTable).IsAssignableFrom(def.thingClass) == true ||
            typeof(Building_ResearchBench).IsAssignableFrom(def.thingClass) == true); //Get all Workbenches.

            Mod_Workbench_Mastery.settings.Data.Initilize();

            foreach (var def in defs) //Adds Workbench Configs to Mastery.
            {
                Workbench_Settings.Instance.AddConfig(def); //Add Workbench.
            }

            try
            {
                harmony.Patch(typeof(Level_Comp_Manager).Method(nameof(Level_Comp_Manager.ActionEvent)), postfix: new HarmonyMethod(typeof(Comp_Manager_ActionEvent_Patch), nameof(Comp_Manager_ActionEvent_Patch.Postfix)));

                if (Action_Manager.AddAction("Workbench", "DoRecipeWork") == false) // Does This ActionPoint Already Exist?
                {
                    harmony.Patch(typeof(Toils_Recipe).Method(nameof(Toils_Recipe.DoRecipeWork)), postfix: new HarmonyMethod(typeof(Toils_Recipe_DoWork_Patch), nameof(Toils_Recipe_DoWork_Patch.Postfix)));
                }

                if (Action_Manager.AddAction("Workbench", "MakeNewToils") == false) // Does This ActionPoint Already Exist?
                {
                    harmony.Patch(typeof(JobDriver_Research).Method("MakeNewToils"), postfix: new HarmonyMethod(typeof(JobDriver_Research_MakeNewToils_Patch), nameof(JobDriver_Research_MakeNewToils_Patch.Postfix)));
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("Workbench Mastery Failed to Patch Rimworld Worktables/ResearchBenches for Mastery. " + ex);
            }
        }
    }

    public static class Comp_Manager_ActionEvent_Patch
    {
        public static void Postfix(Level_Comp_Manager __instance, List<string> __result, string actionType, Def def, Dictionary<string, object> states = null)
        {
            if (actionType == "Workbench" && __result.Contains(Workbench_Settings.Instance.LevelKey) == false) //Does it already have a ActionExtension?
            {
                if (Workbench_Settings.Instance.ActiveOnThing(__instance.parent, out Workbench_Mastery_Comp comp) == true) //Is Mastery enabled?
                {
                    comp.ActionEvent(def, Workbench_Settings.Instance.ActionBase, states);
                }
            }
        }
    }
}