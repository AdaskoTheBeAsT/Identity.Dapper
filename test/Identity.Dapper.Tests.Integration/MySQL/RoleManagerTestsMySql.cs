using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Identity.Dapper.Tests.Integration.MySQL
{
    [Collection(nameof(MySQL))]
    [TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    public class RoleManagerTestsMySql : IClassFixture<MySqlDatabaseFixture>
    {
        private readonly MySqlDatabaseFixture _databaseFixture;
        private readonly RoleManager<DapperIdentityRole> _roleManager;
        private readonly UserManager<DapperIdentityUser> _userManager;

        public RoleManagerTestsMySql(MySqlDatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture ?? throw new ArgumentNullException(nameof(databaseFixture));
            _roleManager = (RoleManager<DapperIdentityRole>)_databaseFixture.TestServer.Host.Services.GetService(typeof(RoleManager<DapperIdentityRole>));
            _userManager = (UserManager<DapperIdentityUser>)_databaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
        }

        [Fact]
        [TestPriority(100)]
        public async Task CanCreateAsync()
        {
            var result = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test" });
            var result2 = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test2" });
            var result3 = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test3" });
            var result4 = await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test5" });

            Assert.True(result.Succeeded);
            Assert.True(result2.Succeeded);
            Assert.True(result3.Succeeded);
            Assert.True(result4.Succeeded);
        }

        [Fact]
        [TestPriority(101)]
        public async Task CanFindByNameAsync()
        {
            var role = await _roleManager.FindByNameAsync("test");

            Assert.NotNull(role);
        }

        [Fact]
        [TestPriority(102)]
        public async Task CanFindByIdAsync()
        {
            var role = await _roleManager.FindByIdAsync(1.ToString(CultureInfo.InvariantCulture));

            Assert.NotNull(role);
        }

        [Fact]
        [TestPriority(103)]
        public async Task CanRemoveAsync()
        {
            await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test4" });

            var role = await _roleManager.FindByNameAsync("test4");

            Assert.NotNull(role);

            var result = await _roleManager.DeleteAsync(role);

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(104)]
        public async Task CanRoleExistsAsync()
        {
            var result = await _roleManager.RoleExistsAsync("test");

            Assert.True(result);
        }

        [Fact]
        [TestPriority(105)]
        public async Task CanUpdateAsync()
        {
            var role = await _roleManager.FindByNameAsync("test");
            role.Name = "testmodified";

            var result = await _roleManager.UpdateAsync(role);

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(106)]
        public async Task CanAddRoleToUserAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "testrole", Email = "test@test.com" }, "123456");

            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.AddToRoleAsync(user, "testmodified");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(107)]
        public async Task CanAddRolesToUserAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.AddToRolesAsync(user, new[] { "test2", "test3", "test5" });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(108)]
        public async Task CanGetRolesAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.GetRolesAsync(user);

            Assert.Contains(
                result,
                x => x.Equals("TESTMODIFIED", StringComparison.OrdinalIgnoreCase) ||
                     x.Equals("TEST2", StringComparison.OrdinalIgnoreCase) ||
                     x.Equals("TEST3", StringComparison.OrdinalIgnoreCase) ||
                     x.Equals("TEST5", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(109)]
        public async Task CanGetUsersInRoleAsync()
        {
            var result = await _userManager.GetUsersInRoleAsync("testmodified");

            Assert.Collection(result, x => x.UserName.Equals("testrole", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(110)]
        public async Task CanCheckIfUserIsInRoleAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.IsInRoleAsync(user, "testmodified");

            Assert.True(result);
        }

        [Fact]
        [TestPriority(111)]
        public async Task CanRemoveUserFromRoleAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.RemoveFromRoleAsync(user, "testmodified");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(112)]
        public async Task CanRemoveUserFromRolesAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.RemoveFromRolesAsync(user, new[] { "test2", "test3" });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(113)]
        public async Task FindByEmailReturnRolesAsync()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(114)]
        public async Task FindByNameReturnRolesAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(115)]
        public async Task FindByIdReturnRolesAsync()
        {
            var user = await _userManager.FindByIdAsync("1");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(116)]
        public async Task FindByLoginReturnRolesAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "testrole2", Email = "test2@test.com" }, "123456");

            var user = await _userManager.FindByNameAsync("testrole2");

            await _userManager.AddToRoleAsync(user, "test5");

            await _userManager.AddLoginAsync(user, new UserLoginInfo("mylogin", "mylogin", "mylogin"));

            var user2 = await _userManager.FindByLoginAsync("mylogin", "mylogin");

            Assert.Collection(user2.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(117)]
        public async Task CanAddRoleClaimAsync()
        {
            var role = await _roleManager.FindByNameAsync("test3");

            var result1 = await _roleManager.AddClaimAsync(role, new Claim("testtype1", "testvalue1"));
            var result2 = await _roleManager.AddClaimAsync(role, new Claim("testtype2", "testvalue2"));

            Assert.True(result1.Succeeded);
            Assert.True(result2.Succeeded);
        }

        [Fact]
        [TestPriority(118)]
        public async Task CanListRoleClaimAsync()
        {
            var role = await _roleManager.FindByNameAsync("test3");

            var claims = await _roleManager.GetClaimsAsync(role);

            Assert.NotEmpty(claims);
            Assert.Equal(2, claims.Count);
        }

        [Fact]
        [TestPriority(119)]
        public async Task CanRemoveRoleClaimAsync()
        {
            var role = await _roleManager.FindByNameAsync("test3");

            var claims = await _roleManager.GetClaimsAsync(role);

            var result = await _roleManager.RemoveClaimAsync(role, claims.First());

            claims = await _roleManager.GetClaimsAsync(role);

            Assert.True(result.Succeeded);
            Assert.Equal(1, claims.Count);
        }
    }
}
