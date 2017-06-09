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
        public void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData) { }
        public void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult) { }
    }
    private static ISequenceReceiver EmptyReceiver = new CEmptySequenceReceiver();
    public class EvaluationData
    {
        public int mTotalCount { private set; get; } 
        public int mFastCount { private set; get; }
        public int mPerfectCount { private set; get; }
        public int mLateCount { private set; get; }

        public float EvaluateValue
        {
            get
            {
                if(mTotalCount == 0)
                {
                    return 0;
                }
                return (float)mPerfectCount / mTotalCount;
            }
        }

        public void AddTotalCount()
        {
            mTotalCount++;
        }

        public void OnCount(InputResult tResult)
        {
            switch (tResult)
            {
                case InputResult.Fast:
                    mFastCount++;
                    break;
                case InputResult.Perfect:
                    mPerfectCount++;
                    break;
                case InputResult.Late:
                    mLateCount++;
                    break;
            }
        }

    }


    public float BPS
    {
        get
        {
            if(mCurrentStageData == null)
            {
                return 0f;
            }
            return mCurrentStageData.BPS;
        }
    }
    public float StartBeatOffset
    {
        get
        {
            if(mCurrentStageData == null)
            {
                return 0f;
            }
            return mCurrentStageData.StartBeatOffset;
        }
    }
    public float PerfectRange
    {
        get
        {
            if(mCurrentStageData == null)
            {
                return 0;
            }
            return mCurrentStageData.PerfectRange;
        }
    }

    private float mCurrentTime = 0;

    private bool mIsPlaying = false;
    private bool mIsMusicPlay = true;

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

            tTime += BPS * StartBeatOffset;

            if(tTime < 0)
            {
                tTime = 0;
            }

            return tTime;
        }
    }

    public float BeatProgress
    {
        get
        {
            return CurrentTrackTime / BPS;
        }
    }

    protected override IPresenter[] Children
    {
        get
        {
            return EmptyChildren;
        }
    }

    public CSequenceData CurrentSequenceData = null;
    public List<CSequenceData> SequenceList = new List<CSequenceData>();
   
    private int mSequenceIndex = 0;
    private int mAlreadySequenceIndex = 0;
    private int mSuccessSequenceIndex = 0;

    private ISequenceReceiver mCurrentReceiver = EmptyReceiver;
    private CStageData mCurrentStageData = null;

    private EvaluationData mCurrentEvaluation = new EvaluationData();

    private Action mOnComplete = null;
    public Action OnComplete
    {
        set
        {
            mOnComplete = value;
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
    public void SetStageData(CStageData tData)
    {
        mCurrentStageData = tData;
    }

    protected override void BeforeInitialize()
    {
        mIsPlaying = false;
        mAudioSource = GetComponent<AudioSource>();
        mAudioSource.clip = mCurrentStageData.Music;
        Debug.Log("BPS : " + BPS);
        Queue<CSequenceData> tInsertSeqData = new Queue<CSequenceData>(mCurrentStageData.SequenceList);

        for (int tBeat = 0; tBeat < mAudioSource.clip.length / BPS;)
        {
            CSequenceData tSeqData = null;
            if (tInsertSeqData.Count > 0)
            {
                if (tBeat < tInsertSeqData.Peek().Beat)
                {
                    tSeqData = new CSequenceData(tBeat, tBeat * BPS);
                    tBeat++;
                }
                else if(tBeat == tInsertSeqData.Peek().Beat)
                {
                    tSeqData = tInsertSeqData.Dequeue();
                    tSeqData.SetTime(tSeqData.Beat * BPS);
                    tBeat++;
                }
                else
                {
                    tSeqData = tInsertSeqData.Dequeue();
                    tSeqData.SetTime(tSeqData.Beat * BPS);
                }
            }
            else
            {
                tSeqData = new CSequenceData(tBeat, tBeat * BPS);
                tBeat++;
            }

            if(tSeqData.Input != InputCode.None)
            {
                mCurrentEvaluation.AddTotalCount();
            }
            SequenceList.Add(tSeqData);
        }
        Debug.Log("Sequence Count : " + SequenceList.Count+"\nInput Count : "+mCurrentEvaluation.mTotalCount);
    }

    protected override void Initialize()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if(mIsPlaying == false && Input.GetKeyUp(KeyCode.Space))
        {
            Play();
        }   
        if (mIsPlaying)
        {
            if(mIsMusicPlay && mAudioSource.isPlaying == false)
            {
                mIsPlaying = false;
                if (mOnComplete != null)
                {
                    mOnComplete.Invoke();
                }

                Stop();
            }

            if (mSequenceIndex != mAlreadySequenceIndex && SequenceList[mSequenceIndex].Time - CurrentTrackTime <= 0.0001f)
            {
                mCurrentReceiver.OnEveryBeat(this, SequenceList[mSequenceIndex]);

                if(SequenceList[mSequenceIndex].Beat == Mathf.Round(BeatProgress))
                {
                    mCurrentReceiver.OnBaseBeat(this, SequenceList[mSequenceIndex]);
                }
                //Debug.Log(SequenceList[mSequenceIndex].Beat);
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

            if(Input.GetKeyDown(KeyCode.C))//Debug
            {
                Debug.Log(BeatProgress);
            }

            InputResult tResult = CheckInputTiming(tInputCode);
            mCurrentReceiver.OnInputResult(this, tResult);
            if(tResult != InputResult.None)
            {
                mSuccessSequenceIndex = mSequenceIndex;
            }
            

            if (CurrentTrackTime - SequenceList[mSequenceIndex].Time > SequenceList[mSequenceIndex + 1].Time - CurrentTrackTime)
            {
                if (mSuccessSequenceIndex < mSequenceIndex &&
                    SequenceList[mSequenceIndex].Input != InputCode.None &&
                    tInputCode == InputCode.None)//입력데이터가 None이 아닌데 입력이 없는 경우(미 입력)
                {
                    mCurrentReceiver.OnInputResult(this, InputResult.Fail);

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


    /// <summary>
    /// 입력에 따른 판정을 실시한다
    /// </summary>
    /// <param name="tInputCode">입력 타입</param>
    /// <param name="tRoundProgress">현재 비트</param>
    /// <param name="tBeatProgress">입력 시 비트 타이밍</param>
    /// <returns>판정 결과</returns>
    private InputResult CheckInputTiming(InputCode tInputCode)
    {

        int tRoundProgress = Mathf.RoundToInt(BeatProgress);

        InputResult tResult = InputResult.None;

        CSequenceData tSeqData = SequenceList[mSequenceIndex];

        //float tSync = 0.05f;
        if (tSeqData.Input != InputCode.None && tSeqData.Input == tInputCode)//입력데이터가 None아니고, 일치한다면 (정확한 입력)
        {
            if (tRoundProgress - mCurrentStageData.PerfectRange /*+ tSync*/ > BeatProgress)
            {
                //Debug.Log("Too fast");
                tResult = InputResult.Fast;
            }
            else if (tRoundProgress + mCurrentStageData.PerfectRange /*+ tSync*/ < BeatProgress)
            {
                //Debug.Log("Too Late");
                tResult = InputResult.Late;
            }
            else
            {
                //Debug.Log("Perfect!!");
                tResult = InputResult.Perfect;
            }
            //Debug.Log(string.Format("{0} [{1}] {2}",
            //    tRoundProgress - mCurrentStageData.PerfectRange,
            //    BeatProgress,
            //    tRoundProgress + mCurrentStageData.PerfectRange));
        }

        mCurrentEvaluation.OnCount(tResult);
        return tResult;
    }

    public void Play()
    {
        mIsPlaying = true;
        if (mIsMusicPlay)
        {
            mAudioSource.Play();
        }
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        mIsPlaying = false;
        mAudioSource.Stop();
        mCurrentTime = 0;

    }

    public void Seek(float tBeat)
    {
        for (int i = 0; i < SequenceList.Count; i++)
        {
            if(SequenceList[i].Beat == tBeat)
            {
                mSequenceIndex = i;
                break;
            }
        }
        mAudioSource.time = SequenceList[mSequenceIndex].Beat * BPS;
    }



    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width*0.05f ,Screen.height * 0.9f,Screen.width,Screen.height * 0.1f),
            string.Format("[{0}] Fast : {1} Perfect : {2} Late : {3}",
            mCurrentEvaluation.EvaluateValue,
            mCurrentEvaluation.mFastCount, mCurrentEvaluation.mPerfectCount, mCurrentEvaluation.mLateCount));
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
