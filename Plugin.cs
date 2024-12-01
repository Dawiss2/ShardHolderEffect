using BepInEx;
using BepInEx.Unity.IL2CPP;
using Bloodstone.API;
using HarmonyLib;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Scripting;
using Stunlock.Core;
using Unity.Collections;
using Unity.Entities;
using VampireCommandFramework;
using UnityEngine;
using ProjectM.Gameplay.Systems;

namespace ShardHolderEffect;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin
{
    Harmony _harmony;
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }
    private static bool HasLoaded()
    {
        // Hack, check to make sure that entities loaded enough because this function
        // will be called when the plugin is first loaded, when this will return 0
        // but also during reload when there is data to initialize with.
        var collectionSystem = VWorld.Server.GetExistingSystemManaged<PrefabCollectionSystem>();
        return collectionSystem?.SpawnableNameToPrefabGuidDictionary.Count > 0;
    }
    public override bool Unload()
    {
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }
}
