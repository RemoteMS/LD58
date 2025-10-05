using UnityEngine;

namespace _Project.Src.Common.Towers
{
    [System.Serializable]
    public struct Ability
    {
        public string abilityName;
        [TextArea] public string description;
        public float damage;
        public float cooldown;
        public float range;
    }
}