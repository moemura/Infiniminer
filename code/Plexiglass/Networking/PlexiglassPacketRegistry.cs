using Plexiglass.Client;
using Plexiglass.Client.States;
using System;
using System.Collections.Generic;

namespace Plexiglass.Networking
{
    public class PlexiglassPacketRegistry : IPacketRegistry
    {
        /// <summary>
        /// PacketRegistrations is the primary registry for registered packet types, with a Packet class as the key
        /// and a PacketHandler class as the value.
        /// </summary>
        private readonly Dictionary<Type, Type> packetRegistrations;

        private readonly IPropertyBag propertyBag;
        private readonly IStateMachine gameInstance;

        /// <summary>
        /// The Directionality of this particular PacketRegistry.
        /// </summary>
        public PacketDirectionality Directionality { get; set; }

        public PlexiglassPacketRegistry(PacketDirectionality directionality, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if(directionality == PacketDirectionality.BIDIRECTIONAL || directionality == PacketDirectionality.POLYDIRECTIONAL)
            {
                Console.WriteLine("[PLXI] [WARNING] A PacketRegistry with "
                                    + Enum.GetName(typeof(PacketDirectionality), directionality)
                                    + " isn't reccomended! This is a direction circumvention of the purpose of directionality.");
            }
            Directionality = directionality;
            packetRegistrations = new Dictionary<Type, Type>();
            this.propertyBag = propertyBag;
            this.gameInstance = gameInstance;
        }

        public void RegisterPacket<TP, TH>() 
            where TP: IPacket
            where TH: IPacketHandler<TP>
        {
            // Have we already registered our packet?
            if (packetRegistrations.ContainsKey(typeof(TP)))
            {
                throw new Exception("Packet type " + typeof(TP).FullName + " has already been registered in PacketRegistry!");
            }

            // An instance of the Packet type for some of the more iffy functions below.
            var pExample = (IPacket)Activator.CreateInstance(typeof(TP));

            // Did the directionality of our registered packet match up with the PacketRegistry's directionality?
            if(!PacketDirectionalityExt.FuzzyEquals(pExample.GetDirectionality(), Directionality))
            {
                throw new Exception("Packet type " + typeof(TP).FullName + " has directionality type "
                    + Enum.GetName(typeof(PacketDirectionality), pExample.GetDirectionality())
                    + ", but PacketRegistry is set for directionality type "
                    + Enum.GetName(typeof(PacketDirectionality), Directionality));
            }

            packetRegistrations.Add(typeof(TP), typeof(TH));
        }

        public object HandlePacket<TP>(TP packet)
            where TP: IPacket
        {
            if(!packetRegistrations.ContainsKey(typeof(TP)))
            {
                throw new KeyNotFoundException("Packet type " + typeof(TP).FullName + " not registered in PacketRegistry!");
            }

            var packetHandler = (IPacketHandler<TP>)Activator.CreateInstance(packetRegistrations[typeof(TP)]);
            return packetHandler.HandlePacket(packet, propertyBag, gameInstance);
        }
    }
}
