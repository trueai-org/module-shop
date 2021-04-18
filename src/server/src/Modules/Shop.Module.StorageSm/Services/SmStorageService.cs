using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Abstractions.Entities;
using Shop.Module.Core.Abstractions.Models;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.StorageSm.Data;
using Shop.Module.StorageSm.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shop.Module.StorageSm.Services
{
    /// <summary>
    /// https://sm.ms/doc/
    /// </summary>
    public class SmStorageService : IStorageService
    {
        private readonly IRepository<Media> _mediaRepository;
        private readonly IAppSettingService _appSettingService;

        public SmStorageService(
            IRepository<Media> mediaRepository,
            IAppSettingService appSettingService)
        {
            _mediaRepository = mediaRepository;
            _appSettingService = appSettingService;
        }

        public async Task DeleteMediaAsync(string fileName)
        {
            await Task.CompletedTask;
        }

        public async Task<string> GetMediaUrl(string fileName)
        {
            return await _mediaRepository.Query().Where(c => c.FileName == fileName).Select(c => c.Url).FirstOrDefaultAsync();
        }

        public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
        {
            var bytes = new byte[mediaBinaryStream.Length];
            using (mediaBinaryStream)
            {
                mediaBinaryStream.Read(bytes, 0, bytes.Length);
            }
            var hsMd5 = Md5Helper.Encrypt(bytes);
            var media = await _mediaRepository.Query(c => c.Md5 == hsMd5).FirstOrDefaultAsync();
            if (media != null)
                return media;

            var result = await Task.Run(() =>
            {
                return Upload(bytes, hsMd5, fileName);
            });
            if (result?.Data == null)
            {
                return null;
            }
            var cusDomain = await _appSettingService.Get<StorageSmOptions>();
            var url = result.Data.Url;
            if (!string.IsNullOrWhiteSpace(cusDomain?.CustomDomain))
            {
                url = $"{cusDomain.CustomDomain.Trim('/')}/{result.Data.Path.Trim('/')}";
            }
            media = new Media()
            {
                MediaType = MediaType.File,
                FileName = result.Data.Filename,
                FileSize = result.Data.Size,
                Hash = result.Data.Hash,
                Url = url,
                Path = result.Data.Path,
                Host = StorageSmKeys.Host,
                Md5 = hsMd5
            };
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                mimeType = mimeType.Trim().ToLower();
                if (mimeType.StartsWith("video"))
                {
                    media.MediaType = MediaType.Video;
                }
                else if (mimeType.StartsWith("image"))
                {
                    media.MediaType = MediaType.Image;
                }
                else
                {
                    media.MediaType = MediaType.File;
                }
            }
            _mediaRepository.Add(media);
            await _mediaRepository.SaveChangesAsync();
            return media;
        }

        private async Task<SmResult> Upload(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            byte[] bytes;
            var uploadFileName = Path.GetFileName(filePath);
            using (var fileStream = File.OpenRead(filePath))
            {
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
            }
            var hsMd5 = Md5Helper.Encrypt(bytes);

            return await Upload(bytes, hsMd5, uploadFileName);
        }

        private async Task<SmResult> Upload(byte[] bytes, string hsMd5, string uploadFileName)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (uploadFileName == null)
                throw new ArgumentNullException(nameof(uploadFileName));

            var oriFileName = Path.GetFileName(uploadFileName);
            var client = new RestClient(StorageSmKeys.Host);
            var request = new RestRequest(Method.POST);
            request.AddFile("smfile", bytes, oriFileName);

            // or
            // request.AddFile("smfile", filePath);

            var response = await client.ExecutePostTaskAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<SmResult>(response.Content);
            }
            else
            {
                throw new Exception("Upload Errord. " + response.Content);
            }
        }
    }
}
