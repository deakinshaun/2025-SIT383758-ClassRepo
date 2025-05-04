using UnityEngine;

namespace Works.Week_8.Scripts
{
    public static class StringUtils
    {
        public static string NormalizeString(string input)
        {
            return input
                .ToLowerInvariant()
                .Replace("&", "and")
                .Replace("  ", " ")
                .Trim();
        }

        public static float Similarity(string a, string b)
        {
            int distance = LevenshteinDistance(a, b);
            return 1f - (float)distance / Mathf.Max(a.Length, b.Length);
        }

        public static int LevenshteinDistance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    dp[i, j] = Mathf.Min(
                        dp[i - 1, j] + 1,
                        dp[i, j - 1] + 1,
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[a.Length, b.Length];
        }
    }
}