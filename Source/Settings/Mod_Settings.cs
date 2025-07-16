using UnityEngine;
using Verse;

namespace Mastery.Workbench.Settings
{
    public class Mod_Settings : ModSettings
    {
        public Workbench_Settings Data = new Workbench_Settings();

        public void Window(Rect inRect) //Showing The Settings.
        {
            Data.Window(inRect);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref Data, "data");
        }
    }
}