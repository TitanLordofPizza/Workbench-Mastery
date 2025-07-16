using RimWorld;

using Mastery.Core.Data.Level_Framework.StatParts;

namespace Mastery.Workbench.Data.StatParts
{
    public class Workbench_StatPart : Mastery_StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            req.Thing.Map.reservationManager.TryGetReserver(req.Thing, req.Thing.Faction, out pawn);

            base.TransformValue(req, ref val);
        }
    }
}