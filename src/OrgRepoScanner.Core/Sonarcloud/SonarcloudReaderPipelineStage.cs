using OrgRepoScanner.Core.Model;
using OrgRepoScanner.Core.Workflow;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Linq;
using OrgRepoScanner.Core.Rankings;
namespace OrgRepoScanner.Core.Sonarcloud
{
    public class SonarcloudReaderPipelineStage : IPipelineStage
    {
        private readonly SonarcloudOptions sonarcloudOptions;
        private readonly HttpClient httpClient;
        public SonarcloudReaderPipelineStage(SonarcloudOptions sonarcloudOptions)
        {
            this.sonarcloudOptions = sonarcloudOptions;
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new System.Uri(sonarcloudOptions.ApiBaseAddress ?? "https://sonarcloud.io/");
            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{sonarcloudOptions.Username}:")));
        }
        public List<CodeUnit> Execute(List<CodeUnit> codeUnit)
        {
            return ExecuteAsync(codeUnit).Result;
        }

        public async Task<List<CodeUnit>> ExecuteAsync(List<CodeUnit> codeUnit)
        {
            if (codeUnit == null)
                throw new System.Exception("No code unit defined");
            foreach (var item in codeUnit)
            {
                try
                {
                    var key = string.Format(sonarcloudOptions.ProjectKeyFormat, sonarcloudOptions.OrganizationKey, item.Name);
                    var metrics = await this.httpClient
                        .GetFromJsonAsync<SonarcloudMetricResponse>($"api/measures/component_tree?component={key}&metricKeys=complexity,coverage,violations,code_smells,critical_violations,conditions_to_cover");
                    foreach (var metric in metrics.BaseComponent.Measures)
                    {
                        item.CodeMetrics.Add(metric.Metric, metric.Value);
                    }
                    item.CalculateRankings();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }
            codeUnit = codeUnit.OrderByDescending(x=>x).ToList();
            return codeUnit;
        }
    }
}
