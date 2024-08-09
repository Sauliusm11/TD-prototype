using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public int id;
    public List<WaveEnemy> Enemies = new List<WaveEnemy>();
}
