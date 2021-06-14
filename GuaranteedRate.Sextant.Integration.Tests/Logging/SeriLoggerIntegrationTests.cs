using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GuaranteedRate.Sextant.Integration.Tests.Logging
{
	[TestFixture]
	public class SeriLoggerIntegrationTests: EncompassSDKBaseTest
	{
		private IEncompassConfig _encompassConfig;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_encompassConfig = new IntegrationEncompassConfig();
			Console.WriteLine("Rawr");
			var _ = SeriLogger.Setup(_encompassConfig);
		}

		[Test]
		public void CanWarn()
		{
			var obj = new Dictionary<string, string>() { { "dog", "woof" }, { "cat", "meow" } };
			var cLogger = SeriLogger.Logger.ForContext<SeriLoggerIntegrationTests>();

			Assert.True(cLogger.IsEnabled(Serilog.Events.LogEventLevel.Fatal));
			cLogger.Fatal(new Exception("This is a test Exception from the Sextant Integration Tests."), "Test my {@Dictionary}", obj);
		}

		[Test]
		public void CanLogAggregate()
		{
			AggregateException deep = new AggregateException(new[] {
				new Exception("Some exception deep"),
				new Exception("Some exception deeper")
			});

			AggregateException ex = new AggregateException(new[] { 
				new Exception("Some exception at top"),
				new ArgumentException("You missed an argument"),
				deep
			});

			var cLogger = SeriLogger.Logger.ForContext<SeriLoggerIntegrationTests>();

			Assert.True(cLogger.IsEnabled(Serilog.Events.LogEventLevel.Fatal));
			cLogger.Fatal(ex, "We succesfully log Aggregate Exceptions");
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Serilog.Log.CloseAndFlush();
		}
	}
}
