using System.IO;
using FaxServer.FaxcomService;
using FaxServer.Model;
using FnsUtility;

namespace FaxServer
{
    public class FaxComponent : IFaxComponent
    {
        private readonly string _fileName;
        
        public FaxComponent(string fileName)
        {
            _fileName = fileName;
        }

       public SendFaxResponse SendFax(FaxQueue q, Sender sender, Recipient recipient)
        {
            var svc = CreateAndLogin(q);
            Assert.IsNotNull(svc, "SendFax Service is null!");
            var sndr = svc.SetSenderInformation(SetSenderInfo(sender));
            Assert.IsTrue(sndr.Body.SetSenderInformationResult.Result, sndr.Body.SetSenderInformationResult.Detail);
            var rm = svc.NewFaxMessage(GetNewFaxRequest(sender, recipient));
            Assert.IsTrue(rm.Body.NewFaxMessageResult.Result, rm.Body.NewFaxMessageResult.Detail);
            var rs = svc.AddAttachment(GetAddAttachmentRequest());
            Assert.IsTrue(rs.Body.AddAttachmentResult.Result);
            var result = svc.SendFax(GetSendFaxRequest());
            return result;
        }

        public static GetMessageStatusByUniqueIDResponse GetFaxResponse(FaxQueue q, string jobId)
        {
            var svc = CreateAndLogin(q);
            Assert.IsNotNull(svc);
            var request = GetMessageStatus(jobId);
            var response = svc.GetMessageStatusByUniqueID(request);
            return response;
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

        private Attachment GetTestAttachment()
        {
            var attachment = new Attachment();
            var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            var fileContents = new byte[fs.Length];
            fs.Read(fileContents, 0, (int)fs.Length);
            attachment.FileContent = fileContents;
            attachment.FileName = Path.GetFileName(_fileName);
            return attachment;
        }

        private static SendFaxRequest GetSendFaxRequest()
        {
            var result = new SendFaxRequest { Body = new SendFaxRequestBody() };
            return result;
        }

        private static NewFaxMessageRequest GetNewFaxRequest(Entity sender, Recipient recipient)
        {
            var result = new NewFaxMessageRequest
            {
                Body = new NewFaxMessageRequestBody
                {
                    recipientName = recipient.Name,
                    priority = 2,
                    sendTime = "0.0",
                    resolution = 1,
                    subject = recipient.Subject,
                    senderName = sender.Name,
                    senderFax = sender.FaxNumber,
                    recipientCompany = recipient.Company,
                    recipientFax = recipient.FaxNumber
                }
            };
            return result;
        }

        private static SetSenderInformationRequest SetSenderInfo(Sender sender)
        {
           return new SetSenderInformationRequest
            {
                Body = new SetSenderInformationRequestBody
                {
                    name = sender.Name,
                    faxNumber = sender.FaxNumber,
                    company = sender.Company,
                    email = sender.Email,
                    voiceNumber = "6178862000"
                }
            };
        }

        public static FAXCOMServiceSoapClient CreateAndLogin(FaxQueue q)
        {
            var svc = new FAXCOMServiceSoapClient();
            svc.SetLegacyMode(GetLegacyModeRequest());
            var response = svc.LogOn(GetLogonRequest(q));
            Assert.IsTrue(response.Body.LogOnResult.Result);
            ErrorLog.DebugLog(response.Body.LogOnResult.Detail, "CreateAndLogin");
            return svc;
        }

        private static SetLegacyModeRequest GetLegacyModeRequest()
        {
            var legacyRequest = new SetLegacyModeRequest { Body = new SetLegacyModeRequestBody { legacyMode = 1 } };
            return legacyRequest;
        }

        private static LogOnRequest GetLogonRequest(FaxQueue q)
        {
            var logonRequest = new LogOnRequest
            {
                Body = new LogOnRequestBody
                {
                    userName = q.Username,
                    faxQueue = q.Queue,
                    userType = 2
                }
            };
            return logonRequest;
        }
    }
}
