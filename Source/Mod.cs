using UnityEngine;
using Verse;

using HarmonyLib;

using Mastery.Workbench.Patches;
using Mastery.Workbench.Settings;

namespace Mastery.Workbench
{
    [StaticConstructorOnStartup]
    public class Setup
    {
        static Setup()
        {
            #region Patches

            try
            {
                Workbench_Patches.Patch();
            }
            catch (System.Exception ex)
            {
                Log.Error("Workbench Mastery Failed to Patch Workbenches. " + ex);
            }

            Mod_Workbench_Mastery.harmony.PatchAll();

            Log.Message("Workbench Mastery Loaded.");

            #endregion
        }
    }

    public class Mod_Workbench_Mastery : Mod
    {
        public static Harmony harmony;

        public static Mod_Settings settings;

        public Mod_Workbench_Mastery(ModContentPack content) : base(content)
        {
            settings = GetSettings<Mod_Settings>();

            harmony = new Harmony(content.PackageIdPlayerFacing);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.Window(inRect);
        }

        public override string SettingsCategory()
        {
            return "Workbench Mastery";
        }
    }
}