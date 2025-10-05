namespace _Project.Src.Common.Towers
{
    [System.Serializable]
    public enum TowerType
    {
        SingleTarget,
        AreaOfEffect,
        Support,
        Special
    }
    
    [System.Serializable]
    public enum TowerRank
    {
        Basic,
        Advanced,
        Elite,
        Legendary
    }
}