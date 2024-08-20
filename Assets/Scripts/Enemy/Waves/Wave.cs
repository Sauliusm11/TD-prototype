using System.Collections.Generic;
/// <summary>
/// Wave class used for parsing and used by the WaveManager
/// </summary>
[System.Serializable]
public class Wave
{
    public int id;
    public List<WaveEnemy> Enemies = new List<WaveEnemy>();
}
