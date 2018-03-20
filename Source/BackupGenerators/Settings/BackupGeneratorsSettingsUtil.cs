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
            Log.Message("BackupGenerators: CreateLowPowerMap Start");
            if (LowPower == null)
            {
                Log.Message("BackupGenerators: CreateLowPowerMap in if clause lowpower gleich null");
                LowPower = new Dictionary<string, float>();
                Log.Message("BackupGenerators: CreateLowPowerMap lowPower gesetzt");
                BackupGeneratorsDefLow = new Dictionary<string, ThingDef>();
                Log.Message("BackupGenerators: CreateLowPowerMap Backupgenerator def gesetzt");
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
                {
                    Log.Message("BackupGenerators: CreateLowPowerMap in foreach schleife");
                    string test = "";
                    List<string> checkComps = new List<string>();
                    foreach (var x in def.comps)
                    {
                        checkComps.Add(x.ToString());
                        test += x;
                    }
                    Log.Message(test);
                    if (checkComps.Contains("DLTD.CompProperties_RefuelableDualConsumption"))
                    {
                        Log.Message("BackupGenerators: CreateLowPowerMap in if schleife defnamestartswith, defname = " + def.defName);
                        
                        DLTD.CompProperties_RefuelableDualConsumption lowPowerTresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                        Log.Message("BackupGenerators: CreateLowPowerMap lowpowertech gesetzt");
                        if (LowPower != null)
                        {
                            //Log.Message("BackupGenerators: CreateLowPowerMap in if schleife lowpower ungleich null, defname = "+ def.defName + " lowerpowertresh = " + lowPowerTresh.lowPowerTreshhold);
                            LowPower.Add(def.defName, lowPowerTresh.lowPowerTreshhold);
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
                    string test = "";
                    List<string> checkComps = new List<string>();
                    foreach (var x in def.comps)
                    {
                        checkComps.Add(x.ToString());
                        test += x;
                    }
                    if (checkComps.Contains("DLTD.CompProperties_RefuelableDualConsumption"))
                    {
                        DLTD.CompProperties_RefuelableDualConsumption highPowerTresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                        if (HighPower != null)
                        {
                            HighPower.Add(def.defName, highPowerTresh.highPowerTreshhold);
                            BackupGeneratorsDefHigh.Add(def.defName, def);
                        }
                    }
                }
            }
        }

        //private static void CreateHighPowerMap()
        //{
        //    if (HighPower == null)
        //    {
        //        HighPower = new Dictionary<string, float>();
        //        BackupGeneratorsDefHigh = new Dictionary<string, ThingDef>();
        //        foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
        //        {
        //            if (def.defName.StartsWith("BackupGenerator"))
        //            {
        //                DLTD.CompProperties_RefuelableDualConsumption highPowerTresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
        //                if (HighPower != null)
        //                {
        //                    HighPower.Add(def.defName, highPowerTresh.highPowerTreshhold);
        //                    BackupGeneratorsDefHigh.Add(def.defName, def);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void ApplyFactor(float newValueLow, float newValueHigh)
        {
            CreateLowPowerMap();
            CreateHighPowerMap();

            foreach (KeyValuePair<string, float> low in LowPower)
            {
                ThingDef def = BackupGeneratorsDefLow[low.Key];
                DLTD.CompProperties_RefuelableDualConsumption lowTresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                lowTresh.lowPowerTreshhold = newValueLow;
            }

            foreach (KeyValuePair<string, float> high in HighPower)
            {
                ThingDef def = BackupGeneratorsDefHigh[high.Key];
                DLTD.CompProperties_RefuelableDualConsumption highTresh = def.GetCompProperties<DLTD.CompProperties_RefuelableDualConsumption>();
                highTresh.highPowerTreshhold = newValueHigh;
            }
        }
    }
}