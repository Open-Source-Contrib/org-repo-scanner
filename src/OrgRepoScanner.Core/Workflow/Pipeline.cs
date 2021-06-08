using Octokit;
using OrgRepoScanner.Core.Github;
using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Output;
using OrgRepoScanner.Core.Sonarcloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Workflow
{
    public class Pipeline : IPipeline
    {
        public const string PIPELINE_GH_SONARCLOUD_MD_COMMIT = "PIPELINE_GH_SONARCLOUD_MD_COMMIT";
        private readonly GithubOptions githubOptions;
        private readonly SonarcloudOptions sonarcloudOptions;
        private readonly MarkdownOutputOptions markdownOptions;
        private List<IPipelineStage> stages;
        public void Execute()
        {
            var codeUnit = new List<CodeUnit>();
            foreach (var stage in stages)
            {
                codeUnit = stage.Execute(codeUnit);
            }
        }

        public async Task ExecuteAsync()
        {
            List<CodeUnit> codeUnit = null;
            foreach (var stage in stages)
            {
                codeUnit = await stage.ExecuteAsync(codeUnit);
            }
        }

        public Pipeline(string pipeLineKind, GithubOptions githubOptions, SonarcloudOptions sonarcloudOptions, MarkdownOutputOptions markdownOptions)
        {
            stages = new List<IPipelineStage>();
            this.githubOptions = githubOptions;
            this.sonarcloudOptions = sonarcloudOptions;
            this.markdownOptions = markdownOptions;
            switch (pipeLineKind)
            {
                case PIPELINE_GH_SONARCLOUD_MD_COMMIT:
                    CreatePipelineGithubSonarCloudMarkdownCommit();
                    break;
                default:
                    Environment.Exit(2);
                    break;
            }
        }

        private void CreatePipelineGithubSonarCloudMarkdownCommit()
        {
            var githubClient = new GitHubClient(new ProductHeaderValue(githubOptions.Organization));
            githubClient.Credentials = new Credentials("pradeep-singh-talabat", githubOptions.GithubToken);
            var scannerStage = new GithubScannerPipelineStage(githubClient, githubOptions);
            var sonarcloudStage = new SonarcloudReaderPipelineStage(this.sonarcloudOptions);
            var markdownStage = new MarkdownOutputPipelineStage(this.markdownOptions);
            stages.Add(scannerStage);
            stages.Add(sonarcloudStage);
            stages.Add(markdownStage);
        }
    }
}
