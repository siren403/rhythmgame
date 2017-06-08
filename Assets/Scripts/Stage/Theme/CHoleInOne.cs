using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;
using System;
using DG.Tweening;


public class CHoleInOne : CThemeBase
{
    private const int BEAT_LEVEL_1 = 39;
    private const int BEAT_LEVEL_2 = 76;
    private const string KEY_TRIGGER_BEAT = "TrigBeat";
    private const string KEY_TRIGGER_SHORT = "TrigShort";
    private const string KEY_TRIGGER_SHOTREADY = "TrigShotReady";
    private const string KEY_TRIGGER_SHOT = "TrigShot";
    private const string KEY_TRIGGER_SHOTFAIL = "TrigShotFail";
    private const string KEY_TRIGGER_MANDRILLBALL = "TrigBall";
    
    protected override IPresenter[] Children
    {
        get
        {
            return EmptyChildren;
        }
    }

    public enum ActionType
    {
        None,
        SEMonkey, SEMandril, SEMonkeyShort,
        ThrowBall,
    }

    public AudioClip SEMonkey = null;
    public AudioClip SEMandril = null;
    public AudioClip SEMonkeyShort = null;

    public Animator AnimMonkey = null;
    public Animator AnimMandrill = null;
    public Animator AnimGolfer = null;

    private Queue<GameObject> mBallPool = new Queue<GameObject>();
    private Queue<GameObject> mActiveBallPool = new Queue<GameObject>();

    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;


    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    public Transform BallFailPoint = null;


    private Dictionary<string, Action<CSequencePlayer, CSequenceData>> mActionList 
        = new Dictionary<string, Action<CSequencePlayer, CSequenceData>>();


    protected override void BeforeInitialize()
    {
        mActionList[CHoleInOneActionCode.SEMONKEY] = PlaySEMonkey;
        mActionList[CHoleInOneActionCode.THROWBALL] = ThrowBall;

        mActionList[CHoleInOneActionCode.MONKEYSHORT] = MonkeyShort;
        mActionList[CHoleInOneActionCode.SEMONKEYSHORT] = PlaySEMonkeyShort;

        mActionList[CHoleInOneActionCode.SEMANDRIL] = (tSeqPlayer, tSeqData) => mAudioSource.PlayOneShot(SEMandril);
        mActionList[CHoleInOneActionCode.MANDRILLBALL] = (tSeqPlayer, tSeqData) => AnimMandrill.SetTrigger(KEY_TRIGGER_MANDRILLBALL);

        mActionList[CHoleInOneActionCode.THROWSTRAIGHTBALL] = ThrowStraightBall;


        mActionList[CHoleInOneActionCode.GOLFERREADY] = (tSeqPlayer, tSeqData) => AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTREADY);
    }

    protected override void Initialize()
    {

    }
    public override void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.SetTrigger(KEY_TRIGGER_BEAT);
        switch ((int)tSeqPlayer.BeatProgress)
        {
            case BEAT_LEVEL_1:
                AnimMandrill.SetInteger("IntBeatLevel", 1);
                break;
            case BEAT_LEVEL_2:
                AnimMandrill.SetInteger("IntBeatLevel", 2);
                break;
        }

        AnimMandrill.SetTrigger(KEY_TRIGGER_BEAT);
        AnimGolfer.SetTrigger(KEY_TRIGGER_BEAT);
        foreach(var code in tData.ActionCode)
        {
            if (mActionList.ContainsKey(code))
            {
                mActionList[code].Invoke(tSeqPlayer, tData);
            }
        }
    }
    public override void OnBaseBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        BeatPanel.material.DOColor(Color.white, tSeqPlayer.BPS * 0.5f).From();

    }
    public override void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult)
    {
        switch (tResult)
        {
            case InputResult.Fast:
                AnimGolfer.SetTrigger(KEY_TRIGGER_SHOT);
                CheckTimingPanel.material.DOColor(Color.red, tSeqPlayer.BPS * 0.8f).From();
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    Destroy(tBall);
                }
                break;
            case InputResult.Perfect:
                CheckTimingPanel.material.DOColor(Color.green, tSeqPlayer.BPS * 0.8f).From();
                AnimGolfer.SetTrigger(KEY_TRIGGER_SHOT);
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    tBall.transform.DOScale(0, tSeqPlayer.BPS * 2.0f).SetEase(Ease.OutQuad);
                    tBall.transform.DOJump(BallPerpectPoint.position, 4, 1, tSeqPlayer.BPS * 2.0f)
                       .SetEase(Ease.OutQuad)
                       .OnComplete(() =>
                       {
                           ReturnPoolBall(tBall);
                       });
                }
                break;
            case InputResult.Late:
                AnimGolfer.SetTrigger(KEY_TRIGGER_SHOT);
                CheckTimingPanel.material.DOColor(Color.blue, tSeqPlayer.BPS * 0.8f).From();
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    Destroy(tBall);
                }
                break;
            case InputResult.Fail:
                AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTFAIL);
                if (mActiveBallPool.Count > 0)
                {
                    GameObject tBall = mActiveBallPool.Dequeue();
                    var tSeq = DOTween.Sequence();
                    tSeq.Append(tBall.transform.DOJump(BallFailPoint.position, 1.5f, 1, tSeqPlayer.BPS)
                        .SetEase(Ease.OutQuad));
                    tSeq.Append(tBall.transform.DOMove(new Vector2(0.5f,-1.0f), tSeqPlayer.BPS)
                        .SetEase(Ease.OutQuad)
                        .SetRelative());
                    tSeq.AppendCallback(()=> 
                    {
                        ReturnPoolBall(tBall);
                    });
                    tSeq.Play();
                }
                break;
        }
    }



    private void PlaySEMonkey(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkey);
        AnimMonkey.SetTrigger(KEY_TRIGGER_SHORT);
    }
    private void ThrowBall(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.SetTrigger(KEY_TRIGGER_SHORT);
        AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTREADY);
        GameObject tBall = RentBall();
        tBall.transform.position = BallStartPoint.position;
        tBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS )
            .SetEase(Ease.Linear);
    }

    private void ThrowStraightBall(CSequencePlayer tSeqPlayer,CSequenceData tData)
    {
        GameObject tBall = RentBall();
        tBall.transform.position = BallStartPoint.position;
        tBall.transform.DOMove(BallEndPoint.position, tSeqPlayer.BPS * 0.1f)
           .SetEase(Ease.Linear);
    }
    private void PlaySEMonkeyShort(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkeyShort);
    }
    private void MonkeyShort(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.SetTrigger(KEY_TRIGGER_SHORT);
        //AnimGolfer.SetTrigger(KEY_TRIGGER_SHOTREADY);
        //GameObject tBall = RentBall();
        //tBall.transform.position = BallStartPoint.position;
        //tBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS)
        //    .SetEase(Ease.Linear);
    }

    private GameObject RentBall()
    {
        GameObject tBall = null;

        if (mBallPool.Count > 0)
        {
            tBall = mBallPool.Dequeue();
        }
        else
        {
            tBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
            tBall.transform.SetParent(this.transform);
        }

        tBall.gameObject.SetActive(true);
        mActiveBallPool.Enqueue(tBall);
        return tBall;
    }
    private void ReturnPoolBall(GameObject tGameObject)
    {
        tGameObject.SetActive(false);
        tGameObject.transform.localScale = Vector3.one;
        mBallPool.Enqueue(tGameObject);
    }
}
