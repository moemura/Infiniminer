using Infiniminer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketResourceUpdateeTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketResourceUpdate_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT, null);
            packetRegistry.RegisterPacket<PacketResourceUpdate, PacketResourceUpdateHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketResourceUpdate_Serializing_Successful()
        {
            var packet = new PacketResourceUpdate();
            packet.newOre =             0xDEADBEEF;
            packet.newCash =            0xC0FFFFEE;
            packet.newWeight =          0xBAADFEED;
            packet.newOreMax =          0x0DDBA112;
            packet.newWeightMax =       0xCAFEBABE;
            packet.newOreTeam =         0xBAADBEEF;
            packet.newTeamRedCash =     0x5CAFF01D;
            packet.newTeamBlueCash =    0xF007BA11;

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

            for (int i = 0; i < data.Length; i++)
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

            Assert.AreEqual((uint)0xDEADBEEF, packet.newOre);
            Assert.AreEqual((uint)0xC0FFFFEE, packet.newCash);
            Assert.AreEqual((uint)0xBAADFEED, packet.newWeight);
            Assert.AreEqual((uint)0x0DDBA112, packet.newOreMax);
            Assert.AreEqual((uint)0xCAFEBABE, packet.newWeightMax);
            Assert.AreEqual((uint)0xBAADBEEF, packet.newOreTeam);
            Assert.AreEqual((uint)0x5CAFF01D, packet.newTeamRedCash);
            Assert.AreEqual((uint)0xF007BA11, packet.newTeamBlueCash);
        }
    }
}
