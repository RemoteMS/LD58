using System;
using System.Collections.Generic;
using _Project.Src.Common.Towers;
using _Project.Src.Common.Towers.Settings;

namespace _Project.Src.Common.GameProcessing.Settings
{
    [Serializable]
    public class TowerSpawnerSettings
    {
        public List<Tower> towers;

        public TowerSettings settings;
    }
}