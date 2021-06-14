using EllieMae.Encompass.Runtime;
using System;

namespace GuaranteedRate.Sextant.Integration.Tests
{
	public class EncompassSDKBaseTest
    {
        static EncompassSDKBaseTest()
        {
            try
            {
                // For some reason this is being called multiple times when running all integration tests
                new RuntimeServices().Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
