using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

[RequireComponent(typeof(AudioSource))]
public abstract class CThemeBase : PresenterBase, ISequenceReceiver
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

    public abstract void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData);

    public abstract void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData);

    public abstract void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult);
}
