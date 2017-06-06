using Assets.Gamelogic.FSM;
using Improbable.Building;
using System.Collections.Generic;
using Assets.Gamelogic.Fire;
using Improbable.Life;
using UnityEngine;

namespace Assets.Gamelogic.Building.Barracks
{
    public class BarracksStateMachine : FiniteStateMachine<BarracksState>
    {
        private readonly BarracksInfo.Writer barracksInfo;
        public BarracksInfoData Data; 

        private readonly StockpileDepository.Writer stockpile;
        private readonly Health.Writer health;
        private readonly FlammableBehaviour flammableBehaviour;

        public BarracksStateMachine(BarracksInfo.Writer inBarracksInfo,
                                    StockpileDepository.Writer inStockpile,
                                    Health.Writer inHealth, 
                                    FlammableBehaviour inFlammableBehaviour, 
                                    NPCSpawnerBehaviour npcSpawnerBehaviour)
        {
            barracksInfo = inBarracksInfo;
            stockpile = inStockpile;
            health = inHealth;
            flammableBehaviour = inFlammableBehaviour;

            var stateList = new Dictionary<BarracksState, IFsmState>
            {
                { BarracksState.UNDER_CONSTRUCTION, new BarracksUnderConstructionState(this, inHealth, npcSpawnerBehaviour) },
                { BarracksState.CONSTRUCTION_FINISHED, new BarracksConstructionFinishedState(this, inHealth, npcSpawnerBehaviour) }
            };
            SetStates(stateList);

            var allowedTransitions = new Dictionary<BarracksState, IList<BarracksState>>()
            {
                { BarracksState.UNDER_CONSTRUCTION, new List<BarracksState> { BarracksState.CONSTRUCTION_FINISHED } },
                { BarracksState.CONSTRUCTION_FINISHED, new List<BarracksState> { BarracksState.UNDER_CONSTRUCTION } }
            };
            SetTransitions(allowedTransitions);
        }

        public void TriggerTransition(BarracksState newState)
        {
            if (barracksInfo == null)
            {
                Debug.LogError("Trying to change state without authority.");
                return;
            }

            if (IsValidTransition(newState))
            {
                Data.barracksState = newState;

                var update = new BarracksInfo.Update();
                update.SetBarracksState(Data.barracksState);
                barracksInfo.Send(update);

                TransitionTo(newState);
            }
            else
            {
                Debug.LogErrorFormat("Barracks: Invalid transition from {0} to {1} detected.", Data.barracksState, newState);
            }
        }

        protected override void OnEnableImpl()
        {
            Data = barracksInfo.Data.DeepCopy();
        }

        public bool EvaluateCanAcceptResources()
        {
            return CurrentState == BarracksState.UNDER_CONSTRUCTION && health.Data.canBeChanged && health.Data.currentHealth < health.Data.maxHealth;
        }

        public void SetCanAcceptResources(bool canAcceptResources)
        {
            if (stockpile == null)
            {
                Debug.LogError("stockpile is null in BarracksStateMachine.");
                return;
            }
            if (stockpile.Data.canAcceptResources != canAcceptResources)
            {
                stockpile.Send(new StockpileDepository.Update().SetCanAcceptResources(canAcceptResources));
            }
        }

        public void EvaluateAndSetFlammability(Health.Update update)
        {
            if (barracksInfo == null)
            {
                Debug.LogError("barracksInfo is null in BarracksStateMachine.");
                return;
            }

            if (flammableBehaviour == null)
            {
                Debug.LogError("flammableBehaviour is null in BarracksStateMachine.");
                return;
            }

            if (update.currentHealth.Value <= 0)
            {
                flammableBehaviour.SelfExtinguish(health, false);
            }
            else
            {
                var canBeIgnited = update.currentHealth.Value > 0;
                flammableBehaviour.SelfSetCanBeIgnited(barracksInfo, canBeIgnited);
            }
        }
    }
}
