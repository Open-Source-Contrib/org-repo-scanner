using System;
using System.Collections.Generic;
using System.Text;

namespace OrgRepoScanner.Core.Model
{
    public record CodeUnit : IComparable
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string FullName { get; init; }
        public string Url { get; init; }
        public string Language { get; init; }
        public int Stars { get; init; }
        public int Watchers { get; init; }
        public string DefaultBranch { get; init; }
        public Dictionary<string,object> CodeMetrics { get; init; }

        public int CompareTo(object obj)
        {
            if(obj is CodeUnit)
            {
                var other = obj as CodeUnit;
                var thisCoverage = this.CodeMetrics.ContainsKey("coverage") ? decimal.Parse(this.CodeMetrics["coverage"].ToString()) : 0m;
                var otherCoverage = other.CodeMetrics.ContainsKey("coverage") ? decimal.Parse(other.CodeMetrics["coverage"].ToString()) : 0m;
                return thisCoverage.CompareTo(otherCoverage);
            }
            throw new NotImplementedException();
        }
    }
}
