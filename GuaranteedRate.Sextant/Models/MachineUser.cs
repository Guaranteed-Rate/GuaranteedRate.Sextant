using GuaranteedRate.Sextant.Loggers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GuaranteedRate.Sextant.Models
{
    /**
    This model captures identifying marks of a user's credentials and machine specifications outside of Encompass.
    This allows differentiation between Encompass user accounts and windows log in. (You can now validate if a person logs into an Encompass account with a non-typical or suspicion Windows credentials)
    This allows differentiation between Encompass user accounts and machine. (You can now validate if a person logs into an Encompass account with a non-typical or suspicion machine)
    This allows differentiation between Windows Login and Machine Name (Logging into a machine with non-typical or suspicion Windows credentials)
    */

    public class MachineUser
    {
        /**
        Captures the Login name from the users active directory
        */

        public static string WindowsLoginName
        {
            get
            {
                try
                {
                    return Environment.UserName;
                }
                catch (Exception ex)
                {
                    Loggly.Error("MachineUser", "Exception in MachineUser while getting Windows Login Name:" + ex);
                    return "";
                }
            }
        }

        /**
        Captures the RunAs name from the process. Example: RunAs Admin
        */

        public static string ProgramRunAsName
        {
            get
            {
                try
                {
                    return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
                catch (Exception ex)
                {
                    Loggly.Error("MachineUser", "Exception in MachineUser while getting Program Run As Name:" + ex);
                    return "";
                }
            }
        }

        /**
        Captures the machine's local IP
        */

        public static string MachineIP
        {
            get
            {
                try
                {
                    IPHostEntry host;
                    string localIP = "";
                    host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localIP = ip.ToString();
                            break;
                        }
                    }
                    return localIP;
                }
                catch (Exception ex)
                {
                    Loggly.Error("MachineUser", "Exception in MachineUser while getting Machine IP" + ex);
                    return "";
                }
            }
        }

        /**
        Captures the machine's name
        */

        public static string ComputerName
        {
            get
            {
                try
                {
                    return Environment.MachineName;
                }
                catch (Exception ex)
                {
                    Loggly.Error("MachineUser", "Exception in MachineUser while getting Computer Name:" + ex);
                    return "";
                }
            }
        }

        public static IDictionary<string, string> GetMachineUserIdentification()
        {
            MachineUser myMachineUser = new MachineUser();

            IDictionary<string, string> result = new Dictionary<string, string>();
            result.Add("WindowsLoginName", WindowsLoginName);
            result.Add("ProgramRunAsName", ProgramRunAsName);
            result.Add("MachineIP", MachineIP);
            result.Add("ComputerName", ComputerName);

            return result;
        }
    }
}