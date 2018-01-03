using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;

namespace BudgetManagerMod
{

    public class MyIEconomyExtensionBase : EconomyExtensionBase
    {
        //private BudgetController weCalc;

        private ItemClass.Service m_testService = ItemClass.Service.Electricity;
        private int m_stepBudget = 10;
        private int m_minDelta = 1;
        private int m_capture_window = 5;

        private int m_beginBudget;
        private int m_beginDemand;
        private int m_prevDemand;
        private int m_count = 0;
        private System.DateTime m_beginDateTime;
        private System.DateTime m_prevDateTime;
        private System.DateTime m_finalDateTime;
        private enum TestState { Initial, Window, Running, Finished, Stop };
        private TestState m_state = TestState.Initial;

        // Thread: Main
        public override void OnCreated(IEconomy economy)
        {
            //Debug.Log("IEconomy Created");
            Logger.output("IEconomy Created");

            //weCalc = new BudgetController();
        }

        // Thread: Main
        public override void OnReleased()
        {

        }

        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            BudgetController.instance.updateLogic();
            //testStepResponse();

            return base.OnUpdateMoneyAmount(internalMoneyAmount);
        }

        private void testStepResponse()
        {
            switch(m_state)
            {
                case TestState.Initial:
                    initialTestStepResponse();
                    break;
                case TestState.Window:
                    windowTestStepResponse();
                    break;
                case TestState.Running:
                    runningTestStepResponse();
                    break;
                case TestState.Finished:
                    finishTestStepResponse();
                    break;
                case TestState.Stop:
                    break;
                default:
                    break;

            }

            m_count++;

        }

        private void initialTestStepResponse()
        {
            bool night = SimulationManager.instance.m_isNightTime;
            m_beginDateTime = SimulationManager.instance.m_currentGameTime;
            m_prevDateTime = m_beginDateTime;

            m_beginDemand = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            m_beginBudget = Singleton<EconomyManager>.instance.GetBudget(ItemClass.Service.Electricity, ItemClass.SubService.None, night);
            int stepBudget = m_beginBudget + m_stepBudget;
            Singleton<EconomyManager>.instance.SetBudget(ItemClass.Service.Electricity, ItemClass.SubService.None, stepBudget, night);
            m_state = TestState.Running;

            string s = "Testing beginning: ";
            s += string.Format("\tImpulse Budget Gain: {0}", m_stepBudget);
            s += string.Format("\nBegin budget: {0}", m_beginBudget);
            s += string.Format("\t Begin Demand: {0}", m_beginDemand);
            s += string.Format("\nBudget set to {0}", stepBudget);
            s += string.Format("\nDate/Time: {0}", m_beginDateTime);
            Logger.output(s);

            m_count = 0;
        }

        private void windowTestStepResponse()
        {
            int currentDemand = DistrictManager.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            System.DateTime currentDateTime = SimulationManager.instance.m_currentGameTime;
            System.TimeSpan timeDiff = currentDateTime.Subtract(m_prevDateTime);

            bool ending = false;

            if (m_count > m_capture_window)
            {
                ending = true;
            }

            string s = "Window Period: \n";
            s += string.Format("Count: {0}", m_count);
            s += string.Format("\t Current Demand: {0}", m_beginDemand);
            s += string.Format("\t Current Time: {0}", currentDateTime);
            s += string.Format("\t Time Diff: {0}", timeDiff);
            Logger.output(s);

            m_prevDemand = currentDemand;
            m_prevDateTime = currentDateTime;

            if (ending)
            {
                m_state = TestState.Running;
            }

        }

        private void runningTestStepResponse()
        {
            int currentDemand = DistrictManager.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            System.DateTime currentDateTime = SimulationManager.instance.m_currentGameTime;
            System.TimeSpan timeDiff = currentDateTime.Subtract(m_prevDateTime);
            int demandDiff = currentDemand - m_prevDemand;

            bool ending = false;

            if (demandDiff <= m_minDelta)
            {
                ending = true;
            }

            string s = "Delta Period: \n";
            s += string.Format("Count: {0}", m_count);
            s += string.Format("\t Current Demand: {0}", m_beginDemand);
            s += string.Format("\t Current Time: {0}", currentDateTime);
            s += string.Format("\t Time Diff: {0}", timeDiff);
            s += string.Format("\t Demand Diff: {0}", demandDiff);
            Logger.output(s);

            m_prevDemand = currentDemand;
            m_prevDateTime = currentDateTime;

            if (ending)
            {
                m_state = TestState.Finished;
                m_finalDateTime = currentDateTime;
            }

        }

        private void finishTestStepResponse()
        {
            //int currentDemand = DistrictManager.instance.m_districts.m_buffer[0].GetElectricityCapacity();
            //System.DateTime endDateTime = SimulationManager.instance.m_currentGameTime;
            System.TimeSpan timeDiff = m_finalDateTime.Subtract(m_beginDateTime);

            string s = "Stopping Step Response Test.";
            s += string.Format("\tTimeSpan: {0}", timeDiff);
            Logger.output(s);

            m_state = TestState.Stop;
        }

    }

    
}
