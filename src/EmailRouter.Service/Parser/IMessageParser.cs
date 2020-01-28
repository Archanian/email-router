using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Parser
{
    public interface IMessageParser
    {
         IMessage Parse(byte[] body, string type);
    }
}