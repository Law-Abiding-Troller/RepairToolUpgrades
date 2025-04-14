using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;
using System.Collections;
using JetBrains.Annotations;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using Nautilus.Handlers;
using rail;
using RepairToolUpgrades;
using PluginInfo = RepairToolUpgrades.PluginInfo;
using RepairToolUpgrades.RepairToolModules;
using System.IO;

namespace LawAbidingTroller.RepairToolUpgrades
{
    [BepInPlugin("com.lawabidingtroller.repairtoolupgrades", "RepairToolUpgrades", "0.1.1")]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

    public IEnumerator SetWelderUpgrades(TechType techtype)
    {
        // Fetch the prefab:
        Logger.LogInfo("Fetching prefab for tech type...");
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(techtype);
        // Wait for the prefab task to complete:
        yield return task;
        Logger.LogInfo("Prefab fetch completed.");
        // Get the prefab:
        GameObject prefab = task.GetResult();
        Logger.LogInfo($"The prefab is {prefab}");
        // Use the prefab to add a storage contianer:
        PrefabUtils.AddStorageContainer(prefab, "WelderUpgradeStorage", "WelderUpgradeStorageChild", 2, 1, true);
        InitializePrefabs();
    }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        /*public static void list()
        {

            string[] lines = { };
            List<GameObject> objects = new List<GameObject>();
            int select = 0;
            foreach (var obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                objects.Add(obj);
            }
            var techTypeValues = Enum.GetValues(typeof(TechType)).Cast<TechType>();
            foreach (var techtype in techTypeValues)
            {
                lines[select] = techtype.ToString();
                select++;
            }
            foreach (var obj in objects)
            {
                lines[select] = obj.ToString();
                var components = obj.GetComponents(typeof(Component));
                foreach (var component in components)
                {
                    lines[select] = component.ToString();
                }
                select++;
            }
            File.WriteAllLines("insertfilepathhere", lines);
        }*/
        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // Set the storage
            StartCoroutine(SetWelderUpgrades(TechType.Welder));

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, "RepairToolUpgrades");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            // create the tabs and move the welder to the tab
            CraftTreeHandler.AddTabNode(CraftTree.Type.Fabricator, "RepairToolTab", "Repair Tool", SpriteManager.Get(TechType.Welder), "Personal", "Tools");
            CraftTreeHandler.AddCraftingNode(CraftTree.Type.Fabricator, TechType.Welder, "Personal", "Tools", "RepairToolTab");
            CraftTreeHandler.RemoveNode(CraftTree.Type.Fabricator, "Personal", "Tools", "Welder");

            //list();
        }
        int timer = 0;
        bool debugmode;
        bool hasalreadyran = false;
        bool enablereset;
        private void Update()
        {
            timer += 1;
            if (debugmode)
            {
                Logger.LogInfo(timer.ToString());
            }
            if (Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.W))
            {
                debugmode = true;
            }
            if (timer == 5000 && !hasalreadyran)
            {
                timer = 0;
                hasalreadyran = true;
                Logger.LogInfo("Initial Update Ran! Timer reset");
            }
            else if (timer == 500 && hasalreadyran)
            {
                timer = 0;
                Logger.LogInfo("Timer Reset.");
            }
            if (Inventory.main == null)
            {
                return;
            }
            PlayerTool heldtool = Inventory.main.GetHeldTool();
            if (heldtool == null)
            {
                return;
            }
            if (heldtool is Welder welder)
            {
                var tempstorage = heldtool.gameObject.GetComponent<StorageContainer>();
                if (tempstorage == null)
                {
                    return;
                }
                var allowedtech = new TechType[] { RepairToolSpeedModuleMk1.mk1weldspeedprefabinfo.TechType, RepairToolSpeedModuleMk2.mk2weldspeedprefabinfo.TechType, RepairToolSpeedModuleMk3.mk3weldspeedprefabinfo.TechType };
                tempstorage.container.SetAllowedTechTypes(allowedtech);
                if (Input.GetKeyDown(KeyCode.B))
                {
                    Logger.LogInfo($"Open Storage Container Key Pressed for {heldtool}");
                    if (tempstorage.open)
                    {
                        return;
                    }
                    tempstorage.container._label = "REPAIR TOOL";
                    tempstorage.Open();
                }
                if (tempstorage.container.Contains(RepairToolSpeedModuleMk1.mk1weldspeedprefabinfo.TechType))
                {
                    IncreaseWeldSpeed(RepairToolSpeedModuleMk1.mk1weldspeed, welder);
                }
                if (tempstorage.container.Contains(RepairToolSpeedModuleMk2.mk2weldspeedprefabinfo.TechType))
                {
                    IncreaseWeldSpeed(RepairToolSpeedModuleMk2.mk2weldspeed, welder);
                }
                if (tempstorage.container.Contains(RepairToolSpeedModuleMk3.mk3weldspeedprefabinfo.TechType))
                {
                    IncreaseWeldSpeed(RepairToolSpeedModuleMk3.mk3weldspeed, welder);
                }
            }
        }
        int defaultselectionincrease = 0;
        int defaultselectiondecrease = 0;
        public float [] defaults;
        public void IncreaseWeldSpeed(float speed, Welder instance)
        {
            defaults[defaultselectionincrease] = instance.healthPerWeld;
            instance.healthPerWeld = defaults[defaultselectionincrease] * speed;
            if (timer == 10)
            {
                Logger.LogInfo($"Weld speed increased by a factor of {speed}");
            }
            enablereset = true;
            defaultselectionincrease += 1;
        }
        public void ResetWeldSpeed(Welder instance)
        {
            if (!enablereset)
            {
                if (timer == 100)
                {
                    Logger.LogWarning("Weld Speed was never increased! It likely didn't even start yet.");
                }
                return;
            }
            instance.healthPerWeld /= defaults[defaultselectiondecrease];
            if (timer == 100)
            {
                Logger.LogInfo("Weld speed Reset.");
            }
            defaultselectiondecrease += 1;
        }
        private void InitializePrefabs()
        {
            RepairToolSpeedModuleMk1.Register();
            RepairToolSpeedModuleMk2.Register();
            RepairToolSpeedModuleMk3.Register();
        }
    }
}