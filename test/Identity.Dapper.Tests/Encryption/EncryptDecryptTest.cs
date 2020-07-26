using Identity.Dapper.Cryptography;
using Identity.Dapper.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Identity.Dapper.Tests.Encryption
{
    public class EncryptDecryptTest
    {
        private readonly string _key = "E546C8DF278CD5931069B522E695D4F2";  // 32 bytes key for AES256
        private readonly string _iv = "SomeReallyCoolIV";                   // 16 bytes vector for AES256

        private readonly Mock<ILogger<EncryptionHelper>> _mockLogger;
        private readonly Mock<IOptions<AesKeys>> _mockKeys;

        public EncryptDecryptTest()
        {
            _mockLogger = new Mock<ILogger<EncryptionHelper>>();
            _mockKeys = new Mock<IOptions<AesKeys>>();

            var aesKeys = new AesKeys
            {
                Key = EncryptionHelper.Base64Encode(_key),
                IV = EncryptionHelper.Base64Encode(_iv),
            };
            _mockKeys.Setup(x => x.Value).Returns(aesKeys);
        }

        [Fact]
        public void EncryptTextThenDecrypt()
        {
            const string textToEncrypt = "I hope I come out whole!";
            var eh = new EncryptionHelper(_mockKeys.Object, _mockLogger.Object);

            var encrypted = eh.EncryptAES256(textToEncrypt);
            var decrypted = eh.TryDecryptAES256(encrypted);

            Assert.NotEqual(encrypted, decrypted);
            Assert.Equal(textToEncrypt, decrypted);
        }

        [Fact]
        public void InvalidKeyAndIvReturnSameInput()
        {
            const string textToEncrypt = "I hope I come out whole!";
            var aesKeys = new AesKeys
            {
                Key = string.Empty,
                IV = string.Empty,
            };

            _mockKeys.Setup(x => x.Value)
                     .Returns(aesKeys);

            var eh = new EncryptionHelper(_mockKeys.Object, _mockLogger.Object);
            var probablyEncrypted = eh.EncryptAES256(textToEncrypt);

            Assert.Equal(textToEncrypt, probablyEncrypted);
        }
    }
}
