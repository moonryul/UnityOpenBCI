using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OpenBCI_GUI
{

    // Put this script on a Camera
    public class DrawLines : MonoBehaviour
    {

        // Fill/drag these in from the editor

        // Choose the Unlit/Color shader in the Material Settings
        // You can change that color, to change the color of the connecting lines
        public Material lineMat;

        public Form1.Chart chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8, chart9;



        // Connect all of the `points` to the `mainPoint`
        void DrawConnectingLines( int chartNum, Form1.Chart chart)
        {
           
                // Loop through each point to connect to the mainPoint
                for (int i =0; i < chart.yValues.Count-1; i++)   {

                 
                    GL.Begin(GL.LINES);
                    lineMat.SetPass(0);
                    GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));

                    GL.Vertex3( (float) i, (float)chartNum + (float)chart.yValues[i], 0.0f);
                    GL.Vertex3((float)(i+1), (float)chartNum + (float)chart.yValues[i+1], 0.0f);

                    GL.End();
                }

        }//DrawConnectingLines



        // To show the lines in the game window whne it is running
        void OnPostRender()  {

        DrawConnectingLines(1, chart1);
        DrawConnectingLines(2, chart2);
        DrawConnectingLines(3, chart3);
        DrawConnectingLines(4, chart4);
        DrawConnectingLines(5, chart5);
        DrawConnectingLines(6, chart6);
        DrawConnectingLines(7, chart7);
        DrawConnectingLines(8, chart8);
        DrawConnectingLines(9, chart9);
    }

        // To show the lines in the editor
        void OnDrawGizmos()  {


        DrawConnectingLines(1, chart1);
        DrawConnectingLines(2, chart2);
        DrawConnectingLines(3, chart3);
        DrawConnectingLines(4, chart4);
        DrawConnectingLines(5, chart5);
        DrawConnectingLines(6, chart6);
        DrawConnectingLines(7, chart7);
        DrawConnectingLines(8, chart8);
        DrawConnectingLines(9, chart9);

        }


}// DrawLines 
} // OpenBCI_GUI
