using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Plexiglass.Networking;
using Plexiglass.Networking.Packets;
using Plexiglass.Networking.Handlers;
using System.Collections.Generic;

namespace PlexiglassTests
{
    [TestClass]
    public class PacketRegistryTest
    {
        [TestMethod]
        public void PacketRegistry_RegisteringPacket_Registers()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER);
            packetRegistry.RegisterPacket<PacketPing, PacketPingHandler>();
        }

        [TestMethod]
        public void PacketRegistry_RegisteringAndHandlingPacket_Handles()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER);
            const uint DATUM = 0xC0FFFEE;
            var packet = new PacketPing(DATUM);
            packetRegistry.RegisterPacket<PacketPing, PacketPingHandler>();

            var data = packetRegistry.HandlePacket(packet);

            Assert.AreEqual("Ping: 0x" + DATUM.ToString("X6"), data, "Packet was handled incorrectly!");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PacketRegistry_RegisteringDuplicatePacket_ThrowsException()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER);

            // Once...
            packetRegistry.RegisterPacket<PacketPing, PacketPingHandler>();

            // Twice... should error here!
            packetRegistry.RegisterPacket<PacketPing, PacketPingHandler>();
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void PacketRegistry_HandlingUnregisteredPacket_ThrowsException()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.CLIENT_TO_SERVER);

            var packet = new PacketPing(0xC0FFEE);

            packetRegistry.HandlePacket(packet);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PacketRegistry_RegisteringWithWrongDirection_ThrowsException()
        {
            var packetRegistry = new PlexiglassPacketRegistry(PacketDirectionality.ANTIDIRECTIONAL);

            packetRegistry.RegisterPacket<PacketPing, PacketPingHandler>();
        }
    }
}
