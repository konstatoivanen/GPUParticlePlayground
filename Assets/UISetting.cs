using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    public class UISetting : MonoBehaviour, ISetting
    {
        public Text header;
        public Text sliderValue;
        public Slider slider; 

        public SettingType type { get; set; }
        public Action<SettingEvent> OnSettingChange { get; set; }
        public Action<SettingType> DisplayHint { get; set; }

        public void OnValueChanged()
        {
            OnSettingChange?.Invoke(new SettingEvent(type, slider.value));
            sliderValue.text = slider.value.ToString("##.#");
        }

        public void OnHover()
        {
            DisplayHint?.Invoke(type);
        }

        public void Initialize(float value)
        {
            var minMax = SettingData.valueRanges[type];
            slider.minValue = minMax.x;
            slider.maxValue = minMax.y;

            slider.value = value;

            sliderValue.text = value.ToString("##.#");

            header.text = SettingData.displayValues[type];
        }
    }
}
