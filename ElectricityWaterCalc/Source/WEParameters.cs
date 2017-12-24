using System;
using System.Collections.Generic;
using System.Text;

namespace WECalc
{
    public class WEParameters
    {
        private static WEParameters m_instance = null;

        public bool m_enable_mod=true;
        public bool m_enable_water=true;
        public bool m_enable_electricity=true;
        private double m_padding_water = 0.15;
        private double m_padding_electricity = 0.15;

        public double WaterPadding
        {
            get { return m_padding_water;  }
            set
            {
                if (value >= -100.0 && value <= 100.0)
                {
                    m_padding_water = value;
                } else if (value < -100.0)
                {
                    m_padding_water = -100.0;
                } else
                {
                    m_padding_water = 100.0;
                }
            }
        }

        public double ElectricityPadding
        {
            get { return m_padding_electricity; }
            set
            {
                if (value >= -100.0 && value <= 100.0)
                {
                    m_padding_electricity = value;
                }
                else if (value < -100.0)
                {
                    m_padding_electricity = -100.0;
                }
                else
                {
                    m_padding_electricity = 100.0;
                }
            }
        }

        public static WEParameters instance()
        {
            if (m_instance == null) {
                m_instance = new WEParameters();
            }
            return m_instance;
        }
    }
}
