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
    public class RepairToolSpeedModuleMk3
    {
        public static float mk3weldspeed = 3.0f;
        public static CustomPrefab mk3weldspeedprefab;
        public static PrefabInfo mk3weldspeedprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk3weldspeedprefabinfo = PrefabInfo.WithTechType("RepairToolSpeedUpgradeMk3", "Repair Tool Speed Upgrade Mk 3", "Mk 3 Speed Upgrade for the Repair Tool. Decreases the repair time to 1/4 of the normal repair time.").WithIcon(SpriteManager.Get(TechType.Welder));
            mk3weldspeedprefab = new CustomPrefab(mk3weldspeedprefabinfo);
            var clone = new CloneTemplate(mk3weldspeedprefabinfo, techType);
            clone.ModifyPrefab += obj =>
            {
                GameObject model = obj.gameObject;
                model.transform.localScale = Vector3.one / (1 + 1 / 2);
            };
            mk3weldspeedprefab.SetGameObject(clone);
            mk3weldspeedprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new CraftData.Ingredient(TechType.Battery, 1),
                    new CraftData.Ingredient(TechType.WiringKit, 1)
                }
            })
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Tools", "RepairToolTab")
            .WithCraftingTime(5f);
            mk3weldspeedprefab.SetUnlock(TechType.Welder);
            mk3weldspeedprefab.Register();
            Plugin.Logger.LogInfo("Prefab RepairToolSpeedUpgradeMk3 successfully initialized!");
        }
    }
}
