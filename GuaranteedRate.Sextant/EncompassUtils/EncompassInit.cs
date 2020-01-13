using System;
using System.Linq;

namespace GuaranteedRate.Sextant.EncompassUtils
{
    public class EncompassInit
    {
        /// <summary>
        /// Init encompass SDK. Must be run before using any of Encompass APIs
        /// </summary>
        public static void InitEncompass()
        {
            // Called through reflection so as not to require a dll reference to the EllieMae.Encompass.Runtime dll
            // which should only be required for standalone applications, not libraries or plugins and forms code.
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "EllieMae.Encompass.Runtime");
            if (assembly == null)
            {
                throw new InvalidOperationException("Could not find assembly EllieMae.Encompass.Runtime in the current app domain");
            }
            var type = assembly.GetType("EllieMae.Encompass.Runtime.RuntimeServices");
            var runtimeServices = Activator.CreateInstance(type);
            type.GetMethod("Initialize").Invoke(runtimeServices, null);
        }

        /// <summary>
        /// Validate if we can open the session
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>True, if could open Encompass Session</returns>
        public static bool Validate(string serverUrl, string userName, string password)
        {
            var session = SessionUtils.GetEncompassSession(serverUrl, userName, password);

            if (session == null || !session.IsConnected) return false;

            session.End();
            return true;
        }
    }
}