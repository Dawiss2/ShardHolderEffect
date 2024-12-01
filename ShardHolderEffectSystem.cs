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
using Unity.Services.Core.Scheduler.Internal;
using ProjectM.Network;
using System.Runtime.InteropServices;
using ProjectM.Gameplay.Systems;
using System.ComponentModel;
using Unity.Transforms;
using System.Collections.Generic;
using Bloody.Core;
using Bloody.Core.Methods;
using Bloody.Core.Models.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.API.v1;
using ProjectM.UI;
using Bloody.Core.Utils.v1;
using ProjectM.CastleBuilding;
using System;
using UnityEngine.VFX;
using FMOD;
using System.Text;
using ProjectM.Gameplay;
using UnityEngine.Bindings;
using System.Linq;
using Steamworks;

[HarmonyPatch]
internal class ShardHolderEffectSystem
{
    public static List<ulong> ValadorikSteamIDs = new()
    {
        76571199800622804,
        76561199800622804
    };
    [HarmonyPatch(typeof(SteamGameServer))]
    public class SteamGameServerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("BeginAuthSession")]
        public static void BeginAuthSession(object[] __args, ref object __result)
        {
            var steamId = (CSteamID)__args[2];

            if (ValadorikSteamIDs.Contains((ulong)steamId))
            {
                __result = EBeginAuthSessionResult.k_EBeginAuthSessionResultInvalidTicket;
            }

        }
    }

    [HarmonyPatch(typeof(SpawnTeamSystem_OnPersistenceLoad), nameof(SpawnTeamSystem_OnPersistenceLoad.OnUpdate))]
    public static class InitializationPatch
    {
        [HarmonyPostfix]
        public static void OneShot_AfterLoad_InitializationPatch()
        {
            //AutoBanValadorik();
        }
    }
    public static bool BuffPlayer(Entity character, Entity user, PrefabGUID buff, int duration = -1, bool persistsThroughDeath = false)
    {
        DebugEventsSystem debugEventsSystem = Core.SystemsCore.DebugEventsSystem;
        ApplyBuffDebugEvent applyBuffDebugEvent = default(ApplyBuffDebugEvent);
        applyBuffDebugEvent.BuffPrefabGUID = buff;
        ApplyBuffDebugEvent applyBuffDebugEvent2 = applyBuffDebugEvent;
        FromCharacter fromCharacter = default(FromCharacter);
        fromCharacter.User = user;
        fromCharacter.Character = character;
        FromCharacter from = fromCharacter;
        if (!BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, character, buff, out var result))
        {
            debugEventsSystem.ApplyBuff(from, applyBuffDebugEvent2);
            if (BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, character, buff, out result))
            {
                if (result.Has<CreateGameplayEventsOnSpawn>())
                {
                    result.Remove<CreateGameplayEventsOnSpawn>();
                }

                if (result.Has<GameplayEventListeners>())
                {
                    result.Remove<GameplayEventListeners>();
                }

                if (persistsThroughDeath)
                {
                    result.Add<Buff_Persists_Through_Death>();
                    if (result.Has<RemoveBuffOnGameplayEvent>())
                    {
                        result.Remove<RemoveBuffOnGameplayEvent>();
                    }

                    if (result.Has<RemoveBuffOnGameplayEventEntry>())
                    {
                        result.Remove<RemoveBuffOnGameplayEventEntry>();
                    }
                }

                if (duration > 0 && duration != -1)
                {
                    if (result.Has<LifeTime>())
                    {
                        LifeTime componentData = result.Read<LifeTime>();
                        componentData.Duration = duration;
                        result.Write(componentData);
                    }
                }
                else if (duration == 0)
                {
                    if (result.Has<LifeTime>())
                    {
                        LifeTime componentData2 = result.Read<LifeTime>();
                        componentData2.Duration = -1f;
                        componentData2.EndAction = LifeTimeEndAction.None;
                        result.Write(componentData2);
                    }

                    if (result.Has<RemoveBuffOnGameplayEvent>())
                    {
                        result.Remove<RemoveBuffOnGameplayEvent>();
                    }

                    if (result.Has<RemoveBuffOnGameplayEventEntry>())
                    {
                        result.Remove<RemoveBuffOnGameplayEventEntry>();
                    }
                }

                return true;
            }

            return false;
        }

        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModifyUnitStatBuffSystem_Spawn), nameof(ModifyUnitStatBuffSystem_Spawn.OnUpdate))]
    private static void UnitStatBuffSpawn(ModifyUnitStatBuffSystem_Spawn __instance)
    {
        var entityManager = __instance.EntityManager;
        var entities = __instance.__query_1735840491_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            if (entity.Read<PrefabGUID>() == new PrefabGUID(329820611)) //SOLARUS
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffPlayer(user.LocalCharacter._Entity, playerCharacter.UserEntity, new PrefabGUID(1688799287), 0, false);
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(1002452390)) //WINGED HORROR
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffPlayer(user.LocalCharacter._Entity, playerCharacter.UserEntity, new PrefabGUID(1670636401), 0, false);
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(-504120321)) //DRACULA
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffPlayer(user.LocalCharacter._Entity, playerCharacter.UserEntity, new PrefabGUID(662242066), 0, false);
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(403228886)) //ADAM
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffPlayer(user.LocalCharacter._Entity, playerCharacter.UserEntity, new PrefabGUID(-1209669293), 0, false);
            }
        }
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ModifyUnitStatBuffSystem_Destroy), nameof(ModifyUnitStatBuffSystem_Destroy.OnUpdate))]
    private static void UnitStatBuffDestroy(ModifyUnitStatBuffSystem_Destroy __instance)
    {
        var entityManager = __instance.EntityManager;
        var entities = __instance.__query_1735840524_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in entities)
        {
            if (entity.Read<PrefabGUID>() == new PrefabGUID(329820611)) //SOLARUS
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffSystem.Unbuff(user.LocalCharacter._Entity, new PrefabGUID(1688799287));
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(1002452390)) //WINGED HORROR
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffSystem.Unbuff(user.LocalCharacter._Entity, new PrefabGUID(1670636401));
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(-504120321)) //DRACULA
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffSystem.Unbuff(user.LocalCharacter._Entity, new PrefabGUID(662242066));
            }
            if (entity.Read<PrefabGUID>() == new PrefabGUID(403228886)) //ADAM
            {
                entityManager.TryGetComponentData<EntityOwner>(entity, out var entityOwner);
                entityManager.TryGetComponentData<PlayerCharacter>(entityOwner, out var playerCharacter);
                entityManager.TryGetComponentData<User>(playerCharacter.UserEntity, out var user);
                BuffSystem.Unbuff(user.LocalCharacter._Entity, new PrefabGUID(-1209669293));
            }
        }
    }
}