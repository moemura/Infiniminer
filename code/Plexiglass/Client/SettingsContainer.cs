using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plexiglass.Client
{
    public class SettingsContainer
    {
        public string playerHandle = "Player";
        public float volumeLevel = 1.0f;
        public float mouseSensitivity = 0.05f;
        public bool renderPretty = true;
        public bool drawFrameRate = false;
        public bool invertMouseYAxis = false;
        public bool noSound = false;
    }
}
