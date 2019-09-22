using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBCI_GUI
{
    class Convert
    {
        public static int Bit16ToInt32(byte[] byteArray)
        {
            int result = (
              ((0xFF & byteArray[0]) << 8) |
               (0xFF & byteArray[1])
              );
            if ((result & 0x00008000) > 0)
            {
                result = (int)((uint)result | (uint)0xFFFF0000);

            }
            else
            {
                result = (int)((uint)result & (uint)0x0000FFFF);
            }
            return result;
        }
        public static int Bit24ToInt32(byte[] byteArray)
        {
            int result = (
                 ((0xFF & byteArray[0]) << 16) |
                 ((0xFF & byteArray[1]) << 8) |
                 (0xFF & byteArray[2])
               );
            if ((result & 0x00800000) > 0)
            {
                result = (int)((uint)result | (uint)0xFF000000);
            }
            else
            {
                result = (int)((uint)result & (uint)0x00FFFFFF);
            }
            return result;
        }

        static double[] ConvertedData = new double[12];
        private static int localByteCounter = 0;
        private static int localChannelCounter = 0;
        private static int PACKET_readstate = 0;
        private static byte[] localAdsByteBuffer = { 0, 0, 0 };
        private static byte[] localAccelByteBuffer = { 0, 0 };

//        Header

//Byte 1: 0xA0
//Byte 2: Sample Number
//EEG Data
//Note: values are 24-bit signed, MSB first

//Bytes 3-5: Data value for EEG channel 1; 3 bytes for each channel
//Bytes 6-8: Data value for EEG channel 2
//Bytes 9-11: Data value for EEG channel 3
//Bytes 12-14: Data value for EEG channel 4
//Bytes 15-17: Data value for EEG channel 5
//Bytes 18-20: Data value for EEG channel 6
//Bytes 21-23: Data value for EEG channel 6
//Bytes 24-26: Data value for EEG channel 8
//Aux Data

//Bytes 27-32: 6 bytes of data defined and parsed based on the Footer below
//Footer

//Byte 33: 0xCX where X is 0-F in hex

        public static double[] interpretBinaryStream(byte actbyte)
        {
            bool flag_copyRawDataToFullData = false;

            switch (PACKET_readstate) // the state transition for the finite automata
            {
                case 0:
                    if (actbyte == 0xC0)//Stop Byte = Byte 33: 0xCX where X is 0-F in hex
                                        //                        The following table is sorted by Stop Byte. 
                                        //Drivers should use the Stop Byte to determine how to parse the 6 AUX bytes.

                        //Stop Byte   Byte 27 Byte 28 Byte 29 Byte 30 Byte 31 Byte 32
                        //0xC0    AX1 AX0 AY1 AY0 AZ1 AZ0
                        //AX1 - AX0: Data value for accelerometer channel X 
                        //AY1 - AY0: Data value for accelerometer channel Y 
                        //AZ1 - AZ0: Data value for accelerometer channel Z

                     {          // poszukiwanie poczatku pakietu
                        PACKET_readstate++; // Stop byte =>  PACKET_readstate becomes 1 for the next byte in the stream
                    }
                    break;
                case 1:
                    if (actbyte == 0xA0) // Packet Header =         Byte 1: 0xA0
                    {          // poszukiwanie poczatku pakietu
                        PACKET_readstate++; //  Header Byte =>  PACKET_readstate becomes 2 for the next byte in the stream
                    }
                    else
                    {
                        PACKET_readstate = 0;
                    }
                    break;
                case 2: // Reading the sample number
                    localByteCounter = 0;
                    localChannelCounter = 0;
                    ConvertedData[localChannelCounter] = actbyte; // localChannelCounter ==0: the sample number

                    localChannelCounter++; // channel counter becomes 1 for the 1st channel

                    PACKET_readstate++; // PACKET_readstate becomes 3 for reading data bytes

                    break;
                case 3: // the state in which to read channel data bytes
                    localAdsByteBuffer[localByteCounter] = actbyte;

                    localByteCounter++;
                    if (localByteCounter == 3) // 3 bytes are read for the current channel
                    {
                        ConvertedData[localChannelCounter] = Bit24ToInt32( localAdsByteBuffer) ;

                        localChannelCounter++; // / Rhe channel counter increases for the next channel
                        if (localChannelCounter == 9) // all the 8 channel data are read
                        {
                            PACKET_readstate++; // PACKET_readstate becomes 4: All of the 8 channel data are read.

                            localByteCounter = 0;
                        }
                        else
                        {
                            localByteCounter = 0; // PACKET_readstate remains to be 3: continue to read channel data bytes.
                        }
                    }
                    break;
                case 4: // All of the 8 channel data are read. read the auxiliary data of 6 bytes
                    localAccelByteBuffer[localByteCounter] = actbyte; //  localByteCounter = 0

                    localByteCounter++;
                    if (localByteCounter == 2)
                    {
                        ConvertedData[ localChannelCounter ] = Bit16ToInt32( localAccelByteBuffer );

                        localChannelCounter++;

                        if (localChannelCounter == 12) // have read the two auxiliary 3 byte items 
                        {
                            PACKET_readstate++; // PACKET_readstate becomes 5
                            localByteCounter = 0;
                        }
                        else
                        {
                            localByteCounter = 0; // PACKET_readstate remains to be the same (4)
                        }
                    }
                    break;
                case 5: // All of the 8 channel data plus the auxiliary 2 * 3 bytes are read
                    if (actbyte == 0xC0) // //Stop Byte   Byte 27 Byte 28 Byte 29 Byte 30 Byte 31 Byte 32
                                         //0xC0    AX1 AX0 AY1 AY0 AZ1 AZ0
                                         //AX1 - AX0: Data value for accelerometer channel X 
                                         //AY1 - AY0: Data value for accelerometer channel Y 
                                         //AZ1 - AZ0: Data value for accelerometer channel Z
                    {
                        flag_copyRawDataToFullData = true;  // the current occurrence of the 8 channel data is completed
                        PACKET_readstate = 1;
                    }
                    else
                    {
                        PACKET_readstate = 0;
                    }

                    break;
                default:
                    PACKET_readstate = 0;
                    break;
            }//    switch (PACKET_readstate)

            if (flag_copyRawDataToFullData)
            {
                flag_copyRawDataToFullData = false;
                return ConvertedData; //// the current occurrence of the 8 channel data is completed => return the converted data
            }
            else
            {
                return null; ///// the current occurrence of the 8 channel data is NOT completed  ==> return null 
            }
        } //  public static double[] interpretBinaryStream(byte actbyte)
    }//    class Convert
}//namespace OpenBCI_GUI
