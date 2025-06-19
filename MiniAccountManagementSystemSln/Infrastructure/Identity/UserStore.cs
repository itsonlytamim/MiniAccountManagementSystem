using Core.Entities;
using Dapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{

        public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
        {
            private readonly DbContext _context;

            public UserStore(DbContext context)
            {
                _context = context;
            }

            #region IUserStore Implementation (Core Functionality)

            public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    // We only pass parameters for columns that exist in our custom Users table schema.
                    await connection.ExecuteAsync("sp_User_Create", new
                    {
                        user.Id,
                        user.UserName,
                        user.NormalizedUserName,
                        user.PasswordHash,
                        user.SecurityStamp,
                        user.ConcurrencyStamp
                        // We explicitly OMIT email, phone number, etc., as they are not in our core design.
                    }, commandType: CommandType.StoredProcedure);
                }
                return IdentityResult.Success;
            }

            public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("sp_User_Delete", new { user.Id }, commandType: CommandType.StoredProcedure);
                }
                return IdentityResult.Success;
            }

            public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_User_FindById", new { Id = userId }, commandType: CommandType.StoredProcedure);
                }
            }

            public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QuerySingleOrDefaultAsync<ApplicationUser>("sp_User_FindByName", new { NormalizedUserName = normalizedUserName }, commandType: CommandType.StoredProcedure);
                }
            }

            public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
            public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
            public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken) { user.UserName = userName; return Task.CompletedTask; }
            public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
            public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken) { user.NormalizedUserName = normalizedName; return Task.CompletedTask; }

            public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("sp_User_Update", new
                    {
                        user.Id,
                        user.UserName,
                        user.NormalizedUserName,
                        user.PasswordHash,
                        user.SecurityStamp,
                        user.ConcurrencyStamp
                    }, commandType: CommandType.StoredProcedure);
                }
                return IdentityResult.Success;
            }

            #endregion

            #region IUserPasswordStore Implementation (Required for Login)

            public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
            {
                user.PasswordHash = passwordHash;
                return Task.CompletedTask;
            }

            public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);
            public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash != null);

            #endregion

            #region IUserRoleStore Implementation (Required for Authorization)

            public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    // Note: The SP uses the normalized role name for lookup, which is a best practice.
                    var normalizedRoleName = roleName.ToUpperInvariant();
                    await connection.ExecuteAsync("sp_User_AddToRole", new { UserId = user.Id, RoleName = normalizedRoleName }, commandType: CommandType.StoredProcedure);
                }
            }

            public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    var normalizedRoleName = roleName.ToUpperInvariant();
                    await connection.ExecuteAsync("sp_User_RemoveFromRole", new { UserId = user.Id, RoleName = normalizedRoleName }, commandType: CommandType.StoredProcedure);
                }
            }

            public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    var roles = await connection.QueryAsync<string>("sp_User_GetRoles", new { UserId = user.Id }, commandType: CommandType.StoredProcedure);
                    return roles.AsList();
                }
            }

            public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = _context.CreateConnection())
                {
                    var normalizedRoleName = roleName.ToUpperInvariant();
                    var count = await connection.ExecuteScalarAsync<int>("sp_User_IsInRole", new { UserId = user.Id, RoleName = normalizedRoleName }, commandType: CommandType.StoredProcedure);
                    return count > 0;
                }
            }

            public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
            {
                // This method is not required for the core features of this project.
                // Throwing NotImplementedException is appropriate.
                throw new NotImplementedException("This feature is not required by the project specification.");
            }

            #endregion

            public void Dispose() { }
        }
 }

