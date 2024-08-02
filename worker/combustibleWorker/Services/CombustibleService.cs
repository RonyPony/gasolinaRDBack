
using CombustiblesrdBack.Handles;
using combustibleWorker.AppSettingModels;
using combustibleWorker.Interface;
using combustibleWorker.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace combustibleWorker.Services
{
    public class CombustibleService : ICombustibleService
    {
        private readonly XPathExpression _xpath;
        private readonly UrlPage _UrlPage;

        public CombustibleService(IOptions<UrlPage> urlPage, IOptions<XPathExpression> xpath)
        {
            this._xpath = xpath.Value;
            this._UrlPage = urlPage.Value;
        }

        public List<Combustible> GetCombustible()
        {
            var htmlDoc = GetHtmlDocument();
            if (htmlDoc == null)
            {
                return new List<Combustible>();
            }
            var combustibles = htmlDoc.GetCombustibles(_xpath);
            return combustibles;
        }
        
        private HtmlDocument GetHtmlDocument()
        {
            HtmlWeb htmlWeb = new();
            try
            {
                return htmlWeb.Load(this._UrlPage.Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo cargar el origen de la data "+ex.Message);
                return null;
            }
        }
    }

}
