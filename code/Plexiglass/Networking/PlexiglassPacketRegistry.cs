using Plexiglass.Client;
using Plexiglass.Client.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Networking
{
    public class PlexiglassPacketRegistry : IPacketRegistry
    {
        /// <summary>
        /// PacketRegistrations is the primary registry for registered packet types, with a Packet class as the key
        /// and a PacketHandler class as the value.
        /// </summary>
        private Dictionary<Type, Type> PacketRegistrations;

        private IPropertyBag propertyBag;
        private IStateMachine gameInstance;

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
            PacketRegistrations = new Dictionary<Type, Type>();
            this.propertyBag = propertyBag;
            this.gameInstance = gameInstance;
        }

        public void RegisterPacket<P, H>() 
            where P: IPacket
            where H: IPacketHandler<P>
        {
            // Have we already registered our packet?
            if (PacketRegistrations.ContainsKey(typeof(P)))
            {
                throw new Exception("Packet type " + typeof(P).FullName + " has already been registered in PacketRegistry!");
            }

            // An instance of the Packet type for some of the more iffy functions below.
            var pExample = (IPacket)Activator.CreateInstance(typeof(P));

            // Did the directionality of our registered packet match up with the PacketRegistry's directionality?
            if(!PacketDirectionalityExt.FuzzyEquals(pExample.GetDirectionality(), Directionality))
            {
                throw new Exception("Packet type " + typeof(P).FullName + " has directionality type "
                    + Enum.GetName(typeof(PacketDirectionality), pExample.GetDirectionality())
                    + ", but PacketRegistry is set for directionality type "
                    + Enum.GetName(typeof(PacketDirectionality), this.Directionality));
            }

            PacketRegistrations.Add(typeof(P), typeof(H));
        }

        public object HandlePacket<P>(P packet)
            where P: IPacket
        {
            if(!PacketRegistrations.ContainsKey(typeof(P)))
            {
                throw new KeyNotFoundException("Packet type " + typeof(P).FullName + " not registered in PacketRegistry!");
            }

            IPacketHandler<P> packetHandler = (IPacketHandler<P>)Activator.CreateInstance(PacketRegistrations[typeof(P)]);
            return packetHandler.HandlePacket(packet, this.propertyBag, this.gameInstance);
        }
    }
}
