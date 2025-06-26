using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RepairToolUpgrades.RepairToolModules;
using UnityEngine;

namespace LawAbidingTroller.RepairToolUpgrades;

[HarmonyPatch(typeof(Welder))]
public class WelderPatches
{
    [HarmonyPatch(nameof(Welder.Update))] [HarmonyPostfix]
    public static void Update_Postfix(Welder __instance)
    {
        if (__instance == null) {Plugin.Logger.LogError($"__instance is null in {nameof(Update_Postfix)}!");return;}
        if (__instance.usedThisFrame) {__instance.Weld(); }
        var tempstorage = __instance.GetComponent<StorageContainer>();
        if (tempstorage == null) {Plugin.Logger.LogError($"tempstorage is null in {nameof(Update_Postfix)}!");return;}
        if (Input.GetKeyDown(Config.OpenUpgradesContainerkeybind))
        {
            if (tempstorage.open) { ErrorMessage.AddWarning("Close 'REPAIR TOOL' to open it" ); return; }
            tempstorage.Open();
        }
        __instance.StopWeldingFX();
    }
    [HarmonyPatch(nameof(Welder.Weld))]
    [HarmonyPrefix]
    public static void Weld_Prefix(Welder __instance)
    {
        __instance.healthPerWeld = 0.5f * Time.deltaTime;
        __instance.weldEnergyCost = 0.005f * Time.deltaTime;
        var tempstorage = __instance.GetComponent<StorageContainer>();
        if (tempstorage == null) {Plugin.Logger.LogError("Failed to get storage container component for Repair tool!");return;}
        UpgradeData tempdata;
        float highestspeed = 0;
        float highestefficiency = 0;
        foreach (var item in tempstorage.container.GetItemTypes())
        {
            if (!UpgradeData.UpgradeDataDict.TryGetValue(item, out tempdata)) { Plugin.Logger.LogError($"Cannot get TechType '{item}' from dictionary '{nameof(UpgradeData.UpgradeDataDict)}'");continue;}

            if (tempdata.Speedmultiplier == 0)
            {
                highestefficiency = Mathf.Max(highestefficiency, tempdata.Efficiency);
            }
            highestspeed = Mathf.Max(highestspeed, tempdata.Speedmultiplier);
        }
        if (highestspeed == 0 && highestefficiency == 0) return;
        if (highestspeed != 0)
        {
            __instance.healthPerWeld *= highestspeed;
        }

        if (highestefficiency != 0)
        {
            __instance.weldEnergyCost *= highestefficiency;
        }
    }
}

[HarmonyPatch(typeof(PlayerTool))]
public class PlayerToolPatches
{
    [HarmonyPatch(nameof(PlayerTool.Awake))]
    [HarmonyPostfix]
    public static void Awake_Postfix(PlayerTool __instance)
    {
        if (__instance == null) return;
        if (__instance is not Welder) return;
        var tempstorage = __instance.GetComponent<StorageContainer>();
        if (tempstorage == null) return;
        if (tempstorage.container == null) {Plugin.Logger.LogError("tempstorage.container is null!");return;}
        tempstorage.container._label = "REPAIR TOOL";
        var allowedtech = new[]
        {
            RepairToolSpeedModuleMk1.Mk1Weldspeedprefabinfo.TechType,
            RepairToolSpeedModuleMk2.Mk2Weldspeedprefabinfo.TechType,
            RepairToolSpeedModuleMk3.Mk3Weldspeedprefabinfo.TechType,
            Plugin.PrefabInfos[1].TechType,
            Plugin.PrefabInfos[2].TechType,
            Plugin.PrefabInfos[3].TechType
        };
        tempstorage.container.SetAllowedTechTypes(allowedtech);
    }
}

[HarmonyPatch(typeof(Welder))]
[HarmonyPatch(nameof(Welder.Weld))]
public static class WelderWeldPatch
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new[] {new CodeMatch(OpCodes.Ldarg_0), new CodeMatch(OpCodes.Ldc_I4_1), new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Welder), nameof(Welder.fxIsPlaying))) })
            .Advance(-12)
            .RemoveInstructions(11)
            .Advance(4)
            .RemoveInstructions(3)
            .InstructionEnumeration();
    }
}

public class UpgradeData
{
    public static Dictionary<TechType, UpgradeData> UpgradeDataDict = new Dictionary<TechType, UpgradeData>();
    public float Speedmultiplier;
    public float Efficiency;


    public UpgradeData(float speedmultiplier = 0, float efficiency = 0)
    {
        this.Speedmultiplier = speedmultiplier;
        this.Efficiency = efficiency;
    }
}