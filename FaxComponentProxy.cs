using FaxServer.FaxcomService;
using FaxServer.Model;

namespace FaxServer
{
    public class FaxComponentProxy : FaxComponent
    {
        public override LoginAndSendNewFaxMessageResponse SendFax(FaxQueue q, Sender sender, Recipient recipient)
        {
            return new LoginAndSendNewFaxMessageResponse
            {
                Body = new LoginAndSendNewFaxMessageResponseBody
                {
                    LoginAndSendNewFaxMessageResult = new ResultMessage
                    {
                        Data = "Message Sent!"
                    }
                }
            };
        }

        public FaxComponentProxy(string fileName) : base(fileName)
        {}
    }
}
