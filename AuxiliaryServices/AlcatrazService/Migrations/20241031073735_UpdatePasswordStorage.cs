using Alcatraz.DTO.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;


#nullable disable

namespace Alcatraz.Context.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePasswordStorage : Migration
    {
        MainDbContext _dbContext;
		public UpdatePasswordStorage(MainDbContext context)
		{
            _dbContext = context;
		}

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
        {
			/* Disabled password updating since v2 services expect the classic passwd format (and causes problems with further migrations).
			 * 
			 *  foreach (var user in _dbContext.Users)
                {
				    user.Password = SecurePasswordHasher.Hash($"{user.Id}-{user.Password}");
			    }
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }

}
