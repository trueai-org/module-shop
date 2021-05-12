using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services
{
    public class MediaService : IMediaService
    {
        private readonly IRepository<Media> _mediaRepository;
        private readonly IStorageService _storageService;

        public MediaService(IRepository<Media> mediaRepository, IStorageService storageService)
        {
            _mediaRepository = mediaRepository;
            _storageService = storageService;
        }

        public async Task<string> GetMediaUrl(Media media)
        {
            if (media == null)
            {
                return await GetMediaUrl(GlobalConfiguration.NoImage);
            }

            return await GetMediaUrl(media.FileName);
        }

        public async Task<string> GetMediaUrl(string fileName)
        {
            return await _storageService.GetMediaUrl(fileName);
        }

        public async Task<string> GetThumbnailUrl(Media media)
        {
            return await GetMediaUrl(media);
        }

        public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null, long fileSize = 0)
        {
            var media = await _storageService.SaveMediaAsync(mediaBinaryStream, fileName, mimeType);
            if (media == null)
                throw new ArgumentNullException(nameof(media));
            return media;
        }

        public Task DeleteMediaAsync(Media media)
        {
            _mediaRepository.Remove(media);
            return DeleteMediaAsync(media.FileName);
        }

        public Task DeleteMediaAsync(string fileName)
        {
            return _storageService.DeleteMediaAsync(fileName);
        }
    }
}
