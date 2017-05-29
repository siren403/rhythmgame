[System.Serializable]
public class CSequenceData
{
    public float beat;
    public float time;
    public InputCode input;
    public int soundCode;
    public int actionCode;

    public CSequenceData(float tBeat, float tTime)
    {
        beat = tBeat;
        time = tTime;
        actionCode = -1;
        soundCode = -1;
        input = InputCode.None;
    }
}

public enum InputResult
{
    None,
    Fast, Perfect, Late,
    Fail,
}
public enum InputCode
{
    None,
    SpaceDown, SpaceUp,
}

