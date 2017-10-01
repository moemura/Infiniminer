using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketResourceUpdateeTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketResourceUpdate_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketResourceUpdate, PacketResourceUpdateHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketResourceUpdate_Serializing_Successful()
        {
            var packet = new PacketResourceUpdate
            {
                NewOre = 0xDEADBEEF,
                NewCash = 0xC0FFFFEE,
                NewWeight = 0xBAADFEED,
                NewOreMax = 0x0DDBA112,
                NewWeightMax = 0xCAFEBABE,
                NewOreTeam = 0xBAADBEEF,
                NewTeamRedCash = 0x5CAFF01D,
                NewTeamBlueCash = 0xF007BA11
            };

            var data = packet.Serialize();
            var comp = new byte[] { 0xEF, 0xBE, 0xAD, 0xDE,
                                    0xEE, 0XFF, 0xFF, 0xC0,
                                    0xED, 0xFE, 0xAD, 0xBA,
                                    0x12, 0xA1, 0xDB, 0x0D,
                                    0xBE, 0xBA, 0xFE, 0xCA,
                                    0xEF, 0xBE, 0xAD, 0xBA,
                                    0x1D, 0xF0, 0xAF, 0x5C,
                                    0x11, 0xBA, 0x07, 0xF0};

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for (var i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketResourceUpdate_Deserialzing_Successful()
        {
            var comp = new byte[] { 0xEF, 0xBE, 0xAD, 0xDE,
                                    0xEE, 0XFF, 0xFF, 0xC0,
                                    0xED, 0xFE, 0xAD, 0xBA,
                                    0x12, 0xA1, 0xDB, 0x0D,
                                    0xBE, 0xBA, 0xFE, 0xCA,
                                    0xEF, 0xBE, 0xAD, 0xBA,
                                    0x1D, 0xF0, 0xAF, 0x5C,
                                    0x11, 0xBA, 0x07, 0xF0};

            var packet = new PacketResourceUpdate();

            packet.Deserialize(comp);

            Assert.AreEqual(0xDEADBEEF, packet.NewOre);
            Assert.AreEqual(0xC0FFFFEE, packet.NewCash);
            Assert.AreEqual(0xBAADFEED, packet.NewWeight);
            Assert.AreEqual((uint)0x0DDBA112, packet.NewOreMax);
            Assert.AreEqual(0xCAFEBABE, packet.NewWeightMax);
            Assert.AreEqual(0xBAADBEEF, packet.NewOreTeam);
            Assert.AreEqual((uint)0x5CAFF01D, packet.NewTeamRedCash);
            Assert.AreEqual(0xF007BA11, packet.NewTeamBlueCash);
        }
    }
}
