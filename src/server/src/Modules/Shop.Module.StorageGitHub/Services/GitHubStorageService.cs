using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.StorageGitHub.Models;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Shop.Module.StorageGitHub
{
    /// <summary>
    /// https://developer.github.com/v3/
    /// https://developer.github.com/v3/repos/contents/
    /// </summary>
    public class GitHubStorageService : IStorageService
    {
        private const string ContentHost = "https://raw.githubusercontent.com";

        private readonly IOptionsMonitor<StorageGitHubOptions> _options;
        private readonly IRepository<Media> _mediaRepository;

        public GitHubStorageService(
            IOptionsMonitor<StorageGitHubOptions> options,
            IRepository<Media> mediaRepository)
        {
            _options = options;
            _mediaRepository = mediaRepository;
        }

        public async Task DeleteMediaAsync(string fileName)
        {
            await Task.CompletedTask;
        }

        public async Task<string> GetMediaUrl(string fileName)
        {
            var options = _options.CurrentValue;

            if (options == null || string.IsNullOrWhiteSpace(fileName))
                return string.Empty;
            var res = options.RepositoryName.Trim().Trim('/');
            var bra = options.BranchName.Trim().Trim('/');
            var path = options.SavePath.Trim().Trim('/');
            var pathInName = $"{fileName.Substring(0, 2)}/{fileName.Substring(2, 2)}/{fileName.Substring(4, 2)}";

            //https://raw.githubusercontent.com/trueai-org/data/master/images/44/b7/96/44b796c968f4703a871b0f6f06fe85b1636844361657344457.jpg
            return $"{ContentHost.TrimEnd('/')}/{res}/{bra}/{path}/{pathInName}/{fileName}";
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
            if (result == null || result.Content == null || string.IsNullOrWhiteSpace(result.Content.DownloadUrl))
            {
                return null;
            }
            media = new Media()
            {
                MediaType = MediaType.File,
                FileName = result.Content.Name,
                FileSize = result.Content.Size,
                Hash = result.Content.Sha,
                Url = result.Content.DownloadUrl,
                Path = result.Content.Path?.Trim('/'),
                Host = await GetHostForUrlFrefix(),
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

        private async Task<GitHubDataResult> Upload(string filePath)
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

        private async Task<string> GetHostForUrlFrefix()
        {
            var options = _options.CurrentValue;
            if (options == null)
                return string.Empty;
            var res = options.RepositoryName.Trim().Trim('/');
            var bra = options.BranchName.Trim().Trim('/');
            return $"{ContentHost.TrimEnd('/')}/{res}/{bra}";
        }

        private async Task<GitHubDataResult> Upload(byte[] bytes, string hsMd5, string uploadFileName)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (string.IsNullOrWhiteSpace(hsMd5))
                throw new ArgumentNullException(nameof(hsMd5));
            if (uploadFileName == null)
                throw new ArgumentNullException(nameof(uploadFileName));

            var setting = _options.CurrentValue;
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));
            if (setting.Host == null)
                throw new ArgumentNullException(nameof(setting.Host));
            if (setting.RepositoryName == null)
                throw new ArgumentNullException(nameof(setting.RepositoryName));
            if (setting.BranchName == null)
                throw new ArgumentNullException(nameof(setting.BranchName));
            if (setting.PersonalAccessToken == null)
                throw new ArgumentNullException(nameof(setting.PersonalAccessToken));
            if (string.IsNullOrWhiteSpace(setting.SavePath))
                setting.SavePath = "/";

            var base64String = Convert.ToBase64String(bytes);

            var oriFileName = Path.GetFileName(uploadFileName);
            // .gif
            var extens = Path.GetExtension(oriFileName);
            // 4d6d5a97f2d9fd752a7802db51ea582a636826260650135261.jpg
            var fileName = $"{hsMd5}{DateTime.Now.Ticks}{extens}";

            // images/4d/6d/5a
            var path = $"{setting.SavePath.TrimEnd('/')}/{fileName.Substring(0, 2)}/{fileName.Substring(2, 2)}/{fileName.Substring(4, 2)}".TrimStart('/');

            var repo = setting.RepositoryName;
            var branch = setting.BranchName;
            var token = setting.PersonalAccessToken;
            var url = setting.Host + $"repos/{repo}/contents/{path}/{fileName}";
            var uri = new Uri(url);
            var body = new
            {
                message = "",
                branch = branch,
                content = base64String,
                path = $"{path}{fileName}"
            };

            var client = new RestClient(uri);
            var request = new RestRequest()
            {
                 Method = Method.Put
            };
            request.AddHeader("Authorization", "token " + token);
            request.AddJsonBody(body);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var data = JsonConvert.DeserializeObject<GitHubDataResult>(response.Content);
                if (data != null && data.Content != null && !string.IsNullOrWhiteSpace(data.Content.DownloadUrl))
                {
                    return data;
                }
                else
                {
                    throw new Exception("Upload Errord. " + data?.Content);
                }
            }
            throw new Exception("Upload Errord. " + response.Content);
        }

    }
}
