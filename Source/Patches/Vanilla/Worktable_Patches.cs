using RimWorld;
using Verse.AI;

using Mastery.Core.Data.Level_Framework.Comps;

using Mastery.Workbench.Settings;

namespace Mastery.Workbench.Patches.Vanilla
{
    public static class Toils_Recipe_DoWork_Patch
    {
        public static void Postfix(Toil __result)
        {
            if (Workbench_Settings.Instance.Active) //Is Mastery Enabled
            {
                var originalTick = __result.tickIntervalAction;

                __result.tickIntervalAction = (int delta) =>
                {
                    var pawn = __result.actor;

                    var driver = pawn.jobs.curDriver as JobDriver_DoBill;
                    var worktable = driver.BillGiver as Building_WorkTable;

                    originalTick?.Invoke(delta);

                    if (driver.workLeft <= 0)
                    {
                        pawn.GetComp<Level_Comp_Manager>().ActionEvent("Workbench", worktable.def);
                    }
                };
            }
        }
    }
}