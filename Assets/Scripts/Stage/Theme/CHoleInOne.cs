using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using DG.Tweening;

public class CHoleInOne : CThemeBase
{
    private const int BEAT_LEVEL_1 = 39;
    private const int BEAT_LEVEL_2 = 76;

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

    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;
    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    private GameObject _CurrentBall = null;
    private GameObject CurrentBall
    {
        get
        {
            if (_CurrentBall == null)
            {
                _CurrentBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
                _CurrentBall.transform.SetParent(this.transform);
            }
            return _CurrentBall;
        }
    }


    private Dictionary<string, Action<CSequencePlayer, CSequenceData>> mActionList 
        = new Dictionary<string, Action<CSequencePlayer, CSequenceData>>();

    protected override void BeforeInitialize()
    {
        mActionList[CHoleInOneActionCode.SEMONKEY] = (tSeqPlayer, tSeqData) => mAudioSource.PlayOneShot(SEMonkey);
        mActionList[CHoleInOneActionCode.SEMANDRIL] = (tSeqPlayer, tSeqData) => mAudioSource.PlayOneShot(SEMandril);

        mActionList[CHoleInOneActionCode.THROWBALL] = ThrowBall;
        mActionList[CHoleInOneActionCode.THROWSTRAIGHTBALL] = ThrowStraightBall;
        mActionList[CHoleInOneActionCode.MONKEYSHORT] = MonkeyShort;

    }

    protected override void Initialize()
    {

    }
    public override void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        AnimMonkey.SetTrigger("TrigBeat");
        switch ((int)tSeqPlayer.BeatProgress)
        {
            case BEAT_LEVEL_1:
                AnimMandrill.SetInteger("IntBeatLevel", 1);
                break;
            case BEAT_LEVEL_2:
                AnimMandrill.SetInteger("IntBeatLevel", 2);
                break;
        }

        AnimMandrill.SetTrigger("TrigBeat");
        if (mActionList.ContainsKey(tData.ActionCode))
        {
            mActionList[tData.ActionCode].Invoke(tSeqPlayer, tData);
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
                CheckTimingPanel.material.DOColor(Color.red, tSeqPlayer.BPS * 0.8f).From();
                break;
            case InputResult.Perfect:
                CheckTimingPanel.material.DOColor(Color.green, tSeqPlayer.BPS * 0.8f).From();
                CurrentBall.transform.DOJump(BallPerpectPoint.position, 2, 1, tSeqPlayer.BPS)
                   .OnComplete(() => CurrentBall.SetActive(false));
                break;
            case InputResult.Late:
                CheckTimingPanel.material.DOColor(Color.blue, tSeqPlayer.BPS * 0.8f).From();
                break;
        }
    }


    private void ThrowBall(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        CurrentBall.transform.position = BallStartPoint.position;
        CurrentBall.SetActive(true);
        CurrentBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS)
            .SetEase(Ease.Linear);
    }
    private void ThrowStraightBall(CSequencePlayer tSeqPlayer,CSequenceData tData)
    {
        CurrentBall.transform.position = BallStartPoint.position;
        CurrentBall.SetActive(true);
        CurrentBall.transform.DOMove(BallEndPoint.position, tSeqPlayer.BPS * 0.1f)
           .SetEase(Ease.Linear);

    }
    private void MonkeyShort(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        mAudioSource.PlayOneShot(SEMonkeyShort);
        DOTween.Kill(CurrentBall.transform);
        CurrentBall.transform.position = BallStartPoint.position;
        CurrentBall.SetActive(true);
        CurrentBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS)
            .SetEase(Ease.Linear);
    }
}
