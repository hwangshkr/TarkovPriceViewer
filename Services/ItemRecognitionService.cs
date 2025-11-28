using System;
using System.Linq;
using TarkovPriceViewer.Models;

namespace TarkovPriceViewer.Services
{
    public interface IItemRecognitionService
    {
        TarkovAPI.Item MatchItemName(string name, string name2, TarkovAPI.Data data);
    }

    public class ItemRecognitionService : IItemRecognitionService
    {
        public TarkovAPI.Item MatchItemName(string name, string name2, TarkovAPI.Data data)
        {
            if (data == null || data.items == null)
            {
                return new TarkovAPI.Item();
            }

            char[] itemname = (name ?? string.Empty).ToLower().Trim().ToCharArray();
            char[] itemname2 = (name2 ?? string.Empty).ToLower().Trim().ToCharArray();

            var result = new TarkovAPI.Item();
            int d = 999;

            foreach (var item in data.items)
            {
                int d2;
                if (itemname.Length > 0)
                {
                    d2 = LevenshteinDistance(itemname, item.name.ToLower().ToCharArray());
                    if (d2 < d)
                    {
                        result = item;
                        d = d2;
                        if (d == 0)
                        {
                            break;
                        }
                    }
                }

                if (itemname2.Length > 0)
                {
                    d2 = LevenshteinDistance(itemname2, item.name.ToLower().ToCharArray());
                    if (d2 < d)
                    {
                        result = item;
                        d = d2;
                        if (d == 0)
                        {
                            break;
                        }
                    }
                }
            }

            if (result.name != null)
            {
                return result;
            }

            if (name == "Encrypted message" || name2 == "Encrypted message")
            {
                result = new TarkovAPI.Item { name = "Encrypted Message" };
            }

            AppLogger.Info("ItemRecognitionService.MatchItemName", $"distance={d}, result={result.name}");
            return result;
        }

        private int GetMinimum(int val1, int val2, int val3)
        {
            int minNumber = val1;
            if (minNumber > val2) minNumber = val2;
            if (minNumber > val3) minNumber = val3;
            return minNumber;
        }

        private int LevenshteinDistance(char[] s, char[] t)
        {
            int m = s.Length;
            int n = t.Length;

            int[,] d = new int[m + 1, n + 1];

            for (int i = 1; i < m; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 1; j < n; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = 1; i < m; i++)
                {
                    if (s[i] == t[j])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = GetMinimum(d[i - 1, j], d[i, j - 1], d[i - 1, j - 1]) + 1;
                    }
                }
            }
            return d[m - 1, n - 1];
        }
    }
}
