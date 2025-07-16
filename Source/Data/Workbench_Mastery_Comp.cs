using Mastery.Core.Data.Level_Framework.Comps;
using Mastery.Core.UI.Tabs;

using Mastery.Workbench.UI;

namespace Mastery.Workbench.Data
{
    public class Workbench_Mastery_Comp : Level_Comp
    {
        public override string LevelKey => "WorkbenchMastery";

        private readonly Workbench_Tab _tab = new Workbench_Tab();

        public override IMastery_Tab Tab => _tab;
    }
}