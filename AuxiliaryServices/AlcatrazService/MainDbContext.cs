using Alcatraz.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using MultiServerLibrary.Extension.LinqSQL;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Alcatraz.Context
{
    // TO run migrations:
    // EntityFrameworkCore\Add-Migration NAME -Project AlcatrazService -StartupProject MultiServerWebServices -Context MainDbContext

    public class MainDbContext : DbContext
	{
        public static DbContextOptionsBuilder OnContextBuilding(DbContextOptionsBuilder opt, DBType type, string connectionString)
		{
			opt.ReplaceService<IMigrationsAssembly, ContextAwareMigrationsAssembly>();

            if (type == DBType.SQLite)
                return opt.UseSqlite(connectionString);

            else if (type == DBType.MySQL)
                return opt.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 25)), conf => conf.CommandTimeout(60));

            return opt;
		}
		public MainDbContext()
			: base()
		{
		}

		public MainDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public Task EnsureSeedData()
		{
			return Task.CompletedTask;
		}

		//------------------------------------------------------------------------------------------
		// Model relations comes here

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<UserRelationship>()
				.HasKey(t => new { t.User1Id, t.User2Id });

			builder.Entity<PlayerStatisticsBoardValue>()
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
			IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
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
        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
		{
			var hasCtorWithDbContext = migrationClass
					.GetConstructor(new[] { typeof(MainDbContext) }) != null;

			if (hasCtorWithDbContext)
			{
				var instance = (Migration)Activator.CreateInstance(migrationClass.AsType(), context);
				instance.ActiveProvider = activeProvider;
				return instance;
			}

			return base.CreateMigration(migrationClass, activeProvider);
		}
	}

}
