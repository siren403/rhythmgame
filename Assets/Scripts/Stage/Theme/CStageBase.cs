using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public abstract class CStageBase : MonoBehaviour, ISequenceReceiver
{
    private AudioSource mCachedAudioSource = null;
    protected AudioSource mAudioSource
    {
        get
        {
            if(mCachedAudioSource == null)
            {
                mCachedAudioSource = GetComponent<AudioSource>();
            }
            return mCachedAudioSource;
        }
    }

    [SerializeField]
    private CStageData mStageData = null;
    public CStageData StageData
    {
        get
        {
            return mStageData;
        }
    }

    public abstract void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData);

    public abstract void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData);

    public abstract void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult);

    public abstract IEnumerator StartPrologue();

    public abstract void SetScene(CScenePlayGame tScene);
}
