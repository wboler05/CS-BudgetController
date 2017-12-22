using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using UnityEngine;
using ChirpLogger;
using System.Collections.Generic;

namespace WECalc
{

    public class MyIEconomyExtensionBase : EconomyExtensionBase
    {
        private WaterElectricController weCalc;

        // Thread: Main
        public override void OnCreated(IEconomy economy)
        {
            //Debug.Log("IEconomy Created");
            Logger.output("IEconomy Created");

            weCalc = new WaterElectricController();
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
