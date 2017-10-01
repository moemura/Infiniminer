using Infiniminer;
using Microsoft.Xna.Framework;

namespace Plexiglass.Client
{
    public class PlayerContainer
    {
        public ICamera PlayerCamera = null;
        public Vector3 PlayerPosition = Vector3.Zero;
        public Vector3 PlayerVelocity = Vector3.Zero;
        public PlayerClass PlayerClass;
        public PlayerTools[] PlayerTools = { Infiniminer.PlayerTools.Pickaxe };
        public int PlayerToolSelected = 0;
        public BlockType[] PlayerBlocks = { BlockType.None };
        public int PlayerBlockSelected = 0;
        public PlayerTeam PlayerTeam = PlayerTeam.Red;
        public bool PlayerDead = true;
        public uint PlayerOre = 0;
        public uint PlayerCash = 0;
        public uint PlayerWeight = 0;
        public uint PlayerOreMax = 0;
        public uint PlayerWeightMax = 0;
        public bool PlayerRadarMute = false;
        public float PlayerToolCooldown = 0;
        public uint PlayerMyId = 0;
        public float RadarCooldown = 0;
        public float RadarDistance = 0;
        public float RadarValue = 0;
        public float ConstructionGunAnimation = 0;
        public string PlayerHandle = "Player";
        public ScreenEffect ScreenEffect = ScreenEffect.None;
        public double ScreenEffectCounter = 0;

    }
}
