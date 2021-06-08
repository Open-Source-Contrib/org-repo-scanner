using CommandLine;
using Microsoft.Extensions.Hosting;
using OrgRepoScanner.Core.Github;
using OrgRepoScanner.Core.Output;
using OrgRepoScanner.Core.Sonarcloud;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Threading.Tasks;

namespace OrgRepoScanner.Runner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = Parser.Default.ParseArguments<ActionInputs>(() => new(), args);
            parser.WithNotParsed(errors =>
            {
                foreach (var item in errors)
                {
                    Console.WriteLine("Error: {0}", item);
                };
                Environment.Exit(1);
            });
            await parser.WithParsedAsync(async inputs =>
            {
                var githubOptions = new GithubOptions { GithubToken = inputs.GithubToken, Organization = inputs.OrganizationName, GithubUser = inputs.GithubUser };
                var sonarcloudOptions = new SonarcloudOptions
                {
                    ApiBaseAddress = inputs.SonarApiUrl,
                    OrganizationKey = inputs.SonarOrgKey,
                    ProjectKeyFormat = inputs.SonarProjectKeyFormat,
                    Username = inputs.SonarToken
                };
                var markdownOutputOptions = new MarkdownOutputOptions { OutputFileName = inputs.MarkdownFileName };
                var pipeline = new Pipeline(inputs.PipelineKind, githubOptions, sonarcloudOptions, markdownOutputOptions);
                await pipeline.ExecuteAsync();
                Environment.Exit(0);
            });
        }
    }
}
