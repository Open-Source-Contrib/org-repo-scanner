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
            sb.AppendLine("# Organization Repo Scan Results\n");
            sb.AppendLine("| Name | Description | Language | Coverage | Critical Violations | Code Smells | Violations | Conditions To Cover | Complexity |");
            sb.AppendLine("| ---- | ----------- | -------- | -------- | ------------------- | ----------- | ---------- | ------------------- | ---------- |");
            foreach (var item in codeUnit)
            {
                var name = $"[{item.Name}]({item.Url})";
                var description = item.Description;
                var language = item.Language;
                var coverageNumber = item.CodeMetrics.ContainsKey("coverage") ? decimal.Parse(item.CodeMetrics["coverage"].ToString()) : 0m;
                var criticalVoilationseNumber = item.CodeMetrics.ContainsKey("critical_violations") ? int.Parse(item.CodeMetrics["critical_violations"].ToString()) : 0;
                var codeSmellsNumber = item.CodeMetrics.ContainsKey("code_smells") ? int.Parse(item.CodeMetrics["code_smells"].ToString()) : 0;
                var voilationsNumber = item.CodeMetrics.ContainsKey("violations") ? int.Parse(item.CodeMetrics["code_smells"].ToString()) : 0;
                var conditionsToCoverNumber = item.CodeMetrics.ContainsKey("conditions_to_cover") ? int.Parse(item.CodeMetrics["conditions_to_cover"].ToString()) : 0;
                var complexityNumber = item.CodeMetrics.ContainsKey("complexity") ? int.Parse(item.CodeMetrics["complexity"].ToString()) : 0;
                sb.AppendLine($"| {name} | {description} | {language} | { FormatMetric(coverageNumber, 30m, 70m) } | { criticalVoilationseNumber } | {codeSmellsNumber} | {voilationsNumber} | {conditionsToCoverNumber} | {complexityNumber} |");
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
