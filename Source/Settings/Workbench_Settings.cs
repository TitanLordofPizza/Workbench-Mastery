using System.Collections.Generic;
using System;

using UnityEngine;
using RimWorld;
using Verse;

using Mastery.Core.Utility;
using Mastery.Core.Utility.UI;
using Mastery.Core.Data.Level_Framework.Extensions;
using Mastery.Core.Settings.Level_Framework.Extensions;

using Mastery.Workbench.Data;

namespace Mastery.Workbench.Settings
{
    public class Workbench_Settings : Extension_Level_Settings<Workbench_Mastery_Comp, Workbench_Mastery_Extension>, IExposable
    {
        #region Data

        public Workbench_Settings()
        {
            Active = true;

            ExtensionBase = new Workbench_Mastery_Extension()
            {
                TitleCurve = new UtilityCurve()
                {
                    Curve = new SimpleCurve(new List<CurvePoint>()
                    {
                        new CurvePoint(0, 0),
                        new CurvePoint(20, 20)
                    })
                },
                ExpCurve = new UtilityCurve()
                {
                    Curve = new SimpleCurve(new List<CurvePoint>()
                    {
                        new CurvePoint(0, 1000),
                        new CurvePoint(9, 10000),
                        new CurvePoint(20, 32000)
                    })
                },

                workSpeedCurve = new UtilityCurve()
                {
                    Curve = new SimpleCurve(new List<CurvePoint>()
                    {
                        new CurvePoint(0, 0),
                        new CurvePoint(9, 0.2f),
                        new CurvePoint(20, 1)
                    }),

                    Percentage = true
                },
                workSpeedType = OperationType.Additive,

                workEfficiencyCurve = new UtilityCurve()
                {
                    Curve = new SimpleCurve(new List<CurvePoint>()
                    {
                        new CurvePoint(0, 0),
                        new CurvePoint(9, 0.2f),
                        new CurvePoint(20, 1)
                    }),

                    Percentage = true
                },
                workEfficiencyType = OperationType.Additive,
            };

            ActionBase = new Level_Action_Extension()
            {
                LevelKey = "WorkbenchMastery",

                expGainCurve = new UtilityCurve()
                {
                    Curve = new SimpleCurve(new List<CurvePoint>()
                    {
                        new CurvePoint(0, 250),
                        new CurvePoint(20, 250)
                    })
                },
                expGainType = OperationType.Additive
            };
        }

        #endregion

        #region Extension Settings

        public override string LevelKey => "WorkbenchMastery";

        private const string baseExtensionName = "masteryBase";

        public static Workbench_Settings Instance;

        public override void AddConfig(Def def)
        {
            base.AddConfig(def);

            isCollapsed.Add(def.defName, true);
            cachedNames.Add(def.defName, $"{def.LabelCap.RawText} ({def.defName})");
        }

        #region UI

        private string search;

        private Vector2 scrollPos;

        private Dictionary<string, bool> isCollapsed = new Dictionary<string, bool>();
        private Dictionary<string, string> cachedNames = new Dictionary<string, string>();

        public void Window(Rect inRect)
        {
            Listing_Standard standard = new Listing_Standard();

            standard.Begin(inRect);

            standard.CheckboxLabeled("Workbench_Mastery_Settings".Translate(), ref Active, "Workbench_Mastery_Description_Settings".Translate());

            standard.CheckboxLabeled("Workbench_Mastery_Tab_Settings".Translate(), ref TabActive, "Workbench_Mastery_Tab_Description_Settings".Translate());

            search = standard.TextEntryLabeled("Workbench_Mastery_Searchbar_Settings".Translate(), search);

            standard.End();

            #region List View

            var outRect = new Rect(inRect.x, inRect.y + standard.CurHeight, inRect.width, inRect.height - standard.CurHeight); //outRect is where the entire ScrollView is.
            var viewRect = new Rect(inRect.x, inRect.y, inRect.width - UIUtility.mediumUISpacing, 0); //inRect is where the contents of the ScrollView is.

            foreach (var isCollapsedKey in isCollapsed.Keys) //Calculate List Height.
            {
                if (cachedNames[isCollapsedKey].IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    viewRect.height += Text.CalcHeight(cachedNames[isCollapsedKey], standard.ColumnWidth);

                    if (isCollapsed[isCollapsedKey] == false)
                    {
                        viewRect.height += UIUtility.smallUISpacing;

                        if (isCollapsedKey != baseExtensionName)
                        {
                            viewRect.height += Text.CalcHeight("Workbench_Mastery_IsIgnored_Settings".Translate(), standard.ColumnWidth);
                        }

                        viewRect.height += (UtilityCurve.UIHeight + UIUtility.smallUISpacing) * 4;

                        viewRect.height += Text.CalcHeight("Workbench_Mastery_TitleCurve_Settings".Translate(), standard.ColumnWidth);
                        viewRect.height += Text.CalcHeight("Workbench_Mastery_ExpCurve_Settings".Translate(), standard.ColumnWidth);

                        viewRect.height += Text.CalcHeight("Workbench_Mastery_WorkSpeedCurve_Settings".Translate(), standard.ColumnWidth);
                        viewRect.height += Text.CalcHeight("Workbench_Mastery_WorkSpeedType_Settings".Translate(), standard.ColumnWidth);

                        viewRect.height += Text.CalcHeight("Workbench_Mastery_EfficiencyCurve_Settings".Translate(), standard.ColumnWidth);
                        viewRect.height += Text.CalcHeight("Workbench_Mastery_EfficiencyType_Settings".Translate(), standard.ColumnWidth);
                    }

                    if (isCollapsedKey != baseExtensionName)
                    {
                        viewRect.height += UIUtility.mediumUISpacing + Text.CalcHeight("Workbench_Mastery_Override_Settings".Translate(), standard.ColumnWidth) + UIUtility.smallUISpacing;
                    }
                }
            }

            Widgets.BeginScrollView(outRect, ref scrollPos, viewRect, true);

            standard.Begin(viewRect);

            if (baseExtensionName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                MasteryItem(viewRect, standard, baseExtensionName);
            }

            foreach (var key in Configs.Keys) //Create List.
            {
                if (isCollapsed.ContainsKey(key) == true && cachedNames[key].IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    MasteryItem(viewRect, standard, key);
                }
            }

            standard.End();

            Widgets.EndScrollView();

            #endregion
        }

