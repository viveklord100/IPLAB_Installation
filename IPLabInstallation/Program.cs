using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;

namespace IPLabInstallation
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Read the System config file
            string[] sysconfigInput = File.ReadAllLines(@"F:\IPLabInstallation\SQUIREIP.txt");
            Dictionary<string, string> sysconfig = new Dictionary<string, string>();
            foreach (string line in sysconfigInput)
            {
                string[] linesp = line.Split(':');
                sysconfig.Add(linesp[0].Trim().ToUpper(), linesp[1].Trim().ToUpper());
                Console.WriteLine(linesp[0].Trim().ToUpper()+":"+sysconfig[linesp[0].Trim().ToUpper()]);
            }

            //Sets Autologon
            RegistryKey defaultPassword = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true);
            defaultPassword.SetValue("DefaultPassWord", "t2pmbHtg2.");

            //Sets PDC number used for installation
            //RegistryKey pdcversion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\PMS\Fusion\CInfraRegistrar_Installation\SystemBOM",true);
            //pdcversion.SetValue("PDCVersion", sysconfig["PDC"]);

            //Sets the Environment variable to pull IP software
            if (sysconfig["PROJECT"] == "TIGER")
            {
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_Main_IPSW_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_Main_IPSW_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("INFRASOURCE", "Allura_Main_Infra_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("INFRASOURCE", "Allura_Main_Infra_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("AUIPROJECT", "Allura_Main_AUI_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("AUIPROJECT", "Allura_Main_AUI_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Process); 
            }
            if (sysconfig["PROJECT"] == "TIGER+")
            {
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_Main_IPSW_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_Main_IPSW_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("INFRASOURCE", "Allura_Main_Infra_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("INFRASOURCE", "Allura_Main_Infra_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("AUIPROJECT", "Allura_Main_AUI_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("AUIPROJECT", "Allura_Main_AUI_PreInt", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Process);
            }
            if (sysconfig["PROJECT"] == "JAGUAR")
            {
                Environment.SetEnvironmentVariable("IPSWPROJECT", "ICC-IPSW-uIPSW", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("IPSWPROJECT", "ICC-IPSW-uIPSW", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("INFRASOURCE", "ICC-Infra-uInfra", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("INFRASOURCE", "ICC-Infra-uInfra", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("AUIPROJECT", "ICC-AUI-uAUI", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("AUIPROJECT", "ICC-AUI-uAUI", EnvironmentVariableTarget.Process);

                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("NAS", "161.85.22.42", EnvironmentVariableTarget.Process);
            }
            else if (sysconfig["PROJECT"]=="LION")
            {
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_8x30.10_IPSW_PreInt", EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("IPSWPROJECT", "Allura_8x30.10_IPSW_PreInt", EnvironmentVariableTarget.Process);                
            }
            
            //To copy all the required files from NAS
            if(sysconfig["FV1"]=="YES")
            {
                runbatch(@"F:\IPLabInstallation\copyfromNAS_PDC.cmd", sysconfig["SYSTEMNAME"]+" "+ sysconfig["PROJECT"]+" FV1");
            }

            if (sysconfig["FV2"] == "YES" && sysconfig["TSM"] == "YES")
            {
                runbatch(@"F:\IPLabInstallation\copyfromNAS_PDC.cmd", sysconfig["SYSTEMNAME"] + " " + sysconfig["PROJECT"] + " FV2 TSM");
            }
            else if(sysconfig["FV2"] == "YES")
            {
                runbatch(@"F:\IPLabInstallation\copyfromNAS_PDC.cmd", sysconfig["SYSTEMNAME"] + " " + sysconfig["PROJECT"] + " FV2");
            }
            else
            {
                runbatch(@"F:\IPLabInstallation\copyfromNAS_PDC.cmd", sysconfig["SYSTEMNAME"] + " " + sysconfig["PROJECT"]);
            }

            //Executes only for Win10
            if(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue("ProductName").ToString().Contains("Windows 10"))
            {
                runbatch("F:/InstallScripts/ChangeWin10SettingsForDevelopment.cmd", "");
            }            

            runbatch(@"F:\IPLabInstallation\Sch_IPLabInstallationPart2.cmd", "");
        }

        public static String runbatch(String FilePath, String Arguments)
        {
            System.Diagnostics.Process pr = new System.Diagnostics.Process();
            pr.StartInfo.UseShellExecute = false;
            pr.StartInfo.RedirectStandardOutput = true;
            pr.StartInfo.FileName = FilePath;
            pr.StartInfo.Arguments = Arguments;
            pr.Start();
            String output = pr.StandardOutput.ReadToEnd();
            pr.WaitForExit();
            Console.WriteLine(output);
            return output;
        }
    }
}
