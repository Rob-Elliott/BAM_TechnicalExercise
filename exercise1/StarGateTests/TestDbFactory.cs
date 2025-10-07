using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StarGateTests
{
    public static class TestDbFactory
    {
        /// <summary>
        /// Creates a seeded database in memory
        /// </summary>
        public static StargateContext CreateSqliteInMemoryContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseSqlite(connection)
                .Options;

            var ctx = new StargateContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }
    }
}
