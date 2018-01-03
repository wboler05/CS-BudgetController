using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;

namespace BudgetManagerMod
{
    public class BudgetController
    {
        private PIDController m_water_budget_controller_day;
        private PIDController m_water_budget_controller_night;
        private PIDController m_electricity_budget_controller_day;
        private PIDController m_electricity_budget_controller_night;
        private PIDController m_education_budget_controller_day;
        private PIDController m_education_budget_controller_night;

        private double m_gainWater = 0.09;
        const double WATER_KP = 1.0;
        const double WATER_KI = 0.5;
        const double WATER_KD = 0.125;
        const int WATER_N = 10;

        /*
        private double m_gainWater = 1;
        const double WATER_KP = 1.0;
        const double WATER_KI = 0.5;
        const double WATER_KD = 1.25;
        const int WATER_N = 10;
        */

        private double m_gainElect = 0.09;
        const double ELECTRICITY_KP = 1.0;
        const double ELECTRICITY_KI = 0.02;
        const double ELECTRICITY_KD = 0.01;
        const int ELECTRICITY_N = 10;

        private double m_gainEducation = 0.09;
        const double EDUCATION_KP = 1.0;
        const double EDUCATION_KI = 0.02;
        const double EDUCATION_KD = 0.01;
        const int EDUCATION_N = 10;

        const float BUDGET_RATE_LIMIT = 7.0f;

