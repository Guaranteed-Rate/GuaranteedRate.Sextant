using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.Tests.Logging
{
	[TestFixture]
	public class SeriLoggerUnitTests
	{
		private IEncompassConfig _encompassConfig;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_encompassConfig = new IntegrationEncompassConfig();
			SeriLogger.Setup(_encompassConfig);
		}

		[TestCase]
		public void ParallelSafeTester()
		{
			var bag = new ConcurrentBag<Serilog.ILogger>();
			Parallel.For(0, 1000, (_) => bag.Add(SeriLogger.Logger));

			// None should be null
			foreach(var item in bag)
			{
				Assert.IsNotNull(item);
			}

			var first = bag.First();

			// All of them should be the same reference
			Assert.True(bag.All(x => Object.ReferenceEquals(x, first)));
		}
	}
}
