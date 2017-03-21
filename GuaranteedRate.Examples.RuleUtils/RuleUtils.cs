using System;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Server;
using Session = EllieMae.EMLite.RemotingServices.Session;

namespace GuaranteedRate.Examples.RuleUtils
{
    class RuleUtils
    {
        static int Main(string[] args)
        {
            if (args != null && args.Length == 3)
            {
                //This is not the same session obj as the SDK
                GuaranteedRate.Util.Rules.RuleUtils.ExtractRule(args[0], args[1], args[2]);

                return 1;
            }
            else
            {
                Console.WriteLine("Usage [output file name] [Encompass url] [Encompass User] [Encompass Password]");
                return 0;
            }
        }
    }
}
