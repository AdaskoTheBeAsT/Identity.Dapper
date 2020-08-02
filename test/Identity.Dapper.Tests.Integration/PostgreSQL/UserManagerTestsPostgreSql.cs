using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Dapper.Entities;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Identity.Dapper.Tests.Integration.PostgreSQL
{
#pragma warning disable MA0026 // Fix TODO comment
    // TODO:
    // There's a little problem with IClassFixture that on EVERY test, the constructor of the class is called (and if implements IDisposable, the Dispose() is called too)
    // So, there's no safe way to clean data of the database.
    // As a workaround, every time you run this test, execute restart.sh to reset all data on Docker container
#pragma warning restore MA0026 // Fix TODO comment
    [Collection(nameof(PostgreSQL))]
    [TestCaseOrderer(TestCollectionOrderer.TypeName, TestCollectionOrderer.AssemblyName)]
    public class UserManagerTestsPostgreSql : IClassFixture<PostgreDatabaseFixture>
    {
        private readonly PostgreDatabaseFixture _databaseFixture;
        private readonly UserManager<DapperIdentityUser> _userManager;

        public UserManagerTestsPostgreSql(PostgreDatabaseFixture databaseFixture)
        {
            if (databaseFixture is null)
            {
                throw new ArgumentNullException(nameof(databaseFixture));
            }

            _databaseFixture = databaseFixture;
            _userManager = (UserManager<DapperIdentityUser>)_databaseFixture.TestServer.Host.Services.GetService(typeof(UserManager<DapperIdentityUser>));
        }

        [Fact]
        [TestPriority(200)]
        public async Task CanCreateUserWithoutPasswordAsync()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com",
            });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(201)]
        public async Task CanCreateUserWithPasswordAsync()
        {
            var result = await _userManager.CreateAsync(
                new DapperIdentityUser
                {
                    UserName = "test2",
                    Email = "test2@test2.com",
                },
                "123456");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(202)]
        public async Task CantCreateDuplicateUserAsync()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com",
            });

            Assert.False(result.Succeeded);
            Assert.Contains(
                result.Errors,
                x => x.Code.Equals(
                    new IdentityErrorDescriber().DuplicateUserName(string.Empty).Code,
                    StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(203)]
        public async Task CanFindUserByNameAsync()
        {
            var result = await _userManager.FindByNameAsync("test");

            Assert.NotNull(result);
        }

        [Fact]
        [TestPriority(204)]
        public async Task CanCreateUserWithEmptyUserNameAsync()
        {
            var result = await _userManager.CreateAsync(new DapperIdentityUser { Email = "test@test.com" });

            Assert.Contains(
                result.Errors,
                x => x.Code.Equals(
                    new IdentityErrorDescriber().InvalidUserName(string.Empty).Code,
                    StringComparison.OrdinalIgnoreCase));
            Assert.False(result.Succeeded);
        }

        [Fact]
        [TestPriority(205)]
        public async Task CanIncreaseAccessFailedCountAsync()
        {
            var user = await _userManager.FindByNameAsync("test");
            var result = await _userManager.AccessFailedAsync(user);

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(206)]
        public async Task CanGetAccessFailedCountAsync()
        {
            var user = await _userManager.FindByNameAsync("test");
            var result = await _userManager.GetAccessFailedCountAsync(user);

            Assert.True(result > 0);
        }

        [Fact]
        [TestPriority(207)]
        public async Task CanAddClaimAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "claim", Email = "claim@claim.com" }, "123456");

            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Actor, "test"));
            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(208)]
        public async Task CanAddClaimsAsync()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.AuthenticationMethod, "test2");
            var claim2 = new Claim(ClaimTypes.AuthorizationDecision, "test3");

            var result = await _userManager.AddClaimsAsync(user, new[] { claim1, claim2 });
            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(209)]
        public async Task CanGetClaimsAsync()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.GetClaimsAsync(user);

            Assert.Collection(
                result,
                x => x.Type.Equals(ClaimTypes.AuthenticationMethod, StringComparison.OrdinalIgnoreCase),
                x => x.Type.Equals(ClaimTypes.AuthorizationDecision, StringComparison.OrdinalIgnoreCase),
                x => x.Type.Equals(ClaimTypes.Actor, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(210)]
        public async Task CanRemoveClaimAsync()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var result = await _userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Actor, "test"));

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(211)]
        public async Task CanRemoveClaimsAsync()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.AuthenticationMethod, "test2");
            var claim2 = new Claim(ClaimTypes.AuthorizationDecision, "test3");

            var result = await _userManager.RemoveClaimsAsync(user, new[] { claim1, claim2 });

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(212)]
        public async Task CanAddLoginAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "login", Email = "login@login.com" }, "123456");

            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.AddLoginAsync(user, new UserLoginInfo("dummy", "dummy", "dummy"));
            var result2 = await _userManager.AddLoginAsync(user, new UserLoginInfo("dummy2", "dummy2", "dummy2"));

            Assert.True(result.Succeeded);
            Assert.True(result2.Succeeded);
        }

        [Fact]
        [TestPriority(213)]
        public async Task CanGetLoginAsync()
        {
            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.GetLoginsAsync(user);

            Assert.Collection(
                result,
                x => x.LoginProvider.Equals("dummy", StringComparison.OrdinalIgnoreCase),
                x => x.LoginProvider.Equals("dummy2", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(214)]
        public async Task CanRemoveLoginAsync()
        {
            var user = await _userManager.FindByNameAsync("login");
            Assert.NotNull(user);

            var result = await _userManager.RemoveLoginAsync(user, "dummy", "dummy");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(215)]
        public async Task CanAddPasswordAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser { UserName = "test3", Email = "test3@test3.com" });

            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.AddPasswordAsync(user, "123456");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(216)]
        public async Task CanChangeEmailAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, "test3changed@test3.com");

            Assert.NotNull(token);

            var result = await _userManager.ChangeEmailAsync(user, "test3changed@test3.com", token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.Equal("test3changed@test3.com", user.Email, ignoreCase: true);
        }

        [Fact]
        [TestPriority(217)]
        public async Task CanChangePasswordAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var result = await _userManager.ChangePasswordAsync(user, "123456", "123456789");

            Assert.True(result.Succeeded);
        }

        [Fact]
        [TestPriority(218)]
        public async Task CanChangePhoneNumberAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, "123");

            Assert.NotNull(token);

            var result = await _userManager.ChangePhoneNumberAsync(user, "123", token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.Equal("123", user.PhoneNumber);
        }

        [Fact]
        [TestPriority(219)]
        public async Task CanCheckPasswordAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.CheckPasswordAsync(user, "123456789");

            Assert.True(result);
        }

        [Fact]
        [TestPriority(220)]
        public async Task CanVerifyEmailAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            Assert.NotNull(user);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            Assert.NotNull(token);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test3");

            Assert.True(user.EmailConfirmed);
        }

        [Fact]
        [TestPriority(221)]
        public async Task CanFindByIdAsync()
        {
            var user = await _userManager.FindByIdAsync(2.ToString(CultureInfo.InvariantCulture));

            Assert.NotNull(user);
        }

        [Fact]
        [TestPriority(222)]
        public async Task CanFindByEmailAsync()
        {
            var user = await _userManager.FindByEmailAsync("test3changed@test3.com");

            Assert.NotNull(user);
        }

        [Fact]
        [TestPriority(223)]
        public async Task CanFindByLoginAsync()
        {
            var user = await _userManager.FindByLoginAsync("dummy2", "dummy2");

            Assert.NotNull(user);
        }

        [Fact]
        [TestPriority(224)]
        public async Task CanGetPhoneNumberAsync()
        {
            var user = await _userManager.FindByNameAsync("test3");

            var result = await _userManager.GetPhoneNumberAsync(user);

            Assert.Equal("123", result);
        }

        [Fact]
        [TestPriority(225)]
        public async Task CanUpdateClaimAsync()
        {
            var user = await _userManager.FindByNameAsync("claim");
            Assert.NotNull(user);

            var claim1 = new Claim(ClaimTypes.Actor, "test");
            await _userManager.AddClaimAsync(user, claim1);

            var claim2 = new Claim(ClaimTypes.Actor, "test2");
            await _userManager.AddClaimAsync(user, claim2);

            var result = await _userManager.ReplaceClaimAsync(user, claim1, claim2);

            Assert.True(result.Succeeded);

            var claims = await _userManager.GetClaimsAsync(user);

            Assert.Collection(
                claims,
                x => x.Type.Equals(ClaimTypes.Actor, StringComparison.OrdinalIgnoreCase),
                x => x.Value.Equals("test2", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        [TestPriority(226)]
        public async Task CanResetAccessFailedCountAsync()
        {
            var user = await _userManager.FindByNameAsync("test");
            Assert.NotNull(user);

            var actualAccessCount = await _userManager.GetAccessFailedCountAsync(user);

            Assert.Equal(1, actualAccessCount);

            var result = await _userManager.ResetAccessFailedCountAsync(user);

            Assert.True(result.Succeeded);

            actualAccessCount = await _userManager.GetAccessFailedCountAsync(user);

            Assert.Equal(0, actualAccessCount);
        }

        [Fact]
        [TestPriority(227)]
        public async Task CanUpdateAsync()
        {
            var user = await _userManager.FindByNameAsync("test");
            Assert.NotNull(user);

            user.Email = "testchanged@test.com";

            var result = await _userManager.UpdateAsync(user);

            Assert.True(result.Succeeded);

            user = await _userManager.FindByNameAsync("test");

            Assert.Equal("testchanged@test.com", user.Email, ignoreCase: true);
        }

        [Fact]
        [TestPriority(228)]
        public async Task CanRemoveUserAsync()
        {
            var user = await _userManager.FindByNameAsync("test");

            var result = await _userManager.DeleteAsync(user);

            Assert.True(result.Succeeded);
        }

        // Fixes https://github.com/grandchamp/Identity.Dapper/issues/72
        [Fact]
        [TestPriority(229)]
        public async Task FindByLoginAsyncReturnsUserAsync()
        {
            await _userManager.CreateAsync(new DapperIdentityUser
            {
                UserName = "test",
                Email = "test@test.com",
            });

            var user = await _userManager.FindByEmailAsync("test@test.com");

            await _userManager.AddLoginAsync(user, new UserLoginInfo("test", "test2", "test3"));

            var loginInfo = await _userManager.FindByLoginAsync("test", "test2");

            Assert.NotNull(loginInfo);
            Assert.NotEqual(0, loginInfo.Id);
        }
    }
}
