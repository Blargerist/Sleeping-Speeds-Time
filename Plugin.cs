using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using Unity.Entities;

namespace SleepingSpeedsTime {
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Plugin : BasePlugin {

        const String pluginGUID = "me.blargerist.SleepingSpeedsTime";
        const String pluginName = "SleepingSpeedsTime";
        const String pluginVersion = "1.0.0";
        public static ManualLogSource logger;

        public override void Load() {
            logger = this.Log;

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(pluginGUID);
            harmony.PatchAll();
        }

        /*
         * Whether a player is sleeping is separate to the player Entity itself.
         * Instead, it exists in the form of an Entity with an associated Buff, SpawnSleepingBuff and InsideBuff.
         */
        public static Unity.Collections.NativeArray<Entity> GetSleepingEntities(EntityManager entityManager) {
            //Creates an EntityQuery to search for entities with the correct component types
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<Buff>(), ComponentType.ReadOnly<SpawnSleepingBuff>(), ComponentType.ReadOnly<InsideBuff>());
            //Queries and returns matching entities
            return query.ToEntityArray(Unity.Collections.Allocator.Temp);
        }

        /*
         * A player Entity is one with an associated PlayerCharacter component
         */
        public static Unity.Collections.NativeArray<Entity> GetPlayerEntities(EntityManager entityManager) {
            //Creates an EntityQuery to search for entities with the correct component type
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerCharacter>());
            //Queries and returns matching entities
            return query.ToEntityArray(Unity.Collections.Allocator.Temp);
        }

        /*
         * Tests whether all players are sleeping by looping through the list of player entities and checking if there is a sleeping Buff component that references their Entity as the target
         */
        public static bool TestAllPlayersSleeping(EntityManager entityManager) {
            //Get all entities for sleep buffs
            var sleepingEntities = GetSleepingEntities(entityManager);
            //Gets all player entities
            var players = GetPlayerEntities(entityManager);

            //Loop through all players
            foreach (var player in players) {
                bool sleeping = false;
                //Loop through all sleep buff entities
                foreach (var sleepingEntity in sleepingEntities) {
                    //Get Buff
                    var buff = entityManager.GetComponentData<Buff>(sleepingEntity);
                    //Test if the player is the Target of the buff
                    if (buff.Target == player) {
                        sleeping = true;
                        break;
                    }
                }
                if (!sleeping) {
                    return false;
                }
            }
            return true;
        }

        //System updates only while a player is inside a coffin
        [HarmonyPatch(typeof(SleepInsideSystem), "OnUpdate")]
        public static class SleepInsideSystemOnUpdatePatch {
            private static void Prefix(SleepInsideSystem __instance) {
                try {
                    //Test if all players are sleeping
                    if (TestAllPlayersSleeping(__instance.EntityManager)) {
                        //Create a new Entity and associate a SetTimeOfDayEvent which adds 1 minute of time
                        var setTimeEntity = __instance.EntityManager.CreateEntity(ComponentType.ReadOnly<SetTimeOfDayEvent>());
                        __instance.EntityManager.SetComponentData<SetTimeOfDayEvent>(setTimeEntity, new SetTimeOfDayEvent() { Day = 0, Hour = 0, Minute = 1, Month = 0, Year = 0, Type = SetTimeOfDayEvent.SetTimeType.Add });
                        //Created Entity will be picked up by another System automatically and used to update the time
                    }
                }
                catch (Exception e) {
                    logger.LogError(e);
                }
            }
        }
    }
}
