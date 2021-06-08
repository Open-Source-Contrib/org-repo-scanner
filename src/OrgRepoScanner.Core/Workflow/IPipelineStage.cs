using OrgRepoScanner.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Workflow
{
    public interface IPipelineStage
    {
        List<CodeUnit> Execute(List<CodeUnit> codeUnit = null);
        Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit = null);
    }
}
