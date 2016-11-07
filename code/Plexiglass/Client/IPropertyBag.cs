using Infiniminer;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Plexiglass.Client.Engine;
using Plexiglass.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public interface IPropertyBag
    {
        void PlaySound(InfiniminerSound sound);
        void PlaySound(InfiniminerSound sound, Vector3 position);
        void SetPlayerClass(PlayerClass playerClass);
        void KillPlayer(string deathMessage);

        PlayerContainer PlayerContainer { get; set; }
        SettingsContainer SettingsContainer { get; set; }
        ChatContainer ChatContainer { get; set; }
        TeamContainer TeamContainer { get; set; }

        bool[,] MapLoadProgress { get; set; }

        NetClient NetClient { get; set; }

        Dictionary<uint, Player> PlayerList { get; set; }

        void RegisterEngine<T>(T engine, string engineName)
            where T: IEngine;
        T GetEngine<T>(string engineName)
            where T: IEngine;
    }
}
