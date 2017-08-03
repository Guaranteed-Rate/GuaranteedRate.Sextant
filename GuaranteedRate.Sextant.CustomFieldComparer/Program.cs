using System;
using CommandLine;
using EllieMae.Encompass.BusinessObjects.Loans;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.CustomFieldComparer
{
    /// <summary>
    /// This tool extracts details of custom fields from Encompass to flat files.
    /// 
    /// If you have a single Encompass environment, this is a useful tool for 
    /// tracking field changes over time.
    /// 
    /// If you are supporting *multiple* environments easily compared flat files
    /// are extremely useful for tracking differences, change management, etc
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var ops = new OptionsConfig();
            var res = Parser.Default.ParseArguments(args, ops);

            if (!String.IsNullOrEmpty(ops.JsonConfig))
            {
                ops = JsonConvert.DeserializeObject<OptionsConfig>(System.IO.File.ReadAllText(ops.JsonConfig));
            }

            Console.WriteLine("SEXTANT FIELD CONFIG DUMP TOOL");
            Console.WriteLine("What does it do?");
            Console.WriteLine("Creates a series of text files containing field data.  These are ideal for checking into source control and monitoring environments for drift.");
           
            if (String.IsNullOrEmpty(ops.EncompassPassword))
            {
                ops.EncompassPassword = GetPassword();
            }
            
            Console.WriteLine($"Connecting to {ops.EncompassPrimaryUrl}...");

            var primarySession = EncompassUtils.SessionUtils.GetEncompassSession(ops.EncompassPrimaryUrl, ops.EncompassUserName,
                ops.EncompassPassword);

            var fieldExtractor = new FieldExtractor(primarySession, ops.OutputPath, ops.CustomOnly, ops.Environment);
            fieldExtractor.Extract();
            
            Console.WriteLine("Done!");
        }


        private static string GetPassword()
        {
            Console.WriteLine("Please enter your passsword.");
            var password = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }
            return password;
        }

    }

    public class OptionsConfig
    {
        [Option('e', "encompassUrl", HelpText = "Url of the server.")]
        public string EncompassPrimaryUrl { get; set; }

        [Option('u', "userName", HelpText = "UserName.")]
        public string EncompassUserName { get; set; }

        [Option('p', "Password", HelpText = "Password.")]
        public string EncompassPassword { get; set; }

        [Option('o', "OutputPath", HelpText = "Output Path.")]
        public string OutputPath { get; set; }

        [Option('c', "CustomOnly", HelpText = "Custom Only.")]
        public bool CustomOnly { get; set; }

        [Option('j', "JsonConfig", HelpText = "Path to the Json config to use instead of arguments.")]
        public string JsonConfig { get; set; }

        [Option('e', "Environment", HelpText = "Encompass Environment")]
        public string Environment { get; set; }
    }
}
