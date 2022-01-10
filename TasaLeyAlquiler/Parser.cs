using HtmlAgilityPack;
using System.Collections.Generic;

namespace TasaLeyAlquiler
{
    public static class Parser
    {
        public static Dictionary<string, string> GetDictFromContent(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var nodes = doc.DocumentNode.SelectNodes("//td");

            var dict = new Dictionary<string, string>();

            for (int i = 0; i < nodes.Count; i += 2)
            {
                var tasa = nodes[i + 1].InnerHtml.Trim();
                var dia = nodes[i].InnerHtml.Trim();

                dict.TryAdd(dia, tasa);
            }

            return dict;
        }
    }
}
