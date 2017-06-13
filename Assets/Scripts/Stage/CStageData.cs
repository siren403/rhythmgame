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

    public float EvaluationFailRatio = 0.3f;
    public float EvaluationNormalRatio = 0.4f;
    public float EvaluationGoodRatio = 0.3f;

    private string mEvaluationTitle;
    private string mEvaluationFailText;
    private string mEvaluationNormalText;
    private string mEvaluationGoodText;
    public string EvaluationTitle
    {
        get
        {
            if(string.IsNullOrEmpty(mEvaluationTitle))
            {
                mEvaluationTitle = "title";
            }
            return mEvaluationTitle;
        }
        set { mEvaluationTitle = value; }
    }
    public string EvaluationFailText
    {
        get
        {
            if (string.IsNullOrEmpty(mEvaluationFailText))
            {
                mEvaluationFailText = "fail";
            }
            return mEvaluationFailText;
        }
        set { mEvaluationFailText = value; }
    }
    public string EvaluationNormalText
    {
        get
        {
            if (string.IsNullOrEmpty(mEvaluationNormalText))
            {
                mEvaluationNormalText = "normal";
            }
            return mEvaluationNormalText;
        }
        set { mEvaluationNormalText = value; }
    }
    public string EvaluationGoodText
    {
        get
        {
            if (string.IsNullOrEmpty(mEvaluationGoodText))
            {
                mEvaluationGoodText = "good";
            }
            return mEvaluationGoodText;
        }
        set { mEvaluationGoodText = value; }
    }

    public AudioClip Music = null;
    public List<CSequenceData> SequenceList = new List<CSequenceData>();

    public List<string> ActionCodeList = new List<string>() { "None" };


}
