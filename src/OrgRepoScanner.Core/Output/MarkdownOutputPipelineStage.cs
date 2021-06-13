using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Markdig;
using System.IO;

namespace OrgRepoScanner.Core.Output
{
    public class MarkdownOutputPipelineStage : IPipelineStage
    {
        private readonly MarkdownOutputOptions markdownOutputOptions;
        public MarkdownOutputPipelineStage(MarkdownOutputOptions markdownOutputOptions)
        {
            this.markdownOutputOptions = markdownOutputOptions;
        }
        public List<CodeUnit> Execute(List<CodeUnit> codeUnit = null)
        {
            return ExecuteAsync(codeUnit).Result;
        }

        public async Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit = null)
        {
            if (codeUnit == null)
                throw new Exception("No codeunits found!");
            var sb = new StringBuilder();
            sb.AppendLine("# Organization Repo Scan Results\n\n");
            sb.AppendLine("|Type           | Captain   |   First mate   |  Sailor  |   Deck Swab   |");
            sb.AppendLine("|---------------|-----------|----------------|----------|---------------|");
            sb.AppendLine("|Badge |<img src=\"icons/captain.png\" alt=\"drawing\" style=\"width: 25px;\"/>|<img src=\"icons/firstmate.png\" alt=\"drawing\" style=\"width: 25px;\"/>|<img src=\"icons/sailor.png\" alt=\"drawing\" style=\"width: 25px;\"/>|<img src=\"icons/deckswab.png\" alt=\"drawing\" style=\"width: 25px; \"/>|");
            sb.AppendLine("|Metric Score   |3          |   2            | 1        |0              |");
            sb.AppendLine("|Reputation     |18-16      |   15-13        | 12-6     |5-0            |\n");

            sb.AppendLine("| Name | Description | Language | Reputation | Coverage | Critical Violations | Violations  | Code Smells | Conditions To Cover | Complexity |");
            sb.AppendLine("| ---- | ----------- | -------- | ---------- |-------- | ------------------- | ----------- | ----------- | ------------------- | ---------- |");
            
            foreach (var item in codeUnit)
            {
                var name = $"[{item.Name}]({item.Url})";
                var description = item.Description;
                var language = item.Language;
                var reputationRank = item.CodeMetrics.ContainsKey("reputation_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["reputation_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var coverageRank = item.CodeMetrics.ContainsKey("coverage_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["coverage_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var criticalVoilationRank = item.CodeMetrics.ContainsKey("critical_violations_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["critical_violations_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var voilationRank = item.CodeMetrics.ContainsKey("voilations_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["voilations_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var codeSmellsRank = item.CodeMetrics.ContainsKey("code_smells_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["code_smells_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var conditionsToCoverRank = item.CodeMetrics.ContainsKey("conditions_to_cover_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["conditions_to_cover_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                var complexityRank = item.CodeMetrics.ContainsKey("complexity_rank") 
                    ? $"<img src=\"icons/{item.CodeMetrics["complexity_rank"].ToString().Replace(" ", "").ToLower()}.png\" alt=\"drawing\" style=\"width: 25px;\"/>" 
                    : string.Empty;
                sb.AppendLine($"| {name} | {description} | {language} | {reputationRank} | {coverageRank} | { criticalVoilationRank } | {voilationRank} | {codeSmellsRank} | {conditionsToCoverRank} | {complexityRank} |");
            }
            await File.WriteAllTextAsync($"{markdownOutputOptions.OutputFileName}.md", sb.ToString());
            return codeUnit;
        }

        private string FormatMetric(int number, int criticalValue, int warningValue)
        {
            var sign = number >= warningValue ? ":heavy_check_mark:"
                : number < warningValue && number >= criticalValue ? ":warning:" : ":x:";
            return $"{sign} {number}";
        }

        private string FormatMetric(decimal number, decimal criticalValue, decimal warningValue)
        {
            var sign = number >= warningValue ? ":heavy_check_mark:"
                : number < warningValue && number >= criticalValue ? ":warning:" : ":x:";
            return $"{sign} {number}";
        }
    }
}
