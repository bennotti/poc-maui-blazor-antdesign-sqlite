using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PocMaui.Core.Contract.Infra.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMaui.Infra.Data.Sqlite
{
    public class SqliteDatabaseFactory : IDatabaseFactory
    {
        private readonly IOptions<DatabaseSettings> _dataSettings;
        public IDbConnection GetDbConnection => new SqliteConnection(_dataSettings.Value.Connection);

        public SqliteDatabaseFactory(IOptions<DatabaseSettings> dataSettings)
        {
            _dataSettings = dataSettings;
        }
    }
}
