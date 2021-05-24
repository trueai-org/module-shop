using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Services;
using Shop.Module.SampleData.Data;
using Shop.Module.SampleData.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Shop.Module.SampleData.Services
{
    public class SampleDataService : ISampleDataService
    {
        private readonly ISqlRepository _sqlRepository;
        private readonly IMediaService _mediaService;
        private readonly IStaticCacheManager _cache;
        private readonly IOptionsSnapshot<ShopOptions> _options;

        public SampleDataService(
            ISqlRepository sqlRepository,
            IMediaService mediaService,
            IStaticCacheManager cache,
            IOptionsSnapshot<ShopOptions> options)
        {
            _sqlRepository = sqlRepository;
            _mediaService = mediaService;
            _cache = cache;
            _options = options;
        }

        public async Task ResetToSampleData(SampleDataOption model)
        {
            if (_options.Value.ShopEnv == ShopEnv.PRO)
            {
                throw new Exception("正式环境不允许此操作！");
            }

            var usePostgres = _sqlRepository.GetDbConnectionType() == "Npgsql.NpgsqlConnection";
            var useSQLite = _sqlRepository.GetDbConnectionType() == "Microsoft.Data.Sqlite.SqliteConnection";
            var useMySql = _sqlRepository.GetDbConnectionType().Contains("MySql");

            var sampleContentFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleContent", model.Industry);
            var filePath = usePostgres ? Path.Combine(sampleContentFolder, "ResetToSampleData_Postgres.sql") :
                useSQLite ? Path.Combine(sampleContentFolder, "ResetToSampleData_SQLite.sql") :
                useMySql ? Path.Combine(sampleContentFolder, "ResetToSampleData_MySql.sql") :
                Path.Combine(sampleContentFolder, "ResetToSampleData.sql");

            var lines = File.ReadLines(filePath);
            var commands = usePostgres || useSQLite ? _sqlRepository.PostgresCommands(lines) :
                  useMySql ? _sqlRepository.MySqlCommand(lines) :
                  _sqlRepository.ParseCommand(lines);
            _sqlRepository.RunCommands(commands);

            await CopyImages(sampleContentFolder);

            _cache.Clear();
        }

        private async Task CopyImages(string sampleContentFolder)
        {
            var imageFolder = Path.Combine(sampleContentFolder, "Images");
            IEnumerable<string> files = Directory.GetFiles(imageFolder);
            foreach (var file in files)
            {
                using (var stream = File.OpenRead(file))// File.Open(file, FileMode.Open, FileAccess.Read)
                {
                    await _mediaService.SaveMediaAsync(stream, Path.GetFileName(file));
                }
            }
        }
    }
}
