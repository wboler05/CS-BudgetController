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
        private BudgetController weCalc;

        // Thread: Main
        public override void OnCreated(IEconomy economy)
        {
            //Debug.Log("IEconomy Created");
            Logger.output("IEconomy Created");

            weCalc = new BudgetController();
        }

        // Thread: Main
        public override void OnReleased()
        {

        }

        public override long OnUpdateMoneyAmount(long internalMoneyAmount)
        {
            weCalc.updateLogic();

            return base.OnUpdateMoneyAmount(internalMoneyAmount);
        }


    }

    
}
