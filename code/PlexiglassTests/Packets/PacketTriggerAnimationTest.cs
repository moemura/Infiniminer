using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketTriggerAnimationTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketTriggerAnimation_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketTriggerAnimation, PacketTriggerAnimationHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketTriggerAnimation_Serializing_Successful()
        {
            var packet = new PacketTriggerAnimation {AnimationTime = 1234.56f};

            var data = packet.Serialize();
            var comp = new byte[] { 0xec, 0x51, 0x9a, 0x44 };

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for (var i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketTriggerAnimation_Deserialzing_Successful()
        {
            var comp = new byte[] { 0xec, 0x51, 0x9a, 0x44 };

            var packet = new PacketTriggerAnimation();

            packet.Deserialize(comp);

            Assert.AreEqual(1234.56f, packet.AnimationTime);
        }
    }
}
