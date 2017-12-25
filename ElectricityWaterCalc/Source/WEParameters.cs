using System;
using System.Collections.Generic;
using System.Text;

namespace BudgetManagerMod
{
    public class BMParameters
    {
        private static BMParameters m_instance = null;

        public bool m_enable_mod=true;
        public bool m_enable_water=true;
        public bool m_enable_electricity=true;
        public bool m_enable_education = true;
        private double m_offset_water = 0.15;
        private double m_offset_electricity = 0.15;
        private double m_offset_education = 0.15;

        public double WaterOffset
        {
            get { return m_offset_water;  }
            set
            {
                if (value >= -100.0 && value <= 100.0)
                {
                    m_offset_water = value;
                } else if (value < -100.0)
                {
                    m_offset_water = -100.0;
                } else
                {
                    m_offset_water = 100.0;
                }
            }
        }

        public double ElectricityOffset
        {
            get { return m_offset_electricity; }
            set
            {
                if (value >= -100.0 && value <= 100.0)
                {
                    m_offset_electricity = value;
                }
                else if (value < -100.0)
                {
                    m_offset_electricity = -100.0;
                }
                else
                {
                    m_offset_electricity = 100.0;
                }
            }
        }

        public double EducationOffset
        {
            get { return m_offset_education;  }
            set
            {
                if (value < -100.0)
                {
                    m_offset_education = -100.0;
                }
                else if (value > 100.0)
                {
                    m_offset_education = 100.0;
                }
                else
                {
                    m_offset_education = value;
                }
            }
        }

        public static BMParameters instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BMParameters();
                }
                return m_instance;
            }
        }
    }
}
