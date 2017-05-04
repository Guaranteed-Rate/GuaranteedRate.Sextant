using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using EllieMae.EMLite.Compiler;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.CustomFieldComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ops = new OptionsConfig();
            var res = Parser.Default.ParseArguments(args, ops);
            Console.WriteLine("SEXTANT FIELD CONFIG DUMP TOOL");
            Console.WriteLine("What does it do?");
            Console.WriteLine("Creates a series of text files containing field data.  These are ideal for checking into source control and monitoring environments for drift.");
         
            if (String.IsNullOrEmpty(ops.EncompassPassword))
            {
                ops.EncompassPassword = GetPassword();
            }

            Console.WriteLine("Connecting....");

            var primarySession = EncompassUtils.SessionUtils.GetEncompassSession(ops.EncompassPrimaryUrl, ops.EncompassUserName,
                ops.EncompassPassword);
            EncompassUtils.FieldUtils.session = primarySession;
            foreach (var f in EncompassUtils.FieldUtils.GetFieldsAndDescriptions())
            {
                var fd = primarySession.Loans.FieldDescriptors.StandardFields[f.Key];

                if (fd == null)
                {
                    fd = primarySession.Loans.FieldDescriptors.VirtualFields[f.Key];
                }


                if (fd == null)
                {
                    fd = primarySession.Loans.FieldDescriptors.CustomFields[f.Key];
                }

                var filePath = ops.OutputPath + fd.FieldID + ".json";
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                if (!ops.CustomOnly || fd.IsCustomField)
                {
                    using (var sw = System.IO.File.AppendText(filePath))
                    {
                        sw.Write(JsonConvert.SerializeObject(fd, Formatting.Indented));
                        sw.Flush();
                    }
                }
            }
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

         
    }
}
