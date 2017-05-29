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
    [System.Serializable]
    public class SequenceData
    {
        public float beat;
        public float time;
        public InputCode input;
        public int soundCode;
        public int actionCode;

        public SequenceData(float tBeat,float tTime)
        {
            beat = tBeat;
            time = tTime;
            actionCode = -1;
            soundCode = -1;
            input = InputCode.None;
        }
    }

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

    public float StartBeatOffset = 0.0f;

    private float mCurrentTime = 0;

    private bool mIsPlaying = false;
    public bool mIsMusicPlay = false;

    private AudioSource mAudioSource = null;
    public List<AudioClip> SEList = new List<AudioClip>();

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

    private float mBeatProgress
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

    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;

    public List<SequenceData> InsertSequenceList = new List<SequenceData>();
    public List<SequenceData> SequenceList = new List<SequenceData>();
   
    private float mPerfectRatio = 0.1f;
    private int mPrevRoundProgress = 0;
    private int mSequenceIndex = 0;
    private int mAlreadySequenceIndex = 0;

    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    private GameObject CurrentBall = null;

    protected override void BeforeInitialize()
    {
        Debug.Log("Seq Before");

        mAudioSource = GetComponent<AudioSource>();
        BPS = 60.0f / (float)BPM;
        Debug.Log("BPS : " + BPS);

        int tInsertActionIndex = 0;
        for (int i = 0; i < mAudioSource.clip.length / BPS; i++)
        {
            bool tIsInsert = false;
            float tBeat = i;
            float tTime = i * BPS;

            for (; tInsertActionIndex < InsertSequenceList.Count;)
            {
                if (tBeat == InsertSequenceList[tInsertActionIndex].beat)
                {
                    var tData = new SequenceData(tBeat, tTime);
                    tData.input = InsertSequenceList[tInsertActionIndex].input;
                    tData.soundCode = InsertSequenceList[tInsertActionIndex].soundCode;
                    tData.actionCode = InsertSequenceList[tInsertActionIndex].actionCode;
                    tIsInsert = true;
                    SequenceList.Add(tData);
                    tInsertActionIndex++;
                    break;
                }
                else
                {
                    if (InsertSequenceList[tInsertActionIndex].beat < tBeat + 1)
                    {
                        var tData = new SequenceData(tBeat, tTime);
                        tData.input = InsertSequenceList[tInsertActionIndex].input;
                        tData.soundCode = InsertSequenceList[tInsertActionIndex].soundCode;
                        tData.actionCode = InsertSequenceList[tInsertActionIndex].actionCode;
                        tIsInsert = true;
                        SequenceList.Add(tData);
                        tInsertActionIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (tIsInsert == false)
            {
                SequenceList.Add(new SequenceData(tBeat, tTime));
            }

        }
        Debug.Log("ActionInfo Count : " + SequenceList.Count);
    }

    protected override void Initialize()
    {
        Debug.Log("Seq Init");
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

            if(CurrentTrackTime - SequenceList[mSequenceIndex].time > SequenceList[mSequenceIndex + 1].time - CurrentTrackTime)
            {
                Debug.Log(string.Format("Left : {0}, Right : {1} [{2}]",
               CurrentTrackTime - SequenceList[mSequenceIndex].time,
               SequenceList[mSequenceIndex + 1].time - CurrentTrackTime,mSequenceIndex));
                mSequenceIndex++;
            }

            if (mSequenceIndex != mAlreadySequenceIndex && SequenceList[mSequenceIndex].time - CurrentTrackTime <= 0.0001f)
            {
                BeatPanel.material.DOColor(Color.white, BPS * 0.5f).From();

                if (SequenceList[mSequenceIndex].soundCode != -1)
                {
                    mAudioSource.PlayOneShot(SEList[SequenceList[mSequenceIndex].soundCode]);
                }
                if(SequenceList[mSequenceIndex].actionCode != -1)
                {
                    if (CurrentBall == null)
                    {
                        CurrentBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
                    }

                    CurrentBall.transform.position = BallStartPoint.position;
                    CurrentBall.SetActive(true);
                    CurrentBall.transform.DOJump(BallEndPoint.position, 2, 1, BPS)
                        .SetEase(Ease.Linear);
                }
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
            switch (tResult)
            {
                case InputResult.Fast:
                    CheckTimingPanel.material.DOColor(Color.red, BPS * 0.8f).From();
                    break;
                case InputResult.Perfect:
                    CheckTimingPanel.material.DOColor(Color.green, BPS * 0.8f).From();
                    CurrentBall.transform.DOJump(BallPerpectPoint.position, 2, 1, BPS)
                       .OnComplete(() => CurrentBall.SetActive(false));
                    //CurrentBall.SetActive(false);
                    break;
                case InputResult.Late:
                    CheckTimingPanel.material.DOColor(Color.blue, BPS * 0.8f).From();
                    break;
                case InputResult.Fail:
                    //CheckTimingPanel.material.DOColor(Color.black, BPS * 0.5f).From();
                    break;
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

        if(tInputCode == InputCode.None)
        {
            return InputResult.None;
        }

        int tRoundProgress = Mathf.RoundToInt(mBeatProgress);

        InputResult tResult = InputResult.Fail;

        SequenceData tSeqData = SequenceList[mSequenceIndex];
        
        if (tSeqData.input != InputCode.None && tSeqData.input == tInputCode)
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
            Debug.Log(tResult.ToString() + " / " + string.Format("{0} [{1}] {2}",
                tRoundProgress - mPerfectRatio,
                mBeatProgress,
                tRoundProgress + mPerfectRatio));
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
