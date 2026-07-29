using System.Linq;
using BepInEx;
using HarmonyLib;
using TeamCherry.Localization;
using System;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace AllNeedolin;

[BepInAutoPlugin(id: "io.github.carrieforle.allneedolin")]
public partial class AllNeedolinPlugin : BaseUnityPlugin
{
    private static ConfigEntry<bool> configEnableMod;
    private static string currentText = "";
    private static ManualLogSource logger;
    private Harmony harmony;

    private void Start()
    {
        harmony = Harmony.CreateAndPatchAll(typeof(AllNeedolinPlugin));
        logger = Logger;
        configEnableMod = Config.Bind("General", "EnableMod", true);
        configEnableMod.SettingChanged += (s, e) =>
        {
            if (configEnableMod.Value)
            {
                harmony.PatchAll(typeof(AllNeedolinPlugin));
            }
            else
            {
                harmony.UnpatchSelf();
            }
        };
    }

#pragma warning disable HARMONIZE003
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalisedTextCollectionData), nameof(LocalisedTextCollectionData.GetRandom))]
    private static void PatchRandom(LocalisedTextCollectionData __instance, ref LocalisedString __result)
    {
        if (!string.IsNullOrEmpty(currentText))
        {
            __result = new LocalisedString(Id, currentText);
            return;
        }

        var data = __instance;
        while (true)
        {
            var resolvedData = data.ResolveAlternatives();
            if (resolvedData == data)
            {
                break;
            }

            data = resolvedData;
        }

        // https://softwareengineering.stackexchange.com/questions/233541/how-to-implement-a-weighted-shuffle
        var indexes = Enumerable.Range(0, data.currentTexts.Length);

        if (data.currentProbabilities != null)
        {
            indexes = indexes.OrderBy(i => Math.Pow(UnityEngine.Random.value, 1.0 / data.currentProbabilities[i]));
        }
        else
        {
            indexes = indexes.OrderBy(i => Math.Pow(UnityEngine.Random.value, 1.0 / data.currentTexts[i].Probability));
        }

        var shuffledTexts = indexes.Select(i => data.currentTexts[i].text.ToString());
        currentText = string.Join("\n", shuffledTexts);

        __result = new LocalisedString(Id, currentText);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LocalisedString), nameof(LocalisedString.ToString), [typeof(bool)])]
    private static bool BypassLocalisedStringCheck(ref LocalisedString __instance, ref string __result)
    {
        if (__instance.Sheet == Id && __instance.Key == currentText)
        {
            __result = currentText;
            return false;
        }

        return true;
    }
    
    // Called every time needoin msg box starts
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NeedolinMsgBox), nameof(NeedolinMsgBox.CycleTexts))]
    private static void ResetCurrentText()
    {
        currentText = "";
    }

    // Overflow needolin texts so it can show more than 3 paragraphs.
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NeedolinMsgBox), nameof(NeedolinMsgBox.Awake))]
    private static void OverflowNeedolin(NeedolinMsgBox __instance)
    {
        foreach (var f in __instance.GetComponentsInChildren<TMProOld.TextMeshPro>())
        {
            f.OverflowMode = TMProOld.TextOverflowModes.Overflow;
        }
    }
}