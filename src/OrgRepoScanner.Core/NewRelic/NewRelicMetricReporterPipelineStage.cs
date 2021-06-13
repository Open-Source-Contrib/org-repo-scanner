using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrgRepoScanner.Core.NewRelic
{
    public class NewRelicMetricReporterPipelineStage : IPipelineStage
    {
        private readonly NewRelicMetricOptions newRelicMetricOptions;
        private readonly HttpClient httpClient;
        public NewRelicMetricReporterPipelineStage(NewRelicMetricOptions newRelicMetricOptions)
        {
            this.newRelicMetricOptions = newRelicMetricOptions;
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new System.Uri(newRelicMetricOptions.ApiBaseAddress ?? "https://log-api.newrelic.com/");
            this.httpClient.DefaultRequestHeaders.Add("X-License-Key", newRelicMetricOptions.ApiKey);
        }
        public List<CodeUnit> Execute(List<CodeUnit> codeUnit = null)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit = null)
        {
            if (codeUnit == null)
                throw new Exception("Invalid code units (null)!");
            foreach (var item in codeUnit)
            {
                await PostCodeUnitToNewRelic(item);
            }
            return codeUnit;
        }

        public async Task PostCodeUnitToNewRelic(CodeUnit codeUnit)
        {
            var metricLog = ComputeMetricLogFromCodeUnit(codeUnit);
            string jsonString = JsonSerializer.Serialize(metricLog);
            var response = await this.httpClient.PostAsJsonAsync("log/v1", metricLog);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to push the code metric for {codeUnit.Name}");
            }
        }

        public NewRelicMetricLog ComputeMetricLogFromCodeUnit(CodeUnit codeUnit)
        {
            var log = new NewRelicMetricLog
            {
                RepoName = codeUnit.Name,
                Timestamp = GetCurrentTimestamp(),
                Ingester = this.newRelicMetricOptions.IngesterName, //"talabat.codequality.scanner"
                Message = "Success",
                Coverage = codeUnit.CodeMetrics.ContainsKey("coverage") ? decimal.Parse(codeUnit.CodeMetrics["coverage"].ToString()) : 0m,
                Voilations = codeUnit.CodeMetrics.ContainsKey("violations") ? int.Parse(codeUnit.CodeMetrics["violations"].ToString()) : 0,
                CriticalVoilations = codeUnit.CodeMetrics.ContainsKey("critical_violations") ? int.Parse(codeUnit.CodeMetrics["critical_violations"].ToString()) : 0,
                CodeSmells = codeUnit.CodeMetrics.ContainsKey("code_smells") ? int.Parse(codeUnit.CodeMetrics["code_smells"].ToString()) : 0,
                Complexity = codeUnit.CodeMetrics.ContainsKey("complexity") ? int.Parse(codeUnit.CodeMetrics["complexity"].ToString()) : 0,
                ConditionsToCover = codeUnit.CodeMetrics.ContainsKey("conditions_to_cover") ? int.Parse(codeUnit.CodeMetrics["conditions_to_cover"].ToString()) : 0,

                CoverageScore = codeUnit.CodeMetrics.ContainsKey("coverage_score") ? int.Parse(codeUnit.CodeMetrics["coverage_score"].ToString()) : 0,
                VoilationsScore = codeUnit.CodeMetrics.ContainsKey("violations_score") ? int.Parse(codeUnit.CodeMetrics["violations_score"].ToString()) : 0,
                CriticalVoilationsScore = codeUnit.CodeMetrics.ContainsKey("critical_violations_score") ? int.Parse(codeUnit.CodeMetrics["critical_violations_score"].ToString()) : 0,
                CodeSmellsScore = codeUnit.CodeMetrics.ContainsKey("code_smells_score") ? int.Parse(codeUnit.CodeMetrics["code_smells_score"].ToString()) : 0,
                ComplexityScore = codeUnit.CodeMetrics.ContainsKey("complexity_score") ? int.Parse(codeUnit.CodeMetrics["complexity_score"].ToString()) : 0,
                ConditionsToCoverScore = codeUnit.CodeMetrics.ContainsKey("conditions_to_cover_score") ? int.Parse(codeUnit.CodeMetrics["conditions_to_cover_score"].ToString()) : 0,
                ReputationScore = codeUnit.CodeMetrics.ContainsKey("reputation_score") ? int.Parse(codeUnit.CodeMetrics["reputation_score"].ToString()) : 0,

                CoverageRank = codeUnit.CodeMetrics.ContainsKey("coverage_rank") ? codeUnit.CodeMetrics["coverage_rank"].ToString() : "Deck Swab",
                VoilationsRank = codeUnit.CodeMetrics.ContainsKey("violations_rank") ? codeUnit.CodeMetrics["violations_rank"].ToString() : "Deck Swab",
                CriticalVoilationsRank = codeUnit.CodeMetrics.ContainsKey("critical_violations_rank") ? codeUnit.CodeMetrics["critical_violations_rank"].ToString() : "Deck Swab",
                CodeSmellsRank = codeUnit.CodeMetrics.ContainsKey("code_smells_rank") ? codeUnit.CodeMetrics["code_smells_rank"].ToString() : "Deck Swab",
                ComplexityRank = codeUnit.CodeMetrics.ContainsKey("complexity_rank") ? codeUnit.CodeMetrics["complexity_rank"].ToString() : "Deck Swab",
                ConditionsToCoverRank = codeUnit.CodeMetrics.ContainsKey("conditions_to_cover_rank") ? codeUnit.CodeMetrics["conditions_to_cover_rank"].ToString() : "Deck Swab",
                ReputationRank = codeUnit.CodeMetrics.ContainsKey("reputation_rank") ? codeUnit.CodeMetrics["reputation_rank"].ToString() : "Deck Swab"
            };
            return log;
        }

        private long GetCurrentTimestamp()
        {
            var tickCount = DateTime.UtcNow.Ticks - (new DateTime(1970, 01, 01)).Ticks;
            tickCount /= TimeSpan.TicksPerSecond;
            return tickCount;
        }
    }
}
