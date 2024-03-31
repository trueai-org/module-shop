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
    /// <summary>
    /// 商品 API 控制器，提供商品相关的 API 接口。
    /// </summary>
    [ApiController]
    [Route("api/goods")]
    public class GoodsApiController
    {
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;

        /// <summary>
        /// 构造函数，注入商品服务和静态缓存管理器。
        /// </summary>
        /// <param name="productService">商品服务接口。</param>
        /// <param name="staticCacheManager">静态缓存管理器接口。</param>
        public GoodsApiController(
            IProductService productService,
            IStaticCacheManager staticCacheManager)
        {
            _productService = productService;
            _staticCacheManager = staticCacheManager;
        }

        /// <summary>
        /// 根据商品 ID 获取商品详细信息。
        /// </summary>
        /// <param name="id">商品 ID。</param>
        /// <returns>商品详细信息。</returns>
        [HttpGet("{id:int:min(1)}")]
        public async Task<Result> Get(int id)
        {
            var result = await _productService.GetGoodsByCache(id);
            return Result.Ok(result);
        }

        /// <summary>
        /// 获取与指定商品相关的商品列表。
        /// </summary>
        /// <param name="id">商品 ID。</param>
        /// <returns>相关商品列表。</returns>
        [HttpGet("related/{id:int:min(1)}")]
        public async Task<Result> Related(int id)
        {
            var result = await _productService.RelatedList(id);
            return Result.Ok(result);
        }

        /// <summary>
        /// 获取指定商品的库存信息。
        /// </summary>
        /// <param name="id">商品 ID。</param>
        /// <returns>商品库存信息。</returns>
        [HttpGet("stocks/{id:int:min(1)}")]
        public async Task<Result> GoodsStocks(int id)
        {
            var result = await _productService.GetGoodsStocks(id);
            return Result.Ok(result);
        }

        /// <summary>
        /// 根据查询参数获取商品列表，支持分页。
        /// </summary>
        /// <param name="param">商品列表查询参数。</param>
        /// <returns>标准表格结果，包含商品列表。</returns>
        [HttpPost("grid")]
        public async Task<Result<StandardTableResult<GoodsListResult>>> Grid([FromBody] StandardTableParam<GoodsListQueryParam> param)
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

        /// <summary>
        /// 获取热门关键词和搜索历史。
        /// </summary>
        /// <returns>关键词结果，包含热门关键词和搜索历史。</returns>
        [HttpGet("keywords")]
        public async Task<Result> Keywords()
        {
            var result = await GetKeywordAsync();
            return Result.Ok(result);
        }

        /// <summary>
        /// 清除搜索历史。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpPost("keywords/clear-histories")]
        public async Task<Result> ClearHistoryKeywords()
        {
            var result = await GetKeywordAsync();
            result.HistoryKeywords.Clear();
            var key = ShopKeys.System + "keywords";
            _staticCacheManager.Set(key, result, 30);
            return Result.Ok();
        }

        /// <summary>
        /// 异步获取关键词结果，包含热门关键词和搜索历史。
        /// </summary>
        /// <returns>关键词结果。</returns>
        private async Task<KeywordResult> GetKeywordAsync()
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

    /// <summary>
    /// 关键词实体，用于搜索功能。
    /// </summary>
    public class Keyword
    {
        public string Name { get; set; }
        public int Heat { get; set; }
    }

    /// <summary>
    /// 关键词结果实体，包含默认关键词、历史关键词和热门关键词。
    /// </summary>
    public class KeywordResult
    {
        public Keyword DefaultKeyword { get; set; } = new Keyword() { Name = "测试" };
        public IList<Keyword> HistoryKeywords { get; set; } = new List<Keyword>();
        public IList<Keyword> HotKeywords { get; set; } = new List<Keyword>();
    }
}