        private static BudgetController m_instance=null;
        public static BudgetController instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BudgetController();
                }
                return m_instance;
            }
        }

        public void updateLogic()
        {
            System.DateTime currentTime = SimulationManager.instance.m_currentGameTime;

            // Check if DistrictManager exists
            if (Singleton<DistrictManager>.exists)
            {
                // Check if mod is enabled
                if (BMParameters.instance.m_enable_mod)
                {
                    // Check if water is enabled
                    if (BMParameters.instance.m_enable_water)
                    {
                        // Update the water budget
                        updateBudget(ItemClass.Service.Water, SimulationManager.instance.m_isNightTime);
                    }

                    // Check if electricity is enabled
                    if (BMParameters.instance.m_enable_electricity)
                    {
                        // Update the electricity budget
                        updateBudget(ItemClass.Service.Electricity, SimulationManager.instance.m_isNightTime);
                    }

                    if (BMParameters.instance.m_enable_education)
                    {
                        updateBudget(ItemClass.Service.Education, SimulationManager.instance.m_isNightTime);
                    }
                }
            } else
            {
                Logger.output("Failed to find DistrictManager.");
            }
        }

        // Update the budget based on service type and whether or not it's night time
        private void updateBudget(ItemClass.Service service, bool night)
        {
            ServiceObject s = getServiceObject(service, night);
            int newBudget = getNewBudget(s);
            Singleton<EconomyManager>.instance.SetBudget(service, ItemClass.SubService.None, newBudget, night);
        }

        // Create and fetch the current ServiceObject based on the service type and whether or not it's night
        private ServiceObject getServiceObject(ItemClass.Service service, bool night)
        {
            ServiceObject s = new ServiceObject();
            s.m_service = service;
            s.m_night = night;
            setServiceOptions(s);
            s.m_capacity = getCapacity(s);
            s.m_consumption = getConsumption(s);
            s.m_padding = getPadding(service);
            s.m_budget = Singleton<EconomyManager>.instance.GetBudget(service, ItemClass.SubService.None, night);
            return s;
        }

        private void setServiceOptions(ServiceObject s)
        {
            // TODO Make robust against s.m_service=null
            if (s.m_service == ItemClass.Service.Water)
            {
                int sewage = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageCapacity() - Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageAccumulation();
                int water = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterCapacity() - Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterConsumption();
                if (water < sewage)
                {
                    s.m_waterBudgetOption = ServiceObject.WaterBudgetOption.Water;
                }
                else
                {
                    s.m_waterBudgetOption = ServiceObject.WaterBudgetOption.Sewage;
                }
            }
            else if (s.m_service == ItemClass.Service.Education)
            {
                int elementary = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation1Capacity();
                int highschool = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation2Capacity();
                int university = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation3Capacity();

                if (elementary == 0)
                {
                    elementary = int.MaxValue;
                } else
                {
                    elementary -= Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation1Need();
                }

                if (highschool == 0)
                {
                    highschool = int.MaxValue;
                } else
                {
                    highschool -= Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation2Need();
                }

                if (university == 0)
                {
                    university = int.MaxValue;
                } else
                {
                    university -= Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation3Need();
                }

                int minValue = elementary;
                ServiceObject.EducationBudgetOption selectedOption = ServiceObject.EducationBudgetOption.Elementary;

                if (minValue > highschool)
                {
                    minValue = highschool;
                    selectedOption = ServiceObject.EducationBudgetOption.HighSchool;
                }
                if (minValue > university)
                {
                    minValue = university;
                    selectedOption = ServiceObject.EducationBudgetOption.University;
                }

                s.m_educationBudgetOption = selectedOption;
            }
        }

        // Get the capacity based on service, with a check for water capacity vs sewage capacity
        private int getCapacity(ServiceObject s)
        {
            if (s.m_service == ItemClass.Service.Water)
            {
                if (s.m_waterBudgetOption == ServiceObject.WaterBudgetOption.Water)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterCapacity();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageCapacity();
                }
            }
            else if (s.m_service == ItemClass.Service.Electricity)
            {
                return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            }
            else if (s.m_service == ItemClass.Service.Education)
            {
                if (s.m_educationBudgetOption == ServiceObject.EducationBudgetOption.Elementary)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation1Capacity();
                }
                else if (s.m_educationBudgetOption == ServiceObject.EducationBudgetOption.HighSchool)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation2Capacity();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation3Capacity();
                }
            }
            else
            {
                return 0;
            }
        }

        // Get the consumption based on service, with a check for water capacity vs sewage capacity
        private int getConsumption(ServiceObject s)
        {
            if (s.m_service == ItemClass.Service.Water)
            {
                if (s.m_waterBudgetOption == ServiceObject.WaterBudgetOption.Water)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterConsumption();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageAccumulation();
                }
            }
            else if (s.m_service == ItemClass.Service.Electricity)
            {
                return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityConsumption();
            }
            else if (s.m_service == ItemClass.Service.Education)
            {
                if (s.m_educationBudgetOption == ServiceObject.EducationBudgetOption.Elementary)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation1Need();
                }
                else if (s.m_educationBudgetOption == ServiceObject.EducationBudgetOption.HighSchool)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation2Need();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetEducation3Need();
                }
            }
            else
            {
                return 0;
            }
        }

        // Get the padding or offset based on service type. 
        // This realigns "0" for the control parameter. 
        private double getPadding(ItemClass.Service service)
        {
            if (service == ItemClass.Service.Water)
            {
                return BMParameters.instance.WaterOffset;
            }
            else if (service == ItemClass.Service.Electricity)
            {
                return BMParameters.instance.ElectricityOffset;
            }
            else if (service == ItemClass.Service.Education)
            {
                return BMParameters.instance.EducationOffset;
            }
            else
            {
                return 0.0;
            }
        }

        // Calculate the new budget based on the service object, and return it's integer value.
        private int getNewBudget(ServiceObject s)
        {
            float newBudget = (float)(selectController(s.m_service, s.m_night).response(getError(s)) + s.m_budget);
            newBudget = Mathf.Clamp(newBudget, 50, 150);

            string msg = string.Format("Service {0} updated budget from ", s.m_service);
            msg += string.Format("{0} to ", s.m_budget);
            msg += string.Format("{0}.", newBudget);
            msg += string.Format("\t{0} / ", s.m_consumption);
            msg += string.Format("\t{0}.", s.m_capacity);
            Logger.output(msg);

            return Mathf.RoundToInt(newBudget);
        }

        // Calculate the budget error based on consumption, capacity, and padding offset.
        private double getError(ServiceObject s)
        {
            double normalizer = ((double)(s.m_consumption)) / 100.0;
            if (normalizer == 0)
            {
                return 0;
            }
            double error = (((double)s.m_consumption * (1.0 + s.m_padding)) - ((double)(s.m_capacity))) / normalizer;

            if (error < 0.0)
            {
                error = -error * error;
            }
            else
            {
                error = error * error;
            }
            error /= 2.0;

            if (error < -BUDGET_RATE_LIMIT)
            {
                error = -BUDGET_RATE_LIMIT;
            }
            else if (error > BUDGET_RATE_LIMIT)
            {
                error = BUDGET_RATE_LIMIT;
            }
            return error;
        }

        private PIDController selectController(ItemClass.Service service)
        {
            return selectController(service, SimulationManager.instance.m_isNightTime);
        }

        // Select the PID controller based on service type and night/day settings
        private PIDController selectController(ItemClass.Service service, bool night)
        {
            if (service == ItemClass.Service.Water)
            {
                if (night)
                {
                    return m_water_budget_controller_night;
                }
                else
                {
                    return m_water_budget_controller_day;
                }
            }
            else if (service == ItemClass.Service.Electricity)
            {
                if (night)
                {
                    return m_electricity_budget_controller_night;
                }
                else
                {
                    return m_electricity_budget_controller_day;
                }
            }
            else if (service == ItemClass.Service.Education)
            {
                if (night)
                {
                    return m_education_budget_controller_night;
                }
                else
                {
                    return m_education_budget_controller_day;
                }
            }
            return null;
        }

        public void setGain(ItemClass.Service service, double value)
        {
            selectController(service).m_gain = value;
        }

        public double gain(ItemClass.Service service)
        {
            return selectController(service).m_gain;
        }

        // Constructor
        public BudgetController()
        {
            m_water_budget_controller_day = new PIDController(WATER_KP, WATER_KI, WATER_KD, WATER_N, m_gainWater);
            m_water_budget_controller_night = new PIDController(WATER_KP, WATER_KI, WATER_KD, WATER_N, m_gainWater);
            m_electricity_budget_controller_day = new PIDController(ELECTRICITY_KP, ELECTRICITY_KI, ELECTRICITY_KD, ELECTRICITY_N, m_gainElect);
            m_electricity_budget_controller_night = new PIDController(ELECTRICITY_KP, ELECTRICITY_KI, ELECTRICITY_KD, ELECTRICITY_N, m_gainElect);
            m_education_budget_controller_day = new PIDController(EDUCATION_KP, EDUCATION_KI, EDUCATION_KD, EDUCATION_N, m_gainEducation);
            m_education_budget_controller_night = new PIDController(EDUCATION_KP, EDUCATION_KI, EDUCATION_KD, EDUCATION_N, m_gainEducation);
        }

    }

}
