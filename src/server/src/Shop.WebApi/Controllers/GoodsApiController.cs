using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers
{
    [ApiController]
    [Route("api/goods")]
    public class GoodsApiController
    {
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;

        public GoodsApiController(
            IProductService productService,
            IStaticCacheManager staticCacheManager)
        {
            _productService = productService;
            _staticCacheManager = staticCacheManager;
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var result = await _productService.GetGoodsByCache(id);
            return Result.Ok(result);
        }

        [HttpGet("related/{id:int:min(1)}")]
        public async Task<Result> Related(int id)
        {
            var result = await _productService.RelatedList(id);
            return Result.Ok(result);
        }

        [HttpGet("stocks/{id:int:min(1)}")]
        public async Task<Result> GoodsStocks(int id)
        {
            var result = await _productService.GetGoodsStocks(id);
            return Result.Ok(result);
        }

        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<GoodsListResult>>> Grid([FromBody]StandardTableParam<GoodsListQueryParam> param)
        {
            var resultKeyword = await GetKeywordAsync();
            if (!string.IsNullOrWhiteSpace(param?.Search?.Name) && !resultKeyword.HistoryKeywords.Any(c => c.Name == param?.Search?.Name))
            {
                resultKeyword.HistoryKeywords.Insert(0, new Keyword()
                {
                    Name = param?.Search?.Name.Trim()
                });
                if (resultKeyword.HistoryKeywords.Count > 10)
                {
                    resultKeyword.HistoryKeywords = resultKeyword.HistoryKeywords.Take(10).ToList();
                }
                var key = ShopKeys.System + "keywords";
                _staticCacheManager.Set(key, resultKeyword, 30);
            }

            var result = await _productService.HomeList(param);
            return Result.Ok(result);
        }

        [HttpGet("keywords")]
        public async Task<Result> Keywords()
        {
            var result = await GetKeywordAsync();
            return Result.Ok(result);
        }

        [HttpPost("keywords/clear-histories")]
        public async Task<Result> ClearHistoryKeywords()
        {
            var result = await GetKeywordAsync();
            result.HistoryKeywords.Clear();
            var key = ShopKeys.System + "keywords";
            _staticCacheManager.Set(key, result, 30);
            return Result.Ok();
        }

        async Task<KeywordResult> GetKeywordAsync()
        {
            var key = ShopKeys.System + "keywords";
            var result = await _staticCacheManager.GetAsync(key, async () =>
            {
                return await Task.Run(() =>
                {
                    var kr = new KeywordResult();
                    kr.HotKeywords.Add(new Keyword { Name = "1" });
                    kr.HotKeywords.Add(new Keyword { Name = "2" });
                    return kr;
                });
            });
            return result;
        }
    }

    public class Keyword
    {
        public string Name { get; set; }
        public int Heat { get; set; }
    }

    public class KeywordResult
    {
        public Keyword DefaultKeyword { get; set; } = new Keyword() { Name = "测试" };
        public IList<Keyword> HistoryKeywords { get; set; } = new List<Keyword>();
        public IList<Keyword> HotKeywords { get; set; } = new List<Keyword>();
    }
}
