using Plexiglass.Networking.Packets;
using System;
using Plexiglass.Client;
using Plexiglass.Client.States;
using Infiniminer;
using Microsoft.Xna.Framework;
using Plexiglass.Client.Engine;

namespace Plexiglass.Networking.Handlers
{
    public class PacketTriggerExplosionHandler : IPacketHandler<PacketTriggerExplosion>
    {
        public object HandlePacket(PacketTriggerExplosion packet, IPropertyBag propertyBag = null, IStateMachine gameInstance = null)
        {
            if (propertyBag == null) return null;

            // Play the explosion sound.
            propertyBag.PlaySound(InfiniminerSound.Explosion, packet.BlockPos);

            // Create some particles.
            propertyBag.GetEngine<IParticleEngine>("particleEngine").CreateExplosionDebris(packet.BlockPos);

            // Figure out what the effect is.
            var distFromExplosive =
                (packet.BlockPos + 0.5f * Vector3.One - propertyBag.PlayerContainer.PlayerPosition).Length();
            if (distFromExplosive < 3)
                propertyBag.KillPlayer(Defines.deathByExpl); //"WAS KILLED IN AN EXPLOSION!");
            else if (distFromExplosive < 8)
            {
                // If we're not in explosion mode, turn it on with the minimum ammount of shakiness.
                if (propertyBag.PlayerContainer.ScreenEffect != ScreenEffect.Explosion)
                {
                    propertyBag.PlayerContainer.ScreenEffect = ScreenEffect.Explosion;
                    propertyBag.PlayerContainer.ScreenEffectCounter = 2;
                }
                // If this bomb would result in a bigger shake, use its value.
                propertyBag.PlayerContainer.ScreenEffectCounter =
                    Math.Min(propertyBag.PlayerContainer.ScreenEffectCounter, (distFromExplosive - 2) / 5);
            }

            return null;
        }
    }
}
