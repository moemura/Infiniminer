using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketPlayerLeftTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketPlayerLeft_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketPlayerLeft, PacketPlayerLeftHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketPlayerKeft_Serializing_Successful()
        {
            const uint PLAYER_ID = 0xDEADBEEF;
            var packet = new PacketPlayerLeft(PLAYER_ID);
            var data = packet.Serialize();
            var comp = new byte[] { 0xef,
                                    0xbe,
                                    0xad,
                                    0xde};

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for(var i = 0;i < data.Length;i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketPlayerLeft_Deserialzing_Successful()
        {
            var comp = new byte[] { 0xef,
                                    0xbe,
                                    0xad,
                                    0xde};

            var packet = new PacketPlayerLeft();

            packet.Deserialize(comp);

            Assert.AreEqual(0xDEADBEEF, packet.PlayerId);

        }
    }
}
