﻿using System.Linq;

namespace LibRPString
{
    public static partial class ComparisonMetrics
    {
        public static double TanimotoCoefficient(this string source, string target)
        {
            double Na = source.Length;
            double Nb = target.Length;
            double Nc = source.Intersect(target).Count();

            return Nc / (Na + Nb - Nc);
        }
    }
}
