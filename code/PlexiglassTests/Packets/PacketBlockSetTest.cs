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
    public class PacketBlockSetTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketBlockSet_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT, null);
            packetRegistry.RegisterPacket<PacketBlockSet, PacketBlockSetHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketBlockSet_Serializing_Successful()
        {
            var packet = new PacketBlockSet();
            packet.x =          0xFEEDAD09;
            packet.y =          0xCAFEBABE;
            packet.z =          0xBA174A11;
            packet.blockType =  0xC001BEA7;
            var data = packet.Serialize();
            var comp = new byte[] { 0x09, 0xAD, 0xED, 0xFE,
                                    0xBE, 0xBA, 0xFE, 0xCA,
                                    0x11, 0x4A, 0x17, 0xBA,
                                    0xA7, 0xBE, 0x01, 0xC0,};

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for (int i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketBlockSet_Deserialzing_Successful()
        {
            var comp = new byte[] { 0x09, 0xAD, 0xED, 0xFE,
                                    0xBE, 0xBA, 0xFE, 0xCA,
                                    0x11, 0x4A, 0x17, 0xBA,
                                    0xA7, 0xBE, 0x01, 0xC0,};


            var packet = new PacketBlockSet();

            packet.Deserialize(comp);

            Assert.AreEqual(0xFEEDAD09, packet.x);
            Assert.AreEqual(0xCAFEBABE, packet.y);
            Assert.AreEqual(0xBA174A11, packet.z);
            Assert.AreEqual(0xC001BEA7, packet.blockType);
        }
    }
}
