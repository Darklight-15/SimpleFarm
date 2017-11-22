using UnityEngine;

public class ProgramScript : MonoBehaviour {

    public int Position;
    public int ProgramID;
    public string Name;
    public string Info;

    public ProgramScript(int p, int id, string n, string i)
    {
        Position = p;
        ProgramID = id;
        Name = n;
        Info = i;
    }

    public ProgramScript(ref ProgramScript other)
    {
        Position = other.Position;
        ProgramID = other.ProgramID;
        Name = other.Name;
        Info = other.Info;
    }
}
