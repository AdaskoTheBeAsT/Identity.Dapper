using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
    [Collection(nameof(PostgreSQL))]
    [TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    public class RoleManagerTestsPostgreSql : IClassFixture<PostgreDatabaseFixture>
    {
        private readonly PostgreDatabaseFixture _databaseFixture;
        private readonly RoleManager<DapperIdentityRole> _roleManager;
        private readonly UserManager<DapperIdentityUser> _userManager;

        public RoleManagerTestsPostgreSql(PostgreDatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture ?? throw new ArgumentNullException(nameof(databaseFixture));
            _roleManager = (RoleManager<DapperIdentityRole>)_databaseFixture.TestServer.Host.Services.GetService(typeof(RoleManager<DapperIdentityRole>));
            _userManager = (UserManager<DapperIdentityUser>)_databaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
        }

        [Fact]
        [TestPriority(300)]
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
        [TestPriority(301)]
        public async Task CanFindByNameAsync()
        {
            var role = await _roleManager.FindByNameAsync("test");

            Assert.NotNull(role);
        }

        [Fact]
        [TestPriority(302)]
        public async Task CanFindByIdAsync()
        {
            var role = await _roleManager.FindByIdAsync(1.ToString(CultureInfo.InvariantCulture));

            Assert.NotNull(role);
        }

        [Fact]
        [TestPriority(303)]
        public async Task CanRemoveAsync()
        {
            await _roleManager.CreateAsync(new DapperIdentityRole { Name = "test4" });

            var role = await _roleManager.FindByNameAsync("test4");

            Assert.NotNull(role);

            var result = await _roleManager.DeleteAsync(role);

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(304)]
        public async Task CanRoleExistsAsync()
        {
            var result = await _roleManager.RoleExistsAsync("test");

            Assert.True(result);
        }

        [Fact]
        [TestPriority(305)]
        public async Task CanUpdateAsync()
        {
            var role = await _roleManager.FindByNameAsync("test");
            role.Name = "testmodified";

            var result = await _roleManager.UpdateAsync(role);

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(306)]
        public async Task CanAddRoleToUserAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "testrole", Email = "test@test.com" }, "123456");

            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.AddToRoleAsync(user, "testmodified");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(307)]
        public async Task CanAddRolesToUserAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.AddToRolesAsync(user, new[] { "test2", "test3", "test5" });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(308)]
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
        [TestPriority(309)]
        public async Task CanGetUsersInRoleAsync()
        {
            var result = await _userManager.GetUsersInRoleAsync("testmodified");

            Assert.Collection(result, x => x.UserName.Equals("testrole", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(310)]
        public async Task CanCheckIfUserIsInRoleAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.IsInRoleAsync(user, "testmodified");

            Assert.True(result);
        }

        [Fact]
        [TestPriority(311)]
        public async Task CanRemoveUserFromRoleAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.RemoveFromRoleAsync(user, "testmodified");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(312)]
        public async Task CanRemoveUserFromRolesAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            var result = await _userManager.RemoveFromRolesAsync(user, new[] { "test2", "test3" });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(313)]
        public async Task FindByEmailReturnRolesAsync()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(314)]
        public async Task FindByNameReturnRolesAsync()
        {
            var user = await _userManager.FindByNameAsync("testrole");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(315)]
        public async Task FindByIdReturnRolesAsync()
        {
            var user = await _userManager.FindByIdAsync("1");

            Assert.Collection(user.Roles, x => x.RoleId.Equals(5));
        }

        [Fact]
        [TestPriority(316)]
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
        [TestPriority(317)]
        public async Task CanAddRoleClaimAsync()
        {
            var role = await _roleManager.FindByNameAsync("test3");

            var result1 = await _roleManager.AddClaimAsync(role, new Claim("testtype1", "testvalue1"));
            var result2 = await _roleManager.AddClaimAsync(role, new Claim("testtype2", "testvalue2"));

            Assert.True(result1.Succeeded);
            Assert.True(result2.Succeeded);
        }

        [Fact]
        [TestPriority(318)]
        public async Task CanListRoleClaimAsync()
        {
            var role = await _roleManager.FindByNameAsync("test3");

            var claims = await _roleManager.GetClaimsAsync(role);

            Assert.NotEmpty(claims);
            Assert.Equal(2, claims.Count);
        }

        [Fact]
        [TestPriority(319)]
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
