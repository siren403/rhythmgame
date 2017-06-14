using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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

    public float EvaluationFailRatio = 0.3f;
    public float EvaluationNormalRatio = 0.4f;
    public float EvaluationGoodRatio = 0.3f;

    public string EvaluationTitle = "Title";
    public string EvaluationFailText = "Fail";
    public string EvaluationNormalText = "Normal";
    public string EvaluationGoodText = "Good";

    public AudioClip Music = null;
    public List<CSequenceData> SequenceList = new List<CSequenceData>();

    public List<string> ActionCodeList = new List<string>() { "None" };


}
