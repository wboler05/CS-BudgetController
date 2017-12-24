using ICities;
using UnityEngine;

namespace WECalc
{

    public class MyIUserMod: IUserMod
    {

        public string Name 
        {
            get { return "Water and Electricity Controller BETA v0.1"; }
        }

        public string Description 
        {
            get { return "Automatically adjusts the budget for water and electricity."; }
			
        }

        public void OnSettingsUI(UIHelperBase helper)
        {

            UIHelperBase group_enable = helper.AddGroup("Enable Settings");
            group_enable.AddCheckbox("Enable Mod", WEParameters.instance().m_enable_mod, (isChecked) => {
                    WEParameters.instance().m_enable_mod = isChecked;
            });
            group_enable.AddCheckbox("Enable Water Budget", WEParameters.instance().m_enable_water, (isChecked) =>
            {
                WEParameters.instance().m_enable_water = isChecked;
            });
            group_enable.AddCheckbox("Enable Electricity Budget", WEParameters.instance().m_enable_electricity, (isChecked) =>
            {
                WEParameters.instance().m_enable_electricity = isChecked;
            });

            UIHelperBase group_offsets = helper.AddGroup("Offset Controls");
            group_offsets.AddSlider("Water Offset", -1.0f, +1.0f, 0.01f, (float)WEParameters.instance().WaterPadding, (value) =>
            {
                WEParameters.instance().WaterPadding = value;
            });
            group_offsets.AddSlider("Electricity Offset", -1.0f, +1.0f, 0.01f, (float)WEParameters.instance().ElectricityPadding, (value) =>
            {
                WEParameters.instance().ElectricityPadding = value;
            });
        }

    }

    public class Logger
    {
        static public void output(string s)
        {
            string newString;
            newString = "WECalc: " + s;
            Debug.Log(newString);
        }
    }

    // Inherit interfaces and implement your mod logic here
    // You can use as many files and subfolders as you wish to organise your code, as long
    // as it remains located under the Source folder.
}
