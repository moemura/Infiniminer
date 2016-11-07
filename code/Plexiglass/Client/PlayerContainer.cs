using Infiniminer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public class PlayerContainer
    {
        public ICamera playerCamera = null;
        public Vector3 playerPosition = Vector3.Zero;
        public Vector3 playerVelocity = Vector3.Zero;
        public PlayerClass playerClass;
        public PlayerTools[] playerTools = new PlayerTools[1] { PlayerTools.Pickaxe };
        public int playerToolSelected = 0;
        public BlockType[] playerBlocks = new BlockType[1] { BlockType.None };
        public int playerBlockSelected = 0;
        public PlayerTeam playerTeam = PlayerTeam.Red;
        public bool playerDead = true;
        public uint playerOre = 0;
        public uint playerCash = 0;
        public uint playerWeight = 0;
        public uint playerOreMax = 0;
        public uint playerWeightMax = 0;
        public bool playerRadarMute = false;
        public float playerToolCooldown = 0;
        public uint playerMyId = 0;
        public float radarCooldown = 0;
        public float radarDistance = 0;
        public float radarValue = 0;
        public float constructionGunAnimation = 0;
        public string playerHandle = "Player";
        public ScreenEffect screenEffect = ScreenEffect.None;
        public double screenEffectCounter = 0;

    }
}
