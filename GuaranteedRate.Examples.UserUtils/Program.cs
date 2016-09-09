using EllieMae.Encompass.BusinessObjects.Users;
using EllieMae.Encompass.Client;
using GuaranteedRate.Sextant.EncompassUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuaranteedRate.Examples.UserUtils
{
    class Program
    {
        static string myUrl;
        static string myLogin;
        static string myPassword;

        // Purposely missing example for TryForceCompanyWidePasswordChange

        static void Main(string[] args)
        {
            if (args != null && args.Length == 3)
            {
                myUrl = args[0];
                myLogin = args[1];
                myPassword = args[2];
            }

            else
            {
                Console.WriteLine("Enter encompass server url.");
                myUrl = Console.ReadLine();

                Console.WriteLine("Enter login.");
                myLogin = Console.ReadLine();

                Console.WriteLine("Enter password.");
                myPassword = Console.ReadLine();
            }

            Session mySession = SessionUtils.GetEncompassSession(myUrl, myLogin, myPassword);

            try
            {
                Console.WriteLine("Session " + mySession.ID + " created on server " + mySession.ServerURI);

                // Display all users on this session's server
                PrintAllUsers(mySession);

                // Display all active users on this session's server
                PrintAllActiveUsers(mySession);

                // Display all users in a specific work folder on this session's server
                PrintAllUsersInMyPipelineWorkingFolder(mySession);

                // Print all active users as json
                PrintAllActiveUsersAsJson(mySession);

                Console.WriteLine("press any key to continue");
                Console.ReadKey();
            }

            finally
            {
                mySession.End();
            }
        }

        private static void PrintAllActiveUsersAsJson(Session mySession)
        {
            ICollection<User> users = mySession.GetAllActiveUsers();
            PrintUserList(users.ToJson(), "All active users as JSON", users.Count);
        }

        private static void PrintAllUsersInMyPipelineWorkingFolder(Session mySession)
        {
            PrintUserList(mySession.GetAllUsersInWorkFolder("My Pipeline"), "All users in \"My Pipeline\"");
        }

        private static void PrintAllActiveUsers(Session mySession)
        {
            PrintUserList(mySession.GetAllActiveUsers(), "All active users");
        }

        private static void PrintAllUsers(Session mySession)
        {
            PrintUserList(mySession.GetAllUsers(), "All users");
        }

        private static void PrintUserList(ICollection<User> users, string title)
        {
            Console.WriteLine(title + ": " + users.Count);
            Console.WriteLine("Press enter to print users.");
            Console.ReadLine();

            foreach (User user in users)
            {
                Console.WriteLine(user.FirstName + " " + user.LastName + ": " + user.ID);
            }

            Console.WriteLine();
            Console.ReadLine();
        }

        private static void PrintUserList(string usersJson, string title, int count)
        {
            Console.WriteLine(title + ": " + count + " as Json");
            Console.WriteLine("Press enter to print users.");
            Console.ReadLine();

            Console.WriteLine(usersJson);

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}