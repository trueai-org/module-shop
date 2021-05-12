using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public interface ICountryService
    {
        Task<IList<StateOrProvinceDto>> GetProvinceByCache(int countryId);

        Task ClearProvinceCache(int countryId);

        IList<ProvinceTreeResult> ProvinceTree(IList<StateOrProvinceDto> list, int? parentId = null);

        /// <summary>
        /// 省市区 例：8 转换为 ['1', '6', '8']
        /// </summary>
        /// <param name="provinces"></param>
        /// <param name="stateOrProvinceId"></param>
        /// <param name="list"></param>
        /// <param name="loop"></param>
        void ProvincesTransformToStringArray(IList<StateOrProvinceDto> provinces, int stateOrProvinceId, ref IList<string> list, int loop = 0);
    }
}
