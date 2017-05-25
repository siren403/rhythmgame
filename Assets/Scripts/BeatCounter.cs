using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 비트의 흐름을 제어하는 기본 클래스
/// </summary>
[RequireComponent(typeof(AudioSource))]
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


    public float BPM = 129;
    private float BPS = 0.0f;

    private float mCurrentTime = 0;

    private bool mIsPlaying = false;
    public bool mIsMusicPlay = false;

    private AudioSource mAudioSource = null;
    public AudioClip mInputSound = null;

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

            tTime -= BPS * 2.25f;

            if(tTime < 0)
            {
                tTime = 0;
            }

            return tTime;
        }
    }

    private float mBeatProgress
    {
        get
        {
            return CurrentTrackTime / BPS;
        }
    }

    private float mPrevBeatCount = 0.0f;

    public Transform[] mCubeArray = null;

    private byte[] mBeatData = new byte[] 
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };
    private int mBeatIndex
    {
        get
        {
            if(mBeatProgress > 1)
            {
                int index = Mathf.RoundToInt(mBeatProgress) - 1;
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
            float tBeatCount = mBeatProgress;
            int tCount = Mathf.RoundToInt(tBeatCount);

            InputCode tInputCode = InputCode.None;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(mBeatIndex);
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

            //if ((int)mPrevBeatCount != tCount)
            //{
            //    mPrevBeatCount = tCount;

            //    int tCubeIndex = (int)Mathf.Repeat(mBeatIndex, 8);
            //    if (tCubeIndex < mCubeArray.Length)
            //    {
            //        mCubeArray[tCubeIndex].localScale = Vector3.one * 0.6f;
            //        mCubeArray[tCubeIndex].DOScale(Vector3.one * 0.5f, 0.0f).SetDelay(BPS);
            //    }

            //}

            if (mIsMusicPlay == false)
            {
                mCurrentTime += Time.deltaTime;
            }
        }
    }
    


    /// <summary>
    /// 입력에 따른 판정을 실시한다
    /// </summary>
    /// <param name="tInputCode">입력 타입</param>
    /// <param name="tCount">현재 비트</param>
    /// <param name="tCurrentTiming">입력 시 비트 타이밍</param>
    /// <returns>판정 결과</returns>
    private InputResult CheckInputTiming(InputCode tInputCode, int tCount, float tCurrentTiming)
    {
        InputResult tResult = InputResult.Fail;

        if (mBeatData[mBeatIndex] != 0)
        {
            if ((tInputCode == InputCode.SpaceDown && mBeatData[mBeatIndex] == 1) ||
                (tInputCode == InputCode.SpaceUp && mBeatData[mBeatIndex] == 2))
            {

                if (tCount - mPerfectRatio > tCurrentTiming)
                {
                    Debug.Log("Too fast");
                    tResult = InputResult.Fast;
                }
                else if (tCount + mPerfectRatio < tCurrentTiming)
                {
                    Debug.Log("Too Late");
                    tResult = InputResult.Late;
                }
                else
                {
                    Debug.Log("Perfect!!");
                    tResult = InputResult.Perfect;
                }

                Debug.Log(tResult.ToString() + " / " + string.Format("{0} [{1}] {2}", tCount - mPerfectRatio, tCurrentTiming, tCount + mPerfectRatio));
            }

        }

        return tResult;
    }

    private void ChangeCubeColor(Color tColor)
    {
        int tCubeIndex = (int)Mathf.Repeat(mBeatIndex, 8);
        var mat = mCubeArray[tCubeIndex].GetComponent<Renderer>().material;
        mat.color = tColor;
        mat.DOColor(Color.white, 0.0f).SetDelay(BPS);
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        mIsPlaying = false;
        mCurrentTime = 0;
    }

    /**
    * @brief
    * 두 파라메터를 입력받아 더한 값을 리턴1
    * @warning 주의 사항
    * @param aa 더할 값 11
    * @param bb 더할 값 21
    * @return a + b를 리턴한다.1
    * @bug 존재하는 버그에 대한 설명
    * @todo. 해야할 목록에 대하여.@
    **/
    public void TestMethod()
    {

    }

    /// <summary>
    /// 테스트 2
    /// </summary>
    /// <Warning>주의사항</Warning>
    /// <param name="none">파라매터</param>
    /// <returns>리턴</returns>
    /// @code{.cs}
    /// public void Sample()
    /// {
    ///     Debug.Log("sads");
    /// }
    /// 
    /// int tA = 0;
    /// @endcode
    /// <Bug>버그</Bug>
    /// <Todo>해야할 일</Todo>
    public void TestMethod2()
    {

    }
}
