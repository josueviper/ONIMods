﻿using System;
using UnityEngine;
using PeterHan.PLib.Detours;

namespace AquaticFarm
{
    public class AquaticFarm : KMonoBehaviour
    {
#pragma warning disable CS0649
        [MyCmpReq]
        private PlantablePlot plantablePlot;
#pragma warning restore CS0649

        private static readonly Action<SimComponent> SimUnregister = typeof(SimComponent).Detour<Action<SimComponent>>("SimUnregister");
        private static readonly Action<SimComponent> SimRegister = typeof(SimComponent).Detour<Action<SimComponent>>("SimRegister");

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int)GameHashes.OccupantChanged, OnOccupantChanged);
            OnOccupantChanged(plantablePlot.Occupant);
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            Unsubscribe((int)GameHashes.OccupantChanged, OnOccupantChanged);
        }

        private void OnOccupantChanged(object data)
        {
            var elementConsumers = GetComponents<PassiveElementConsumer>();
            foreach (PassiveElementConsumer elementConsumer in elementConsumers)
            {
                elementConsumer.EnableConsumption(false);
            }

            if (data != null)
            {
                var consumed_infos = ((GameObject)data)?.GetSMI<IrrigationMonitor.Instance>()?.def.consumedElements;
                if (consumed_infos != null)
                {
                    foreach (var consumeInfo in consumed_infos)
                    {
                        foreach (var elementConsumer in elementConsumers)
                        {
                            var element = ElementLoader.FindElementByHash(elementConsumer.elementToConsume);
                            if (element != null)
                            {
                                if (element.tag != consumeInfo.tag)
                                {
                                    //var traverse = Traverse.Create(elementConsumer);
                                    //traverse.Method("SimUnregister").GetValue();
                                    SimUnregister.Invoke(elementConsumer);
                                    elementConsumer.elementToConsume = ElementLoader.GetElementID(consumeInfo.tag);
                                    //traverse.Method("SimRegister").GetValue();
                                    SimRegister.Invoke(elementConsumer);
                                }
                                elementConsumer.consumptionRate = consumeInfo.massConsumptionRate * 1.5f;
                                elementConsumer.EnableConsumption(true);
                            }
                        }
                    }
                }
            }
        }
    }
}
