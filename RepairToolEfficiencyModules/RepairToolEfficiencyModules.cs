using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace LawAbidingTroller.RepairToolUpgrades.RepairToolEfficiencyModules;

public class RepairToolEfficiencyModules
{
    public static void Register(UpgradeData upgrade, PrefabInfo prefabinfo, CustomPrefab prefab, int mk, List<CraftData.Ingredient> recipe)
    {
        prefabinfo = PrefabInfo.WithTechType($"RepairToolEfficiencyUpgradeMk{mk}", $"Repair Tool Efficiency Upgrade Mk {mk}", $"Mk {mk} efficiency upgrade for the Repair Tool. Increases the repair efficiency by {upgrade.Efficiency}x normal efficiency.");
        UpgradeData.UpgradeDataDict.Add(prefabinfo.TechType, upgrade);
        prefab = new CustomPrefab(prefabinfo);
        var clone = new CloneTemplate(prefabinfo, TechType.VehiclePowerUpgradeModule);
        clone.ModifyPrefab += obj =>
        {
            GameObject model = obj.gameObject;
            model.transform.localScale = Vector3.one / (1 + 0.5f);
        };
        prefab.SetGameObject(clone);
        prefab.SetRecipe(new Nautilus.Crafting.RecipeData()
        {
            craftAmount = 1,
            Ingredients = recipe
        }).WithFabricatorType(UpgradesLIB.Items.Equipment.Handheldprefab.HandheldfabTreeType)
        .WithStepsToFabricatorTab("Tools", "RepairToolTab")
        .WithCraftingTime(5f);
        prefab.SetUnlock(TechType.Welder);
        prefab.SetPdaGroupCategory(UpgradesLIB.Plugin.toolupgrademodules, Plugin.Repairtoolupgrades);
        prefab.Register();
        Plugin.Logger.LogInfo($"Prefab RepairToolEfficiencyUpgradeMk{mk} successfully initialized!");
    }
}

public class IngredientList
{
    public List<CraftData.Ingredient> Ingredients = new();

    public IngredientList(params  CraftData.Ingredient[] ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            this.Ingredients.Add(ingredient);
        }
    }
}