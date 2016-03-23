using FaxServer.FaxcomService;
using FaxServer.Model;

namespace FaxServer
{
    public interface IFaxComponent
    {
        LoginAndSendNewFaxMessageResponse SendFax(FaxQueue q, Sender sender, Recipient recipient);
    }
}