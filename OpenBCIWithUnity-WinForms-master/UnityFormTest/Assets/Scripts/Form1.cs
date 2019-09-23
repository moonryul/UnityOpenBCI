using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

//https://forum.unity.com/threads/monitor-components-by-bipbipspil-released.278524/
//https://stackoverflow.com/questions/27491406/understanding-c-sharp-field-initialization-requirements/27492048

namespace OpenBCI_GUI
{
    public partial class Form1 : Form
    {
        //https://stackoverflow.com/questions/38099520/i-dont-understand-why-we-need-the-new-keyword
        //The variable initializers are executed in the textual order in which they appear in the class declaration.
        //A variable initializer for an instance field cannot reference the instance being created.
        //Thus, it is a compile-time error to reference this in a variable initializer, 
        //as it is a compile-time error for a variable initializer to reference any instance member through a simple-name
        //Since all field initializers are translated into instructions in the constructor(s) 
        //which go before any other statements in the constructor,
        //there is no technical reason why this should not be possible. So it is a design choice.
        public struct AxisY
        {
            public int Maximum;
            public int Minimum;
        }

        public struct Chart
        {
            public List<double> yValues;
            public AxisY axisY;

        };



        public Chart chart1, chart2, chart3, chart4, chart5, chart6, chart7, chart8, chart9;




        //https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?redirectedfrom=MSDN&view=netframework-4.8#methods

       

        StreamWriter sw;
        bool istniejeSW = false;

        public Form1()
        {
            chart1 = new Chart();
            chart1.yValues = new List<double>();
         

            chart2 = new Chart();
            chart2.yValues = new List<double>();

            chart3 = new Chart();
            chart3.yValues = new List<double>();

            chart4 = new Chart();
            chart4.yValues = new List<double>();

            chart5 = new Chart();
            chart5.yValues = new List<double>();

            chart6 = new Chart();
            chart6.yValues = new List<double>();

            chart7 = new Chart();
            chart7.yValues = new List<double>();

            chart8 = new Chart();
            chart8.yValues = new List<double>();

            chart9 = new Chart();
            chart9.yValues = new List<double>();





            InitializeComponent();

            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }

            //The Cyton board has an on-board RFDuino radio module acting as a "Device". 
            //The Cyton system includes a USB dongle for the PC, which acts as the RFDuino "Host".

            // USB has the rate of data transfer 12 Mbps for disk-drives and 1.5Mbps for devices
            // that need less bandwidth.
            // Serial ports are also known as communication (COM) ports = 11200bps.
            //https://github.com/OpenBCI/Docs/blob/master/Hardware/03-Cyton_Data_Format.md
            //Each FTDI adapter contains a USB microcontroller which talks a proprietary protocol 
            //via USB and transforms that into the regular UART signals and vice versa.

            //The RFDuino USB dongle (the RFDuino "Host") is connected to an FTDI (USB<->Serial)
            // converter configured to appear to the computer as if it is a standard serial port running 
            //at a rate of 115200 baud using the typical 8-N-1.

            comboBox1.SelectedItem = "COM16";

            //textBox2.Text = Application.StartupPath;
            textBox3.Text = "test.txt";

            radioButton4.Checked = true;
            radioButton5.Checked = true;
            radioButton8.Checked = true;

            button1.Enabled = true;

            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;

            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
        }
        
        struct DaneSerialPort // bytes array at arrived at the serial port
        {
            public byte[] zmienna; // buffer
        };

        Queue<DaneSerialPort> driver = new Queue<DaneSerialPort>(); // driver = queue of the arrived byte arrays.

        // Event Handler
        void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            DaneSerialPort odebrane_dane; //the arrived byte array.

            byte[] buffer = new byte[serialPort1.BytesToRead];

            serialPort1.Read(buffer, 0, buffer.Length);

            odebrane_dane.zmienna = buffer;

