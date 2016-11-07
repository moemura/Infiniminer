using Microsoft.Xna.Framework;

namespace Plexiglass.Client.Engine
{
    public interface IParticleEngine : IEngine
    {
        void CreateExplosionDebris(Vector3 explosionPosition);
    }
}