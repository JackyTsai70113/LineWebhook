using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using BL.Service.Line;
using BL.Service.Sinopac;
using BL.Service.Tests.Map;
using BL.Service.TWSE_Stock;
using DA.Managers.CambridgeDictionary;
using isRock.LineBot;
using NUnit.Framework;

namespace BL.Service.Tests
{
    public class LineWebhookServiceTests
    {
        private LineWebhookService LineWebhookService { set; get; }

        [SetUp]
        public void Setup()
        {
            LineWebhookService = new LineWebhookService(
                new CambridgeDictionaryManager(),
                new FakeExchangeRateService(),
                new MaskInstitutionService(),
                new FakeMapHereService(),
                new TradingVolumeService()
                );
        }

        /// <summary>
        /// 文字輸入，正確回應
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="messages">回應訊息</param>
        [Test, TestCaseSource(nameof(ValidTextInputs))]
        public void TestForValidTextInput(Event @event, List<MessageBase> messages)
        {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Act
            List<MessageBase> actual = LineWebhookService.GetReplyMessages(@event);

            // Assert
            Assert.That(JsonSerializer.Serialize(actual), Is.EqualTo(JsonSerializer.Serialize(messages)),
                JsonSerializer.Serialize(messages) + "\n" + JsonSerializer.Serialize(actual));
        }

        public static IEnumerable ValidTextInputs
        {
            get
            {
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "cd cat"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("cat noun uk /kæt/ us /kæt/\n\na small animal with fur, four legs, a tail, and claws, usually kept as a pet or for catching mice \n貓\n\nany member of the group of animals similar to the cat, such as the lion\n貓科動物\n------------------------\nthe cat family")
                    }
                );
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "cj 倉頡"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("倉: http://input.foruto.com/cjdict/Images/CJZD_JPG/ADDC.JPG\n頡: http://input.foruto.com/cjdict/Images/CJZD_JPG/BE65.JPG")
                    }
                );
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "er"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("美金報價\n---------------------\n銀行買入： 27.7750\n銀行賣出： 27.8700\n報價時間： 2022-01-28 15:30:38")
                    }
                );
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "st 1 1"
                        }
                    },
                    new List<MessageBase>{
                        new StickerMessage(1, 1),
                        new StickerMessage(1, 2),
                        new StickerMessage(1, 3),
                        new StickerMessage(1, 4),
                        new StickerMessage(1, 5)
                    }
                );
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "sp"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("sp")
                    }
                );
            }
        }

        [Test, TestCaseSource(nameof(InvalidTextInputs))]
        public void InvalidTextInput(Event @event, List<MessageBase> messages)
        {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Act
            List<MessageBase> actual = LineWebhookService.GetReplyMessages(@event);

            // Assert
            Assert.That(JsonSerializer.Serialize(actual), Is.EqualTo(JsonSerializer.Serialize(messages)),
                JsonSerializer.Serialize(messages) + "\n" + JsonSerializer.Serialize(actual));
        }

        public static IEnumerable InvalidTextInputs
        {
            get
            {
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "cd 123"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("未能找到符合字詞: 123")
                    }
                );
                yield return new TestCaseData(
                    new Event
                    {
                        type = "message",
                        message = new Message
                        {
                            type = "text",
                            text = "st 1 a"
                        }
                    },
                    new List<MessageBase>{
                        new TextMessage("此指令用來輸出最多五個的貼圖，\n" +
                        "用法：st {貼圖包Id} {貼圖Id}\n範例：st 1 1\n" +
                        "貼圖包/貼圖 如官方文件所定義：https://developers.line.biz/zh-hant/docs/messaging-api/sticker-list/#sticker-definitions")
                    }
                );
            }
        }

    }
}