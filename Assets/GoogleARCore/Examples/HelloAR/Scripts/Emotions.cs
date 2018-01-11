using SQLite4Unity3d;
using System;
public class Emotions
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Emotion { get; set; }
    public float Score { get; set; }
    public DateTime Time { get; set; }
    public string HighestObject { get; set; }
    public string Object { get; set; }
        
    public override string ToString()
    {
        return string.Format("[Emotions: Id={0}, Emotion={1},  Score={2}, Time={3}, HighestObject={4}, Object={5}]", Id, Emotion, Score, Time, HighestObject, Object);
    }
}