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
    public class PacketTriggerExplosionTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketTriggerExplosionTest_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT, null, null);
            packetRegistry.RegisterPacket<PacketTriggerExplosion, PacketTriggerExplosionHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketTriggerExplosionTest_Serializing_Successful()
        {
            var packet = new PacketTriggerExplosion();
            packet.blockPos = new Vector3(1.2f, 3.4f, 5.6f);

            var data = packet.Serialize();
            var comp = new byte[] { 0x9a,
                                    0x99,
                                    0x99,
                                    0x3f,
                                    0x9a,
                                    0x99,
                                    0x59,
                                    0x40,
                                    0x33,
                                    0x33,
                                    0xb3,
                                    0x40 };

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for(int i =0;i < data.Length;i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketTriggerExplosionTest_Deserialzing_Successful()
        {
            var comp = new byte[] { 0x9a,
                                    0x99,
                                    0x99,
                                    0x3f,
                                    0x9a,
                                    0x99,
                                    0x59,
                                    0x40,
                                    0x33,
                                    0x33,
                                    0xb3,
                                    0x40 };

            var packet = new PacketTriggerExplosion();

            packet.Deserialize(comp);

            Assert.AreEqual(packet.blockPos, new Vector3(1.2f, 3.4f, 5.6f));
        }
    }
}
