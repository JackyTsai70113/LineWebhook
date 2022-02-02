using NUnit.Framework;
using BL.Services.Tests.Map;
using BL.Services.TWSE_Stock;
using BL.Services.Sinopac;
using System.Text;

namespace BL.Services.Tests {

    public class Tests {
        private LineWebhookService LineWebhookService { get; set; }

        [SetUp]
        public void Setup() {
            LineWebhookService = new LineWebhookService(
                new FakeExchangeRateService(),
                new FakeMapHereService(),
                new TradingVolumeService()
                );
        }

        [Test]
        [TestCase("sp", "美金報價\n---------------------\n銀行買入： 27.7750\n銀行賣出： 27.8700\n報價時間： 2022-01-28 15:30:38")]
        [TestCase("cj 倉頡", "倉: http://input.foruto.com/cjdict/Images/CJZD_JPG/ADDC.JPG\n頡: http://input.foruto.com/cjdict/Images/CJZD_JPG/BE65.JPG\n")]
        [TestCase("12345", "12345")]
        public void Test1(string command, string expected) {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            // Act
            string testResult = LineWebhookService.GetReplyTextByText(command);

            // Assert
            Assert.AreEqual(expected, testResult);
        }
    }
}