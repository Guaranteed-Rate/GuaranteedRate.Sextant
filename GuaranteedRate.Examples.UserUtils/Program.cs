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
            Console.WriteLine("Enter encompass server url.");
            myUrl = Console.ReadLine();

            Console.WriteLine("Enter login.");
            myLogin = Console.ReadLine();

            Console.WriteLine("Enter password.");
            myPassword = Console.ReadLine();

            Session mySession = SessionUtils.GetEncompassSession(myUrl, myLogin, myPassword);

            try
            {
                Console.WriteLine("Session " + mySession.ID + " created on server " + mySession.ServerURI);

                // Display all users on this session's server
                List<User> allUsersOnThisServer = mySession.GetAllUsers();

                Console.WriteLine("All Users: " + allUsersOnThisServer.Count);
                Console.WriteLine("Press enter to print users.");
                Console.ReadLine();

                foreach (User user in allUsersOnThisServer)
                {
                    Console.WriteLine(user.FirstName + " " + user.LastName + ": " + user.ID);
                }

                Console.WriteLine();
                Console.ReadLine();

                // Display all active users on this session's server
                List<User> allActiveUsersOnThisServer = mySession.GetAllActiveUsers();

                Console.WriteLine("All Active Users: " + allActiveUsersOnThisServer.Count);
                Console.WriteLine("Press enter to print users.");
                Console.ReadLine();

                foreach (User user in allActiveUsersOnThisServer)
                {
                    Console.WriteLine(user.FirstName + " " + user.LastName + ": " + user.ID);
                }

                Console.WriteLine();
                Console.ReadLine();

                // Display all users in a specific work folder on this session's server
                List<User> allUsersOnThisServerInSpecificWorkFolder = mySession.GetAllUsersInWorkFolder("My Pipeline");

                Console.WriteLine("All Users in My Pipeline folder: " + allUsersOnThisServerInSpecificWorkFolder.Count);
                Console.WriteLine("Press enter to print users.");
                Console.ReadLine();

                foreach (User user in allUsersOnThisServerInSpecificWorkFolder)
                {
                    Console.WriteLine(user.FirstName + " " + user.LastName + ": " + user.ID);
                }

                Console.WriteLine();
                Console.ReadLine();
            }

            finally
            {
                mySession.End();
            }
        }
    }
}
