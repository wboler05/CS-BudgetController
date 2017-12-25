using ICities;
using UnityEngine;

namespace BudgetManagerMod
{

    public class MyIUserMod: IUserMod
    {

        public string Name 
        {
            get { return "Water and Electricity Controller BETA v0.3"; }
        }

        public string Description 
        {
            get { return "Automatically adjusts the budget for water and electricity."; }
			
        }

        public void OnSettingsUI(UIHelperBase helper)
        {

            UIHelperBase group_enable = helper.AddGroup("Enable Settings");
            group_enable.AddCheckbox("Enable Mod", BMParameters.instance.m_enable_mod, (isChecked) => {
                    BMParameters.instance.m_enable_mod = isChecked;
            });
            group_enable.AddCheckbox("Enable Water Budget", BMParameters.instance.m_enable_water, (isChecked) =>
            {
                BMParameters.instance.m_enable_water = isChecked;
            });
            group_enable.AddCheckbox("Enable Electricity Budget", BMParameters.instance.m_enable_electricity, (isChecked) =>
            {
                BMParameters.instance.m_enable_electricity = isChecked;
            });
            group_enable.AddCheckbox("Enable Education Budget", BMParameters.instance.m_enable_electricity, (isChecked) =>
            {
                BMParameters.instance.m_enable_electricity = isChecked;
            });

            UIHelperBase group_offsets = helper.AddGroup("Offset Controls");
            group_offsets.AddSlider("Water Offset", -1.0f, +1.0f, 0.01f, (float)BMParameters.instance.WaterOffset, (value) =>
            {
                BMParameters.instance.WaterOffset = value;
            });
            group_offsets.AddSlider("Electricity Offset", -1.0f, +1.0f, 0.01f, (float)BMParameters.instance.ElectricityOffset, (value) =>
            {
                BMParameters.instance.ElectricityOffset = value;
            });
            group_offsets.AddSlider("Education Offset", -1.0f, +1.0f, 0.01f, (float)BMParameters.instance.EducationOffset, (value) =>
            {
                BMParameters.instance.EducationOffset = value;
            });
        }

    }

    public class Logger
    {
        static public void output(string s)
        {
            string newString;
            newString = "BudgetManagerMod: " + s;
            Debug.Log(newString);
        }
    }

    // Inherit interfaces and implement your mod logic here
    // You can use as many files and subfolders as you wish to organise your code, as long
    // as it remains located under the Source folder.
}
