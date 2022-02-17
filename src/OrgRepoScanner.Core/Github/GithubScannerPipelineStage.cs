using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Microsoft.Extensions.Options;
using System.Linq;
using System.IO;
namespace OrgRepoScanner.Core.Github
{
    public class GithubScannerPipelineStage : IPipelineStage
    {
        private readonly IGitHubClient gitHubClient;
        private readonly GithubOptions githubOptions;
        public GithubScannerPipelineStage(IGitHubClient gitHubClient, GithubOptions githubOptions)
        {
            this.gitHubClient = gitHubClient;
            this.githubOptions = githubOptions;
        }

        public async Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit = null)
        {
            if (codeUnit != null)
            {
                throw new Exception("GithubScannerPipelineStage needs to be the first stage");
            }
            var repositories = await gitHubClient.Repository.GetAllForOrg(this.githubOptions.Organization);
            codeUnit = repositories.Select(x => new CodeUnit
            {
                Name = x.Name,
                DefaultBranch = x.DefaultBranch,
                Description = x.Description,
                CodeMetrics = new Dictionary<string, object>(),
                FullName = x.FullName,
                Language = x.Language,
                Stars = x.StargazersCount,
                Watchers = x.WatchersCount,
                Url = Path.Join("https://github.com", (new Uri(x.Url)).LocalPath.Replace("/repos", ""))
            }).ToList();
            return codeUnit;
        }

        List<CodeUnit> IPipelineStage.Execute(List<CodeUnit> codeUnit)
        {
            return ExecuteAsync(codeUnit).Result;
        }
    }
}
