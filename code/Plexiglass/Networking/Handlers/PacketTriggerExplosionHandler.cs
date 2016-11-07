using Plexiglass.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            // Play the explosion sound.
            propertyBag.PlaySound(InfiniminerSound.Explosion, packet.blockPos);

            // Create some particles.
            propertyBag.GetEngine<IParticleEngine>("particleEngine").CreateExplosionDebris(packet.blockPos);

            // Figure out what the effect is.
            float distFromExplosive = (packet.blockPos + 0.5f * Vector3.One - propertyBag.PlayerContainer.playerPosition).Length();
            if (distFromExplosive < 3)
                propertyBag.KillPlayer(Defines.deathByExpl);//"WAS KILLED IN AN EXPLOSION!");
            else if (distFromExplosive < 8)
            {
                // If we're not in explosion mode, turn it on with the minimum ammount of shakiness.
                if (propertyBag.PlayerContainer.screenEffect != ScreenEffect.Explosion)
                {
                    propertyBag.PlayerContainer.screenEffect = ScreenEffect.Explosion;
                    propertyBag.PlayerContainer.screenEffectCounter = 2;
                }
                // If this bomb would result in a bigger shake, use its value.
                propertyBag.PlayerContainer.screenEffectCounter = Math.Min(propertyBag.PlayerContainer.screenEffectCounter, (distFromExplosive - 2) / 5);
            }

            return null;
        }
    }
}