            driver.Enqueue(odebrane_dane); // add the current buffer content to the buffer queue
        }

        // Event Handler
        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text;
            serialPort1.BaudRate = 115200;
            try
            {
                serialPort1.Open();
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Nie można otworzyć portu" + comboBox1.Text.ToString());
            }
            serialPort1.DataReceived += serialPort1_DataReceived;
            // delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e) DataReceived ; => this creates a new delegate (pointer)
        }

        // Event Handler
        private void button2_Click(object sender, EventArgs e)
        {
            turnOFF_SW();
            turnOFF_transmision();
            serialPort1.Close();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
        }

        //Once the OpenBCI has initialized itself and sent the $$$, it waits for commands.
        //    In other words, it sends no data until it is told to start sending data.To begin data transfer, transmit a single ASCII b.
        //    Once the b is received, continuous transfer of data in binary format will ensue. To turn off the fire hose, send an s.
        private void button3_Click(object sender, EventArgs e)
        {
            char[] buff = new char[1];
            buff[0] = 'b';  //  Once the b is received, continuous transfer of data in binary format will ensue
            try
            {
                serialPort1.Write(buff, 0, 1);// Write(char[] buffer, int offset, int count);
            }
            catch
            {
                MessageBox.Show("Nie można uruchomic.");
            }
            button3.Enabled = false;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            turnOFF_SW();
            turnOFF_transmision();////To turn off the fire hose, send an s.
            button4.Enabled = false;
            button3.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.CreateDirectory(textBox2.Text);
                sw = new StreamWriter(Path.Combine(textBox2.Text, textBox3.Text));
                istniejeSW = true;
            }
            catch
            {
                MessageBox.Show("Nie można utworzyć katalogu bezpośrednio na głównej partycji.");
            }
            button5.Enabled = false;
            button6.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            turnOFF_SW();
            button5.Enabled = true;
            button6.Enabled = false;
        }

        // Event Handler
        private void timer1_Tick(object sender, EventArgs e)
        {
            double[] daneRys;

            // read all the data in the queue:

            while (driver.Count > 0) // driver = the queue of buffer contents
            {
                // get the current data buffer at the front of the queue
                DaneSerialPort data = driver.Dequeue(); // driver.Count decreases
                for (int g = 0; g < data.zmienna.Length; g++)
                {   // interpret the current byte data.zmienna[g]

                    daneRys = Convert.interpretBinaryStream( data.zmienna[g] ); // public byte[] zmienna; // buffer

                    if (daneRys != null) // the 8 channel data is completely read
                    {
                        daneRys = ValueOrZero(daneRys); // daneRys = array of 8 places => ValueOrZero sets the non-activated channel to 0
                                                        //  which indicates which channel is active.
                        writeToFile(daneRys);
                        createPlot(daneRys); 
                    }

                   // else: get the next byte
                } // get the next buffer at the queue
            }

        }

        private void writeToFile(double[] daneRys)
        {
            //ScaleFactor (Volts/count) = 4.5 Volts / gain / (2^23 - 1); gain =24; 

            double mnoz = (4.5 / 24 / (Math.Pow(2, 23) - 1)) * (Math.Pow(10, 6));

            for (int i = 0; i < 8; i++)
            {
                daneRys[i + 1] = daneRys[i + 1] * mnoz;
            }
            if (istniejeSW)
            {
                sw.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}", daneRys[0], daneRys[1], daneRys[2], daneRys[3], daneRys[4], daneRys[5], daneRys[6], daneRys[7], daneRys[8], daneRys[9], daneRys[10], daneRys[11]);
            }
        }

        private void createPlot(double[] dane)
        {
            dane = filtering(dane);
            //DataPoint Add(params double[] y);
            // public int AddXY(double xValue, double yValue);

            // Draw Lines:
            //https://gamedev.stackexchange.com/questions/96964/how-to-correctly-draw-a-line-in-unity
            //https://docs.unity3d.com/ScriptReference/Array.html
            //https://loadofprogrammer.tistory.com/145
            //https://www.educba.com/c-sharp-array-vs-list/

            chart1.yValues.Add(dane[0]); // X= 1, Y= dane[0] = the sample number => chart1; chart1.Series[i] is the ith graph; dane[0] is the first element of params array y
            chart2.yValues.Add(dane[1]);  // X=2; Y= dane[1] = the 3 byte data for the channel 1 => chart2. 
            chart3.yValues.Add(dane[2]);  // X=3, Y= dane[2] = the 3 byte data for the channel 2 => chart3
                chart4.yValues.Add(dane[3]);//...
                chart5.yValues.Add(dane[4]);
                chart6.yValues.Add(dane[5]);
                chart7.yValues.Add(dane[6]);
                chart8.yValues.Add(dane[7]);
                chart9.yValues.Add(dane[8]);   // X=9, Y = dane[9] = the 3 byte data for the channel 9 => chart10

            //while (chart1.Count > 1250)  { // chart1 has more than 1250 points ; Points are added as a group of 8 points each time when this method
            //                                              // is executed => remove the data until it is less than 1250

            //    chart1.RemoveAt(chart1.Count - 1);
            //    chart2.RemoveAt(chart2.Count - 1);
            //    chart3.RemoveAt(chart3.Count - 1);
            //    chart4.RemoveAt(chart4.Count - 1);
            //    chart5.RemoveAt(chart5.Count - 1);
            //    chart6.RemoveAt(chart6.Count - 1);
            //    chart7.RemoveAt(chart7.Count - 1);
            //    chart8.RemoveAt(chart8.Count - 1);
            //    chart9.RemoveAt(chart9.Count - 1);


            //}// while (chart1.Count > 1250)  // chart1 has more than 1250 data 


            chart1.yValues.RemoveRange( 1250, chart1.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart2.yValues.RemoveRange(1250, chart2.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart3.yValues.RemoveRange(1250, chart3.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart4.yValues.RemoveRange(1250, chart4.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart5.yValues.RemoveRange(1250, chart5.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart6.yValues.RemoveRange(1250, chart6.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart7.yValues.RemoveRange(1250, chart7.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart8.yValues.RemoveRange(1250, chart8.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            chart9.yValues.RemoveRange(1250, chart9.yValues.Count - 1250); // ( count1.Count-1 ) - (1250-1)
            


        }//private void drawPlot(double[] dane)

        private double[] ValueOrZero(double[] dane)
        {
            if (!checkBox1.Checked) // channel 1 is not set for reading
            {
                dane[1] = 0;  // channel 1 is not set for reading => set the read data to zero
            }
            if (!checkBox2.Checked) // channel 2 is not set for reading
            {
                dane[2] = 0; // channel 2 is not set for reading => set the read data to zero
            }
            if (!checkBox3.Checked)
            {
                dane[3] = 0;// channel 3 is not set for reading => set the read data to zero
            }
            if (!checkBox4.Checked)
            {
                dane[4] = 0; // channel 4 is not set for reading => set the read data to zero
            }
            if (!checkBox5.Checked)
            {
                dane[5] = 0;
            }
            if (!checkBox6.Checked)
            {
                dane[6] = 0;
            }
            if (!checkBox7.Checked)
            {
                dane[7] = 0;
            }
            if (!checkBox8.Checked)
            {
                dane[8] = 0;
            }
            return dane;
        }

        //https://neurobb.com/t/openbci-why-are-1-50hz-bandpass-and-60hz-notch-filters-both-applied-by-default/23/2

        private double[] filtering(double[] dane)
        {
            int standard = 0;
            int notch = 0;

            if (radioButton5.Checked)
            {
                notch = 0;
            }
            else if (radioButton6.Checked)
            {
                notch = 1;
            }
            else if (radioButton7.Checked)
            {
                notch = 2;
            }
            if (radioButton8.Checked)
            {
                standard = 0;
            }
            if (radioButton9.Checked)
            {
                standard = 1;
            }
            if (radioButton10.Checked)
            {
                standard = 2;
            }
            if (radioButton11.Checked)
            {
                standard = 3;
            }
            if (radioButton12.Checked)
            {
                standard = 4;
            }

            for (int i = 0; i < 8; i++)
            {
                dane[i + 1] = Filters.FiltersSelect(standard, notch, dane[i + 1], i);
            }

            return dane;
        }

        private void turnOFF_SW()
        {
            if (istniejeSW)
            {
                istniejeSW = false;
                sw.Close();
            }
        }
        private void turnOFF_transmision()
        {
            char[] buff = new char[1];
            buff[0] = 's';//To turn off the fire hose, send an s.
            serialPort1.Write(buff, 0, 1);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            //-1V 1V
            chart2.axisY.Maximum = 1000000;
            chart2.axisY.Minimum = -1000000;

            chart3.axisY.Maximum = 1000000;
            chart3.axisY.Minimum = -1000000;

            chart4.axisY.Maximum = 1000000;
            chart4.axisY.Minimum = -1000000;

            chart5.axisY.Maximum = 1000000;
            chart5.axisY.Minimum = -1000000;

            chart6.axisY.Maximum = 1000000;
            chart6.axisY.Minimum = -1000000;

            chart7.axisY.Maximum = 1000000;
            chart7.axisY.Minimum = -1000000;

            chart8.axisY.Maximum = 1000000;
            chart8.axisY.Minimum = -1000000;

            chart9.axisY.Maximum = 1000000;
            chart9.axisY.Minimum = -1000000;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            //-100mV 100mV
            chart2.axisY.Maximum = 100000;
            chart2.axisY.Minimum = -100000;

            chart3.axisY.Maximum = 100000;
            chart3.axisY.Minimum = -100000;

            chart4.axisY.Maximum = 100000;
            chart4.axisY.Minimum = -100000;

            chart5.axisY.Maximum = 100000;
            chart5.axisY.Minimum = -100000;

            chart6.axisY.Maximum = 100000;
            chart6.axisY.Minimum = -100000;

            chart7.axisY.Maximum = 100000;
            chart7.axisY.Minimum = -100000;

            chart8.axisY.Maximum = 100000;
            chart8.axisY.Minimum = -100000;

            chart9.axisY.Maximum = 100000;
            chart9.axisY.Minimum = -100000;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //-10mv 10mv
            chart2.axisY.Maximum = 10000;
            chart2.axisY.Minimum = -10000;

            chart3.axisY.Maximum = 10000;
            chart3.axisY.Minimum = -10000;

            chart4.axisY.Maximum = 10000;
            chart4.axisY.Minimum = -10000;

            chart5.axisY.Maximum = 10000;
            chart5.axisY.Minimum = -10000;

            chart6.axisY.Maximum = 10000;
            chart6.axisY.Minimum = -10000;

            chart7.axisY.Maximum = 10000;
            chart7.axisY.Minimum = -10000;

            chart8.axisY.Maximum = 10000;
            chart8.axisY.Minimum = -10000;

            chart9.axisY.Maximum = 10000;
            chart9.axisY.Minimum = -10000;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            //-1mV
            chart2.axisY.Maximum = 1000;
            chart2.axisY.Minimum = -1000;

            chart3.axisY.Maximum = 1000;
            chart3.axisY.Minimum = -1000;

            chart4.axisY.Maximum = 1000;
            chart4.axisY.Minimum = -1000;

            chart5.axisY.Maximum = 1000;
            chart5.axisY.Minimum = -1000;

            chart6.axisY.Maximum = 1000;
            chart6.axisY.Minimum = -1000;

            chart7.axisY.Maximum = 1000;
            chart7.axisY.Minimum = -1000;

            chart8.axisY.Maximum = 1000;
            chart8.axisY.Minimum = -1000;

            chart9.axisY.Maximum = 1000;
            chart9.axisY.Minimum = -1000;
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            //-100uV
            chart2.axisY.Maximum = 100;
            chart2.axisY.Minimum = -100;

            chart3.axisY.Maximum = 100;
            chart3.axisY.Minimum = -100;

            chart4.axisY.Maximum = 100;
            chart4.axisY.Minimum = -100;

            chart5.axisY.Maximum = 100;
            chart5.axisY.Minimum = -100;

            chart6.axisY.Maximum = 100;
            chart6.axisY.Minimum = -100;

            chart7.axisY.Maximum = 100;
            chart7.axisY.Minimum = -100;

            chart8.axisY.Maximum = 100;
            chart8.axisY.Minimum = -100;

            chart9.axisY.Maximum = 100;
            chart9.axisY.Minimum = -100;
        }

    } // class Form1 
} //namespace OpenBCI_GUI
