using System;
using System.IO;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace IPLabInstallationIPFV_OS
{
    class Program
    {
        private static string _osInstallationReq="N";

        static void Main()
        {
            string IPPC_FR_IP = "172.22.3.136";
            string IPPC_LT_IP = "172.22.3.40";
            string FVPC_IP = "172.22.1.240";

            string requestedIPos = File.ReadAllText("C:/Program Files (x86)/PMS/Headless/IPOS/$OEM$/$1/version.txt").Trim();
            Console.WriteLine("IPOS available on Host:"+requestedIPos);

            if (Pingres(IPPC_FR_IP) =="Success")
            {
                string ipfrOs = Runbatch("C:/Test/CAConsole.exe /getosversion "+IPPC_FR_IP);
                string currentFripos = "";
                
                foreach (var caline in ipfrOs.Split(new string[] {Environment.NewLine},StringSplitOptions.RemoveEmptyEntries))
                {
                    if(caline.Contains("version=")) currentFripos=caline.Substring(caline.IndexOf("version=")).Split('=')[1].Trim();
                }
                Console.WriteLine("Current OS version of Frontal IPPC:" + currentFripos);
                ReinstallOs(currentFripos, requestedIPos, IPPC_FR_IP);
            }else
            {
                Console.WriteLine("Error: IPPC_Frontal is NOT UP");
            }

            if (Pingres(IPPC_LT_IP) == "Success")
            {
                string ipltOs = Runbatch("C:/test/CAConsole.exe /getosversion " + IPPC_LT_IP);
                string currentLtipos = "";

                foreach (var caline in ipltOs.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (caline.Contains("version=")) currentLtipos = caline.Substring(caline.IndexOf("version=")).Split('=')[1].Trim();
                }
                Console.WriteLine("Current OS version of Lateral IPPC:" + currentLtipos);
                ReinstallOs(currentLtipos, requestedIPos, IPPC_LT_IP);
            }

            if (Pingres(FVPC_IP) == "Success")
            {
                string requestedFVos;
                if (Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ProductName").ToString().Contains("Windows 10"))
                {
                    requestedFVos = File.ReadAllText("C:/Program Files (x86)/PMS/Headless/FVOS/$OEM$/$1/version.txt").Trim();
                    Console.WriteLine("FVOS available on Host:"+requestedFVos);
                }
                else
                {
                    requestedFVos = File.ReadAllText("C:/Program Files (x86)/PMS/Headless/FlexVisionOS_W7_/$OEM$/$1/version.txt").Trim();
                    Console.WriteLine("FVOS available on Host:"+requestedFVos);
                }

                string fvOs = Runbatch("C:/test/CAConsole.exe /getosversion " + FVPC_IP);
                string currentFVos = "";
                foreach (var caline in fvOs.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (caline.Contains("version=")) currentFVos = caline.Substring(caline.IndexOf("version=")).Split('=')[1].Trim();
                }
                Console.WriteLine("Current OS version of FV PC:" + currentFVos);
                ReinstallOs(currentFVos, requestedFVos, FVPC_IP);
            }

            if (_osInstallationReq == "Y")
            {
                Console.WriteLine("OS Installation wait time start"+DateTime.Now);
                System.Threading.Thread.Sleep(2100000);
                Console.WriteLine("IPPC Frontal Status: "+Pingres(IPPC_FR_IP));
                Console.WriteLine("IPPC Lateral Status: " + Pingres(IPPC_LT_IP));
                Console.WriteLine("FV PC Status: " + Pingres(FVPC_IP));
            }
                        
        }

        static String Pingres(String ipaddress)
        {
            Ping pg = new Ping();
            PingReply pr = pg.Send(ipaddress);
            return pr.Status.ToString();
        }

        public static string Runbatch(string command)
        {
            System.Diagnostics.ProcessStartInfo prInfo = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/C "+command);
            prInfo.UseShellExecute = false;
            prInfo.RedirectStandardOutput = true;            
            System.Diagnostics.Process pr = new System.Diagnostics.Process();
            pr.StartInfo = prInfo;
            pr.Start();
            string output = pr.StandardOutput.ReadToEnd();
            pr.WaitForExit();
            return output;
        }

        static void ReinstallOs(string installedOs,string reqOs,string ipAddress)
        {
            if (installedOs != reqOs)
            {
                Console.WriteLine(ipAddress+" need to be installed with " + reqOs);
                Runbatch("C:/Test/CAConsole.exe /reinstallOS "+ipAddress);
                _osInstallationReq = "Y";
            }
            else
            {
                Console.WriteLine(ipAddress + " is already with " + reqOs+"OS version");
            }
        }
    }
}
