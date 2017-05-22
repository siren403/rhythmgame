using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(AudioSource))]
public class MusicSample : MonoBehaviour
{
    [SerializeField]
    private int mBPM = 100;
    [SerializeField]
    private float mBPS = 0.0f;
    private float mBeatCountFromStart = 0.0f;
    private float mPrevBeatCountFromStart = 0.0f;
    private bool mIsPlayPrevFrame = false;
    private bool mMusicFinished = false;


    private AudioSource mAudioSource = null;

    public float Length
    {
        get
        {
            return mAudioSource.clip.length / mBPS;
        }
    }

    public GameObject mCube = null;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    private void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mBPS = 60.0f / mBPM;
        mMusicFinished = false;
    }
    private void Update()
    {
        
        if(mAudioSource.isPlaying)
        {
            mPrevBeatCountFromStart = mBeatCountFromStart;
            mBeatCountFromStart = mAudioSource.time / mBPS;
            mIsPlayPrevFrame = true;

            if ((int)mPrevBeatCountFromStart != (int)mBeatCountFromStart)
            {
                mCube.transform.DOScale(1.2f, mBPS / 2)
                    .OnComplete(() => 
                    {
                        mCube.transform.DOScale(1.0f, mBPS / 2);
                    });
                Debug.Log(string.Format("{0} / {1}", (int)mBeatCountFromStart,(int)this.Length));
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mAudioSource.Play();
            }
            if (mIsPlayPrevFrame
                && !(0<mAudioSource.timeSamples && mAudioSource.timeSamples < mAudioSource.clip.samples))
            {
                mMusicFinished = true;
            }
            mIsPlayPrevFrame = false;
        }
    }
}
