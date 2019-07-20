using ICities;
using UnityEngine;

namespace BudgetManagerMod
{

    public class MyIUserMod: IUserMod
    {

        public string Name 
        {
            get { return "Budget Controller"; }
        }

        public string Description 
        {
            get { return "Automatically adjusts the budget for water, sewage, electricity, and education"; }
			
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

            /*
            UIHelperBase group_gains = helper.AddGroup("Gain Controls");
            ItemClass.Service service = ItemClass.Service.Water;
            float defaultGain = (float)BudgetController.instance.gain(service);
            group_gains.AddSlider("Water Gain", 0.01f*defaultGain, 10f*defaultGain, 0.01f, defaultGain, (value) =>
            {
                service = ItemClass.Service.Water;
                BudgetController.instance.setGain(service, value);

                string msg = string.Format("New Gain Set for {0}: ", service);
                msg += string.Format("{1}", value);
                Logger.output(msg);
            });

            service = ItemClass.Service.Electricity;
            defaultGain = (float)BudgetController.instance.gain(service);
            group_gains.AddSlider("Electricity Gain", 0.01f * defaultGain, 10f * defaultGain, 0.01f, defaultGain, (value) =>
            {
                BudgetController.instance.setGain(service, value);

                string msg = string.Format("New Gain Set for {0}: ", service);
                msg += string.Format("{1}", value);
                Logger.output(msg);
            });

            service = ItemClass.Service.Education;
            defaultGain = (float)BudgetController.instance.gain(service);
            group_gains.AddSlider("Education Gain", 0.01f * defaultGain, 10f * defaultGain, 0.01f, defaultGain, (value) =>
            {
                BudgetController.instance.setGain(service, value);

                string msg = string.Format("New Gain Set for {0}: ", service);
                msg += string.Format("{1}", value);
                Logger.output(msg);
            });
            */
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
