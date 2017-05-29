using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System;

/// <summary>
/// 비트의 흐름을 제어하는 기본 클래스
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CSequencePlayer : PresenterBase
{
    private class CEmptySequenceReceiver : ISequenceReceiver
    {
        public void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData) { }
        public void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult) { }
    }
    private static ISequenceReceiver EmptyReceiver = new CEmptySequenceReceiver();

    public float BPM = 129;
    private float mBPS = 0.0f;
    public float BPS
    {
        get
        {
            return mBPS;
        }
    }

    public float StartBeatOffset = 0.0f;

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

            tTime += mBPS * StartBeatOffset;

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
            return CurrentTrackTime / mBPS;
        }
    }

    protected override IPresenter[] Children
    {
        get
        {
            return EmptyChildren;
        }
    }

    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;

    public CSequenceData CurrentSequenceData = null;
    public List<CSequenceData> InsertSequenceList = new List<CSequenceData>();
    public List<CSequenceData> SequenceList = new List<CSequenceData>();
   
    private float mPerfectRatio = 0.1f;
    private int mSequenceIndex = 0;
    private int mAlreadySequenceIndex = 0;
    private int mSuccessSequenceIndex = 0;

    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    private GameObject CurrentBall = null;

    private ISequenceReceiver mCurrentReceiver = EmptyReceiver;

    protected override void BeforeInitialize()
    {
        mAudioSource = GetComponent<AudioSource>();
        mBPS = 60.0f / (float)BPM;
        Debug.Log("BPS : " + mBPS);

        Queue<CSequenceData> tInsertSeqData = new Queue<CSequenceData>(InsertSequenceList);
        for (int tBeat = 0; tBeat < mAudioSource.clip.length / mBPS;)
        {
            CSequenceData tSeqData = null;
            if (tInsertSeqData.Count > 0)
            {
                if (tBeat < tInsertSeqData.Peek().beat)
                {
                    tSeqData = new CSequenceData(tBeat, tBeat * mBPS);
                    tBeat++;
                }
                else if(tBeat == tInsertSeqData.Peek().beat)
                {
                    tSeqData = tInsertSeqData.Dequeue();
                    tSeqData.time = tSeqData.beat * mBPS;
                    tBeat++;
                }
                else
                {
                    tSeqData = tInsertSeqData.Dequeue();
                    tSeqData.time = tSeqData.beat * mBPS;
                }
            }
            else
            {
                tSeqData = new CSequenceData(tBeat, tBeat * mBPS);
                tBeat++;
            }
            SequenceList.Add(tSeqData);
        }
        Debug.Log("ActionInfo Count : " + SequenceList.Count);
    }

    protected override void Initialize()
    {
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

            if (mSequenceIndex != mAlreadySequenceIndex && SequenceList[mSequenceIndex].time - CurrentTrackTime <= 0.0001f)
            {
                mCurrentReceiver.OnEveryBeat(this, SequenceList[mSequenceIndex]);
                mAlreadySequenceIndex = mSequenceIndex;
            }

            InputCode tInputCode = InputCode.None;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceDown;

            }
            else if(Input.GetKeyUp(KeyCode.Space))
            {
                tInputCode = InputCode.SpaceUp;
            }

            InputResult tResult = CheckInputTiming(tInputCode);
            mCurrentReceiver.OnInputResult(this, tResult);
            if(tResult != InputResult.None)
            {
                mSuccessSequenceIndex = mSequenceIndex;
            }
            

            if (CurrentTrackTime - SequenceList[mSequenceIndex].time > SequenceList[mSequenceIndex + 1].time - CurrentTrackTime)
            {
                if (mSuccessSequenceIndex < mSequenceIndex &&
                    SequenceList[mSequenceIndex].input != InputCode.None &&
                    tInputCode == InputCode.None)//입력데이터가 None이 아닌데 입력이 없는 경우(미 입력)
                {
                    Debug.Log("Fail");
                }
                mSequenceIndex++;
                CurrentSequenceData = SequenceList[mSequenceIndex];
            }

            if (mIsMusicPlay == false)
            {
                mCurrentTime += Time.deltaTime;
            }
        }
    }

    public void SetReceiver(ISequenceReceiver tReceiver)
    {
        if (tReceiver != null)
        {
            mCurrentReceiver = tReceiver;
        }
        else
        {
            mCurrentReceiver = EmptyReceiver;
        }
    }

    /// <summary>
    /// 입력에 따른 판정을 실시한다
    /// </summary>
    /// <param name="tInputCode">입력 타입</param>
    /// <param name="tRoundProgress">현재 비트</param>
    /// <param name="tBeatProgress">입력 시 비트 타이밍</param>
    /// <returns>판정 결과</returns>
    private InputResult CheckInputTiming(InputCode tInputCode)
    {

        int tRoundProgress = Mathf.RoundToInt(mBeatProgress);

        InputResult tResult = InputResult.None;

        CSequenceData tSeqData = SequenceList[mSequenceIndex];
        

        if (tSeqData.input != InputCode.None && tSeqData.input == tInputCode)//입력데이터가 None아니고, 일치한다면 (정확한 입력)
        {
            if (tRoundProgress - mPerfectRatio > mBeatProgress)
            {
                Debug.Log("Too fast");
                tResult = InputResult.Fast;
            }
            else if (tRoundProgress + mPerfectRatio < mBeatProgress)
            {
                Debug.Log("Too Late");
                tResult = InputResult.Late;
            }
            else
            {
                Debug.Log("Perfect!!");
                tResult = InputResult.Perfect;

            }
        }
      

        return tResult;
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
