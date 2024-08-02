
using CombustiblesrdBack.AppSettingModels;
using CombustiblesrdBack.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace CombustiblesrdBack.Handles
{
    public static class Handle
    {
        public static List<Combustible> GetCombustibles(this HtmlDocument htmlDoc, XPathExpression xpath)
        {
            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes(xpath.XPath);
            List<Combustible> allFuels = new List<Combustible>();
            foreach (HtmlNode item in htmlNodes.ToList())
            {
                string[] precios = item.InnerText.ToString().Replace("\n","").Replace("\t","").Split("$");
                allFuels= splitChars(precios);
            }
            //var combustibles = htmlNodes.Select(n => new Combustible()
            //{
            //    Nombre = n.SelectNodes("span")[(int)EnumCombistible.nombre].InnerText, 
            //    Precio =  n.SelectNodes("div")[(int)EnumCombistible.precio].InnerText
            //}).ToList();
            return allFuels;
        }

        private static List<Combustible> splitChars(string[] precios)
        {
            List<Combustible> comb = new List<Combustible>();   
            foreach (string preciox in precios)
            {
                if (preciox == "")
                {
                    continue;
                }
                string price = "";
                string name = "";
                foreach (char item in preciox.Trim())
                {
                    if (char.IsDigit(item) || item=='.')
                    {
                        price+= item.ToString();
                    }
                    else
                    {
                        name+=item.ToString();
                    }
                }
                comb.Add(new Combustible()
                {
                    Nombre = name,
                    Precio = price
                });
            }
            return comb;
        }
    }
}