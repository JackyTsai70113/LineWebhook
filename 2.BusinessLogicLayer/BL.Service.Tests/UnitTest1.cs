using System.Text;

namespace BL.Service.Tests {

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