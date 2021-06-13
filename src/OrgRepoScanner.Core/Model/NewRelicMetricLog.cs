using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Model
{
    public record NewRelicMetricLog
    {
        [JsonPropertyName("ingester")]
        public string Ingester { get; init; }
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }
        [JsonPropertyName("message")]
        public string Message { get; init; }
        [JsonPropertyName("repo_name")]
        public string RepoName { get; init; }
        [JsonPropertyName("coverage")]
        public decimal Coverage { get; init; }
        [JsonPropertyName("complexity")]
        public int Complexity { get; init; }
        [JsonPropertyName("voilations")]
        public int Voilations { get; init; }
        [JsonPropertyName("code_smells")]
        public int CodeSmells { get; init; }
        [JsonPropertyName("critical_voilations")]
        public int CriticalVoilations { get; init; }
        [JsonPropertyName("conditions_to_cover")]
        public int ConditionsToCover { get; init; }

        [JsonPropertyName("coverage_score")]
        public int CoverageScore { get; init; }
        [JsonPropertyName("complexity_score")]
        public int ComplexityScore { get; init; }
        [JsonPropertyName("voilations_score")]
        public int VoilationsScore { get; init; }
        [JsonPropertyName("code_smells_score")]
        public int CodeSmellsScore { get; init; }
        [JsonPropertyName("critical_voilations_score")]
        public int CriticalVoilationsScore { get; init; }
        [JsonPropertyName("conditions_to_cover_score")]
        public int ConditionsToCoverScore { get; init; }

        [JsonPropertyName("reputation_score")]
        public int ReputationScore { get; set; }

        [JsonPropertyName("coverage_rank")]
        public string CoverageRank { get; init; }
        [JsonPropertyName("complexity_rank")]
        public string ComplexityRank { get; init; }
        [JsonPropertyName("voilations_rank")]
        public string VoilationsRank { get; init; }
        [JsonPropertyName("code_smells_rank")]
        public string CodeSmellsRank { get; init; }
        [JsonPropertyName("critical_voilations_rank")]
        public string CriticalVoilationsRank { get; init; }
        [JsonPropertyName("conditions_to_cover_rank")]
        public string ConditionsToCoverRank { get; init; }

        [JsonPropertyName("reputation_rank")]
        public string ReputationRank { get; set; }
    }
}
