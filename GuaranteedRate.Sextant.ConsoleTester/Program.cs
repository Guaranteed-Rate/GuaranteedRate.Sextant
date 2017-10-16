using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.EncompassUtils;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new JsonEncompassConfig();
            config.Init(System.IO.File.ReadAllText("../../ConsoleTester.json"));
            var server = config.GetValue<string>("encompass-url", String.Empty);
            var userid = config.GetValue<string>("encompass-userid", String.Empty);
            var password = config.GetValue<string>("encompass-password", String.Empty);
            var loanNumbers = config.GetValue<List<string>>("test-loans", new List<string>());

            if (server == String.Empty)
            {
                Console.Write("Please enter the url of your Encompass instance and press enter.");
                server = Console.ReadLine();
            }

            if (userid == String.Empty)
            {
                Console.Write("Please enter your Encompass userid and press enter.");
                userid = Console.ReadLine();
            }

            if (password == String.Empty)
            {
                Console.Write("Please enter your Encompass password and press enter.");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    password += key.KeyChar;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Logging into Encompass");
            using (var sess = SessionUtils.GetEncompassSession(server, userid, password))
            {
                if (!loanNumbers.Any())
                {
                    Console.WriteLine("Please enter a loan number to test and press enter.");
                    loanNumbers.Add(Console.ReadLine());
                }

                var sw = new Stopwatch();
                foreach (var ln in loanNumbers)
                {
                    Console.WriteLine($"opening loan {ln}" );
                    var loan =  SessionUtils.OpenLoanFromLoanNumber(sess,ln);
                    Console.WriteLine("Extracting");
                    FieldUtils.session=sess;
                    sw.Start();
                    var serialized = JsonConvert.SerializeObject(LoanDataUtils.ExtractEverything(loan));
                    System.IO.File.WriteAllText(String.Format(@"c:\junk\{0}.json", ln), serialized);
                    sw.Stop();
                    Console.WriteLine($"Extracted loan {ln} in {sw.ElapsedMilliseconds/1000} seconds.");
                    sw.Reset();
                    loan.Close();
                }
Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
