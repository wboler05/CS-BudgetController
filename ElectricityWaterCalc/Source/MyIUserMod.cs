using ICities;
using UnityEngine;

namespace WECalc
{

    public class MyIUserMod: IUserMod
    {

        public string Name 
        {
            get { return "Water and Electricity Controller BETA v0.1"; }
        }

        public string Description 
        {
            get { return "Automatically adjusts the budget for water and electricity."; }
			
        }
    }

    public class Logger
    {
        static public void output(string s)
        {
            string newString;
            newString = "WECalc: " + s;
            Debug.Log(newString);
        }
    }

    // Inherit interfaces and implement your mod logic here
    // You can use as many files and subfolders as you wish to organise your code, as long
    // as it remains located under the Source folder.
}
