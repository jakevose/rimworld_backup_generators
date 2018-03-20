using Harmony;
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
            var harmony = HarmonyInstance.Create("com.backupgenerators.rimworld.mod");
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
            BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerTresh.AsFloat, Settings.HighPowerTresh.AsFloat);
        }
    }

    [HarmonyPatch(typeof(GameComponentUtility), "LoadedGame")]
    static class Patch_GameComponentUtility_LoadedGame
    {
        static void Postfix()
        {
            BackupGeneratorsSettingsUtil.ApplyFactor(Settings.LowPowerTresh.AsFloat, Settings.HighPowerTresh.AsFloat);
        }
    }
}
