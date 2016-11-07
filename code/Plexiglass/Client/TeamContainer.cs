using Infiniminer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public class TeamContainer
    { 
        public uint teamOre = 0;
        public uint teamRedCash = 0;
        public uint teamBlueCash = 0;
        public PlayerTeam teamWinners = PlayerTeam.None;
        public Dictionary<Vector3, Beacon> BeaconList { get; set; }
    }
}
