using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudgetManagerMod
{
    public class ServiceObject
    {
        public enum WaterBudgetOption { Water = 0, Sewage = 1 };
        public enum EducationBudgetOption { Elementary = 0, HighSchool = 1, University = 2 };

        public int m_capacity;
        public int m_consumption;
        public double m_budget;
        public double m_padding;
        public ItemClass.Service m_service;
        public bool m_night;
        public WaterBudgetOption m_waterBudgetOption;
        public EducationBudgetOption m_educationBudgetOption;

        public ServiceObject()
        {
            m_capacity = 0;
            m_consumption = 0;
            m_budget = 0;
            m_padding = 0;
            m_service = ItemClass.Service.None;
            m_night = false;
            m_waterBudgetOption = WaterBudgetOption.Water;
            m_educationBudgetOption = EducationBudgetOption.Elementary;
        }

        public ServiceObject(int capacity, int consumption, double budget, double padding, ItemClass.Service service, bool night, WaterBudgetOption waterOption, EducationBudgetOption educationOption)
        {
            m_capacity = capacity;
            m_consumption = consumption;
            m_budget = budget;
            m_padding = padding;
            m_service = service;
            m_night = night;
            m_waterBudgetOption = waterOption;
            m_educationBudgetOption = educationOption;
        }
    }
}
