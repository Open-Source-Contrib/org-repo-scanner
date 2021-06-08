using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Model
{
    public class Paging
    {
        [JsonPropertyName("pageIndex")]
        public int PageIndex { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class Measure
    {
        [JsonPropertyName("metric")]
        public string Metric { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("bestValue")]
        public bool BestValue { get; set; }
    }

    public class BaseComponent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        [JsonPropertyName("measures")]
        public List<Measure> Measures { get; set; }
    }

    public class Component
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("qualifier")]
        public string Qualifier { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("measures")]
        public List<Measure> Measures { get; set; }
    }

    public class SonarcloudMetricResponse
    {
        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("baseComponent")]
        public BaseComponent BaseComponent { get; set; }

        [JsonPropertyName("components")]
        public List<Component> Components { get; set; }
    }
}
