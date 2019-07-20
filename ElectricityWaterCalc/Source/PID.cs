using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudgetManagerMod
{
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

        protected double m_k_p, m_k_d, m_k_i;
        public double m_gain;
        private Queue<double> m_queue;
        private int m_window;

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
}
