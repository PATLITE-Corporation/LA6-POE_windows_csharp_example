using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LA6_POE_Sample_CSharp
{
    class Program
    {
        private static Socket sock = null;

        // product category
        public static ushort PNS_PRODUCT_ID = 0x4142;

        // PNS command identifier
        private static readonly byte PNS_SMART_MODE_COMMAND = 0x54;         // smart mode control command
        private static readonly byte PNS_MUTE_COMMAND = 0x4D;               // mute command
        private static readonly byte PNS_STOP_PULSE_INPUT_COMMAND = 0x50;   // stop/pulse input command
        private static readonly byte PNS_RUN_CONTROL_COMMAND = 0x53;        // operation control command
        private static readonly byte PNS_DETAIL_RUN_CONTROL_COMMAND = 0x44; // detailed operation control command
        private static readonly byte PNS_CLEAR_COMMAND = 0x43;              // clear command
        private static readonly byte PNS_REBOOT_COMMAND = 0x42;             // reboot command
        private static readonly byte PNS_GET_DATA_COMMAND = 0x47;           // get status command
        private static readonly byte PNS_GET_DETAIL_DATA_COMMAND = 0x45;    // get detail status command

        // response data for PNS command
        private static readonly byte PNS_ACK = 0x06;                        // normal response
        private static readonly byte PNS_NAK = 0x15;                        // abnormal response

        // mode
        private static readonly byte PNS_LED_MODE = 0x00;                   // signal light mode
        private static readonly byte PNS_SMART_MODE = 0x01;                 // smart mode

        // LED unit for motion control command
        private static readonly byte PNS_RUN_CONTROL_LED_OFF = 0x00;        // light off
        private static readonly byte PNS_RUN_CONTROL_LED_ON = 0x01;         // light on
        private static readonly byte PNS_RUN_CONTROL_LED_BLINKING = 0x02;   // flashing
        private static readonly byte PNS_RUN_CONTROL_LED_NO_CHANGE = 0x09;  // no change

        // buzzer for motion control command
        private static readonly byte PNS_RUN_CONTROL_BUZZER_STOP = 0x00;        // stop
        private static readonly byte PNS_RUN_CONTROL_BUZZER_PATTERN1 = 0x01;    // pattern 1
        private static readonly byte PNS_RUN_CONTROL_BUZZER_PATTERN2 = 0x02;    // pattern 2
        private static readonly byte PNS_RUN_CONTROL_BUZZER_TONE = 0x03;        // buzzer tone for simultaneous buzzer input
        private static readonly byte PNS_RUN_CONTROL_BUZZER_NO_CHANGE = 0x09;   // no change

        // LED unit for detailed operation control command
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_OFF = 0x00;             // light off
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_RED = 0x01;       // red
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_YELLOW = 0x02;    // yellow
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_LEMON = 0x03;     // Graduates
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_GREEN = 0x04;     // green
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_SKY_BLUE = 0x05;  // sky blue
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_BLUE = 0x06;      // blue
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_PURPLE = 0x07;    // purple
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_PEACH = 0x08;     // peach
        private static readonly byte PNS_DETAIL_RUN_CONTROL_LED_COLOR_WHITE = 0x09;     // white

        // blinking action for detailed action control command
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BLINKING_OFF = 0x00;        // blinking off
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BLINKING_ON = 0x01;         // blinking ON

        // buzzer for detailed action control command
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_STOP = 0x00;         // stop
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN1 = 0x01;     // pattern 1
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN2 = 0x02;     // pattern 2
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN3 = 0x03;     // pattern 3
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN4 = 0x04;     // pattern 4
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN5 = 0x05;     // pattern 5
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN6 = 0x06;     // pattern 6
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN7 = 0x07;     // pattern 7
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN8 = 0x08;     // pattern 8
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN9 = 0x09;     // pattern 9
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN10 = 0x0A;    // pattern 10
        private static readonly byte PNS_DETAIL_RUN_CONTROL_BUZZER_PATTERN11 = 0x0B;    // pattern 11

        // operation control data structure
        public class PNS_RUN_CONTROL_DATA
        {
            // 1st LED unit pattern
            public byte led1Pattern = 0;

            // 2nd LED unit pattern
            public byte led2Pattern = 0;

            // 3rd LED unit pattern
            public byte led3Pattern = 0;

            // 4th LED unit pattern.
            public byte led4Pattern = 0;

            // 5th LED unit pattern.
            public byte led5Pattern = 0;

            // buzzer pattern 1 to 3
            public byte buzzerPattern = 0;
        };

        // detail operation control data structure
        public class PNS_DETAIL_RUN_CONTROL_DATA
        {
            // 1st color of LED unit.
            public byte led1Color = 0;

            // 2nd color of LED unit.
            public byte led2Color = 0;

            // 3rd color of LED unit.
            public byte led3Color = 0;

            // 4th color of LED unit.
            public byte led4Color = 0;

            // 5th color of LED unit.
            public byte led5Color = 0;

            // blinking action
            public byte blinkingControl = 0;

            // buzzer pattern 1 to 11
            public byte buzzerPattern = 0;
        };

        // status data of operation control
        public class PNS_STATUS_DATA
        {
            // input 1 to 8
            public byte[] input = new byte[8];

            // mode
            public byte mode = 0;

            // status data when running signal light mode
            public PNS_LED_MODE_DATA ledModeData = null;

            // status data during smart mode execution
            public PNS_SMART_MODE_DATA smartModeData = null;
        };

        // status data when running in signal light mode
        public class PNS_LED_MODE_DATA
        {
            // 1st LED unit pattern
            public byte led1Pattern = 0;

            // 2nd LED unit pattern
            public byte led2Pattern = 0;

            // 3rd LED unit pattern
            public byte led3Pattern = 0;

            // 4th LED unit pattern.
            public byte led4Pattern = 0;

            // 5th LED unit pattern.
            public byte led5Pattern = 0;

            // buzzer patterns 1 through 11
            public byte buzzerPattern = 0;
        };

        // state data when running smart mode
        public class PNS_SMART_MODE_DATA
        {
            // group number
            public byte groupNo = 0;

            // mute
            public byte mute = 0;

            // STOP input
            public byte stopInput = 0;

            // pattern number
            public byte patternNo = 0;
        };

        // status data of detailed operation control
        public class PNS_DETAIL_STATUS_DATA
        {
            // MAC address
            public byte[] macAddress = new byte[6];

            // Input 1 to 8
            public byte[] input = new byte[8];

            // mode
            public byte mode = 0;

            // detailed status data when running signal light mode
            public PNS_LED_MODE_DETAIL_DATA ledModeDetalData = null;

            // detailed state data when running in smart mode
            public PNS_SMART_MODE_DETAIL_DATA smartModeDetalData = null;
        };

        // detailed state data when running in signal light mode
        public class PNS_LED_MODE_DETAIL_DATA
        {
            // 1st stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit1Data = null;

            // 2nd stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit2Data = null;

            // 3rd stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit3Data = null;

            // 4th stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit4Data = null;

            // 5th stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit5Data = null;

            // buzzer pattern 1 to 11
            public byte buzzerPattern = 0;
        };

        // LED unit data
        public class PNS_LED_UNIT_DATA
        {
            // status
            public byte ledPattern = 0;

            // R
            public byte red = 0;

            // G
            public byte green = 0;

            // B
            public byte blue = 0;
        };

        // detail state data for smart mode execution
        public class PNS_SMART_MODE_DETAIL_DATA
        {
            // smart mode state
            public PNS_SMART_MODE_DETAIL_STATE_DATA smartModeData = null;

            // 1st stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit1Data = null;

            // 2nd stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit2Data = null;

            // 3rd stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit3Data = null;

            // 4th stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit4Data = null;

            // 5th stage of LED unit
            public PNS_LED_UNIT_DATA ledUnit5Data = null;

            // buzzer pattern 1 to 11
            public byte buzzerPattern = 0;
        };

        // smart mode status data
        public class PNS_SMART_MODE_DETAIL_STATE_DATA
        {
            // group number
            public byte groupNo = 0;

            // mute
            public byte mute = 0;

            // STOP input
            public byte stopInput = 0;

            // pattern number
            public byte patternNo = 0;

            // last pattern
            public byte lastPattern = 0;
        };

        // PHN command identifier
        private static readonly byte PHN_WRITE_COMMAND = 0x57;      // write command
        private static readonly byte PHN_READ_COMMAND = 0x52;       // read command

        // response data for PNS command
        private static readonly byte[] PHN_ACK = { 0x41, 0x43, 0x4B };  // normal response
        private static readonly byte[] PHN_NAK = { 0x4E, 0x41, 0x4B };  // abnormal response

        // action data of PHN command
        private static readonly byte PHN_LED_UNIT1_BLINKING = 0x20;     // 1st LED unit blinking
        private static readonly byte PHN_LED_UNIT2_BLINKING = 0x40;     // 2nd LED unit blinking
        private static readonly byte PHN_LED_UNIT3_BLINKING = 0x80;     // 3rd LED unit blinking
        private static readonly byte PHN_BUZZER_PATTERN1 = 0x8;         // buzzer pattern 1
        private static readonly byte PHN_BUZZER_PATTERN2 = 0x10;        // buzzer pattern 2
        private static readonly byte PHN_LED_UNIT1_LIGHTING = 0x1;      // 1st LED unit lighting
        private static readonly byte PHN_LED_UNIT2_LIGHTING = 0x2;      // 2nd LED unit lighting
        private static readonly byte PHN_LED_UNIT3_LIGHTING = 0x4;      // 3rd LED unit lighting

        /// <summary>
        /// Main Function
        /// </summary>
        static void Main()
        {
            int ret;

            // Connect to LA-POE
            ret = SocketOpen("192.168.10.1", 10000);
            if (ret == -1)
                return;

            // Get the command identifier specified by the command line argument
            string commandId = "";
            string[] cmds = System.Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
                commandId = cmds[1];

            switch (commandId)
            {
                case "T":
                    {
                        // smart mode control command
                        if (cmds.Length >= 3)
                            PNS_SmartModeCommand(byte.Parse(cmds[2]));
                        break;
                    }

                case "M":
                    {
                        // mute command
                        if (cmds.Length >= 3)
                            PNS_MuteCommand(byte.Parse(cmds[2]));
                        break;
                    }

                case "P":
                    {
                        // stop/pulse input command
                        if (cmds.Length >= 3)
                            PNS_StopPulseInputCommand(byte.Parse(cmds[2]));
                        break;
                    }

                case "S":
                    {
                        // operation control command
                        if (cmds.Length >= 8)
                        {
                            PNS_RUN_CONTROL_DATA runControlData = new PNS_RUN_CONTROL_DATA()
                            {
                                led1Pattern = byte.Parse(cmds[2]),
                                led2Pattern = byte.Parse(cmds[3]),
                                led3Pattern = byte.Parse(cmds[4]),
                                led4Pattern = byte.Parse(cmds[5]),
                                led5Pattern = byte.Parse(cmds[6]),
                                buzzerPattern = byte.Parse(cmds[7])
                            };
                            PNS_RunControlCommand(runControlData);
                        }

                        break;
                    }

                case "D":
                    {
                        // detailed operation control command
                        if (cmds.Length >= 9)
                        {
                            PNS_DETAIL_RUN_CONTROL_DATA detalRunControlData = new PNS_DETAIL_RUN_CONTROL_DATA()
                            {
                                led1Color = byte.Parse(cmds[2]),
                                led2Color = byte.Parse(cmds[3]),
                                led3Color = byte.Parse(cmds[4]),
                                led4Color = byte.Parse(cmds[5]),
                                led5Color = byte.Parse(cmds[6]),
                                blinkingControl = byte.Parse(cmds[7]),
                                buzzerPattern = byte.Parse(cmds[8])
                            };
                            PNS_DetailRunControlCommand(detalRunControlData);
                        }

                        break;
                    }

                case "C":
                    {
                        // clear command
                        PNS_ClearCommand();
                        break;
                    }

                case "B":
                    {
                        // reboot command
                        if (cmds.Length >= 3)
                            PNS_RebootCommand(cmds[2]);
                        break;
                    }

                case "G":
                    {
                        // get status command
                        PNS_STATUS_DATA statusData = new PNS_STATUS_DATA();
                        ret = PNS_GetDataCommand(out statusData);
                        if (ret == 0)
                        {
                            // Display acquired data
                            Console.WriteLine("Response data for status acquisition command");
                            // Input1
                            Console.WriteLine("Input1 : " + statusData.input[0].ToString());
                            // Input2
                            Console.WriteLine("Input2 : " + statusData.input[1].ToString());
                            // Input3
                            Console.WriteLine("Input3 : " + statusData.input[2].ToString());
                            // Input4
                            Console.WriteLine("Input4 : " + statusData.input[3].ToString());
                            // Input5
                            Console.WriteLine("Input5 : " + statusData.input[4].ToString());
                            // Input6
                            Console.WriteLine("Input6 : " + statusData.input[5].ToString());
                            // Input7
                            Console.WriteLine("Input7 : " + statusData.input[6].ToString());
                            // Input8
                            Console.WriteLine("Input8 : " + statusData.input[7].ToString());
                            // mode
                            if (statusData.mode == PNS_LED_MODE)
                            {
                                // signal light mode
                                Console.WriteLine("signal light mode");
                                // 1st LED unit pattern
                                Console.WriteLine("1st LED unit pattern : " + statusData.ledModeData.led1Pattern.ToString());
                                // 2nd LED unit pattern
                                Console.WriteLine("2nd LED unit pattern : " + statusData.ledModeData.led2Pattern.ToString());
                                // 3rd LED unit pattern
                                Console.WriteLine("3rd LED unit pattern : " + statusData.ledModeData.led3Pattern.ToString());
                                // 4th LED unit pattern
                                Console.WriteLine("4th LED unit pattern : " + statusData.ledModeData.led4Pattern.ToString());
                                // 5th LED unit pattern
                                Console.WriteLine("5th LED unit pattern : " + statusData.ledModeData.led5Pattern.ToString());
                                // buzzer pattern
                                Console.WriteLine("buzzer pattern: " + statusData.ledModeData.buzzerPattern.ToString());
                            }
                            else
                            {
                                // smart mode
                                Console.WriteLine("smart mode");
                                // group number
                                Console.WriteLine("group number : " + statusData.smartModeData.groupNo.ToString());
                                // mute
                                Console.WriteLine("mute : " + statusData.smartModeData.mute.ToString());
                                // STOP input
                                Console.WriteLine("STOP input : " + statusData.smartModeData.stopInput.ToString());
                                // pattern number
                                Console.WriteLine("pattern number : " + statusData.smartModeData.patternNo.ToString());
                            }
                        }

                        break;
                    }

                case "E":
                    {
                        // get detail status command
                        PNS_DETAIL_STATUS_DATA detailStatusData = new PNS_DETAIL_STATUS_DATA();
                        ret = PNS_GetDetailDataCommand(out detailStatusData);
                        if (ret == 0)
                        {
                            // Display acquired data
                            Console.WriteLine("Response data for status acquisition command");
                            // MAC address
                            Console.WriteLine("MAC address : " + Convert.ToString(detailStatusData.macAddress[0], 16) + "-"
                                                               + Convert.ToString(detailStatusData.macAddress[1], 16) + "-"
                                                               + Convert.ToString(detailStatusData.macAddress[2], 16) + "-"
                                                               + Convert.ToString(detailStatusData.macAddress[3], 16) + "-"
                                                               + Convert.ToString(detailStatusData.macAddress[4], 16) + "-"
                                                               + Convert.ToString(detailStatusData.macAddress[5], 16));
                            // Input1
                            Console.WriteLine("Input1 : " + detailStatusData.input[0].ToString());
                            // Input2
                            Console.WriteLine("Input2 : " + detailStatusData.input[1].ToString());
                            // Input3
                            Console.WriteLine("Input3 : " + detailStatusData.input[2].ToString());
                            // Input4
                            Console.WriteLine("Input4 : " + detailStatusData.input[3].ToString());
                            // Input5
                            Console.WriteLine("Input5 : " + detailStatusData.input[4].ToString());
                            // Input6
                            Console.WriteLine("Input6 : " + detailStatusData.input[5].ToString());
                            // Input7
                            Console.WriteLine("Input7 : " + detailStatusData.input[6].ToString());
                            // Input8
                            Console.WriteLine("Input8 : " + detailStatusData.input[7].ToString());
                            // mode
                            if (detailStatusData.mode == PNS_LED_MODE)
                            {
                                // signal light mode
                                Console.WriteLine("signal light mode");
                                // 1st LED unit
                                Console.WriteLine("1st LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.ledModeDetalData.ledUnit1Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.ledModeDetalData.ledUnit1Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.ledModeDetalData.ledUnit1Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.ledModeDetalData.ledUnit1Data.blue.ToString());
                                // 2nd LED unit
                                Console.WriteLine("2nd LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.ledModeDetalData.ledUnit2Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.ledModeDetalData.ledUnit2Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.ledModeDetalData.ledUnit2Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.ledModeDetalData.ledUnit2Data.blue.ToString());
                                // 3rd LED unit
                                Console.WriteLine("3rd LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.ledModeDetalData.ledUnit3Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.ledModeDetalData.ledUnit3Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.ledModeDetalData.ledUnit3Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.ledModeDetalData.ledUnit3Data.blue.ToString());
                                // 4th LED unit
                                Console.WriteLine("4th LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.ledModeDetalData.ledUnit4Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.ledModeDetalData.ledUnit4Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.ledModeDetalData.ledUnit4Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.ledModeDetalData.ledUnit4Data.blue.ToString());
                                // 5th LED unit
                                Console.WriteLine("5th LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.ledModeDetalData.ledUnit5Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.ledModeDetalData.ledUnit5Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.ledModeDetalData.ledUnit5Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.ledModeDetalData.ledUnit5Data.blue.ToString());
                                // buzzer pattern
                                Console.WriteLine("buzzer pattern: " + detailStatusData.ledModeDetalData.buzzerPattern.ToString());
                            }
                            else
                            {
                                // smart mode
                                Console.WriteLine("smart mode");
                                // group number
                                Console.WriteLine("group number : " + detailStatusData.smartModeDetalData.smartModeData.groupNo.ToString());
                                // mute
                                Console.WriteLine("mute : " + detailStatusData.smartModeDetalData.smartModeData.mute.ToString());
                                // STOP input
                                Console.WriteLine("STOP input : " + detailStatusData.smartModeDetalData.smartModeData.stopInput.ToString());
                                // pattern number
                                Console.WriteLine("pattern number : " + detailStatusData.smartModeDetalData.smartModeData.patternNo.ToString());
                                // last pattern
                                Console.WriteLine("last pattern : " + detailStatusData.smartModeDetalData.smartModeData.lastPattern.ToString());
                                // 1st LED unit
                                Console.WriteLine("1st LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.smartModeDetalData.ledUnit1Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.smartModeDetalData.ledUnit1Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.smartModeDetalData.ledUnit1Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.smartModeDetalData.ledUnit1Data.blue.ToString());
                                // 2nd LED unit
                                Console.WriteLine("2nd LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.smartModeDetalData.ledUnit2Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.smartModeDetalData.ledUnit2Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.smartModeDetalData.ledUnit2Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.smartModeDetalData.ledUnit2Data.blue.ToString());
                                // 3rd LED unit
                                Console.WriteLine("3rd LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.smartModeDetalData.ledUnit3Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.smartModeDetalData.ledUnit3Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.smartModeDetalData.ledUnit3Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.smartModeDetalData.ledUnit3Data.blue.ToString());
                                // 4th LED unit
                                Console.WriteLine("4th LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.smartModeDetalData.ledUnit4Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.smartModeDetalData.ledUnit4Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.smartModeDetalData.ledUnit4Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.smartModeDetalData.ledUnit4Data.blue.ToString());
                                // 5th LED unit
                                Console.WriteLine("5th LED unit");
                                // pattern
                                Console.WriteLine("pattern : " + detailStatusData.smartModeDetalData.ledUnit5Data.ledPattern.ToString());
                                // R
                                Console.WriteLine("R : " + detailStatusData.smartModeDetalData.ledUnit5Data.red.ToString());
                                // G
                                Console.WriteLine("G : " + detailStatusData.smartModeDetalData.ledUnit5Data.green.ToString());
                                // B
                                Console.WriteLine("B : " + detailStatusData.smartModeDetalData.ledUnit5Data.blue.ToString());
                                // buzzer pattern
                                Console.WriteLine("buzzer pattern: " + detailStatusData.smartModeDetalData.buzzerPattern.ToString());
                            }
                        }

                        break;
                    }

                case "W":
                    {
                        // write command
                        if (cmds.Length >= 3)
                            PHN_WriteCommand(byte.Parse(cmds[2]));
                        break;
                    }

                case "R":
                    {
                        // read command
                        byte runData;
                        ret = PHN_ReadCommand(out runData);
                        if (ret == 0)
                        {
                            // Display acquired data
                            Console.WriteLine("Response data for read command");
                            // LED unit flashing
                            Console.WriteLine("LED unit flashing");
                            // 1st LED unit
                            Console.WriteLine("1st LED unit : " + ((runData & 0x20) != 0 ? 1 : 0).ToString());
                            // 2nd LED unit
                            Console.WriteLine("2nd LED unit : " + ((runData & 0x40) != 0 ? 1 : 0).ToString());
                            // 3rd LED unit
                            Console.WriteLine("3rd LED unit : " + ((runData & 0x80) != 0 ? 1 : 0).ToString());
                            // buzzer pattern
                            Console.WriteLine("buzzer pattern");
                            // pattern1
                            Console.WriteLine("pattern1 : " + ((runData & 0x8) != 0 ? 1 : 0).ToString());
                            // pattern2
                            Console.WriteLine("pattern2 : " + ((runData & 0x10) != 0 ? 1 : 0).ToString());
                            // LED unit lighting
                            Console.WriteLine("LED unit lighting");
                            // 1st LED unit
                            Console.WriteLine("1st LED unit : " + ((runData & 0x1) != 0 ? 1 : 0).ToString());
                            // 2nd LED unit
                            Console.WriteLine("2nd LED unit : " + ((runData & 0x2) != 0 ? 1 : 0).ToString());
                            // 3rd LED unit
                            Console.WriteLine("3rd LED unit : " + ((runData & 0x4) != 0 ? 1 : 0).ToString());
                        }

                        break;
                    }
            }

            // Close the socket
            SocketClose();
        }

        /// <summary>
        /// Connect to LA-POE
        /// </summary>
        /// <param name="ip">IP address</param>
        /// <param name="port">port number</param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int SocketOpen(string ip, int port)
        {
            try
            {
                // Set the IP address and port
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a socket
                sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                if (sock == null)
                {
                    Console.Write("failed to create socket");
                    return -1;
                }

                // Connect to LA-POE
                sock.Connect(remoteEP);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                sock.Close();
                sock.Dispose();
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Close the socket.
        /// </summary>
        public static void SocketClose()
        {
            if (sock != null)
            {
                // Close the socket.
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                sock.Dispose();
            }

        }

        /// <summary>
        /// Send command
        /// </summary>
        /// <param name="sendData">send data</param>
        /// <param name="recvData">received data</param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int SendCommand(byte[] sendData, out byte[] recvData)
        {
            int ret;
            recvData = null;

            try
            {
                if (sock == null)
                {
                    Console.Write("socket is not");
                    return -1;
                }

                // Send
                ret = sock.Send(sendData);
                if (ret < 0)
                {
                    Console.Write("failed to send");
                    return -1;
                }

                // Receive response data
                byte[] bytes = new byte[1024];
                int recvSize = sock.Receive(bytes);
                if (recvSize < 0)
                {
                    Console.Write("failed to recv");
                    return -1;
                }
                recvData = new byte[recvSize];
                Array.Copy(bytes, recvData, recvSize);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send smart mode control command for PNS command
        /// Smart mode can be executed for the number specified in the data area
        /// </summary>
        /// <param name="groupNo">Group number to execute smart mode (0x01(Group No.1) to 0x1F(Group No.31))</param>
        /// <returns>Success: 0, Failure: non-zero</returns>
        public static int PNS_SmartModeCommand(byte groupNo)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (T)
                sendData = sendData.Concat(new byte[] { PNS_SMART_MODE_COMMAND }).ToArray();

                // Empty (0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)Marshal.SizeOf(groupNo)).Reverse()).ToArray();

                // Data area
                sendData = sendData.Concat(new byte[] { groupNo }).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send mute command for PNS command
        /// Can control the buzzer ON/OFF while Smart Mode is running
        /// </summary>
        /// <param name="mute">Buzzer ON/OFF (ON: 1, OFF: 0)</param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PNS_MuteCommand(byte mute)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (M)
                sendData = sendData.Concat(new byte[] { PNS_MUTE_COMMAND }).ToArray();

                // Empty (0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)Marshal.SizeOf(mute)).Reverse()).ToArray();

                // Data area
                sendData = sendData.Concat(new byte[] { mute }).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send stop/pulse input command for PNS command
        /// Transmit during time trigger mode operation to control stop/resume of pattern (STOP input)
        /// Sending this command during pulse trigger mode operation enables pattern transition (trigger input).
        /// </summary>
        /// <param name="input">STOP input/trigger input (STOP input ON/trigger input: 1, STOP input: 0)</param>
        /// <returns>Success: 0, failure: non-zero</returns>
        public static int PNS_StopPulseInputCommand(byte input)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (P)
                sendData = sendData.Concat(new byte[] { PNS_STOP_PULSE_INPUT_COMMAND }).ToArray();

                // empty(0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)Marshal.SizeOf(input)).Reverse()).ToArray();

                // Data area
                sendData = sendData.Concat(new byte[] { input }).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send operation control command for PNS command
        /// Each stage of the LED unit and the buzzer (1 to 3) can be controlled by the pattern specified in the data area
        /// Operates with the color and buzzer set in the signal light mode
        /// </summary>
        /// <param name="runControlData">
        /// Pattern of the 1st to 5th stage of the LED unit and buzzer (1 to 3)
        /// Pattern of LED unit (off: 0, on: 1, blinking: 2, no change: 9)
        /// Pattern of buzzer (stop: 0, pattern 1: 1, pattern 2: 2, buzzer tone when input simultaneously with buzzer: 3, no change: 9)
        /// </param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PNS_RunControlCommand(PNS_RUN_CONTROL_DATA runControlData)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command Identifier(S)
                sendData = sendData.Concat(new byte[] { PNS_RUN_CONTROL_COMMAND }).ToArray();

                // Empty(0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // data size, data area
                byte[] data = {
                    runControlData.led1Pattern,     // 1st LED unit pattern
                    runControlData.led2Pattern,     // 2nd LED unit pattern
                    runControlData.led3Pattern,     // 3rd LED unit pattern
                    runControlData.led4Pattern,     // 4th LED unit pattern
                    runControlData.led5Pattern,     // 5th LED unit pattern
                    runControlData.buzzerPattern    // Buzzer pattern 1 to 3
                };
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)data.Length).Reverse()).ToArray();
                sendData = sendData.Concat(data).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send detailed operation control command for PNS command
        /// The color and operation pattern of each stage of the LED unit and the buzzer pattern (1 to 11) can be specified and controlled in the data area
        /// </summary>
        /// <param name="detailRunControlData">
        /// Pattern of the 1st to 5th stage of the LED unit, blinking operation and buzzer (1 to 3)
        /// Pattern of LED unit (off: 0, red: 1, yellow: 2, lemon: 3, green: 4, sky blue: 5, blue: 6, purple: 7, peach: 8, white: 9)
        /// Flashing action (Flashing OFF: 0, Flashing ON: 1)
        /// Buzzer pattern (Stop: 0, Pattern 1: 1, Pattern 2: 2, Pattern 3: 3, Pattern 4: 4, Pattern 5: 5, Pattern 6: 6, Pattern 7: 7, Pattern 8: 8, Pattern 9: 9, Pattern 10: 10, Pattern 11: 11)
        /// </param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PNS_DetailRunControlCommand(PNS_DETAIL_RUN_CONTROL_DATA detailRunControlData)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (D)
                sendData = sendData.Concat(new byte[] { PNS_DETAIL_RUN_CONTROL_COMMAND }).ToArray();

                // Empty(0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // data size, data area
                byte[] data = {
                    detailRunControlData.led1Color,          // 1st color of LED unit
                    detailRunControlData.led2Color,          // 2nd color of LED unit
                    detailRunControlData.led3Color,          // 3rd color of LED unit
                    detailRunControlData.led4Color,          // 4th color of LED unit
                    detailRunControlData.led5Color,          // 5th color of LED unit
                    detailRunControlData.blinkingControl,    // blinking operation
                    detailRunControlData.buzzerPattern       // buzzer pattern 1 to 11
                };
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)data.Length).Reverse()).ToArray();
                sendData = sendData.Concat(data).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send clear command for PNS command
        /// Turn off the LED unit and stop the buzzer
        /// </summary>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PNS_ClearCommand()
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (C)
                sendData = sendData.Concat(new byte[] { PNS_CLEAR_COMMAND }).ToArray();

                // Empty (0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)0)).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send restart command for PNS command
        /// LA6-POE can be restarted
        /// </summary>
        /// <param name="password">Password set in the password setting of Web Configuration</param>.
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PNS_RebootCommand(string password)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (B)
                sendData = sendData.Concat(new byte[] { PNS_REBOOT_COMMAND }).ToArray();

                // Empty (0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((ushort)password.Length).Reverse()).ToArray();

                // Data area
                sendData = sendData.Concat(System.Text.Encoding.ASCII.GetBytes(password)).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send status acquisition command for PNS command
        /// Signal line/contact input status and LED unit and buzzer status can be acquired
        /// </summary>
        /// <param name="statusData">Received data of status acquisition command (status of signal line/contact input and status of LED unit and buzzer)</param>
        /// <returns>Success: 0, failure: non-zero</returns>
        public static int PNS_GetDataCommand(out PNS_STATUS_DATA statusData)
        {
            int ret;
            statusData = new PNS_STATUS_DATA();

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (G)
                sendData = sendData.Concat(new byte[] { PNS_GET_DATA_COMMAND }).ToArray();

                // Empty (0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((short)0).Reverse()).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }

                // Input 1 to 8
                statusData.input = new byte[8];
                Array.Copy(recvData, statusData.input, statusData.input.Length);

                // Mode
                statusData.mode = recvData[8];

                // Check the mode
                if (statusData.mode == PNS_LED_MODE)
                {
                    // signal light mode
                    statusData.ledModeData = new PNS_LED_MODE_DATA
                    {
                        led1Pattern = recvData[9],      // 1st LED unit pattern
                        led2Pattern = recvData[10],     // 2nd LED unit pattern
                        led3Pattern = recvData[11],     // 3rd LED unit pattern
                        led4Pattern = recvData[12],     // 4th LED unit pattern
                        led5Pattern = recvData[13],     // 5th LED unit pattern
                        buzzerPattern = recvData[14],   // buzzer pattern 1 to 11
                    };
                }
                else
                {
                    // smart mode
                    statusData.smartModeData = new PNS_SMART_MODE_DATA
                    {
                        groupNo = recvData[9],      // group number
                        mute = recvData[10],        // mute
                        stopInput = recvData[11],   // STOP input
                        patternNo = recvData[12],   // pattern number
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send command to get detailed status of PNS command
        /// Signal line/contact input status, LED unit and buzzer status, and color information for each stage can be acquired
        /// </summary>
        /// <param name="detailStatusData">Received data of detail status acquisition command (status of signal line/contact input, status of LED unit and buzzer, and color information of each stage)</param>
        /// <returns>Success: 0, failure: non-zero</returns>
        public static int PNS_GetDetailDataCommand(out PNS_DETAIL_STATUS_DATA detailStatusData)
        {
            int ret;
            detailStatusData = new PNS_DETAIL_STATUS_DATA();

            try
            {
                byte[] sendData = { };

                // Product Category (AB)
                sendData = sendData.Concat(BitConverter.GetBytes(PNS_PRODUCT_ID).Reverse()).ToArray();

                // Command identifier (E)
                sendData = sendData.Concat(new byte[] { PNS_GET_DETAIL_DATA_COMMAND }).ToArray();

                // Empty(0)
                sendData = sendData.Concat(new byte[] { 0 }).ToArray();

                // Data size
                sendData = sendData.Concat(BitConverter.GetBytes((short)0).Reverse()).ToArray();

                // Send PNS command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] == PNS_NAK)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }

                // MAC Address
                detailStatusData.macAddress = new byte[6];
                Array.Copy(recvData, detailStatusData.macAddress, detailStatusData.macAddress.Length);

                // Input 1 to 8
                detailStatusData.input = new byte[8];
                Array.Copy(recvData, 6, detailStatusData.input, 0, detailStatusData.input.Length);

                // Mode
                detailStatusData.mode = recvData[14];

                // Check the mode
                if (detailStatusData.mode == PNS_LED_MODE)
                {
                    // signal light mode
                    detailStatusData.ledModeDetalData = new PNS_LED_MODE_DETAIL_DATA
                    {
                        // 1st stage of LED unit
                        ledUnit1Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[19],  // state
                            red = recvData[20],         // R
                            green = recvData[21],       // G
                            blue = recvData[22],        // B
                        },

                        // 2nd stage of LED unit
                        ledUnit2Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[23],  // state
                            red = recvData[24],         // R
                            green = recvData[25],       // G
                            blue = recvData[26],        // B
                        },

                        // 3rd stage of LED unit
                        ledUnit3Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[27],  // state
                            red = recvData[28],         // R
                            green = recvData[29],       // G
                            blue = recvData[30],        // B
                        },

                        // 4th stage of LED unit
                        ledUnit4Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[31],  // state
                            red = recvData[32],         // R
                            green = recvData[33],       // G
                            blue = recvData[34],        // B
                        },

                        // 5th stage of LED unit
                        ledUnit5Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[35],  // state
                            red = recvData[36],         // R
                            green = recvData[37],       // G
                            blue = recvData[38],        // B
                        },

                        // buzzer patterns 1-11
                        buzzerPattern = recvData[39]
                    };
                }
                else
                {
                    // smart mode
                    detailStatusData.smartModeDetalData = new PNS_SMART_MODE_DETAIL_DATA
                    {
                        // smart mode status
                        smartModeData = new PNS_SMART_MODE_DETAIL_STATE_DATA
                        {
                            groupNo = recvData[19],     // group number
                            mute = recvData[20],        // mute
                            stopInput = recvData[21],   // STOP input
                            patternNo = recvData[22],   // pattern number
                            lastPattern = recvData[23], // last pattern
                        },

                        // 1st stage of LED unit
                        ledUnit1Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[24],  // state
                            red = recvData[25],         // R
                            green = recvData[26],       // G
                            blue = recvData[27],        // B
                        },

                        // 2nd stage of LED unit
                        ledUnit2Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[28],  // state
                            red = recvData[29],         // R
                            green = recvData[30],       // G
                            blue = recvData[31],        // B
                        },

                        // 3rd stage of LED unit
                        ledUnit3Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[32],  // state
                            red = recvData[33],         // R
                            green = recvData[34],       // G
                            blue = recvData[35],        // B
                        },

                        // 4th stage of LED unit
                        ledUnit4Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[36], // state
                            red = recvData[37],         // R
                            green = recvData[38],       // G
                            blue = recvData[39],        // B
                        },

                        // 5th stage of LED unit
                        ledUnit5Data = new PNS_LED_UNIT_DATA
                        {
                            ledPattern = recvData[40],  // state
                            red = recvData[41],         // R
                            green = recvData[42],       // G
                            blue = recvData[43],        // B
                        },

                        // buzzer patterns 1-11
                        buzzerPattern = recvData[44]
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send PHN command write command
        /// Can control the lighting and blinking of LED units 1 to 3 stages, and buzzer patterns 1 and 2
        /// </summary>
        /// <param name="data">
        /// Operation data for lighting and blinking of LED unit 1 to 3 stages, and buzzer pattern 1 and 2
        /// bit7: 3rd LED unit blinking (OFF: 0, ON: 1)
        /// bit6: 2nd LED unit blinking (OFF: 0, ON: 1)
        /// bit5: 1st LED unit blinking (OFF: 0, ON: 1)
        /// bit4: Buzzer pattern 2 (OFF: 0, ON: 1)
        /// bit3: Buzzer pattern 1 (OFF: 0, ON: 1)
        /// bit2: 3rd LED unit lighting (OFF: 0, ON: 1)
        /// bit1: 2nd LED unit lighting (OFF: 0, ON: 1)
        /// bit0: 1st LED unit lighting (OFF: 0, ON: 1)
        /// </param>
        /// <returns>success: 0, failure: non-zero</returns>
        public static int PHN_WriteCommand(byte runData)
        {
            int ret;

            try
            {
                byte[] sendData = { };

                // Command identifier (W)
                sendData = sendData.Concat(new byte[] { PHN_WRITE_COMMAND }).ToArray();

                // Operation data
                sendData = sendData.Concat(new byte[] { runData }).ToArray();

                // send PHN command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData.SequenceEqual(PHN_NAK))
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Send command to read PHN command
        /// Get information about LED unit 1 to 3 stage lighting and blinking, and buzzer pattern 1 and 2
        /// </summary>
        /// <param name="runData">Received data of read command (operation data of LED unit 1 to 3 stages lighting and blinking, buzzer pattern 1,2)</param>
        /// <returns>Success: 0, failure: non-zero</returns>
        public static int PHN_ReadCommand(out byte runData)
        {
            int ret;
            runData = 0;

            try
            {
                byte[] sendData = { };

                // Command identifier (R)
                sendData = sendData.Concat(new byte[] { PHN_READ_COMMAND }).ToArray();

                // send PHN command
                byte[] recvData;
                ret = SendCommand(sendData, out recvData);
                if (ret != 0)
                {
                    Console.Write("failed to send data");
                    return -1;
                }

                // check the response data
                if (recvData[0] != PHN_READ_COMMAND)
                {
                    // receive abnormal response
                    Console.Write("negative acknowledge");
                    return -1;
                }

                // Response data
                runData = recvData[1];
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }

            return 0;
        }
    }
}
