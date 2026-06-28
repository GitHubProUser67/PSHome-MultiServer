using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using MultiServerLibrary.Extension.LinqSQL;
using WebAPIService.LeaderboardService.Context.Entities;

namespace WebAPIService.LeaderboardService
{
    // TO run migrations:
    // EntityFrameworkCore\Add-Migration NAME -Project WebAPIService -StartupProject MultiServerWebServices -Context LeaderboardDbContext

    public class LeaderboardDbContext : DbContext
    {
        private static readonly Type BaseType = typeof(ScoreboardEntryBase);
        private static List<Type> _discoveredEntryTypes;

        public static DbContextOptionsBuilder OnContextBuilding(
            DbContextOptionsBuilder opt,
            DBType type,
            string connectionString
        )
        {
            opt.ReplaceService<IMigrationsAssembly, ContextAwareMigrationsAssembly>();

            return type switch
            {
                DBType.SQLite => opt.UseSqlite(connectionString),
                DBType.MySQL => opt.UseMySql(
                    connectionString,
                    new MySqlServerVersion(new Version(8, 0, 25)),
                    conf => conf.CommandTimeout(60)
                ),
                _ => opt,
            };
        }

        public static DbContextOptions<LeaderboardDbContext> BuildOptions(
            DBType type,
            string connectionString
        )
        {
            var builder = new DbContextOptionsBuilder<LeaderboardDbContext>();

            OnContextBuilding(builder, type, connectionString);

            return builder.Options;
        }

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public LeaderboardDbContext()
            : base() { }

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public LeaderboardDbContext(DbContextOptions<LeaderboardDbContext> options)
            : base(options) { }

        public static string GetDefaultDbPath()
        {
            var dbDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                "static",
                "wwwapiroot",
                "LeaderboardsService"
            );
            Directory.CreateDirectory(dbDir);

            return Path.Combine(dbDir, "Leaderboards.sqlite");
        }

        public static Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }

        //------------------------------------------------------------------------------------------
        // Model relations comes here

        protected override void OnModelCreating(ModelBuilder builder)
        {
            _discoveredEntryTypes =
            [
                .. AppDomain
                    .CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && BaseType.IsAssignableFrom(t)),
            ];

            foreach (var type in _discoveredEntryTypes)
            {
                var entityBuilder = builder.Entity(type);

                entityBuilder.ToTable(type.Name);

                entityBuilder.HasKey("Id");

                entityBuilder.Property("PlayerId").IsRequired();
                entityBuilder.Property("Score").IsRequired();

                entityBuilder.Property("ExtraData1").HasMaxLength(255).IsRequired(false);
                entityBuilder.Property("ExtraData2").HasMaxLength(255).IsRequired(false);
                entityBuilder.Property("ExtraData3").HasMaxLength(255).IsRequired(false);
                entityBuilder.Property("ExtraData4").HasMaxLength(255).IsRequired(false);
                entityBuilder.Property("ExtraData5").HasMaxLength(255).IsRequired(false);
            }

            base.OnModelCreating(builder);
        }
    }

    public class ContextAwareMigrationsAssembly(
        ICurrentDbContext currentContext,
        IDbContextOptions options,
        IMigrationsIdGenerator idGenerator,
        IDiagnosticsLogger<DbLoggerCategory.Migrations> logger
    ) : MigrationsAssembly(currentContext, options, idGenerator, logger)
    {
        private readonly LeaderboardDbContext context = (LeaderboardDbContext)
            currentContext.Context;

        /// <summary>
        /// Modified from https://web.archive.org/web/20181021034610/http://weblogs.thinktecture.com/pawel/2018/06/entity-framework-core-changing-db-migration-schema-at-runtime.html
        /// </summary>
        /// <param name="migrationClass"></param>
        /// <param name="activeProvider"></param>
        /// <returns></returns>
        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            var hasCtorWithDbContext =
                migrationClass.GetConstructor([typeof(LeaderboardDbContext)]) != null;

            if (hasCtorWithDbContext)
            {
                var instance = (Migration)
                    Activator.CreateInstance(migrationClass.AsType(), context);
                instance.ActiveProvider = activeProvider;
                return instance;
            }

            return base.CreateMigration(migrationClass, activeProvider);
        }
    }
}
