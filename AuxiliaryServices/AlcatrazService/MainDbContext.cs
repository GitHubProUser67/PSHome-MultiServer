using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Alcatraz.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using MultiServerLibrary.Extension.LinqSQL;

namespace AlcatrazService
{
    // TO run migrations:
    // EntityFrameworkCore\Add-Migration NAME -Project AlcatrazService -StartupProject MultiServerWebServices -Context MainDbContext

    public class MainDbContext : DbContext
    {
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

        public static DbContextOptions<MainDbContext> BuildOptions(
            DBType type,
            string connectionString
        )
        {
            var builder = new DbContextOptionsBuilder<MainDbContext>();

            OnContextBuilding(builder, type, connectionString);

            return builder.Options;
        }

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public MainDbContext()
            : base() { }

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options) { }

        public static Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }

        //------------------------------------------------------------------------------------------
        // Model relations comes here

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserRelationship>().HasKey(t => new { t.User1Id, t.User2Id });

            builder
                .Entity<PlayerStatisticsBoardValue>()
                .HasOne(rp => rp.PlayerBoard)
                .WithMany(r => r.Values)
                .HasForeignKey(rp => rp.PlayerBoardId);

            base.OnModelCreating(builder);
        }

        //------------------------------------------------------------------------------------------
        // Database tables itself

        // USERS
        public DbSet<User> Users { get; set; }
        public DbSet<UserRelationship> UserRelationships { get; set; }

        public DbSet<PlayerStatisticsBoard> PlayerStatisticBoards { get; set; }
        public DbSet<PlayerStatisticsBoardValue> PlayerStatisticBoardValues { get; set; }
    }

    public class ContextAwareMigrationsAssembly : MigrationsAssembly
    {
        private readonly MainDbContext context;

        public ContextAwareMigrationsAssembly(
            ICurrentDbContext currentContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger
        )
            : base(currentContext, options, idGenerator, logger)
        {
            context = (MainDbContext)currentContext.Context;
        }

        /// <summary>
        /// Modified from https://web.archive.org/web/20181021034610/http://weblogs.thinktecture.com/pawel/2018/06/entity-framework-core-changing-db-migration-schema-at-runtime.html
        /// </summary>
        /// <param name="migrationClass"></param>
        /// <param name="activeProvider"></param>
        /// <returns></returns>
        public override Migration CreateMigration(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                TypeInfo migrationClass,
            string activeProvider
        )
        {
            var hasCtorWithDbContext =
                migrationClass.GetConstructor(new[] { typeof(MainDbContext) }) != null;

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
