using FaxServer.FaxcomService;
using System.Diagnostics;
using System.IO;

namespace FaxServer
{
    public class FaxComponent
    {
        public const string companyName = "Sedgwick CMS";
        public const string senderName = "John Gwynn";
        public const string senderFax = "6175550000";
        public const string senderEmail = "johngwynn@sedgwickcms.com";
        private const string filespec = @"C:\src\fnsnet\";



        private SenderInfo GetSenderInfo()
        {
            return new SenderInfo()
            {
                Name = senderName,
                FaxNumber = senderFax,
                Email = senderEmail,
                Company = senderName,
                VoiceNumber = "6178862000"
            };
        }

        private RecipientInfo GetRecipientInfo()
        {
            return new RecipientInfo
            {
                Name = "FAXCOMService",
                Company = companyName,
                FaxNumber = "9014153301"
            };
        }

        private Attachment GetAttachment(string fileName)
        {
            var attachment = new Attachment();
            var fs = new FileStream(filespec + fileName, FileMode.Open, FileAccess.Read);
            byte[] fileContents = new byte[fs.Length];
            Debug.Assert(fs.Length <= int.MaxValue);
            fs.Read(fileContents, 0, (int)fs.Length);
            attachment.FileContent = fileContents;
            attachment.FileName = fileName;
            return attachment;
        }

        private LoginAndSendNewFaxMessageRequestBody GetBody()
        {
            var body = new LoginAndSendNewFaxMessageRequestBody();
            body.faxQueue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ";
            body.userName = "Administrator";
            body.userType = 2;
            body.priority = 2;
            body.sendTime = "0.0";
            body.resolution = 0;
            body.subject = "First Notice of Loss";
            body.memo = "This is a test";
            body.senderInfo = GetSenderInfo();
            var recipient = GetRecipientInfo();
            body.recipients = new RecipientInfo[] { recipient };
            body.attachments = new Attachment[] { GetAttachment("WALOPM02SEDPP-2197.tif") };
            return body;
        }

        public string SendFax()
        {
            var request = new LoginAndSendNewFaxMessageRequest();
            request.Body = GetBody();
            var svc = new FAXCOMServiceSoapClient();
            var response = svc.LoginAndSendNewFaxMessage(request);
            return response.Body.LoginAndSendNewFaxMessageResult.Detail ?? "Nothing returned!";
        }
    }
}
