using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop.Module.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Module.SampleData.Data
{
    public class SqlRepository : ISqlRepository
    {
        private readonly DbContext _dbContext;
        private readonly ILogger _logger;

        public SqlRepository(ShopDbContext dbContext, ILogger<SqlRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void RunCommand(string command)
        {
            _logger.LogDebug(command);

            //_dbContext.Database.ExecuteSqlCommand(command);

            // 使用 ExecuteSqlRaw 替换 ExecuteSqlCommand
            _dbContext.Database.ExecuteSqlRaw(command);
        }

        public void RunCommands(IEnumerable<string> commands)
        {
            using (var tran = _dbContext.Database.BeginTransaction())
            {
                foreach (var command in commands)
                {
                    //_dbContext.Database.ExecuteSqlCommand(command);

                    // 使用 ExecuteSqlRaw 替换 ExecuteSqlCommand
                    _dbContext.Database.ExecuteSqlRaw(command);
                }
                tran.Commit();
            }
        }

        public IEnumerable<string> ParseCommand(IEnumerable<string> lines)
        {
            var sb = new StringBuilder();
            var commands = new List<string>();
            foreach (var line in lines)
            {
                if (string.Equals(line, "GO", StringComparison.OrdinalIgnoreCase))
                {
                    if (sb.Length > 0)
                    {
                        // JSON 特殊处理 
                        // N'{"CategoryIds":[9,10,15,14,5]}'
                        // N'{{"CategoryIds":[9,10,15,14,5]}}'
                        var sql = sb.ToString().Replace("{", "{{").Replace("}", "}}");
                        commands.Add(sql);

                        sb = new StringBuilder();
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        sb.Append(line);
                    }
                }
            }

            return commands;
        }

        public IEnumerable<string> MySqlCommand(IEnumerable<string> lines)
        {
            var commands = new List<string>();
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine(line.Replace("{", "{{").Replace("}", "}}"));
                }
            }

            commands.Add(sb.ToString());

            return commands;
        }

        public IEnumerable<string> PostgresCommands(IEnumerable<string> lines)
        {
            var commands = new List<string>();
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    commands.Add(line);
                }
            }

            return commands;
        }

        public string GetDbConnectionType()
        {
            var dbConntionType = _dbContext.Database.GetDbConnection().GetType();
            return dbConntionType.ToString();
        }
    }
}
