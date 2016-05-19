using FaxServer.FaxcomService;
using FaxServer.Model;

namespace FaxServer
{
    public interface IFaxComponent
    {
        SendFaxResponse SendFax(FaxQueue q, Sender sender, Recipient recipient);
    }
}