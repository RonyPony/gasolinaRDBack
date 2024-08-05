
using CombustiblesrdBack.AppSettingModels;
using CombustiblesrdBack.Handles;
using CombustiblesrdBack.Interface;
using CombustiblesrdBack.Models;
using CombustiblesrdBack.Repository;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CombustiblesrdBack.Services
{
    public class CombustibleService : ICombustibleService
    {
        private readonly XPathExpression _xpath;
        private readonly UrlPage _UrlPage;
        private readonly IDataRepository repo;

        public CombustibleService(IOptions<UrlPage> urlPage, IOptions<XPathExpression> xpath,IDataRepository repox)
        {
            this._xpath = xpath.Value;
            this._UrlPage = urlPage.Value;
            this.repo = repox;
        }

        public List<Combustible> GetCombustible()
        {
            var htmlDoc = GetHtmlDocument();
            var combustibles = htmlDoc.GetCombustibles(_xpath);
            return combustibles;
        }

        public async Task<IEnumerable<Combustible>> GetCombustiblesLocalAsync()
        {
            
            IEnumerable<Combustible> response = await repo.GetAllAsync();
            return response;
        }

        public async Task<IEnumerable<List<Combustible>>> GetCombustiblesHistory()
        {
            IEnumerable<List<Combustible>> response = await repo.GetHistory();
            return response;
        }

        private HtmlDocument GetHtmlDocument()
        {
            HtmlWeb htmlWeb = new();
            return htmlWeb.Load(this._UrlPage.Url);
        }
    }

}
