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
    public class RepairToolSpeedModuleMk1
    {
        public static float mk1weldspeed = 1.5f;
        public static CustomPrefab mk1weldspeedprefab;
        public static PrefabInfo mk1weldspeedprefabinfo;
        public static TechType techType = TechType.VehiclePowerUpgradeModule;
        public static void Register()
        {
            mk1weldspeedprefabinfo = PrefabInfo.WithTechType("RepairToolSpeedUpgradeMk1", "Repair Tool Speed Upgrade Mk 1", "Mk 1 Speed Upgrade for the Repair Tool. Decreases the repair time to 3/4s of the normal repair time").WithIcon(SpriteManager.Get(TechType.Welder));
            mk1weldspeedprefab = new CustomPrefab(mk1weldspeedprefabinfo);
            var clone = new CloneTemplate(mk1weldspeedprefabinfo, techType);
            clone.ModifyPrefab += obj =>
            {
                GameObject model = obj.gameObject;
                model.transform.localScale = Vector3.one / (1 + 1 / 2);
            };
            mk1weldspeedprefab.SetGameObject(clone);
            mk1weldspeedprefab.SetRecipe(new Nautilus.Crafting.RecipeData()
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
            mk1weldspeedprefab.SetUnlock(TechType.Welder);
            mk1weldspeedprefab.Register();
            Plugin.Logger.LogInfo("Prefab RepairToolSpeedUpgradeMk1 successfully initialized!");
        }
    }
}
