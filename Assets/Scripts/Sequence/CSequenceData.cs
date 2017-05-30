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
    private int _SoundCode;
    public int SoundCode
    {
        get
        {
            return _SoundCode;
        }
    }
    [SerializeField]
    private int _ActionCode;
    public int ActionCode
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
        _ActionCode = -1;
        _SoundCode = -1;
        _Input = InputCode.None;
    }
    public CSequenceData(float tBeat)
    {
        _Beat = tBeat;
        _ActionCode = -1;
        _SoundCode = -1;
        _Input = InputCode.None;
    }
    public void SetTime(float tTime)
    {
        _Time = tTime;
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

