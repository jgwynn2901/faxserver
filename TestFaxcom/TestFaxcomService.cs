using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaxServer.FaxcomService;
using System;
using System.IO;
using FaxServer;
using FaxServer.Model;

namespace TestFaxcom
{
    [TestClass]
    public class TestFaxcomService
    {
        private const string Filespec = @"C:\src\fnsnet\WALOPM02SEDPP-2197.tif";
        private FaxQueue _queue;
        private Sender _sender;
        private Recipient _recipient;

        [TestInitialize]
        public void Startup()
        {
            _queue = new FaxQueue { Queue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ", Username = "Administrator" };
            _sender = new Sender { Company = "IFN", Email = "gwynnj@us.innovation-group.com", FaxNumber = "6175554123", Name = "Shoemaker"};
            _recipient = new Recipient { Account = "QBE", Company = "Sedgwick", FaxNumber = "7812465325", Name = "Fred Flintstone"};

        }

        [TestMethod]
        public void TestFaxcomLogin()
        {
            var svc = CreateAndLogin();
            var request = new ReleaseSessionRequest {Body = new ReleaseSessionRequestBody()};
            var results = svc.ReleaseSession(request);
            Assert.IsNotNull(results);
            Console.WriteLine(results.Body.ReleaseSessionResult.Detail);
            Assert.IsTrue(results.Body.ReleaseSessionResult.Result);
        }

        [TestMethod]
        public void TestFaxComponent()
        {
            var faxResult = new FaxComponentProxy(Filespec).SendFax(_queue, _sender, _recipient).Body.SendFaxResult.Data;
            Assert.IsTrue(faxResult == "Message Sent!");
            Console.WriteLine(faxResult);
        }

        [TestMethod]
        public void TestSendFaxLoginAndSend()
        {
            var body = new LoginAndSendNewFaxMessageRequestBody
            {
                faxQueue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ",
                userName = "Administrator",
                userType = 2,
                senderInfo = new SenderInfo()
                {
                    Name = "John Gwynn",
                    FaxNumber = "555441234",
                    Email = "john.gwynn@sedgwickcms.com",
                    Company = "Sedgwick"
                }
            };
            var recipient = new RecipientInfo {
                Name = "FAXCOMService",
                Company = "IFN",
                FaxNumber = "7812465325"
            };
            body.recipients = new[] { recipient };             
            body.attachments = GetAttachments();

            // put it all together now
            var request = new LoginAndSendNewFaxMessageRequest {Body = body};
            var svc = new FAXCOMServiceSoapClient();
            var response = svc.LoginAndSendNewFaxMessage(request);
            Console.WriteLine(response.Body.LoginAndSendNewFaxMessageResult.Detail);
        }

        [TestMethod]
        public void TestResults()
        {
            var svc = CreateAndLogin();
            Assert.IsNotNull(svc);
            var request = new GetMessageStatusesCountRequest();
            var count = svc.GetMessageStatusesCount(request);
            Console.WriteLine(count.GetMessageStatusesCountResult);
            if (count.GetMessageStatusesCountResult <= 0) return;
            var req = new GetMessageStatusesRequest
            {
                Body = new GetMessageStatusesRequestBody()
                {
                    sortColumn = 1,
                    @ascending = true
                }
            };
            var messages = svc.GetMessageStatuses(req);
            for (var i = 0; i < count.GetMessageStatusesCountResult; ++i)
            {
                var msg = messages.Body.GetMessageStatusesResult[i];
                Console.WriteLine("{0} {1} {2}", msg.IDTag, msg.StatusName, msg.StatusText);
            }
        }

        [TestMethod]
        public void TestResult()
        {
            var svc = CreateAndLogin();
            Assert.IsNotNull(svc);
            var request = GetMessageStatus("LTR1FX03_WS_1606092024270007");
            var response = svc.GetMessageStatusByUniqueID(request);
            Console.WriteLine(response.Body.GetMessageStatusByUniqueIDResult.StatusText);
            Console.WriteLine(response.Body.GetMessageStatusByUniqueIDResult.PagesTransmitted.ToString());
            Console.WriteLine(response.Body.GetMessageStatusByUniqueIDResult.RecipientName);
        }

        private static GetMessageStatusByUniqueIDRequest GetMessageStatus(string key)
        {
            var result = new GetMessageStatusByUniqueIDRequest
            {
                Body = new GetMessageStatusByUniqueIDRequestBody
                {
                    uniqueID = key
                }
            };
            return result;
        }

        [TestMethod]
        public void TestSendFax()
        {
            var svc = CreateAndLogin();
            Assert.IsNotNull(svc);
            var sndr = svc.SetSenderInformation(SetSenderInfo());
            Console.WriteLine(sndr.Body.SetSenderInformationResult.Detail);
            Assert.IsTrue(sndr.Body.SetSenderInformationResult.Result);
            var rm = svc.NewFaxMessage(GetNewFaxRequest());
            Console.WriteLine(rm.Body.NewFaxMessageResult.Detail);
            Assert.IsTrue(rm.Body.NewFaxMessageResult.Result);
            var rs = svc.AddAttachment(GetAddAttachmentRequest());
            Console.WriteLine(rs.Body.AddAttachmentResult.Detail);
            Assert.IsTrue(rs.Body.AddAttachmentResult.Result);
            var result = svc.SendFax(GetSendFaxRequest());
            Console.WriteLine(result.Body.SendFaxResult.Detail);
            Assert.IsTrue(result.Body.SendFaxResult.Result);

        }

        private SendFaxRequest GetSendFaxRequest()
        {
            var result = new SendFaxRequest {Body = new SendFaxRequestBody()};
            return result;
        }

        private static AddAttachmentRequest GetAddAttachmentRequest()
        {
            var results = new AddAttachmentRequest();
            var attachment = GetTestAttachment();
            results.Body = new AddAttachmentRequestBody
            {
                attachment = attachment.FileContent,
                attname = attachment.FileName
            };
            return results;
        }

        private static Attachment[] GetAttachments()
        {
            return new[] { GetTestAttachment() };
        }

        private static Attachment GetTestAttachment()
        {
            var attachment = new Attachment();
            var fs = new FileStream(Filespec, FileMode.Open, FileAccess.Read);
            var fileContents = new byte[fs.Length];
            Assert.IsFalse(fs.Length > int.MaxValue);
            fs.Read(fileContents, 0, (int)fs.Length);
            attachment.FileContent = fileContents;
            attachment.FileName = "WALOPM02SEDPP-2197.tif";
            return attachment;
        }

        private static NewFaxMessageRequest GetNewFaxRequest()
        {
            var result = new NewFaxMessageRequest
            {
                Body = new NewFaxMessageRequestBody
                {
                    recipientName = "Test Fax Recipient",
                    priority = 2,
                    sendTime = "0.0",
                    resolution = 1,
                    subject = "IFN Test",
                    memo = "test MEMO",
                    senderName = "Shoemaker",
                    senderFax = "6178862064",
                    recipientCompany = "IFN",
                    recipientFax = "5017475674"
                    //recipientFax = "8668538459"
                }
            };
            return result;
        }

        private static SetSenderInformationRequest SetSenderInfo()
        {
            var sender = new SetSenderInformationRequest
            {
                Body = new SetSenderInformationRequestBody
                {
                    name = "John Gwynn",
                    faxNumber = "7812465325",
                    company = "Sedgwick CMS",
                    email = "johngwynn@sedgwickcms.com",
                    voiceNumber = "6172753024"
                }
            };
            return sender;
        }

        private static SetLegacyModeRequest GetLegacyModeRequest()
        {
            var legacyRequest = new SetLegacyModeRequest {Body = new SetLegacyModeRequestBody {legacyMode = 1}};
            return legacyRequest;

        }

        private static LogOnRequest GetLogonRequest()
        {
            var logonRequest = new LogOnRequest
            {
                Body = new LogOnRequestBody
                {
                    userName = "Administrator",
                    faxQueue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ",
                    userType = 2
                }
            };
            return logonRequest;
        }

       

        private static FAXCOMServiceSoapClient CreateAndLogin()
        {
            var svc = new FAXCOMServiceSoapClient();
            

            svc.SetLegacyMode(GetLegacyModeRequest());
            var response = svc.LogOn(GetLogonRequest());
            Assert.IsTrue(response.Body.LogOnResult.Result);
            Console.WriteLine(response.Body.LogOnResult.Detail);
            return svc;

        }
    }
}
