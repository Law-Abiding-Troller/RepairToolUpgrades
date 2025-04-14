using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LawAbidingTroller.RepairToolUpgrades;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace RepairToolUpgrades.RepairToolModules
{
    public class RepairToolSpeedModuleMk2
    {
        public static float mk2weldspeed = 2.0f;
        public static CustomPrefab mk2weldspeedprefab;
        public static PrefabInfo mk2weldspeedprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk2weldspeedprefabinfo = PrefabInfo.WithTechType("RepairToolSpeedUpgradeMk2", "Repair Tool Speed Upgrade Mk 2", "Mk 2 Speed Upgrade for the Repair Tool. Halfs the normal repair time.").WithIcon(SpriteManager.Get(TechType.Welder));
            mk2weldspeedprefab = new CustomPrefab(mk2weldspeedprefabinfo);
            var clone = new CloneTemplate(mk2weldspeedprefabinfo, techType);
            clone.ModifyPrefab += obj =>
            {
                GameObject model = obj.gameObject;
                model.transform.localScale = Vector3.one / (1 + 1 / 2);
            };
            mk2weldspeedprefab.SetGameObject(clone);
            mk2weldspeedprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.Lubricant, 1),
                    new CraftData.Ingredient(TechType.WiringKit, 1),
                    new CraftData.Ingredient(RepairToolSpeedModuleMk1.mk1weldspeedprefabinfo.TechType, 1)
                }
            })
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Tools", "RepairToolTab")
            .WithCraftingTime(5f);
            mk2weldspeedprefab.SetUnlock(TechType.Welder);
            mk2weldspeedprefab.Register();
            Plugin.Logger.LogInfo("Prefab RepairToolSpeedUpgradeMk3 successfully initialized!");
        }
    }
}
