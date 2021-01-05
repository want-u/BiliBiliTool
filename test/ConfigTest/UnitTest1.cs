using System;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ray.BiliBiliTool.Config;
using Ray.BiliBiliTool.Config.Options;
using Ray.BiliBiliTool.Console;
using Xunit;
using Ray.BiliBiliTool.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace ConfigTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Program.PreWorks(new string[] { "-closeConsoleWhenEnd=1" });

            Debug.WriteLine(Global.ConfigurationRoot["CloseConsoleWhenEnd"]);

            string s = Global.ConfigurationRoot["BiliBiliCookie:UserId"];
            Debug.WriteLine(s);

            string logLevel = Global.ConfigurationRoot["Serilog:WriteTo:0:Args:restrictedToMinimumLevel"];
            Debug.WriteLine(logLevel);

            var options = Global.ServiceProviderRoot.GetRequiredService<IOptionsMonitor<BiliBiliCookieOptions>>();

            Debug.WriteLine(JsonSerializer.Serialize(options.CurrentValue, new JsonSerializerOptions { WriteIndented = true }));
            Assert.True(!string.IsNullOrWhiteSpace(options.CurrentValue.UserId));
        }

        /// <summary>
        /// ���Ի�������Key�ķָ���
        /// </summary>
        [Fact]
        public void TestEnvKeyDelimiter()
        {
            Environment.SetEnvironmentVariable("Ray_BiliBiliCookie__UserId", "123");
            Program.PreWorks(null);

            string result = Global.ConfigurationRoot["BiliBiliCookie:UserId"];

            Assert.Equal("123", result);
        }

        [Fact]
        public void LoadPrefixConfigByEnvWithNoError()
        {
            Environment.SetEnvironmentVariable("Ray_BiliBiliCookie", "UserId: 123");
            Program.PreWorks(new string[] { "-closeConsoleWhenEnd=1" });

            string result = Global.ConfigurationRoot["BiliBiliCookie"];

            Assert.Equal("UserId: 123", result);
            Environment.SetEnvironmentVariable("Ray_BiliBiliCookie", null);
        }

        [Fact]
        public void LoadPrefixConfigByEnvWhenValueIsNullWithNoError2()
        {
            Environment.SetEnvironmentVariable("Ray_BiliBiliCookie", null);
            Program.PreWorks(new string[] { "-closeConsoleWhenEnd=1" });

            string result = Global.ConfigurationRoot["BiliBiliCookie"];

            Assert.Null(result);
        }

        [Fact]
        public void CoverConfigByEnvWithNoError()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
            Program.PreWorks(new string[] { "-closeConsoleWhenEnd=1" });

            string result = Global.ConfigurationRoot["IsPrd"];

            Assert.Equal("True", result);
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        }

        /// <summary>
        /// Ϊ�����ֶ���ֵ
        /// </summary>
        [Fact]
        public void TestSetConfiguration()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Program.PreWorks(new string[] { });

            var options = Global.ServiceProviderRoot.GetRequiredService<IOptionsMonitor<BiliBiliCookieOptions>>();
            Debug.WriteLine(options.CurrentValue.ToJson());

            //�ֶ���ֵ
            //RayConfiguration.Root["BiliBiliCookie:UserId"] = "123456";
            options.CurrentValue.SetUserId("123456");

            Debug.WriteLine($"��Configuration��ȡ��{Global.ConfigurationRoot["BiliBiliCookie:UserId"]}");

            Debug.WriteLine($"����options��ȡ��{options.CurrentValue.ToJson()}");

            var optionsNew = Global.ServiceProviderRoot.GetRequiredService<IOptionsMonitor<BiliBiliCookieOptions>>();
            Debug.WriteLine($"����options��ȡ��{optionsNew.CurrentValue.ToJson()}");
        }

        /// <summary>
        /// Ϊ�����ֶ���ֵ
        /// </summary>
        [Fact]
        public void TestHostDefaults()
        {
            Debug.WriteLine(Environment.GetEnvironmentVariable(HostDefaults.EnvironmentKey));
        }
    }
}
