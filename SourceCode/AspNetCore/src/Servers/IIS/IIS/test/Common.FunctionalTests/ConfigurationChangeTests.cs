// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IIS.FunctionalTests.Utilities;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Server.IntegrationTesting.IIS;
using Microsoft.AspNetCore.Testing;
using Microsoft.AspNetCore.Testing.xunit;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.AspNetCore.Server.IISIntegration.FunctionalTests
{
    [Collection(PublishedSitesCollection.Name)]
    public class ConfigurationChangeTests : IISFunctionalTestBase
    {
        public ConfigurationChangeTests(PublishedSitesFixture fixture) : base(fixture)
        {
        }

        [ConditionalFact]
        public async Task ConfigurationChangeStopsInProcess()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.InProcess);

            var deploymentResult = await DeployAsync(deploymentParameters);

            await deploymentResult.AssertStarts();

            // Just "touching" web.config should be enough
            deploymentResult.ModifyWebConfig(element => { });

            await deploymentResult.AssertRecycledAsync();
        }

        [ConditionalFact]
        public async Task ConfigurationChangeForcesChildProcessRestart()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.OutOfProcess);

            var deploymentResult = await DeployAsync(deploymentParameters);

            var processBefore = await deploymentResult.HttpClient.GetStringAsync("/ProcessId");

            // Just "touching" web.config should be enough
            deploymentResult.ModifyWebConfig(element => { });

            // Have to retry here to allow ANCM to receive notification and react to it
            // Verify that worker process gets restarted with new process id
            await deploymentResult.HttpClient.RetryRequestAsync("/ProcessId", async r => await r.Content.ReadAsStringAsync() != processBefore);
        }

        [ConditionalFact]
        public async Task OutOfProcessToInProcessHostingModelSwitchWorks()
        {
            var deploymentParameters = Fixture.GetBaseDeploymentParameters(HostingModel.OutOfProcess);

            var deploymentResult = await DeployAsync(deploymentParameters);

            await deploymentResult.AssertStarts();

            deploymentResult.ModifyWebConfig(element => element
                .Descendants("system.webServer")
                .Single()
                .GetOrAdd("aspNetCore")
                .SetAttributeValue("hostingModel", "inprocess"));

            // Have to retry here to allow ANCM to receive notification and react to it
            // Verify that inprocess application was created and tried to start
            await deploymentResult.HttpClient.RetryRequestAsync("/HelloWorld", r => r.StatusCode == HttpStatusCode.InternalServerError);

            StopServer();
            EventLogHelpers.VerifyEventLogEvent(deploymentResult, EventLogHelpers.CouldNotFindHandler(), Logger);
        }

        [ConditionalTheory]
        [InlineData(HostingModel.InProcess)]
        [InlineData(HostingModel.OutOfProcess)]
        [Flaky("https://github.com/aspnet/AspNetCore-Internal/issues/1794", FlakyOn.All)]
        public async Task ConfigurationTouchedStress(HostingModel hostingModel)
        {
            var deploymentResult = await DeployAsync(Fixture.GetBaseDeploymentParameters(hostingModel));

            await deploymentResult.AssertStarts();
            var load = Helpers.StressLoad(deploymentResult.HttpClient, "/HelloWorld", response =>
            {
                var statusCode = (int)response.StatusCode;
                Assert.True(statusCode == 200 || statusCode == 503, "Status code was " + statusCode);
            });

            for (int i = 0; i < 100; i++)
            {
                // ModifyWebConfig might fail if web.config is being read by IIS
                RetryHelper.RetryOperation(
                    () => deploymentResult.ModifyWebConfig(element => { }),
                    e => Logger.LogError($"Failed to touch web.config : {e.Message}"),
                    retryCount: 3,
                    retryDelayMilliseconds: RetryDelay.Milliseconds);
            }

            try
            {
                await load;
            }
            catch (HttpRequestException ex) when (ex.InnerException is IOException | ex.InnerException is SocketException)
            {
                // IOException in InProcess is fine, just means process stopped
                if (hostingModel != HostingModel.InProcess)
                {
                    throw;
                }
            }
        }
    }
}
