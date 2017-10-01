using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketPlayerJoinedTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketPlayerJoined_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketPlayerJoined, PacketPlayerJoinedHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketPlayerJoined_Serializing_Successful()
        {
            const uint PLAYER_ID = 0xDEADBEEF;
            const string PLAYER_NAME = "TestPlayer";
            var packet = new PacketPlayerJoined(PLAYER_ID, PLAYER_NAME, true, false);
            var data = packet.Serialize();
            var comp = new byte[] { 0xef,
                                    0xbe,
                                    0xad,
                                    0xde,
                                    0x54,
                                    0x65,
                                    0x73,
                                    0x74,
                                    0x50,
                                    0x6c,
                                    0x61,
                                    0x79,
                                    0x65,
                                    0x72,
                                    0x00,
                                    0x01,
                                    0x00};

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for(var i = 0;i < data.Length;i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketPlayerJoined_Deserialzing_Successful()
        {
            var comp = new byte[] { 0xef,
                                    0xbe,
                                    0xad,
                                    0xde,
                                    0x54,
                                    0x65,
                                    0x73,
                                    0x74,
                                    0x50,
                                    0x6c,
                                    0x61,
                                    0x79,
                                    0x65,
                                    0x72,
                                    0x00,
                                    0x01,
                                    0x00};

            var packet = new PacketPlayerJoined();

            packet.Deserialize(comp);

            Assert.AreEqual(0xDEADBEEF, packet.PlayerId);
            Assert.AreEqual("TestPlayer", packet.PlayerName);
            Assert.AreEqual(true, packet.ThisIsMe);
            Assert.AreEqual(false, packet.PlayerAlive);
        }
    }
}
