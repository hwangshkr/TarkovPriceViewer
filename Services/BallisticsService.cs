using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TarkovPriceViewer.Models;

namespace TarkovPriceViewer.Services
{
    public interface IBallisticsService
    {
        Dictionary<string, Ballistic> BallisticsData { get; }
        bool IsLoaded { get; }
        Task LoadBallisticsAsync();
    }

    public class BallisticsService : IBallisticsService
    {
        private const string WikiUrl = "https://escapefromtarkov.fandom.com/wiki/Ballistics";
        private readonly IHttpClientFactory _httpClientFactory;
        
        public Dictionary<string, Ballistic> BallisticsData { get; private set; } = new Dictionary<string, Ballistic>();
        public bool IsLoaded { get; private set; }

        public BallisticsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task LoadBallisticsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10); // Increased timeout a bit
                AppLogger.Info("BallisticsService.LoadBallisticsAsync", $"Requesting {WikiUrl}");
                
                string html = await client.GetStringAsync(WikiUrl);
                
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                
                HtmlNode node_tm = doc.DocumentNode.SelectSingleNode("//table[4]"); 
                
                if (node_tm != null)
                {
                    node_tm = node_tm.SelectSingleNode(".//tbody");
                    if (node_tm != null)
                    {
                        var nodes = node_tm.SelectNodes(".//tr");
                        if (nodes != null)
                        {
                            BallisticsData.Clear();
                            List<Ballistic> sub_blist = new List<Ballistic>();
                            
                            foreach (HtmlNode node in nodes)
                            {
                                if (!node.GetAttributeValue("id", "").Equals(""))
                                {
                                    sub_blist = new List<Ballistic>();
                                }
                                
                                var sub_nodes = node.SelectNodes(".//td");
                                if (sub_nodes != null && sub_nodes.Count >= 15)
                                {
                                    ParseRow(sub_nodes, sub_blist);
                                }
                            }
                        }
                    }
                }
                IsLoaded = true;
                AppLogger.Info("BallisticsService.LoadBallisticsAsync", "Finished getting Ballistics!");
            }
            catch (Exception e)
            {
                AppLogger.Error("BallisticsService.LoadBallisticsAsync", "Error with Ballistics", e);
            }
        }

        private void ParseRow(HtmlNodeCollection sub_nodes, List<Ballistic> sub_blist)
        {
            int first = sub_nodes[0].GetAttributeValue("rowspan", 1) == 1 ? 0 : 1;
            if (sub_nodes[0].InnerText.Trim().Equals("40x46 mm"))
            {
                first = 1;
            }
            
            String name = sub_nodes[first++].InnerText.Trim();
            String special = ParseSpecial(ref name);
            name = name.Replace("*", "").Trim();
            
            String damage = sub_nodes[first++].InnerText.Trim();
            damage = ParseDamage(damage);

            Ballistic b = new Ballistic(
                name
                , damage
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , sub_nodes[first++].InnerText.Trim()
                , special
                , sub_blist
            );
            
            sub_blist.Add(b);
            if (!BallisticsData.ContainsKey(name))
            {
                BallisticsData.Add(name, b);
            }
        }

        private string ParseSpecial(ref string name)
        {
            string special = "";
            if (name.EndsWith(" S T")) { name = new Regex("(S T)$").Replace(name, ""); special = @"Subsonic & Tracer"; }
            else if (name.EndsWith(" T")) { name = new Regex("T$").Replace(name, ""); special = @"Tracer"; }
            else if (name.EndsWith(" S")) { name = new Regex("S$").Replace(name, ""); special = @"Subsonic"; }
            else if (name.EndsWith(" S T FM")) { name = new Regex("(S T FM)$").Replace(name, ""); special = @"Subsonic & Tracer"; }
            else if (name.EndsWith(" T FM")) { name = new Regex("T FM$").Replace(name, ""); special = @"Tracer"; }
            else if (name.EndsWith(" S FM")) { name = new Regex("S FM$").Replace(name, ""); special = @"Subsonic"; }
            else if (name.EndsWith(" FM")) { name = new Regex("FM$").Replace(name, ""); special = @""; }
            return special;
        }

        private string ParseDamage(string damage)
        {
            if (damage.Contains("x"))
            {
                String[] temp_d = damage.Split('x');
                int mul = 1;
                try
                {
                    foreach (String d in temp_d)
                    {
                        mul *= Int32.Parse(d);
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Error("BallisticsService.ParseDamage", "Error 14", ex);
                }
                damage += " = " + mul;
            }
            return damage;
        }
    }
}
