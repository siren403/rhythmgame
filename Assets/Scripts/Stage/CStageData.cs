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

    public AudioClip Music = null;
    public List<AudioClip> SoundEffects = new List<AudioClip>(); 
    public List<CSequenceData> SequenceList = new List<CSequenceData>();
}
