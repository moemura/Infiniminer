using Infiniminer;
using System.Collections.Generic;

namespace Plexiglass.Client
{
    public class ChatContainer
    {
        public ChatMessageType ChatMode = ChatMessageType.None;
        public int ChatMaxBuffer = 5;
        public List<ChatMessage> ChatBuffer = new List<ChatMessage>(); // chatBuffer[0] is most recent
        public List<ChatMessage> ChatFullBuffer = new List<ChatMessage>(); //same as above, holds last several messages
        public string ChatEntryBuffer = "";
    }
}
