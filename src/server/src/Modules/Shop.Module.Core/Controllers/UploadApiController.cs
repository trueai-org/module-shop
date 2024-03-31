using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Shop.Module.Core.Controllers
{
    /// <summary>
    /// 上传服务相关 API
    /// </summary>
    [ApiController]
    [Route("api/upload")]
    [Authorize()] //Roles = "admin"
    public class UploadApiController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IStorageService _storageService;

        public UploadApiController(
            IMediaService mediaService,
            IStorageService storageService)
        {
            _mediaService = mediaService;
            _storageService = storageService;
        }

        /// <summary>
        /// 单文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<Result> Upload(IFormFile file)
        {
            if (file == null)
            {
                return Result.Fail("Please select upload file");
            }
            var headerValue = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var originalFileName = headerValue.FileName.Trim('"');
            var fileName = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(originalFileName)}";

            var result = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, file.ContentType, file.Length);

            var url = await _storageService.GetMediaUrl(result.FileName);
            return Result.Ok(new
            {
                result.Id,
                result.FileName,
                url
            });
        }

        /// <summary>
        /// 多文件上传
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost("multiple")]
        public async Task<Result> MultiUpload(IFormCollection formCollection)
        {
            if (formCollection == null || formCollection.Files.Count <= 0)
            {
                return Result.Fail("Please select upload files");
            }

            var error = string.Empty;
            var list = new List<Media>();
            foreach (var file in formCollection.Files)
            {
                try
                {
                    var headerValue = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var originalFileName = headerValue.FileName.Trim('"');
                    var fileName = $"{Guid.NewGuid().ToString("N")}{Path.GetExtension(originalFileName)}";
                    var media = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, file.ContentType, file.Length);
                    list.Add(media);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            if (list.Count > 0)
            {
                var result = Result.Ok(list,
                    list.Count != formCollection.Files.Count ? $"成功: {list.Count}, 失败: {formCollection.Files.Count - list.Count}" : "全部上传完成");
                return result;
            }
            return Result.Fail(error);
        }
    }
}
