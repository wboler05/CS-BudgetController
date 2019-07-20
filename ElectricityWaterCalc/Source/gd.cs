using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudgetManagerMod
{
    public class GD
    {

        public double response(double error)
        {
            double output = - m_gain * error;
            return output;
        }

        public double m_gain;

        public GD(double gain)
        {
            m_gain = gain;
        }
    }
}
