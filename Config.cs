using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace LawAbidingTroller.RepairToolUpgrades;

[Menu("Repair Tool Upgrades")]
public class Config : ConfigFile
{
    [Keybind("Open Upgrades Container Keybind"), OnChange(nameof(KeyBindChangeEvent))]
    public KeyCode OpenUpgradesContainerKeybind = KeyCode.B;
    public static KeyCode OpenUpgradesContainerkeybind = KeyCode.B;

    void KeyBindChangeEvent(KeybindChangedEventArgs args)
    {
        OpenUpgradesContainerkeybind = args.Value;
    }
}