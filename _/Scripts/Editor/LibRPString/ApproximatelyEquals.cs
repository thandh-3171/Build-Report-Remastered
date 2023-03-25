using System;
using System.Collections.Generic;
using System.Linq;

namespace LibRPString
{
    public static partial class ComparisonMetrics
    {
        public static double GetFuzzyEqualityScore(this string source, string target, params LibRPStringComparisonOptions[] options)
        {
            List<double> comparisonResults = new List<double>();

            if (!options.Contains(LibRPStringComparisonOptions.CaseSensitive))
            {
                source = source.Capitalize();
                target = target.Capitalize();
            }

            // Min: 0    Max: source.Length = target.Length
            if (options.Contains(LibRPStringComparisonOptions.UseHammingDistance))
            {
                if (source.Length == target.Length)
                {
                    comparisonResults.Add(source.HammingDistance(target) / target.Length);
                }
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseJaccardDistance))
            {
                comparisonResults.Add(source.JaccardDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseJaroDistance))
            {
                comparisonResults.Add(source.JaroDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseJaroWinklerDistance))
            {
                comparisonResults.Add(source.JaroWinklerDistance(target));
            }

            // Min: 0    Max: LevenshteinDistanceUpperBounds - LevenshteinDistanceLowerBounds
            // Min: LevenshteinDistanceLowerBounds    Max: LevenshteinDistanceUpperBounds
            if (options.Contains(LibRPStringComparisonOptions.UseNormalizedLevenshteinDistance))
            {
                comparisonResults.Add(Convert.ToDouble(source.NormalizedLevenshteinDistance(target)) /
                                             Convert.ToDouble((Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target))));
            }
            else if (options.Contains(LibRPStringComparisonOptions.UseLevenshteinDistance))
            {
                comparisonResults.Add(Convert.ToDouble(source.LevenshteinDistance(target)) /
                                             Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target)));
            }

            if (options.Contains(LibRPStringComparisonOptions.UseLongestCommonSubsequence))
            {
                comparisonResults.Add(1 -
                                             Convert.ToDouble((source.LongestCommonSubsequence(target).Length) /
                                                                    Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            if (options.Contains(LibRPStringComparisonOptions.UseLongestCommonSubstring))
            {
                comparisonResults.Add(1 -
                                             Convert.ToDouble((source.LongestCommonSubstring(target).Length) /
                                                                    Convert.ToDouble(Math.Min(source.Length, target.Length))));
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseSorensenDiceDistance))
            {
                comparisonResults.Add(source.SorensenDiceDistance(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseOverlapCoefficient))
            {
                comparisonResults.Add(1 - source.OverlapCoefficient(target));
            }

            // Min: 0    Max: 1
            if (options.Contains(LibRPStringComparisonOptions.UseRatcliffObershelpSimilarity))
            {
                comparisonResults.Add(1 - source.RatcliffObershelpSimilarity(target));
            }

            return comparisonResults.Average();
        }


        public static bool ApproximatelyEquals(this string source, string target, LibRPStringComparisonTolerance tolerance, params LibRPStringComparisonOptions[] options)
        {
            if (options.Length == 0)
            {
                return false;
            }

            var score = source.GetFuzzyEqualityScore(target, options);

            if (tolerance == LibRPStringComparisonTolerance.Strong)
            {
                if (score < 0.25)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == LibRPStringComparisonTolerance.Normal)
            {
                if (score < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == LibRPStringComparisonTolerance.Weak)
            {
                if (score < 0.75)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (tolerance == LibRPStringComparisonTolerance.Manual)
            {
                if (score > 0.6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
