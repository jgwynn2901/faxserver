using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaxServer.FaxcomService;
using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using FaxServer;

namespace TestFaxcom
{
    [TestClass]
    public class TestFaxcomService
    {
        private const string filespec = @"C:\src\fnsnet\WALOPM02SEDPP-2197.tif";

        [TestMethod]
        public void TestFaxcomLogin()
        {
            var svc = CreateAndLogin();
            var request = new ReleaseSessionRequest();
            request.Body = new ReleaseSessionRequestBody();
            var results = svc.ReleaseSession(request);
            Console.WriteLine(results.Body.ReleaseSessionResult.Detail);
            Assert.IsTrue(results.Body.ReleaseSessionResult.Result);
        }

        [TestMethod]
        public void TestFaxComponent()
        {
            var fax = new FaxComponent();
            Console.WriteLine(fax.SendFax());
        }

        [TestMethod]
        public void TestSendFaxLoginAndSend()
        {
            var body = new LoginAndSendNewFaxMessageRequestBody();
            body.faxQueue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ";
            body.userName = "Administrator";
            body.userType = 2;
            body.senderInfo = new SenderInfo() {
                Name = "John Gwynn",
                FaxNumber = "555441234",
                Email = "john.gwynn@sedgwickcms.com",
                Company = "Sedgwick"
            };
            var recipient = new RecipientInfo {
                Name = "FAXCOMService",
                Company = "IFN",
                FaxNumber = "7812465325"
            };
            body.recipients = new RecipientInfo[] { recipient };             
            body.attachments = GetAttachments();

            // put it all together now
            var request = new LoginAndSendNewFaxMessageRequest();
            request.Body = body;
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
            if (count.GetMessageStatusesCountResult > 0)
            {
                var req = new GetMessageStatusesRequest();
                req.Body = new GetMessageStatusesRequestBody()
                {
                    sortColumn = 1,
                    ascending = true
                };
                var messages = svc.GetMessageStatuses(req);
                for (var i = 0; i < count.GetMessageStatusesCountResult; ++i)
                {
                    var msg = messages.Body.GetMessageStatusesResult[i];
                    Console.WriteLine("{0} {1} {2}", msg.IDTag, msg.StatusName, msg.StatusText);
                }
            }
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
            var result = new SendFaxRequest();
            result.Body = new SendFaxRequestBody();
            return result;
        }

        private AddAttachmentRequest GetAddAttachmentRequest()
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

        private Attachment[] GetAttachments()
        {
            return new Attachment[] { GetTestAttachment() };
        }

        private Attachment GetTestAttachment()
        {
            var attachment = new Attachment();
            var fs = new FileStream(filespec, FileMode.Open, FileAccess.Read);
            byte[] fileContents = new byte[fs.Length];
            Assert.IsFalse(fs.Length > int.MaxValue);
            fs.Read(fileContents, 0, (int)fs.Length);
            attachment.FileContent = fileContents;
            attachment.FileName = "WALOPM02SEDPP-2197.tif";
            return attachment;
        }

        private NewFaxMessageRequest GetNewFaxRequest()
        {
            var result = new NewFaxMessageRequest();
            result.Body = new NewFaxMessageRequestBody
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
                recipientFax = "7812465325"
            };
            return result;
        }

        private SetSenderInformationRequest SetSenderInfo()
        {
            var sender = new SetSenderInformationRequest();
            sender.Body = new SetSenderInformationRequestBody
            {
                name = "John Gwynn",
                faxNumber = "7812465325",
                company = "Sedgwick CMS",
                email = "johngwynn@sedgwickcms.com",
                voiceNumber = "6172753024"
            };
            return sender;
        }

        private SetLegacyModeRequest GetLegacyModeRequest()
        {
            var legacyRequest = new SetLegacyModeRequest();
            legacyRequest.Body = new SetLegacyModeRequestBody();
            legacyRequest.Body.legacyMode = 1;
            return legacyRequest;

        }

        private LogOnRequest GetLogonRequest()
        {
            var logonRequest = new LogOnRequest();
            logonRequest.Body = new LogOnRequestBody();
            logonRequest.Body.userName = "Administrator";
            logonRequest.Body.faxQueue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ";
            logonRequest.Body.userType = 2;
            return logonRequest;
        }

        private FAXCOMServiceSoapClient CreateAndLogin()
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
