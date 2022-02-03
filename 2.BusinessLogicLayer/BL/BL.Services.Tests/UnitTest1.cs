using NUnit.Framework;
using BL.Services.Tests.Map;
using BL.Services.TWSE_Stock;
using BL.Services.Sinopac;
using System.Text;

namespace BL.Services.Tests {

    public class Tests {

        [SetUp]
        public void Setup() {
        }

        [Test]
        [TestCase(1, 1)]
        public void Test1(int source, int expected) {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Act
            int actual = source;
            
            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}