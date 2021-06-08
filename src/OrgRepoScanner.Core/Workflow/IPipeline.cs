using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Workflow
{
    public interface IPipeline
    {
        void Execute();
        Task ExecuteAsync();
    }
}
