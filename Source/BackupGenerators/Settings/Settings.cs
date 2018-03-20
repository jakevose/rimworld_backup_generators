using RimWorld;
using UnityEngine;
using Verse;

namespace BackupGenerators
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "BackupGenerators";
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            GUI.BeginGroup(new Rect(0, 60, 600, 200));
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 40, 300, 20), "Low Power Treshhold" + ":");
            Settings.LowPowerTresh.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.LowPowerTresh.AsString);
            Widgets.Label(new Rect(0, 60, 300, 20), "High Power Treshhold" + ":");
            Settings.HighPowerTresh.AsString = Widgets.TextField(new Rect(320, 60, 100, 20), Settings.HighPowerTresh.AsString);
            if (Widgets.ButtonText(new Rect(320, 85, 100, 20), "Apply"))
            {
                if (Settings.LowPowerTresh.ValidateInput() && Settings.HighPowerTresh.ValidateInput())
                {
                    base.GetSettings<Settings>().Write();
                    Messages.Message("New Teshhold applied", MessageTypeDefOf.PositiveEvent);
                }
            }
            Widgets.Label(new Rect(20, 100, 400, 30), "Wd Value at which generators will turn on");
            if (Current.Game != null)
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerTresh.AsFloat, Settings.HighPowerTresh.AsFloat);
            }
            GUI.EndGroup();
        }
    }

    class Settings : ModSettings
    {
        public static readonly FloatInput LowPowerTresh = new FloatInput("Low Power Treshhold");
        public static readonly FloatInput HighPowerTresh = new FloatInput("High Power Treshhold");

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref (LowPowerTresh.AsString), "BackupGenerators.LowPowerTresh", "200", false);
            Scribe_Values.Look<string>(ref (HighPowerTresh.AsString), "BackupGenerators.HighPowerTresh", "1000", false);
        }
    }
}