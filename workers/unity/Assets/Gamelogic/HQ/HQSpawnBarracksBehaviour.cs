using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplate;
using Assets.Gamelogic.Life;
using Improbable;
using Improbable.Building;
using Improbable.Core;
using Improbable.Team;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using System.Collections;
using System.Collections.Generic;
using Assets.Gamelogic.Utils;
using Improbable.Entity.Component;
using Improbable.Unity.Entity;
using UnityEngine;

namespace Assets.Gamelogic.HQ
{
    public class HQSpawnBarracksBehaviour : MonoBehaviour
    {
        [Require] private HQInfo.Writer hqInfo;
        [Require] private TeamAssignment.Reader teamAssignment;

        private Coroutine spawnBarracksPeriodicallyCoroutine;
        private readonly HashSet<GameObject> barracksSet = new HashSet<GameObject>();
        private float barracksSpawnRadius;

        private void OnEnable()
        {
            barracksSpawnRadius = SimulationSettings.DefaultHQBarracksSpawnRadius;
            spawnBarracksPeriodicallyCoroutine = StartCoroutine(TimerUtils.CallRepeatedly(SimulationSettings.SimulationTickInterval * 5f, SpawnBarracks));
            hqInfo.CommandReceiver.OnRegisterBarracks.RegisterResponse(OnRegisterBarracks);
            hqInfo.CommandReceiver.OnUnregisterBarracks.RegisterResponse(OnUnregisterBarracks);
            hqInfo.ComponentUpdated.Add(OnComponentUpdated);
            PopulateBarracksDictionary();
        }

        private void OnDisable()
        {
            hqInfo.CommandReceiver.OnRegisterBarracks.DeregisterResponse();
            hqInfo.CommandReceiver.OnUnregisterBarracks.DeregisterResponse();
            hqInfo.ComponentUpdated.Remove(OnComponentUpdated);
            CancelSpawnBarracksPeriodicallyCoroutine();
        }

        private Nothing OnRegisterBarracks(RegisterBarracksRequest request, ICommandCallerInfo callerinfo)
        {
            var newBarracks = new Improbable.Collections.List<EntityId>(hqInfo.Data.barracks);
            newBarracks.Add(request.entityId);
            hqInfo.Send(new HQInfo.Update().SetBarracks(newBarracks));
            return new Nothing();
        }

        private Nothing OnUnregisterBarracks(UnregisterBarracksRequest request, ICommandCallerInfo callerinfo)
        {
            var barracks = new Improbable.Collections.List<EntityId>(hqInfo.Data.barracks);
            if (barracks.Contains(request.entityId))
            {
                barracks.Remove(request.entityId);
            }
            hqInfo.Send(new HQInfo.Update().SetBarracks(barracks));
            return new Nothing();
        }

        private void PopulateBarracksDictionary()
        {
            for (var i = 0; i < hqInfo.Data.barracks.Count; i++)
            {
                var barracksEntityObject = LocalEntities.Instance.Get(hqInfo.Data.barracks[i]);
                if (barracksEntityObject != null)
                {
                    var barracksGameObject = barracksEntityObject.UnderlyingGameObject;
                    if (!barracksSet.Contains(barracksGameObject))
                    {
                        barracksSet.Add(barracksGameObject);
                    }
                }
            }
        }

        private void CancelSpawnBarracksPeriodicallyCoroutine()
        {
            if (spawnBarracksPeriodicallyCoroutine != null)
            {
                StopCoroutine(spawnBarracksPeriodicallyCoroutine);
                spawnBarracksPeriodicallyCoroutine = null;
            }
        }

        private void OnComponentUpdated(HQInfo.Update update)
        {
            if (update.barracks.HasValue)
            {
                PopulateBarracksDictionary();
            }
        }

        private void SpawnBarracks()
        {
            if (AllBarracksAtFullHealth())
            {
                SpawnUnconstructedBarracksAtRandomLocation();
            }
        }

        private bool AllBarracksAtFullHealth()
        {
            var barracksEnumerator = Physics.OverlapSphere(transform.position, barracksSpawnRadius);

            var allBarracksFullHealth = true;
            for(var i = 0; i < barracksEnumerator.Length; i++)
            { 
                if(barracksEnumerator[i].gameObject.name.Contains("Barracks"))
                {
                    var health = barracksEnumerator[i].gameObject.GetComponent<HealthVisualizer>();
                    if (health.CurrentHealth < health.MaxHealth)
                    {
                        allBarracksFullHealth = false;
                    }
                }
            }
            return allBarracksFullHealth;
        }

        private void SpawnUnconstructedBarracksAtRandomLocation()
        {
            var spawnPosition = FindSpawnLocation();
            if (SpawnLocationInvalid(spawnPosition))
            {
                Debug.LogError("HQ failed to find place to spawn barracks.");
                return;
            }

            var teamId = teamAssignment.Data.teamId;
            var template = EntityTemplateFactory.CreateBarracksTemplate(spawnPosition.ToCoordinates(), BarracksState.UNDER_CONSTRUCTION, teamId);
            SpatialOS.Commands.CreateEntity(hqInfo, template)
                .OnFailure(_ =>
                {
                    Debug.LogWarning("HQ failed to spawn barracks due to timeout.");
                })
                .OnSuccess(_ =>
                {
                    PopulateBarracksDictionary();
                });
        }

        private bool SpawnLocationInvalid(Vector3 position)
        {
            return position.y < 0f;
        }

        private Vector3 FindSpawnLocation()
        {
            while (true)
            {
                for (var attemptNum = 0; attemptNum < SimulationSettings.HQBarracksSpawnPositionPickingRetries; attemptNum++)
                {
                    var spawnLocation = PickRandomLocationNearby();
                    if (NotCollidingWithAnything(spawnLocation))
                    {
                        return spawnLocation;
                    }
                }
                if (barracksSpawnRadius > SimulationSettings.MaxHQBarracksSpawnRadius)
                {
                    return Vector3.down;
                }
                barracksSpawnRadius += SimulationSettings.HQBarracksSpawnRadiusIncrease;
            }
        }

        private Vector3 PickRandomLocationNearby()
        {
            var randomOffset = new Vector3(Random.Range(-barracksSpawnRadius, barracksSpawnRadius), 0f, Random.Range(-barracksSpawnRadius, barracksSpawnRadius));
            return transform.position + randomOffset;
        }

        private bool NotCollidingWithAnything(Vector3 spawnLocation)
        {
            return NotCollidingWithHQ(spawnLocation) && NotCollidingWithOtherBarracks(spawnLocation);
        }

        private bool NotCollidingWithHQ(Vector3 spawnLocation)
        {
            return Vector3.Distance(transform.position, spawnLocation) > SimulationSettings.HQBarracksSpawningSeparation;
        }

        private bool NotCollidingWithOtherBarracks(Vector3 spawnLocation)
        {
            foreach (GameObject barracks in barracksSet)
            {
                if (Vector3.Distance(barracks.transform.position, spawnLocation) <= SimulationSettings.HQBarracksSpawningSeparation)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
