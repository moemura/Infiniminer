using Infiniminer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Plexiglass.Client
{
    public class TeamContainer
    { 
        public uint TeamOre = 0;
        public uint TeamRedCash = 0;
        public uint TeamBlueCash = 0;
        public PlayerTeam TeamWinners = PlayerTeam.None;
        public Dictionary<Vector3, Beacon> BeaconList { get; set; }
    }
}
