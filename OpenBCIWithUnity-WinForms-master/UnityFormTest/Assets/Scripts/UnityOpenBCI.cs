//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//Using UnityWinform in https://libraries.io/github/Meragon/Unity-WinForms:
//Attach Application script to GameObject;
//Add Arial font to resources;
//Add other fonts and icons;
//Use keyword 'new' to create new Controls(don't need to use Application.Run);

using UnityEngine;

//using UnityWinForms.Examples;

namespace OpenBCI_GUI
{
    //static class Program
   // {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       // [STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

        public class UnityOpenBCI : MonoBehaviour
        {
            //public static Material s_chartGradient;
            //public Material ChartGradient;

            private void Start()
            {
                //s_chartGradient = ChartGradient;

                var form = new Form1();

                form.Show();
            }
        }//UnityOpenBCI 
}//OpenBCI_GUI

