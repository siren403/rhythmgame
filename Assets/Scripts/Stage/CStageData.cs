using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStageData : ScriptableObject
{
    public string StageName;
    public int BPM;
    public float BPS
    {
        get
        {
            if(BPM == 0)
            {
                return 0;
            }
            return 60.0f / (float)BPM;
        }
    }
    public float StartBeatOffset;
    public float PerfectRange = 0.1f;

    public float EvaluationLowRatio = 0.3f;
    public float EvaluationMiddleRatio = 0.4f;
    public float EvaluationHighRatio = 0.3f;

    public AudioClip Music = null;
    public List<CSequenceData> SequenceList = new List<CSequenceData>();

    public List<string> ActionCodeList = new List<string>() { "None" };
}
