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
            Widgets.Label(new Rect(0, 40, 300, 20), "Low Power Threshold" + ":");
            Settings.LowPowerThresh.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.LowPowerThresh.AsString);
            Widgets.Label(new Rect(0, 60, 300, 20), "High Power Threshold" + ":");
            Settings.HighPowerThresh.AsString = Widgets.TextField(new Rect(320, 60, 100, 20), Settings.HighPowerThresh.AsString);
            if (Widgets.ButtonText(new Rect(320, 85, 100, 20), "Apply"))
            {
                if (Settings.LowPowerThresh.ValidateInput() && Settings.HighPowerThresh.ValidateInput())
                {
                    base.GetSettings<Settings>().Write();
                    Messages.Message("New Threshold applied", MessageTypeDefOf.PositiveEvent);
                }
            }
            Widgets.Label(new Rect(20, 100, 400, 30), "Wd Value at which generators will turn on");
            if (Current.Game != null)
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerThresh.AsFloat, Settings.HighPowerThresh.AsFloat);
            }
            GUI.EndGroup();
        }
    }

    class Settings : ModSettings
    {
        public static readonly FloatInput LowPowerThresh = new FloatInput("Low Power Threshold");
        public static readonly FloatInput HighPowerThresh = new FloatInput("High Power Threshold");

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref (LowPowerThresh.AsString), "BackupGenerators.LowPowerThresh", "200", false);
            Scribe_Values.Look<string>(ref (HighPowerThresh.AsString), "BackupGenerators.HighPowerThresh", "1000", false);
        }
    }
}
