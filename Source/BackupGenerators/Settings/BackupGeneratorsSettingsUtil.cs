using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BackupGenerators
{
    internal static class BackupGeneratorsSettingsUtil
    {
        public static Dictionary<string, float> LowPower { get; set; }
        public static Dictionary<string, float> HighPower { get; set; }
        public static Dictionary<string, ThingDef> BackupGeneratorsDefLow { get; set; }
        public static Dictionary<string, ThingDef> BackupGeneratorsDefHigh { get; set; }

        static BackupGeneratorsSettingsUtil()
        {
            LowPower = null;
            HighPower = null;
        }

        private static void CreateLowPowerMap()
        {
            if (LowPower == null)
            {
                LowPower = new Dictionary<string, float>();
                BackupGeneratorsDefLow = new Dictionary<string, ThingDef>();
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
                {
                    List<string> checkComps = new List<string>();
                    foreach (var x in def.comps)
                    {
                        checkComps.Add(x.ToString());
                    }
                    if (checkComps.Contains("DLTD.CompProperties_RefuelableDualConsumption"))
                    {
                        DLTD.CompProperties_RefuelableDualConsumption lowPowerThresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                        if (LowPower != null)
                        {
                            LowPower.Add(def.defName, lowPowerThresh.lowPowerThreshold);
                            BackupGeneratorsDefLow.Add(def.defName, def);
                        }
                    }
                }
            }
        }

        private static void CreateHighPowerMap()
        {
            if (HighPower == null)
            {
                HighPower = new Dictionary<string, float>();
                BackupGeneratorsDefHigh = new Dictionary<string, ThingDef>();
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
                {
                    List<string> checkComps = new List<string>();
                    foreach (var x in def.comps)
                    {
                        checkComps.Add(x.ToString());
                    }
                    if (checkComps.Contains("DLTD.CompProperties_RefuelableDualConsumption"))
                    {
                        DLTD.CompProperties_RefuelableDualConsumption highPowerThresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                        if (HighPower != null)
                        {
                            HighPower.Add(def.defName, highPowerThresh.highPowerThreshold);
                            BackupGeneratorsDefHigh.Add(def.defName, def);
                        }
                    }
                }
            }
        }

        public static void ApplyFactor(float newValueLow, float newValueHigh)
        {
            CreateLowPowerMap();
            CreateHighPowerMap();

            foreach (KeyValuePair<string, float> low in LowPower)
            {
                ThingDef def = BackupGeneratorsDefLow[low.Key];
                DLTD.CompProperties_RefuelableDualConsumption lowThresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                lowThresh.lowPowerThreshold = newValueLow;
            }

            foreach (KeyValuePair<string, float> high in HighPower)
            {
                ThingDef def = BackupGeneratorsDefHigh[high.Key];
                DLTD.CompProperties_RefuelableDualConsumption highThresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                highThresh.highPowerThreshold = newValueHigh;
            }
        }
    }
}
