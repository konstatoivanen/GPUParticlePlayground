using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.UI
{
    //Implementation of ISetting interface
    public class UISetting : MonoBehaviour, ISetting
    {
        public Text header;
        public Text sliderValue;
        public Slider slider; 

        public SettingType type { get; set; }
        public Action<SettingEvent> OnSettingChange { get; set; }
        public Action<SettingType> DisplayHint { get; set; }

        //Unity event receiver method
        //Invokes the OnSettingChange delegate to inform the listener of a setting change.
        public void OnValueChanged()
        {
            OnSettingChange?.Invoke(new SettingEvent(type, slider.value));
            sliderValue.text = String.Format("{0:0.00}", slider.value);
        }

        //Unity event receiver method
        //Updates the value text to display the current slider value
        public void OnSliderMove()
        {
            sliderValue.text = String.Format("{0:0.00}", slider.value);
        }

        //Unity event receiver method
        //Invokes the Displayhint delegate to inform the listener of a hover over event
        public void OnHover()
        {
            DisplayHint?.Invoke(type);
        }

        //Interface initialization method for passing down initial values
        public void Initialize(float value)
        {
            var minMax = SettingData.valueRanges[type];
            slider.minValue = minMax.x;
            slider.maxValue = minMax.y;

            slider.value = value;

            sliderValue.text = String.Format("{0:0.00}", value);

            header.text = SettingData.displayValues[type];
        }
    }
}
