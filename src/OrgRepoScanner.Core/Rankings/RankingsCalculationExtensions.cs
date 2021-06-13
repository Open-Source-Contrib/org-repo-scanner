using OrgRepoScanner.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgRepoScanner.Core.Rankings
{
    /*
     * 1. Coverage:
     *  Above 80%: 3 points     //Captain
     *  60-80%: 2 points        //First Mate 
     *  40-60%: 1 point         //Sailor
     *  Less than 40%: 0 Point  //Deck Swab
     * 2. Voilations: 
     *  2 or less voilations: 3 Points      //Captain
     *  5-2 voilations: 2 Points            //First Mate 
     *  8-5 voilations: 1 Point             //Sailor
     *  More than 8 Voilations: 0 Point     //Deck Swab
     * 3. Critical Voilations:
     *  0 voilations: 3 Points
     *  1 or more voilations: 0 points
     * 4. Code Smells:
     *  0 Code smells: 3 points
     *  1-5 Code smells: 2 points
     *  5-8 Code smells: 1 Point
     *  More than 8 smells: 0 Points
     * 5. Complexity:
     *  Less than 500: 3 Points
     *  500-1000: 2 points
     *  1000-2500: 1 Point
     *  Greater than 2500: 0 Points
     * 6. Conditions To Cover:
     *  Less than 10: 3 Points
     *  10-100: 2 Points
     *  100-250: 1 Point
     *  More than 250: 0 Points
     */

    /*
     * <=18 to >15  : Captain
     * <=15 to >12  : First Mate
     * <=12 to >5   : Sailor
     * <=5          : Deck Swab
     */
    public static class RankingsCalculationExtensions
    {
        public static void CalculateRankings(this CodeUnit codeUnit) => AddMetricsPoints(codeUnit);

        static void AddMetricsPoints(CodeUnit codeUnit)
        {
            decimal coverage = codeUnit.CodeMetrics.ContainsKey("coverage") ? decimal.Parse(codeUnit.CodeMetrics["coverage"].ToString()) : -1m;
            int voilations = codeUnit.CodeMetrics.ContainsKey("violations") ? int.Parse(codeUnit.CodeMetrics["violations"].ToString()) : int.MaxValue;
            int criticalVoilations = codeUnit.CodeMetrics.ContainsKey("critical_violations") ? int.Parse(codeUnit.CodeMetrics["critical_violations"].ToString()) : int.MaxValue;
            int codeSmells = codeUnit.CodeMetrics.ContainsKey("code_smells") ? int.Parse(codeUnit.CodeMetrics["code_smells"].ToString()) : int.MaxValue;
            int complexity = codeUnit.CodeMetrics.ContainsKey("complexity") ? int.Parse(codeUnit.CodeMetrics["complexity"].ToString()) : int.MaxValue;
            int conditionsToCover = codeUnit.CodeMetrics.ContainsKey("conditions_to_cover") ? int.Parse(codeUnit.CodeMetrics["conditions_to_cover"].ToString()) : int.MaxValue;
            var coveragePoints = CalculateCoveragePoints(coverage);
            var voilationsPoints = CalculateVoilationsPoints(voilations);
            var criticalVoilationsPoints = CalculateCriticalVoilationsPoints(criticalVoilations);
            var codeSmellsPoints = CalculateCodeSmellsPoints(codeSmells);
            var complexityPoints = CalculateComplexityPoints(complexity);
            var conditionsToCoverPoints = CalculateConditionsToCoverPoints(conditionsToCover);
            AddScores(codeUnit, coveragePoints, voilationsPoints, criticalVoilationsPoints, codeSmellsPoints, complexityPoints, conditionsToCoverPoints);
            AddPointRanks(codeUnit, coveragePoints, voilationsPoints, criticalVoilationsPoints, codeSmellsPoints, complexityPoints, conditionsToCoverPoints);
        }

        static void AddPointRanks(CodeUnit codeUnit, int coveragePoints, int voilationsPoints, int criticalVoilationsPoints, int codeSmellsPoints, int complexityPoints, int conditionsToCoverPoints)
        {
            var overallReputation = coveragePoints + voilationsPoints + criticalVoilationsPoints + codeSmellsPoints + complexityPoints + conditionsToCoverPoints;
            codeUnit.CodeMetrics.Add("coverage_rank", CalculateMetricRank(coveragePoints));
            codeUnit.CodeMetrics.Add("voilations_rank", CalculateMetricRank(voilationsPoints));
            codeUnit.CodeMetrics.Add("critical_violations_rank", CalculateMetricRank(criticalVoilationsPoints));
            codeUnit.CodeMetrics.Add("code_smells_rank", CalculateMetricRank(codeSmellsPoints));
            codeUnit.CodeMetrics.Add("complexity_rank", CalculateMetricRank(complexityPoints));
            codeUnit.CodeMetrics.Add("conditions_to_cover_rank", CalculateMetricRank(conditionsToCoverPoints));
            codeUnit.CodeMetrics.Add("reputation_rank", CalculateReputationRank(overallReputation));
        }

        static void AddScores(CodeUnit codeUnit, int coveragePoints, int voilationsPoints, int criticalVoilationsPoints, int codeSmellsPoints, int complexityPoints, int conditionsToCoverPoints)
        {
            var overallReputation = coveragePoints + voilationsPoints + criticalVoilationsPoints + codeSmellsPoints + complexityPoints + conditionsToCoverPoints;
            codeUnit.CodeMetrics.Add("coverage_score", coveragePoints);
            codeUnit.CodeMetrics.Add("voilations_score", voilationsPoints);
            codeUnit.CodeMetrics.Add("critical_violations_score", criticalVoilationsPoints);
            codeUnit.CodeMetrics.Add("code_smells_score", codeSmellsPoints);
            codeUnit.CodeMetrics.Add("complexity_score", complexityPoints);
            codeUnit.CodeMetrics.Add("conditions_to_cover_score", conditionsToCoverPoints);
            codeUnit.CodeMetrics.Add("reputation_score", overallReputation);
        }

        static string CalculateReputationRank(int reputationPoints) =>
            reputationPoints <= 18 && reputationPoints > 15 ? "Captain"
            : reputationPoints <= 15 && reputationPoints > 12 ? "First Mate"
            : reputationPoints <= 12 && reputationPoints > 5 ? "Sailor"
            : "Deck Swab";

        static string CalculateMetricRank(int points) => 
            points >= 3 ? "Captain"
                : points == 2 ? "First Mate"
                : points == 1 ? "Sailor"
                : "Deck Swab";

        static int CalculateComplexityPoints(int value)
        {
            /*
            * Complexity:
             *  Less than 500: 3 Points
             *  500-1000: 2 points
             *  1000-2500: 1 Point
             *  Greater than 2500: 0 Points
             */
            return value <= 500 ? 3
                : (value > 500 && value <= 1000) ? 2
                : (value > 1000 && value <= 2500) ? 1
                : 0;
        }

        static int CalculateConditionsToCoverPoints(int value)
        {
            /*
            * Conditions To Cover:
             *  Less than 10: 3 Points
             *  10-100: 2 Points
             *  100-250: 1 Point
             *  More than 250: 0 Points
             */
            return value <= 10 ? 3
                : (value > 10 && value <= 100) ? 2
                : (value > 100 && value <= 250) ? 1
                : 0;
        }

        static int CalculateCodeSmellsPoints(int value)
        {
            /*
            * Code Smells:
             *  0 Code smells: 3 points
             *  1-5 Code smells: 2 points
             *  5-8 Code smells: 1 Point
             *  More than 8 smells: 0 Points
             */
            return value <= 1 ? 3
                : (value > 1 && value <= 5) ? 2
                : (value > 5 && value <= 8) ? 1
                : 0;
        }

        static int CalculateCriticalVoilationsPoints(int value)
        {
            /*
            * Critical Voilations:
             *  0 voilations: 3 Points
             *  1 or more voilations: 0 points
             */
            return value < 1 ? 3
                : 0; ;
        }

        static int CalculateCoveragePoints(decimal value)
        {
            /*
             * Coverage:
             *  Above 80%: 3 points     //Quarter Master
             *  60-80%: 2 points        //First Mate 
             *  40-60%: 1 point         //Sailor
             *  Less than 40%: 0 Point  //Deck Swab
             */
            return value > 80 ? 3
                : (value <= 80 && value > 60) ? 2
                : (value <= 60 && value >= 40) ? 1
                : 0;
        }

        static int CalculateVoilationsPoints(int value)
        {
            /*
             * Voilations: 
             *  2 or less voilations: 3 Points      //Quarter Master
             *  5-2 voilations: 2 Points            //First Mate 
             *  8-5 voilations: 1 Point             //Sailor
             *  More than 8 Voilations: 0 Point     //Deck Swab
             */
            return value <= 2 ? 3
                : (value > 2 && value <= 5) ? 2
                : (value > 5 && value <= 8) ? 1
                : 0;
        }
    }
}
