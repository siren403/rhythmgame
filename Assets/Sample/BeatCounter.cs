using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeatCounter : MonoBehaviour
{
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


    public int BPM = 129;
    private float BPS = 0.0f;

    private float mCurrentTime = 0;

    private bool mIsPlaying = false;
    public bool mIsMusicPlay = false;

    private AudioSource mAudioSource = null;

    private float CurrentTrackTime
    {
        get
        {
            float tTime = 0;
            if (mAudioSource.isPlaying)
            {
                tTime = mAudioSource.time;
            }
            else
            {
                tTime = mCurrentTime;
            }

            tTime -= BPS * 2;

            if(tTime < 0)
            {
                tTime = 0;
            }

            return tTime;
        }
    }

    private float mBeatCount
    {
        get
        {
            return CurrentTrackTime / BPS;
        }
    }

    private float mPrevBeatCount = 0.0f;

    public Transform[] mCubeArray = null;

    private byte[] mBeatData = new byte[] { 0, 1, 0, 1, 0, 1, 0, 1 };
    private int mBeatIndex
    {
        get
        {
            if(mBeatCount > 1)
            {
                int index = Mathf.RoundToInt(mBeatCount) - 1;
                return index - ((int)(index / mBeatData.Length)) * mBeatData.Length;
            }
            return 0;
        }
    }

    private float mPerfectRatio = 0.2f;



    // Use this for initialization
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        BPS = 60.0f / BPM;
    }

    // Update is called once per frame
    void Update()
    {
        if(mIsPlaying == false && Input.GetKeyUp(KeyCode.Space))
        {
            mIsPlaying = true;
            if (mIsMusicPlay)
            {
                mAudioSource.Play();
            }
        }   
        if (mIsPlaying)
        {
            if(mIsMusicPlay && mAudioSource.isPlaying == false)
            {
                Stop();
            }
            float tBeatCount = mBeatCount;
            int tCount = Mathf.RoundToInt(tBeatCount);

            InputCode tInputCode = InputCode.None;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceDown;
            }
            else if(Input.GetKeyUp(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceUp;
            }
            InputResult tResult = CheckInputTiming(tInputCode, tCount, tBeatCount);
            switch (tResult)
            {
                case InputResult.Fast:
                    ChangeCubeColor(Color.red);
                    break;
                case InputResult.Perfect:
                    ChangeCubeColor(Color.green);
                    break;
                case InputResult.Late:
                    ChangeCubeColor(Color.blue);
                    break;
                case InputResult.Fail:
                    //ChangeCubeColor(Color.black);
                    break;
            }
            

            if ((int)mPrevBeatCount != tCount)
            {
                mPrevBeatCount = tCount;

                mCubeArray[mBeatIndex].localScale = Vector3.one * 0.6f;
                mCubeArray[mBeatIndex].DOScale(Vector3.one * 0.5f, 0.0f).SetDelay(BPS);

            }

            if (mIsMusicPlay == false)
            {
                mCurrentTime += Time.deltaTime;
            }
        }
    }
    private InputResult CheckInputTiming(InputCode tInputCode, int tCount, float tCurrentTiming)
    {
        if ((tInputCode == InputCode.SpaceDown && mBeatData[mBeatIndex] == 1) ||
            (tInputCode == InputCode.SpaceUp && mBeatData[mBeatIndex] == 2))
        {
            Debug.Log(string.Format("{0} [{1}] {2}", tCount - mPerfectRatio, tCurrentTiming, tCount + mPerfectRatio));

            if (tCount - mPerfectRatio > tCurrentTiming)
            {
                Debug.Log("Too fast");
                return InputResult.Fast;
            }
            else if (tCount + mPerfectRatio < tCurrentTiming)
            {
                Debug.Log("Too Late");
                return InputResult.Late;
            }
            else
            {
                Debug.Log("Perfect!!");
                return InputResult.Perfect;
            }
        }
        return InputResult.Fail;
    }

    private void ChangeCubeColor(Color tColor)
    {
        var mat = mCubeArray[mBeatIndex].GetComponent<Renderer>().material;
        mat.color = tColor;
        mat.DOColor(Color.white, 0.0f).SetDelay(BPS);
    }

    private void Stop()
    {
        mIsPlaying = false;
        mCurrentTime = 0;
    }
}
