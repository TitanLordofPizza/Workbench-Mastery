using Verse;

using Mastery.Core.Utility;
using Mastery.Core.Data.Level_Framework.Extensions;

namespace Mastery.Workbench.Data
{
    public class Workbench_Mastery_Extension : Level_Effect_Extension, IDuplicable<Workbench_Mastery_Extension>
    {
        public UtilityCurve workSpeedCurve; //This is how much Work Speed is Increased Per Level.
        public OperationType workSpeedType;

        public UtilityCurve workEfficiencyCurve; //This is how much Efficiency is Increased Per Level.
        public OperationType workEfficiencyType;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref workSpeedCurve, "workSpeedCurve");
            Scribe_Values.Look(ref workSpeedType, "workSpeedType");

            Scribe_Deep.Look(ref workEfficiencyCurve, "workEfficiencyCurve");
            Scribe_Values.Look(ref workEfficiencyType, "workEfficiencyType");
        }

        public void CopyTo(Workbench_Mastery_Extension target)
        {
            base.CopyTo(target);

            target.workSpeedCurve = workSpeedCurve.Duplicate();
            target.workSpeedType = workSpeedType;

            target.workEfficiencyCurve = workEfficiencyCurve.Duplicate();
            target.workEfficiencyType = workEfficiencyType;
        }

        public Workbench_Mastery_Extension Duplicate()
        {
            var duplicate = new Workbench_Mastery_Extension();

            CopyTo(duplicate);

            return duplicate;
        }
    }
}