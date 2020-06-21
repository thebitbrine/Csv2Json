using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheBitBrine
{
    class Csv2Json
    {
        public static string GetJson(string CSV, bool Minify = false)
        {
            var Raw = CSV.Replace("#N/A", "");
            var Headers = GetHeaders(Raw);
            var Rows = Raw.Split("\r\n");

            StringBuilder JSON = new StringBuilder();
            JSON.Append("[");

            for (int i = 1; i < Rows.Length - 1; i++)
            {
                var Object = GetObject(Headers, Rows[i]);
                if (!string.IsNullOrWhiteSpace(Object))
                {
                    JSON.Append("{");
                    JSON.Append(Object);
                    JSON.Append("}");

                    if (i != Rows.Length - 2)
                        JSON.Append(",");
                }
            }

            JSON.Append("]");

            if (Minify)
                return JSON.ToString();
            else
                return FormatJson(JSON.ToString());
        }

        private static string GetObject(string[] Headers, string Row)
        {
            StringBuilder JSON = new StringBuilder();
            var Parts = ProperSplit(Row, true, ',', true).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            for (int j = 0; j < Parts.Length - 1; j++)
            {
                JSON.Append($"\"{Headers[j].Trim()}\": \"{Parts[j]}\"");
                if (j != Parts.Length - 2)
                    JSON.Append(",");
            }
            return JSON.ToString();
        }

        private static string[] GetHeaders(string CSV)
        {
            return ProperSplit(CSV.Split("\n").First().Replace("\r", ""), true, ',', true);
        }

        private static string FormatJson(string json)
        {
            const string indentString = " ";
            int indentation = 0;
            int quoteCount = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quoteCount++ : quoteCount
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indentString, indentation)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indentString, ++indentation)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + string.Concat(Enumerable.Repeat(indentString, --indentation)) + ch : ch.ToString()
                select lineBreak ?? (openChar.Length > 1 ? openChar : closeChar);

            return string.Concat(result);
        }

        private static string[] ProperSplit(string Text, bool RemoveQuotes = true, char SplitChar = ' ', bool Clean = false)
        {
            List<string> Slices = new List<string>();
            string Slice = "";
            bool InQuotes = false;
            for (int i = 0; i < Text.Length; i++)
            {
                if (Text[i] == '\"' || Text[i] == '\'')
                    InQuotes = !InQuotes;
                if (InQuotes)
                    Slice += Text[i];
                else
                {
                    if (Text[i] != SplitChar)
                    {
                        Slice += Text[i];
                    }
                    else
                    {
                        Slices.Add(Slice);
                        Slice = "";
                    }
                }
            }
            Slices.Add(Slice);
            if (Clean || RemoveQuotes)
            {
                for (int i = 0; i < Slices.Count; i++)
                {
                    if (Clean)
                        Slices[i] = Slices[i].Replace(SplitChar.ToString(), "");
                    if (RemoveQuotes)
                        Slices[i] = Slices[i].Replace("\'", "").Replace("\"", "");
                }
            }
            return Slices.ToArray();
        }

    }
}
