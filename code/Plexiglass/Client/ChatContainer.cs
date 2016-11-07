using Infiniminer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public class ChatContainer
    {
        public ChatMessageType chatMode = ChatMessageType.None;
        public int chatMaxBuffer = 5;
        public List<ChatMessage> chatBuffer = new List<ChatMessage>(); // chatBuffer[0] is most recent
        public List<ChatMessage> chatFullBuffer = new List<ChatMessage>(); //same as above, holds last several messages
        public string chatEntryBuffer = "";
    }
}
