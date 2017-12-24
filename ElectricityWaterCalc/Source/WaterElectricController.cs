﻿using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;

namespace WECalc
{
    public class WaterElectricController
    {
        private PIDController m_water_budget_controller_day;
        private PIDController m_water_budget_controller_night;
        private PIDController m_electricity_budget_controller_day;
        private PIDController m_electricity_budget_controller_night;

        private double m_gainWater = 0.09;
        const double WATER_KP = 1.0;
        const double WATER_KI = 0.5;
        const double WATER_KD = 0.125;
        //const double WATER_PADDING = 0.15;
        const int WATER_N = 10;

        private double m_gainElect = 0.09;
        const double ELECTRICITY_KP = 1.0;
        const double ELECTRICITY_KI = 0.02;
        const double ELECTRICITY_KD = 0.01;
        //const double ELECTRICITY_PADDING = 0.15;
        const int ELECTRICITY_N = 10;

        const float BUDGET_RATE_LIMIT = 7.0f;

        public void updateLogic()
        {
            System.DateTime currentTime = SimulationManager.instance.m_currentGameTime;

            // Check if DistrictManager exists
            if (Singleton<DistrictManager>.exists)
            {
                // Check if mod is enabled
                if (WEParameters.instance().m_enable_mod)
                {
                    // Check if water is enabled
                    if (WEParameters.instance().m_enable_water)
                    {
                        // Update the water budget
                        updateBudget(ItemClass.Service.Water, SimulationManager.instance.m_isNightTime);
                    }

                    // Check if electricity is enabled
                    if (WEParameters.instance().m_enable_electricity)
                    {
                        // Update the electricity budget
                        updateBudget(ItemClass.Service.Electricity, SimulationManager.instance.m_isNightTime);
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
            ServiceObject s;
            bool checkWaterLevel = isWaterCapacityLower();
            s.m_capacity = getCapacity(service, checkWaterLevel);
            s.m_consumption = getConsumption(service, checkWaterLevel);
            s.m_padding = getPadding(service);
            s.m_budget = Singleton<EconomyManager>.instance.GetBudget(service, ItemClass.SubService.None, night);
            s.m_service = service;
            s.m_night = night;
            return s;
        }

        // Check if water capacity is lower than sewer and return true if so.
        private bool isWaterCapacityLower()
        {
            int sewage = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageCapacity();
            int water = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterCapacity();
            if (water < sewage)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        // Get the capacity based on service, with a check for water capacity vs sewage capacity
        private int getCapacity(ItemClass.Service service, bool waterLevelLower)
        {
            if (service == ItemClass.Service.Water)
            {
                if (waterLevelLower)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterCapacity();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageCapacity();
                }
            }
            else if (service == ItemClass.Service.Electricity)
            {
                return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            }
            else
            {
                return 0;
            }
        }

        // Get the consumption based on service, with a check for water capacity vs sewage capacity
        private int getConsumption(ItemClass.Service service, bool waterLevelLower)
        {
            if (service == ItemClass.Service.Water)
            {
                if (waterLevelLower)
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetWaterConsumption();
                }
                else
                {
                    return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetSewageAccumulation();
                }
            }
            else if (service == ItemClass.Service.Electricity)
            {
                return Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityConsumption();
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
                return WEParameters.instance().WaterPadding;
            }
            else if (service == ItemClass.Service.Electricity)
            {
                return WEParameters.instance().ElectricityPadding;
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

            string msg = string.Format("Service {0} updated budget from ", s.m_night);
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
            return null;
        }

        // Constructor
        public WaterElectricController()
        {
            m_water_budget_controller_day = new PIDController(WATER_KP, WATER_KI, WATER_KD, WATER_N, m_gainWater);
            m_water_budget_controller_night = new PIDController(WATER_KP, WATER_KI, WATER_KD, WATER_N, m_gainWater);
            m_electricity_budget_controller_day = new PIDController(ELECTRICITY_KP, ELECTRICITY_KI, ELECTRICITY_KD, ELECTRICITY_N, m_gainElect);
            m_electricity_budget_controller_night = new PIDController(ELECTRICITY_KP, ELECTRICITY_KI, ELECTRICITY_KD, ELECTRICITY_N, m_gainElect);
        }


    }

    public class PIDController
    {

        public double response(double error)
        {

            double output = 0;
            double prop = m_gain * m_k_p * error;

            double integ = 0, prevError = 0;
            int i = 0;
            if (m_queue != null)
            {
                if (m_queue.Count > 0)
                {
                    foreach (double e in m_queue)
                    {
                        integ += e;
                        if (i++ == 0)
                        {
                            prevError = e;
                        }
                    }
                    integ /= m_queue.Count;
                }
            }
            integ *= m_gain * m_k_i;

            double deriv = 0;
            if (m_queue != null)
            {
                if (m_queue.Count > 0)
                {
                    deriv = m_gain * m_k_d * (error - prevError);
                }
            }

            output = prop + integ + deriv;

            m_queue.Enqueue(error);
            if (m_queue.Count > m_window)
            {
                m_queue.Dequeue();
            }

            return output;
        }

        private double m_k_p, m_k_d, m_k_i;
        private Queue<double> m_queue;
        private int m_window;
        public  double m_gain;

        public PIDController(double p, double i, double d, int w, double gain)
        {
            m_k_p = p;
            m_k_i = i;
            m_k_d = d;
            m_window = w;
            m_gain = gain;

            m_queue = new Queue<double>();
            for (int idx = 0; idx < m_window; idx++)
            {
                m_queue.Enqueue(0);
            }
        }
    }

    public struct ServiceObject
    {
        public int m_capacity;
        public int m_consumption;
        public double m_budget;
        public double m_padding;
        public ItemClass.Service m_service;
        public bool m_night;

        public ServiceObject(int capacity, int consumption, double budget, double padding, ItemClass.Service service, bool night)
        {
            m_capacity = capacity;
            m_consumption = consumption;
            m_budget = budget;
            m_padding = padding;
            m_service = service;
            m_night = night;
        }
    }
}
