using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private LibPcapLiveDeviceList devices = null;
        private int packetIndex = 0;
        private int packetIndex2 = 0;
        private int port = 54090;
        private Queue<int> queue = new Queue<int>(); // When port # is the destination port
        private Queue<List<byte>> udpPayloadDataQueue = new Queue<List<byte>>();
        private Queue<int> queue2 = new Queue<int>(); // When port # is the source port
        private Stack<int> outOfBaitStack = new Stack<int>();
        private Stack<int> fishAgainStack = new Stack<int>();
        private LibPcapLiveDevice device = null;
        private Control control;
        private Control control2;
        private string outputFile = "temp";
        private int cin = 4;
        private List<int> mogHouseCompletedCraftValues = new List<int> { 164, 165, 166, 185, 186, 187, 188, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 311 };
        private List<int> startCraftValues = new List<int> { 181, 183, 184, 185, 186, 187, 188, 189, 190 };
        private List<int> completeCraftValues = null;
        private List<int> passedCraftValue = new List<int> { 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 255 };
        private List<int> failedCraftValues = new List<int> { 163, 164, 165, 166 };
        private List<int> valueHistory = new List<int>();
        private List<int> windurstWatersFishHookedValues = new List<int> { 198, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 229, 230, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 253, 255, 260, 267, 297, 311, 341, 366, 373 };
        private List<int> windurstWatersPutFishingRodAwayValues = new List<int> { 143, 144, 145, 146, 147, 148, 149, 163, 175, 179, 180, 181, 182, 183 };
        private List<int> windurstWatersOutOfBaitValues = new List<int> { 65, 66, 67, 68, 69 };

        private List<int> windurstWoodsFishHookedValues = new List<int> { 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 312, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555 };
        private List<int> windurstWoodsPutFishingRodAwayValues = new List<int> { 148, 161, 162, 163, 164, 165, 166, 167, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 239, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390 };
        private List<int> windurstWoodsOutOfBaitValues = new List<int> { };

        private List<int> saromugueChampaignFishHookedValues = new List<int> { 49, 204, 205, 208 };
        private List<int> saromugueChampaignPutFishingRodAwayValues = new List<int> { 146, 147, 159 };
        private List<int> saromugueChampaignOutOfBaitValues = new List<int> { };

        private List<int> westRonfaureFishHookedValues = new List<int> { 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 441, 442, 443, 444, 445, 446, 501 };
        private List<int> westRonfaurePutFishingRodAwayValues = new List<int> { 146, 147, 148, 149, 182, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389 };
        private List<int> westRonfaureOutOfBaitValues = new List<int> { };

        private List<int> fishHookedValues = null;
        private List<int> putFishingRodAwayValues = null;
        private List<int> outOfBaitValues = null;
        private List<int> fishMessageValues = new List<int> { 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93 };
        private List<int> craftMessageValues = new List<int> { 72, 73, 74, 75, 76, 80, 81, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94 };
        private bool logData = false;

        private string command = @"C:\Program Files\AutoHotkey\AutoHotkey.exe";
        private string craftArgs = @"Autohotkey\StartCraft.exe";
        private string craftArgs2 = @"Autohotkey\StartCraft2.exe";
        private string fishArgs = @"Autohotkey\ThrowFishingLine.exe";
        private string fishArgs2 = @"Autohotkey\FishingGame.exe";
        private string fishArgs3 = @"Autohotkey\EquipBait.exe";
        private string fishArgs4 = @"Autohotkey\AltMacro1Warp.exe";

        bool macroStarted = false;
        int macroDuration = 7000;
        private int count = 0;
        private Random randomNumberGenerator = new Random();
        Process process = null;
        public Form1()
        {
            InitializeComponent();
            control = textBox1;
            control2 = textBox2;

            devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count < 1)
            {
                textBox1.AppendText("No devices were found on this machine.");
                return;
            }

            textBox1.AppendText("Select your wireless or wired network adapter from the dropdown menu and click the 'Capture Packet' button...");
            textBox1.AppendText(Environment.NewLine);

            for (int i = 0; i < devices.Count; i++)
            {
                comboBox1.Items.Add(devices[i].Description);
            }

            //int cin = int.Parse(Interaction.InputBox("Please choose a device to capture on:", "Device ID", "Default", 0, 0));
            //string outputFile = Interaction.InputBox("Please enter an output file name: ", "Output File Name", "Default", 0, 0);
        }

        private void appendText(string s)
        {
           control.BeginInvoke((MethodInvoker)delegate ()
           {
               DateTime timestamp = DateTime.Now;
               textBox1.AppendText(timestamp.ToString() + " " + s);
               textBox1.AppendText(Environment.NewLine);
           });
        }

        private void appendText2(string s)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                DateTime timestamp = DateTime.Now;
                textBox2.AppendText(timestamp.ToString() + " " + s);
                textBox2.AppendText(Environment.NewLine);
            });
        }


        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private bool craft = false;
        private Queue<bool> completedCraftQueue = new Queue<bool>();
        private Queue<bool> startedCraftQueue = new Queue<bool>();
        private int startedCraftCooldown = 10000;
        private int craftIdleCooldown = 40000;
        private void button1_Click(object sender, EventArgs e)
        {
            if (craft == false)
            {
                if (comboBox3.SelectedIndex == -1)
                {
                    appendText("Select a zone, then click 'Craft'...");
                    appendText2("Select a zone, then click 'Craft'...");
                    return;
                }
                else if ((string)comboBox3.SelectedItem == "Mog House")
                {
                    completeCraftValues = mogHouseCompletedCraftValues;
                }
                else
                {
                    appendText("Please select a zone from the dropdown...");
                    appendText2("Please select a zone from the dropdown...");
                    return;
                }

                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                button3.Enabled = false;
                button6.Enabled = false;
                button5.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = true;

                appendText("Starting bot crafting...");
                appendText2("Starting bot crafting...");
                //appendText("Listening on " + device.Name + " " + device.Description);
                
                craft = true;

                var prc = Process.GetProcessesByName("wingsloader");
                if (prc.Length > 0)
                {
                    SetForegroundWindow(prc[0].MainWindowHandle);
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo(craftArgs);
                processStartInfo.Verb = "runas";
                processStartInfo.UseShellExecute = true;
                Process.Start(processStartInfo);
                startedCraftQueue.Enqueue(true);
                new Thread(() =>
                {
                    Thread.Sleep(startedCraftCooldown);
                    startedCraftQueue.Dequeue();
                }).Start();

                idleQueue.Enqueue(true);
                new Thread(() =>
                {
                    Thread.Sleep(craftIdleCooldown);
                    idleQueue.Dequeue();
                }).Start();
            }
            
            Thread thread = new Thread(() =>
            {
                while (craft == true)
                {
                    while (udpPayloadDataQueue.Count > 1)
                    {
                        if (craft == true)
                        {
                            if (craftMessageValues.Contains(udpPayloadDataQueue.Peek().Count))
                            {
                                if (udpPayloadDataQueue.Count > 1)
                                {
                                    appendText("Craft...? (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                    appendText2("Craft...? (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                    if (udpPayloadDataQueue.Count > 1)
                                    {
                                        udpPayloadDataQueue.Dequeue();
                                    }

                                    if (completeCraftValues.Contains(udpPayloadDataQueue.Peek().Count))
                                    {
                                        count = 0;
                                        if (startedCraftQueue.Count > 0)
                                        {
                                            appendText("Got a message to start craft, but I'm going to ignore it...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            appendText2("Got a message to start craft, but I'm going to ignore it...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            udpPayloadDataQueue.Dequeue();
                                        }
                                        else
                                        {
                                            appendText("Start crafting again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            appendText2("Start crafting again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");

                                            startedCraftQueue.Enqueue(true);
                                            new Thread(() =>
                                            {
                                                Thread.Sleep(startedCraftCooldown);
                                                startedCraftQueue.Dequeue();
                                            }).Start();

                                            idleQueue.Enqueue(true);
                                            new Thread(() =>
                                            {
                                                Thread.Sleep(craftIdleCooldown);
                                                idleQueue.Dequeue();
                                            }).Start();

                                            Thread.Sleep(1000);
                                            ProcessStartInfo processStartInfo = new ProcessStartInfo(craftArgs2);
                                            processStartInfo.UseShellExecute = true;
                                            processStartInfo.Verb = "runas";
                                            Process.Start(processStartInfo);
                                            udpPayloadDataQueue.Dequeue();
                                        }
                                    }
                                    else
                                    {
                                        appendText("No action...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                        appendText2("No action...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                    }
                                }
                            }
                            else
                            {
                                if (count > 50)
                                {
                                    if (idleQueue.Count > 0)
                                    {
                                        count = 0;
                                        appendText("I haven't received any crafting messages, but I'm on cooldown to do anything...(" + idleQueue.Count.ToString() + ")");
                                        appendText2("I haven't received any crafting messages, but I'm on cooldown to do anything...(" + idleQueue.Count.ToString() + ")");
                                    }
                                    else
                                    {
                                        count = 0;
                                        appendText("I've been idle for a while, so I'm going to start crafting again...(no code)");
                                        appendText2("I've been idle for a while, so I'm going to start crafting again...(no code)");

                                        startedCraftQueue.Enqueue(true);
                                        new Thread(() =>
                                        {
                                            Thread.Sleep(startedCraftCooldown);
                                            startedCraftQueue.Dequeue();
                                        }).Start();

                                        idleQueue.Enqueue(true);
                                        new Thread(() =>
                                        {
                                            Thread.Sleep(craftIdleCooldown);
                                            idleQueue.Dequeue();
                                        }).Start();

                                        ProcessStartInfo processStartInfo = new ProcessStartInfo(craftArgs);
                                        processStartInfo.UseShellExecute = true;
                                        processStartInfo.Verb = "runas";
                                        Process.Start(processStartInfo);
                                    }
                                }
                                else
                                {
                                    count++;
                                    appendText2(udpPayloadDataQueue.Peek().Count.ToString());
                                    udpPayloadDataQueue.Dequeue();
                                }
                            }
                        }
                    }
                }
            });

            thread.Start();
        }

        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            RawCapture rawPacket = e.GetPacket();
            if (rawPacket.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                EthernetPacket ethernetPacket = (EthernetPacket)packet;
                IPv4Packet ipv4Packet = ethernetPacket.Extract<IPv4Packet>();
                UdpPacket udpPacket = ipv4Packet.Extract<UdpPacket>();
                if (((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).DestinationPort == port)
                {
                    int length = ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).PayloadData.Length;

                    queue.Enqueue(length);
                    List<byte> list = new List<byte>();
                    for (int i = 0; i < udpPacket.PayloadData.Length; i++)
                    {
                        list.Add(udpPacket.PayloadData[i]);
                    }
                    udpPayloadDataQueue.Enqueue(list);
                    if (udpPayloadDataQueue.Count > 50)
                    {
                        udpPayloadDataQueue.Dequeue();
                    }
                    if (queue.Count > 50)
                    {
                        queue.Dequeue();
                    }
                }
                else if (((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).SourcePort == port)
                {
                    int length = ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).PayloadData.Length;
                    queue2.Enqueue(length);

                    List<byte> list = new List<byte>();
                    for (int i = 0; i < udpPacket.PayloadData.Length; i++)
                    {
                        list.Add(udpPacket.PayloadData[i]);
                    }
           
                    udpPayloadDataQueue.Enqueue(list);
                    if (udpPayloadDataQueue.Count > 50)
                    {
                        udpPayloadDataQueue.Dequeue();
                    }

                    if (queue2.Count > 50)
                    {
                        queue2.Dequeue();
                    }
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = false;
            button1.Enabled = true;
            appendText("Stopping bot crafting...");
            appendText2("Stopping bot crafting...");

            while (udpPayloadDataQueue.Count > 0)
            {
                udpPayloadDataQueue.Dequeue();
            }
            craft = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            while (queue.Count > 0)
            {
                appendText(queue.Peek().ToString());
                queue.Dequeue();
            }
        }

        bool fish = false;
        int equippedBaitMacroDuration = 22000;
        int startedFishAgainMacroDuration = 27500;
        int PlayedFishingGameMacroDuration = 3000;
        private int minigameCooldownAfterStartedFishing = 9000;
        int fishAgainAfterMiniGameCooldown = 3500;
        int idleCooldown = 35000;
        string lastAction = "";
        private Queue<bool> startedFishAgainQueue = new Queue<bool>();
        private Queue<bool> playedFishingGameQueue = new Queue<bool>();
        private Queue<bool> idleQueue = new Queue<bool>();
        private Queue<bool> equippedBaitQueue = new Queue<bool>();
        private void button5_Click(object sender, EventArgs e)
        {
            if (fish == false)
            {
                if (comboBox2.SelectedIndex == -1)
                {
                    appendText("Select a zone, then click 'Fish'...");
                    appendText2("Select a zone, then click 'Fish'...");
                    return;
                }
                else if ((string)comboBox2.SelectedItem == "Windurst Waters")
                {
                    fishHookedValues = windurstWatersFishHookedValues;
                    putFishingRodAwayValues = windurstWatersPutFishingRodAwayValues;
                    outOfBaitValues = windurstWatersOutOfBaitValues;
                    startedFishAgainMacroDuration = 27500;
                }
                else if ((string)comboBox2.SelectedItem == "Windurst Woods")
                {
                    fishHookedValues = windurstWoodsFishHookedValues;
                    putFishingRodAwayValues = windurstWoodsPutFishingRodAwayValues;
                    outOfBaitValues = windurstWoodsOutOfBaitValues;
                }
                else if ((string)comboBox2.SelectedItem == "Saromugue Champaign")
                {
                    fishHookedValues = saromugueChampaignFishHookedValues;
                    putFishingRodAwayValues = saromugueChampaignPutFishingRodAwayValues;
                    outOfBaitValues = saromugueChampaignOutOfBaitValues;
                }
                else if ((string)comboBox2.SelectedItem == "West Ronfaure")
                {
                    fishHookedValues = westRonfaureFishHookedValues;
                    putFishingRodAwayValues = westRonfaurePutFishingRodAwayValues;
                    outOfBaitValues = westRonfaureOutOfBaitValues;
                    startedFishAgainMacroDuration = 20000;
                    minigameCooldownAfterStartedFishing = 9000;
                }
                else 
                {
                    appendText("Please select a zone from the dropdown...");
                    appendText2("Please select a zone from the dropdown...");
                    return;
                }
                appendText("Starting fish bot...");
                appendText2("Starting fish bot...");

                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                button3.Enabled = false;
                button1.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = true;
                button2.Enabled = false;

                var prc = Process.GetProcessesByName("wingsloader");
                if (prc.Length > 0)
                {
                    SetForegroundWindow(prc[0].MainWindowHandle);
                }

                fish = true;
                count = 0;
                lastAction = "Fish";

                idleQueue.Enqueue(true);
                new Thread(() =>
                {
                    Thread.Sleep(idleCooldown);
                    idleQueue.Dequeue();
                }).Start();

                ProcessStartInfo processStartInfo = new ProcessStartInfo(fishArgs);
                processStartInfo.Verb = "runas";
                processStartInfo.UseShellExecute = true;
                process = Process.Start(processStartInfo);
                process.WaitForExit();
                process = null;
                appendText("Process finished: ThrowFishingLine.exe...");

                startedFishAgainQueue.Enqueue(true);
                new Thread(() =>
                {
                    Thread.Sleep(startedFishAgainMacroDuration);
                    startedFishAgainQueue.Dequeue();
                }).Start();

                playedFishingGameQueue.Enqueue(true);
                new Thread(() =>
                {
                    Thread.Sleep(minigameCooldownAfterStartedFishing);
                    playedFishingGameQueue.Dequeue();
                }).Start();

                while (udpPayloadDataQueue.Count > 0)
                {
                    udpPayloadDataQueue.Dequeue();
                }
            }

            Thread thread = new Thread(() =>
            {
                while (fish == true)
                {
                    while (udpPayloadDataQueue.Count > 1)
                    {
                        if (fish == true)
                        {
                            if (fishMessageValues.Contains(udpPayloadDataQueue.Peek().Count))
                            {
                                if (udpPayloadDataQueue.Count > 1)
                                {
                                    appendText("Start fishing or play the mini-game...? (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                    appendText2("Start fishing or play the mini-game... ?(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                    if (udpPayloadDataQueue.Count > 1)
                                    {
                                        udpPayloadDataQueue.Dequeue();
                                    }
                                    if (lastAction == "Mini-game")
                                    {
                                        if (putFishingRodAwayValues.Contains(udpPayloadDataQueue.Peek().Count))
                                        {
                                            count = 0;
                                            if (startedFishAgainQueue.Count == 0)
                                            {
                                                appendText("Fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                appendText2("Fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");

                                                lastAction = "Fish";

                                                idleQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(idleCooldown);
                                                    idleQueue.Dequeue();
                                                }).Start();

                                                //int t = 500;
                                                //Thread.Sleep(3000 + t * randomNumberGenerator.Next(0, 6));
                                                ProcessStartInfo processStartInfo = new ProcessStartInfo(fishArgs);
                                                processStartInfo.UseShellExecute = true;
                                                processStartInfo.Verb = "runas";
                                                process = Process.Start(processStartInfo);
                                                process.WaitForExit();
                                                process = null;
                                                appendText("Process finished: ThrowFishingLine.exe...");

                                                startedFishAgainQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(startedFishAgainMacroDuration);
                                                    startedFishAgainQueue.Dequeue();
                                                }).Start();

                                                playedFishingGameQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(minigameCooldownAfterStartedFishing);
                                                    playedFishingGameQueue.Dequeue();
                                                }).Start();

                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                            else
                                            {
                                                appendText("I'm on cooldown from starting fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                appendText2("I'm on cooldown from starting fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            appendText("No action (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            appendText2("No action (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                        }
                                    }
                                    else if (lastAction == "Fish") {
                                        if (fishHookedValues.Contains(udpPayloadDataQueue.Peek().Count))
                                        {
                                            count = 0;
                                            if (playedFishingGameQueue.Count == 0)
                                            {
                                                appendText("Start fishing mini-game...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                appendText2("Start fishing mini-game...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");

                                                lastAction = "Mini-game";

                                                idleQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(idleCooldown);
                                                    idleQueue.Dequeue();
                                                }).Start();

                                                ProcessStartInfo processStartInfo = new ProcessStartInfo(fishArgs2);
                                                processStartInfo.UseShellExecute = true;
                                                processStartInfo.Verb = "runas";
                                                process = Process.Start(processStartInfo);
                                                process.WaitForExit();
                                                process = null;
                                                appendText("Process finished: FishingGame.exe");

                                                playedFishingGameQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(PlayedFishingGameMacroDuration);
                                                    playedFishingGameQueue.Dequeue();
                                                }).Start();

                                                startedFishAgainQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(fishAgainAfterMiniGameCooldown);
                                                    startedFishAgainQueue.Dequeue();
                                                }).Start();

                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                            else
                                            {
                                                appendText("I'm on cooldown from playing the mini-game again...(" + startedFishAgainQueue.Count.ToString() + ")");
                                                appendText2("I'm on cooldown from playing the mini-game again...(" + startedFishAgainQueue.Count.ToString() + ")");

                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                        }
                                        else if (putFishingRodAwayValues.Contains(udpPayloadDataQueue.Peek().Count))
                                        {
                                            count = 0;
                                            if (startedFishAgainQueue.Count == 0)
                                            {
                                                appendText("Fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                appendText2("Fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");

                                                lastAction = "Fish";

                                                idleQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(idleCooldown);
                                                    idleQueue.Dequeue();
                                                }).Start();

                                                //int t = 500;
                                                //Thread.Sleep(3000 + t * randomNumberGenerator.Next(0, 6));
                                                ProcessStartInfo processStartInfo = new ProcessStartInfo(fishArgs);
                                                processStartInfo.UseShellExecute = true;
                                                processStartInfo.Verb = "runas";
                                                process = Process.Start(processStartInfo);
                                                process.WaitForExit();
                                                process = null;
                                                appendText("Process finished: ThrowFishingLine.exe...");

                                                startedFishAgainQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(startedFishAgainMacroDuration);
                                                    startedFishAgainQueue.Dequeue();
                                                }).Start();

                                                playedFishingGameQueue.Enqueue(true);
                                                new Thread(() =>
                                                {
                                                    Thread.Sleep(minigameCooldownAfterStartedFishing);
                                                    playedFishingGameQueue.Dequeue();
                                                }).Start();

                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                            else
                                            {
                                                appendText("I'm on cooldown from starting fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                appendText2("I'm on cooldown from starting fishing again...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                                if (udpPayloadDataQueue.Count > 0)
                                                {
                                                    udpPayloadDataQueue.Dequeue();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            appendText("No action (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            appendText2("No action (" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                        }
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                if (count > 50)
                                {
                                    if (idleQueue.Count > 0)
                                    {
                                        appendText("Got a message that I've been idle, but I'm going to ignore it...(" + idleQueue.Count + ")");
                                        appendText2("Got a message that I've been idle, but I'm going to ignore it...(" + idleQueue.Count + ")");
                                        count = 0;
                                    }
                                    else
                                    {
                                        appendText("I've been idle for a while, so I'm going to equip some bait and start fishing again...(no code)");
                                        appendText2("I've been idle for a while, so I'm going to equip some bait and start fishing again...(no code)");
                                        count = 0;
                                        lastAction = "Fish";
                                        ProcessStartInfo processStartInfo = null;

                                        if (equippedBaitQueue.Count == 0)
                                        {
                                            appendText("Equipping bait...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");
                                            appendText2("Equipping bait...(" + udpPayloadDataQueue.Peek().Count.ToString() + ")");

                                            equippedBaitQueue.Enqueue(true);
                                            new Thread(() =>
                                            {
                                                Thread.Sleep(equippedBaitMacroDuration);
                                                equippedBaitQueue.Dequeue();
                                            }).Start();

                                            idleQueue.Enqueue(true);
                                            new Thread(() =>
                                            {
                                                Thread.Sleep(idleCooldown);
                                                idleQueue.Dequeue();
                                            }).Start();

                                            processStartInfo = new ProcessStartInfo(fishArgs3);
                                            processStartInfo.UseShellExecute = true;
                                            processStartInfo.Verb = "runas";
                                            process = Process.Start(processStartInfo);
                                            process.WaitForExit();
                                            appendText("Process finished: EquipBait.exe...");
                                            //Thread.Sleep(10000);
                                        }

                                        idleQueue.Enqueue(true);
                                        new Thread(() =>
                                        {
                                            Thread.Sleep(idleCooldown);
                                            idleQueue.Dequeue();
                                        }).Start();

                                        processStartInfo = new ProcessStartInfo(fishArgs);
                                        processStartInfo.Verb = "runas";
                                        processStartInfo.UseShellExecute = true;
                                        process = Process.Start(processStartInfo);
                                        process.WaitForExit();
                                        appendText("Process Finished: ThrowFishingLine.exe...");

                                        playedFishingGameQueue.Enqueue(true);
                                        new Thread(() =>
                                        {
                                            Thread.Sleep(minigameCooldownAfterStartedFishing);
                                            playedFishingGameQueue.Dequeue();
                                        }).Start();

                                        startedFishAgainQueue.Enqueue(true);
                                        new Thread(() =>
                                        {
                                            Thread.Sleep(startedFishAgainMacroDuration);
                                            startedFishAgainQueue.Dequeue();
                                        }).Start();

                                    }
                                }
                                else
                                {
                                    count++;
                                    appendText2(udpPayloadDataQueue.Peek().Count.ToString());
                                    if (udpPayloadDataQueue.Count > 0)
                                    {
                                        udpPayloadDataQueue.Dequeue();
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            });

            thread.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            appendText("Stopping fish bot...");
            appendText2("Stopping fish bot...");
            fish = false;
            if (process == null)
            {

            }
            else
            {
                process.Kill();
                process.WaitForExit();

            }
            
            while (queue.Count > 0)
            {
                queue.Dequeue();
            }
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button6.Enabled = false;
            button3.Enabled = true;
            button1.Enabled = true;
            button5.Enabled = true;
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (device == null)
            {
                // do nothing
            }
            else if (device.Started == true)
            {
                device.StopCapture();
                if (device.Opened == true)
                {
                    device.Close();
                    device.Dispose();
                }
                else
                {
                    device.Dispose();
                }
            }
            else if (device.Opened == true)
            {
                device.Close();
                device.Dispose();
            }
            else
            {
                device.Dispose();
            }

            if (comboBox1.SelectedItem == null)
            {
                button1.Enabled = false;
                button5.Enabled = false;
                textBox1.AppendText("Nothing selected in dropdown menu, choose an option and then click the button!");
                textBox1.AppendText(Environment.NewLine);
            }
            else
            {
                cin = comboBox1.SelectedIndex;
                device = devices[cin];
                device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

                int readTimeoutInMilliseconds = 1000;
                device.Open(DeviceModes.Promiscuous | DeviceModes.DataTransferUdp | DeviceModes.NoCaptureLocal, readTimeoutInMilliseconds);

                string filter = "ip and udp";
                device.Filter = filter;
                
                appendText("Listening on " + device.Name + " " + device.Description);
                device.StartCapture();

                button1.Enabled = true;
                button5.Enabled = true;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
