using System;
using System.Text;
using System.Text.Json;
using EmailRouter.Service.Messages;

namespace EmailRouter.Service.Parser
{
    public class MessageParser : IMessageParser
    {
        public IMessage Parse(byte[] body, string type)
        {
            Type messageType = Type.GetType(type, throwOnError: false);
            if (messageType == null)
            {
                return null;
            }

            string messageRaw = Encoding.UTF8.GetString(body);
            var parsedMessage = JsonSerializer.Deserialize(
                messageRaw,
                messageType,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            
            return parsedMessage as IMessage;
        }
    }
}