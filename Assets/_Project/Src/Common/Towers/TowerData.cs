using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Src.Common.Towers
{
    [Serializable]
    public class TowerData
    {
        [Header("General Settings")] public string towerName = "New Tower";
        public TowerType towerType;
        public TowerRank towerRank;

        [Header("Abilities")] public List<Ability> abilities;

        [Header("Requirements")] public List<Requirement> requirements;

        [Header("Timer Settings")] public bool hasTimer;

        [Tooltip("Time in seconds for attack/action cooldown if timer is enabled")]
        public float timerDuration;
    }
}