using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Github
{
    public class GithubCommitPipelineStage : IPipelineStage
    {
        public List<CodeUnit> Execute(List<CodeUnit> codeUnit = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit = null)
        {
            throw new NotImplementedException();
        }
    }
}
