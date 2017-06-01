using UnityEngine;

[System.Serializable]
public class CSequenceData
{
    [SerializeField]
    private float _Beat;
    public float Beat
    {
        get
        {
            return _Beat;
        }
    }
    private float _Time;
    public float Time
    {
        get
        {
            return _Time;
        }
    }
    [SerializeField]
    private InputCode _Input;
    public InputCode Input
    {
        get
        {
            return _Input;
        }
    }

    [SerializeField]
    private string _ActionCode = string.Empty;
    public string ActionCode
    {
        get
        {
            return _ActionCode;
        }
    }

    public CSequenceData(float tBeat, float tTime)
    {
        _Beat = tBeat;
        _Time = tTime;
    }
    public CSequenceData(float tBeat)
    {
        _Beat = tBeat;
    }
    public CSequenceData() { }

    public void SetTime(float tTime)
    {
        _Time = tTime;
    }

    public CSequenceData ToCopy(float tBeat)
    {
        CSequenceData tCopy = new CSequenceData(tBeat);
        tCopy._Input = Input;
        tCopy._ActionCode = ActionCode;
        return tCopy;
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

