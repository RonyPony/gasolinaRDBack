using combustibleWorker.AppSettingModels;
using combustibleWorker.Models;
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
            if (htmlNodes == null)
            {
                return new List<Combustible>();
            }
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
            return precios
        .Where(precio => !string.IsNullOrWhiteSpace(precio))
        .Select(precio =>
        {
            var trimmedPrecio = precio.Trim();
            var name = new string(trimmedPrecio.Where(c => !char.IsDigit(c) && c != '.').ToArray());
            var price = new string(trimmedPrecio.Where(c => char.IsDigit(c) || c == '.').ToArray());
            return new Combustible
            {
                Nombre = name,
                Precio = price,
                UpdateDate=DateTime.Now,
            };
        })
        .ToList();
        }
    }
}