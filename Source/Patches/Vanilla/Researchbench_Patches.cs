using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse.AI;
using Verse;

using Mastery.Core.Data.Level_Framework.Comps;

using Mastery.Workbench.Settings;

namespace Mastery.Workbench.Patches.Vanilla
{
    public static class JobDriver_Research_MakeNewToils_Patch
    {
        public static void Postfix(JobDriver_Research __instance, ref IEnumerable<Toil> __result)
        {
            if (Workbench_Settings.Instance.Active) //Is Mastery Enabled
            {
                var toils = __result.ToList();

                for (int i = 0; i < toils.Count; i++)
                {
                    var toil = toils[i];

                    if (toil.tickIntervalAction != null)
                    {
                        var originalTick = toil.tickIntervalAction;

                        toil.tickIntervalAction = (int delta) =>
                        {
                            var project = Find.ResearchManager.GetProject();

                            originalTick?.Invoke(delta);

                            if (Workbench_Settings.Instance.ActiveConfig(__instance.job.targetA.Thing.def.defName) == true) // Is This Not Ignored?
                            {
                                if (Find.ResearchManager.GetProgress(project) >= project.baseCost)
                                {
                                    toil.actor.GetComp<Level_Comp_Manager>().ActionEvent("Workbench", __instance.job.targetA.Thing.def);
                                }
                            }
                        };

                        break;
                    }
                }

                __result = toils;
            }
        }
    }
}