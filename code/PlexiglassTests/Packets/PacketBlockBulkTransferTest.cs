using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Handlers;
using Plexiglass.Networking.Packets;
using System;

namespace PlexiglassTests.Packets
{
    [TestClass]
    public class PacketBlockBulkTransferTest
    {

        [TestMethod]
        [TestCategory("Packet Registration")]
        public void PacketBlockBulkTransfer_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.SERVER_TO_CLIENT);
            packetRegistry.RegisterPacket<PacketBlockBulkTransfer, PacketBlockBulkTransferHandler>();
        }

        [TestMethod]
        [TestCategory("Packet Serialization")]
        public void PacketBlockBulkTransfer_Serializing_Successful()
        {
            const uint X = 0xDEADBEEF;
            const uint Y = 0xC0FFFFEE;
            const bool IS_COMPRESSING = true;

            var packet = new PacketBlockBulkTransfer(X, Y, IS_COMPRESSING, new ushort[PacketBlockBulkTransfer.Y_SIZE, PacketBlockBulkTransfer.Z_SIZE]);

            var data = packet.Serialize();
            var comp = new byte[PacketBlockBulkTransfer.PACKET_SIZE];
            new byte[] { 0x01, 0xEF, 0xBE, 0xAD, 0xDE, 0xEE, 0xFF, 0xFF, 0xC0 }.CopyTo(comp, 0);
            for (var i = PacketBlockBulkTransfer.WITHOUT_BLOCKLIST_SIZE; i < PacketBlockBulkTransfer.PACKET_SIZE;i++)
            {
                comp[i] = 0x00;
            }

            Assert.AreEqual(comp.Length, data.Length, "Serialized data was not expected length!");

            for(var i =0;i < data.Length;i++)
            {
                Assert.AreEqual(comp[i], data[i]);
            }

        }

        [TestMethod]
        [TestCategory("Packet Deserialization")]
        public void PacketBlockBulkTransfer_Deserialzing_Successful()
        {
            var comp = new byte[PacketBlockBulkTransfer.PACKET_SIZE];
            new byte[] { 0x01, 0xEF, 0xBE, 0xAD, 0xDE, 0xEE, 0xFF, 0xFF, 0xC0 }.CopyTo(comp, 0);
            for (var i = PacketBlockBulkTransfer.WITHOUT_BLOCKLIST_SIZE; i < PacketBlockBulkTransfer.PACKET_SIZE; i++)
            {
                comp[i] = 0x00;
            }

            var packet = new PacketBlockBulkTransfer();

            packet.Deserialize(comp);

            Assert.AreEqual(true, packet.IsCompressed);
            Assert.AreEqual(0xDEADBEEF, packet.X);
            Assert.AreEqual(0xC0FFFFEE, packet.Y);

            var iterator = PacketBlockBulkTransfer.WITHOUT_BLOCKLIST_SIZE;

            for(var y =0;y < PacketBlockBulkTransfer.Y_SIZE;y++)
            {
                for(var z =0;z < PacketBlockBulkTransfer.Z_SIZE;z++)
                {
                    Assert.AreEqual(packet.BlockList[y, z], BitConverter.ToUInt16(comp, iterator));
                    iterator += 2;
                }
            }
        }
    }
}
