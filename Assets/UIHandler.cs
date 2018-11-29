﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    public class UIHandler : MonoBehaviour
    {
        public Text hintText;
        public GameObject settingsRoot;
        public RectTransform settingsParent;
        public GameObject settingPrefab;

        private ISimulationSettings simulationSettings;

    	void Awake ()
        {
            simulationSettings = Camera.main.GetComponent<ISimulationSettings>();

            var values = (SettingType[])Enum.GetValues(typeof(SettingType));

            foreach(var v in values)
            {
                var setting = Instantiate(settingPrefab, settingsParent).GetComponent<ISetting>();
                setting.type = v;
                setting.OnSettingChange = OnSettingChange;
                setting.DisplayHint = DisplayContext;
                setting.Initialize(GetSettingValue(v));
            }

    	}

        public void ToggleConfig()
        {
            settingsRoot.SetActive(!settingsRoot.activeSelf);
            simulationSettings.AllowInput = !settingsRoot.activeSelf;
        }

        private void OnSettingChange(SettingEvent evt)
        {
            switch(evt.type)
            {
                case SettingType.Count:             simulationSettings.SetBoidCount(Mathf.RoundToInt(evt.value));          break;
                case SettingType.Timescale:         simulationSettings.SimulationParameters.Timescale         = evt.value; break;
                case SettingType.Drag:              simulationSettings.SimulationParameters.Drag              = evt.value; break;
                case SettingType.BodyMass:          simulationSettings.SimulationParameters.BodyMass          = evt.value * evt.value; break;
                case SettingType.BodyRadius:        simulationSettings.SimulationParameters.BodyRadius        = evt.value; break;
                case SettingType.Bounciness:        simulationSettings.SimulationParameters.Bounciness        = evt.value; break;
                case SettingType.UnitScale:         simulationSettings.SimulationParameters.UnitScale         = evt.value; break;
                case SettingType.BoidMass:          simulationSettings.SimulationParameters.BoidMass          = evt.value; break;
                case SettingType.BoidMaxForce:      simulationSettings.SimulationParameters.BoidMaxForce      = evt.value; break;
                case SettingType.RepelWeight:       simulationSettings.SimulationParameters.RepelWeight       = evt.value; break;
                case SettingType.RepelDistance:     simulationSettings.SimulationParameters.RepelDistance     = evt.value; break;
                case SettingType.CohesionWeight:    simulationSettings.SimulationParameters.CohesionWeight    = evt.value; break;
                case SettingType.CohesionCoef:      simulationSettings.SimulationParameters.CohesionCoef      = evt.value; break;
                case SettingType.CohesionDistance:  simulationSettings.SimulationParameters.CohesionDistance  = evt.value; break;
                case SettingType.AlignWeight:       simulationSettings.SimulationParameters.AlignWeight       = evt.value; break;
                case SettingType.AlignCoef:         simulationSettings.SimulationParameters.AlignCoef         = evt.value; break;
                case SettingType.AlignDistance:     simulationSettings.SimulationParameters.AlignDistance     = evt.value; break;
            }
        }

        private float GetSettingValue(SettingType type)
        {
            switch(type)
            {
                case SettingType.Count:             return simulationSettings.SimulationParameters.Count; 
                case SettingType.Timescale:         return simulationSettings.SimulationParameters.Timescale;
                case SettingType.Drag:              return simulationSettings.SimulationParameters.Drag;
                case SettingType.BodyMass:          return Mathf.Sqrt(simulationSettings.SimulationParameters.BodyMass);
                case SettingType.BodyRadius:        return simulationSettings.SimulationParameters.BodyRadius;
                case SettingType.Bounciness:        return simulationSettings.SimulationParameters.Bounciness;
                case SettingType.UnitScale:         return simulationSettings.SimulationParameters.UnitScale;
                case SettingType.BoidMass:          return simulationSettings.SimulationParameters.BoidMass;
                case SettingType.BoidMaxForce:      return simulationSettings.SimulationParameters.BoidMaxForce;
                case SettingType.RepelWeight:       return simulationSettings.SimulationParameters.RepelWeight;
                case SettingType.RepelDistance:     return simulationSettings.SimulationParameters.RepelDistance;
                case SettingType.CohesionWeight:    return simulationSettings.SimulationParameters.CohesionWeight;
                case SettingType.CohesionCoef:      return simulationSettings.SimulationParameters.CohesionCoef;
                case SettingType.CohesionDistance:  return simulationSettings.SimulationParameters.CohesionDistance;
                case SettingType.AlignWeight:       return simulationSettings.SimulationParameters.AlignWeight;
                case SettingType.AlignCoef:         return simulationSettings.SimulationParameters.AlignCoef;
                case SettingType.AlignDistance:     return simulationSettings.SimulationParameters.AlignDistance;
            }

            return 1;
        }

        private void DisplayContext(SettingType type)
        {
            hintText.text = SettingData.displayInfo[type];
        }
    }
}