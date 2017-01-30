namespace GuaranteedRate.Sextant.EncompassUtils
{
    public class EncompassInit
    {
        /// <summary>
        /// Init encompass SDK. Must be run before using any of Encompass APIs
        /// </summary>
        public static void InitEncompass()
        {
              new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();
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