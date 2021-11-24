using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using GitHub.Runner.Common.Util;
using System.Threading.Channels;
using GitHub.Runner.Sdk;
using System.Linq;

namespace GitHub.Runner.Common.Tests
{
    public sealed class PackagesTrimL0
    {

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public async Task RunnerLayoutParts_1()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Tracing trace = hc.GetTrace();
                var runnerCoreAssertsFile = Path.Combine(TestUtil.GetSrcPath(), @"Misc/runnercoreassets");
                var runnerDotnetRuntimeFile = Path.Combine(TestUtil.GetSrcPath(), @"Misc/runnerdotnetruntimeasserts");
                string layoutBin = Path.Combine(TestUtil.GetSrcPath(), @"../_layout/bin");
                var newFiles = new List<string>();
                if (Directory.Exists(layoutBin))
                {
                    var coreAssets = await File.ReadAllLinesAsync(runnerCoreAssertsFile);
                    var runtimeAssets = await File.ReadAllLinesAsync(runnerDotnetRuntimeFile);
                    foreach (var file in Directory.GetFiles(layoutBin, "*", SearchOption.AllDirectories))
                    {
                        if (!coreAssets.Any(x => file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).EndsWith(x)) &&
                            !runtimeAssets.Any(x => file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).EndsWith(x)))
                        {
                            newFiles.Add(file);
                        }
                    }

                    if (newFiles.Count > 0)
                    {
                        Assert.True(false, $"Found new files '{string.Join('\n', newFiles)}'. These will break runner update using trimmed packages.");
                    }
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public async Task RunnerLayoutParts_2()
        {
            using (TestHostContext hc = new TestHostContext(this))
            {
                Tracing trace = hc.GetTrace();
                var runnerCoreAssertsFile = Path.Combine(TestUtil.GetSrcPath(), @"Misc/runnercoreassets");
                var runnerDotnetRuntimeFile = Path.Combine(TestUtil.GetSrcPath(), @"Misc/runnerdotnetruntimeasserts");

                var coreAssets = await File.ReadAllLinesAsync(runnerCoreAssertsFile);
                var runtimeAssets = await File.ReadAllLinesAsync(runnerDotnetRuntimeFile);

                foreach (var line in coreAssets)
                {
                    if (runtimeAssets.Contains(line, StringComparer.OrdinalIgnoreCase))
                    {
                        Assert.True(false, $"'Misc/runnercoreassets' and 'Misc/runnerdotnetruntimeasserts' should not overlap.");
                    }
                }
            }
        }
    }
}
