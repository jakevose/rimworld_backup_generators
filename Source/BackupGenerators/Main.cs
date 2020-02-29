using HarmonyLib;
using RimWorld;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace BackupGenerators
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.backupgenerators.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("BackupGenerators: Adding Harmony Postfix to GameComponentUtility.StartedNewGame");
            Log.Message("BackupGenerators: Adding Harmony Postfix to GameComponentUtility.LoadedGame");
        }
    }

    [HarmonyPatch(typeof(GameComponentUtility), "StartedNewGame")]
    static class Patch_GameComponentUtility_StartedNewGame
    {
        static void Postfix()
        {
			if (Settings.LowPowerThresh.AsFloat == 1 && Settings.HighPowerThresh.AsFloat == 1)
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(200, 1000);
            }
            else
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerThresh.AsFloat, Settings.HighPowerThresh.AsFloat);
            }
        }
    }

    [HarmonyPatch(typeof(GameComponentUtility), "LoadedGame")]
    static class Patch_GameComponentUtility_LoadedGame
    {
        static void Postfix()
        {
			if (Settings.LowPowerThresh.AsFloat == 1 && Settings.HighPowerThresh.AsFloat == 1)
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(200, 1000);
            }
            else
            {
                BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerThresh.AsFloat, Settings.HighPowerThresh.AsFloat);
            }
        }
    }
}
