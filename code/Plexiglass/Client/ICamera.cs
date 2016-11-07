using Microsoft.Xna.Framework;

namespace Plexiglass.Client
{
    public interface ICamera
    {
        float Pitch { get; set; }
        float Yaw { get; set; }
        Vector3 Position { get; set; }
        Matrix ViewMatrix { get; set; }
        Matrix ProjectionMatrix { get; set; }

        Vector3 GetLookVector();
        Vector3 GetRightVector();
        void Update();
    }
}