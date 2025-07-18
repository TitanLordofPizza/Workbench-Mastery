using RimWorld;

using Mastery.Core.Data.Level_Framework.StatParts;
using Mastery.Workbench.Settings;

namespace Mastery.Workbench.Data.StatParts
{
    public class Workbench_StatPart : Mastery_StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            req.Thing.Map.reservationManager.TryGetReserver(req.Thing, req.Thing.Faction, out pawn);

            if (Workbench_Settings.Instance.ActiveOnThing(pawn, req.Thing.def.defName) == true)
            {
                base.TransformValue(req, ref val);
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            req.Thing.Map.reservationManager.TryGetReserver(req.Thing, req.Thing.Faction, out pawn);

            if (Workbench_Settings.Instance.ActiveOnThing(pawn, req.Thing.def.defName) == true)
            {
                return base.ExplanationPart(req);
            }

            return "";
        }
    }
}