using Infiniminer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketSetBeaconTest
    {
        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketSetBeacon_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketSetBeacon, PacketSetBeaconHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketSetBeacon_Serializing_Successful()
        {
            var packet = new PacketSetBeacon
            {
                Position = new Vector3(1.337f, 4.200f, 9.001f),
                Text = "PingPong",
                Team = PlayerTeam.Red
            };

            var data = packet.Serialize();
            var comp = new byte[]{  0x50, // Ripped directly from the output of a successful serialization.
                                    0x69, // Good practice? Not really.
                                    0x6e, // But, if it works for this one,
                                    0x67, // it'll work for the next, and will
                                    0x50, // protect against future bad modifications.
                                    0x6f,
                                    0x6e,
                                    0x67,
                                    0x00,
                                    0xd1,
                                    0x22,
                                    0xab,
                                    0x3f,
                                    0x66,
                                    0x66,
                                    0x86,
                                    0x40,
                                    0x19,
                                    0x04,
                                    0x10,
                                    0x41,
                                    0x01 };

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for (var i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }
        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketSetBeacon_Deserialzing_Successful()
        {
            var comp = new byte[]{  0x50, // Ripped directly from the output of a successful serialization.
                                    0x69, // Good practice? Not really.
                                    0x6e, // But, if it works for this one,
                                    0x67, // it'll work for the next, and will
                                    0x50, // protect against future bad modifications.
                                    0x6f,
                                    0x6e,
                                    0x67,
                                    0x00,
                                    0xd1,
                                    0x22,
                                    0xab,
                                    0x3f,
                                    0x66,
                                    0x66,
                                    0x86,
                                    0x40,
                                    0x19,
                                    0x04,
                                    0x10,
                                    0x41,
                                    0x01 };

            var packet = new PacketSetBeacon();

            packet.Deserialize(comp);

            Assert.AreEqual(PlayerTeam.Red, packet.Team);
            Assert.AreEqual("PingPong", packet.Text);
            Assert.AreEqual(1.337f, packet.Position.X);
            Assert.AreEqual(4.200f, packet.Position.Y);
            Assert.AreEqual(9.001f, packet.Position.Z);
        }
    }
}