        public void MasteryItem(Rect viewRect, Listing_Standard standard, string key)
        {
            if (key != baseExtensionName)
                standard.GapLine(UIUtility.mediumUISpacing);

            bool foldoutIsCollapsed = isCollapsed[key];
            UIUtility.Foldout(standard, cachedNames[key], ref foldoutIsCollapsed);
            isCollapsed[key] = foldoutIsCollapsed;

            standard.verticalSpacing = UIUtility.smallUISpacing;

            if (key != baseExtensionName)
            {
                bool previousOverride = Configs[key].Override;

                standard.CheckboxLabeled("Workbench_Mastery_Override_Settings".Translate(), ref Configs[key].Override);

                if (Configs[key].Override == true && previousOverride != true)
                {
                    if (Configs[key].Value == null)
                    {
                        Configs[key].Value = GetConfig(key).Duplicate();
                    }
                }
            }

            if (isCollapsed[key] == false)
            {
                var masteryConfig = (key == baseExtensionName ? ExtensionBase : GetConfig(key));

                var active = (key == baseExtensionName ? false : Configs[key].Override);

                if (key != baseExtensionName)
                {
                    standard.CheckboxLabeled("Workbench_Mastery_IsIgnored_Settings".Translate(), ref masteryConfig.isIgnored);
                }

                var options = new List<string> 
                { 
                    "Mastery_Core_Additive".Translate(),
                    "Mastery_Core_Subtractive".Translate(),
                    "Mastery_Core_Multiplicative".Translate(),
                    "Mastery_Core_Divisive".Translate()
                };

                masteryConfig.TitleCurve.Editor(standard, "Workbench_Mastery_TitleCurve_Settings".Translate(), active: active);
                masteryConfig.ExpCurve.Editor(standard, "Workbench_Mastery_ExpCurve_Settings".Translate(), active: active);

                masteryConfig.workSpeedCurve.Editor(standard, "Workbench_Mastery_WorkSpeedCurve_Settings".Translate(), active: active);
                UIUtility.Dropdown(standard, "Workbench_Mastery_WorkSpeedType_Settings".Translate(), (int)masteryConfig.workSpeedType, options, (int selected) =>
                {
                    if (active == true)
                    {
                        masteryConfig.workSpeedType = (OperationType)selected;
                    }
                });

                masteryConfig.workEfficiencyCurve.Editor(standard, "Workbench_Mastery_EfficiencyCurve_Settings".Translate(), active: active);
                UIUtility.Dropdown(standard, "Workbench_Mastery_EfficiencyType_Settings".Translate(), (int)masteryConfig.workEfficiencyType, options, (int selected) =>
                {
                    if (active == true)
                    {
                        masteryConfig.workEfficiencyType = (OperationType)selected;
                    }
                });
            }
        }

        #endregion

        public override void Initilize()
        {
            base.Initilize();

            Instance = this;

            if (isCollapsed == null)
                isCollapsed = new Dictionary<string, bool>();

            if (cachedNames == null)
                cachedNames = new Dictionary<string, string>();

            isCollapsed.Add(baseExtensionName, true);
            cachedNames.Add(baseExtensionName, baseExtensionName);
        }

        #endregion
    }
}