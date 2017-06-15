using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    

    private static AudioManager mInstance = null;
    public static AudioManager Inst
    {
        get
        {
            if(mInstance == null)
            {
                mInstance = FindObjectOfType<AudioManager>();
                if(mInstance == null)
                {
                    var go = new GameObject("AudioManager");
                    mInstance = go.AddComponent<AudioManager>();
                    mInstance.Init();
                }
                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    private AudioSource mCachedAudioSource = null;
    private AudioSource mAudioSource
    {
        get
        {
            if(mCachedAudioSource == null)
            {
                mCachedAudioSource = GetComponent<AudioSource>();
                mCachedAudioSource.playOnAwake = false;
            }
            return mCachedAudioSource;
        }
    }

    private AudioDataObject mAudioData = null;

    private bool mIsInit = false;

    public void Init()
    {
        if(mIsInit == false)
        {
            mAudioData = Resources.Load<AudioDataObject>("AudioDataObject");
            mAudioData.Init();
            mIsInit = true;
        }
    }

    public void PlaySE(string key)
    {
        AudioClip tClip = null;
        if (mAudioData.TryGetSE(key, out tClip))
        {
            mAudioSource.PlayOneShot(tClip);
        }
    }

    public void PlayBGM(string key)
    {
        AudioClip tClip = null;
        if (mAudioData.TryGetBGM(key,out tClip))
        {
            mAudioSource.clip = tClip;
            mAudioSource.DOFade(1, 0);
            mAudioSource.Play();
        }
    }

    public void StopBGM()
    {
        mAudioSource.DOFade(0, 0.3f)
            .OnComplete(()=>mAudioSource.Stop());
    }
}
