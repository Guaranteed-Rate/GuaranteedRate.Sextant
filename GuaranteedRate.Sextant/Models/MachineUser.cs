using EllieMae.Encompass.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Models
{
    public class MachineUser
    {
        public static string WindowsLoginName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public static string ProgramRunAsName
        {
            get
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }

        public static string MachineIP
        {
            get
            {
                string ips = "";
                foreach (IPAddress ipAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    ips += ipAddress + ",";
                }
                ips = ips.Substring(0, ips.Length - 1);
                return ips;
            }
        }

        public static string ComputerName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public static IDictionary<string, object> GetMachineUserIdentification()
        {
            MachineUser myMachineUser = new MachineUser();

            IDictionary<string, object> result = new Dictionary<string, object>();
            result.Add("WindowsLoginName", WindowsLoginName);
            result.Add("ProgramRunAsName", ProgramRunAsName);
            result.Add("MachineIP", MachineIP);
            result.Add("ComputerName", ComputerName);

            return result;
        }
    }
}
