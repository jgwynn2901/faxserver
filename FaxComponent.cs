using System.IO;
using FaxServer.FaxcomService;
using FaxServer.Model;

namespace FaxServer
{
    public class FaxComponent : IFaxComponent
    {
        private readonly string _fileName;
        
        public FaxComponent(string fileName)
        {
            _fileName = fileName;
        }

       public virtual LoginAndSendNewFaxMessageResponse SendFax(FaxQueue q, Sender sender, Recipient recipient)
        {
            var body = new LoginAndSendNewFaxMessageRequestBody
            {
                faxQueue = q.Queue,
                userName = q.Username,
                userType = 2,
                senderInfo = new SenderInfo()
                {
                    Name = sender.Name,
                    FaxNumber = sender.FaxNumber,
                    Email = sender.Email,
                    Company = sender.Company
                }
            };
            var recip = new RecipientInfo
            {
                Name = recipient.Name,
                Account = recipient.Account,
                Company = recipient.Company,
                FaxNumber = recipient.FaxNumber
            };
            body.recipients = new[] { recip};

            var attachment = new Attachment();
            var fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            var fileContents = new byte[fs.Length];
            //Assert.IsFalse(fs.Length > int.MaxValue);
            fs.Read(fileContents, 0, (int)fs.Length);
            attachment.FileContent = fileContents;
            attachment.FileName = _fileName;

            body.attachments = new[] { attachment };

            // put it all together now
            var request = new LoginAndSendNewFaxMessageRequest { Body = body };
            var svc = new FAXCOMServiceSoapClient();
           return svc.LoginAndSendNewFaxMessage(request);
        }
    }
}
