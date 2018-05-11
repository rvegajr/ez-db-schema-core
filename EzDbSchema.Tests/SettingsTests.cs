using System;
using System.Collections.Generic;
using EzDbSchema.Core;
using Xunit;

namespace EzDbSchema.Tests
{
	public class SettingsTests
	{
		[Fact]
		public void AppSettingsTests()
		{
			Assert.True(AppSettings.Instance.ConnectionString.Length > 0, "Nothing returned in Connection string");
		}
	}
}