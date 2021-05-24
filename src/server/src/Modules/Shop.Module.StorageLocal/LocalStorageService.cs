using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using System.IO;
using System.Threading.Tasks;

namespace Shop.Module.StorageLocal
{
    public class LocalStorageService : IStorageService
    {
        private const string MediaRootFoler = "user-content";

        private readonly string host = "";
        private readonly IRepository<Media> _mediaRepository;

        public LocalStorageService(
             IRepository<Media> mediaRepository,
             IOptionsMonitor<ShopOptions> options)
        {
            host = options.CurrentValue.ApiHost;
            _mediaRepository = mediaRepository;
        }

        public async Task DeleteMediaAsync(string fileName)
        {
            var filePath = Path.Combine(GlobalConfiguration.WebRootPath, MediaRootFoler, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public async Task<string> GetMediaUrl(string fileName)
        {
            if (fileName == GlobalConfiguration.NoImage)
            {
                return await Task.FromResult($"{host.Trim('/')}/{fileName}");
            }

            return await Task.FromResult($"{host.Trim('/')}/{MediaRootFoler}/{fileName}");
        }

        public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
        {
            var hsMd5 = string.Empty;
            var size = 0;
            Media media = null;

            var filePath = Path.Combine(GlobalConfiguration.WebRootPath, MediaRootFoler, fileName);
            using (var output = new FileStream(filePath, FileMode.Create))
            {
                //if (!File.Exists(filePath))

                await mediaBinaryStream.CopyToAsync(output);

                var bytes = new byte[mediaBinaryStream.Length];
                mediaBinaryStream.Read(bytes, 0, bytes.Length);
                hsMd5 = Md5Helper.Encrypt(bytes);
                size = bytes.Length;
            }

            if (!string.IsNullOrWhiteSpace(hsMd5))
            {
                media = await _mediaRepository.Query(c => c.Md5 == hsMd5 || c.FileName == fileName).FirstOrDefaultAsync();
            }

            if (media == null)
            {
                media = new Media()
                {
                    MediaType = MediaType.File,
                    FileName = fileName,
                    FileSize = size,
                    Hash = "",
                    Url = $"{host.Trim('/')}/{MediaRootFoler}/{fileName}",
                    Path = $"/{MediaRootFoler}/{fileName}",
                    Host = host,
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
            }
            else
            {
                media.Url = $"{host.Trim('/')}/{MediaRootFoler}/{fileName}";
                media.Path = $"/{MediaRootFoler}/{fileName}";
                media.Host = host;
            }

            await _mediaRepository.SaveChangesAsync();
            return media;
        }
    }
}