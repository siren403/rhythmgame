using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using DG.Tweening;

public class CHoleInOne : CThemeBase, ISequenceReceiver
{
    protected override IPresenter[] Children
    {
        get
        {
            return EmptyChildren;
        }
    }
    public List<AudioClip> SEList = new List<AudioClip>();
    public Renderer BeatPanel = null;
    public Renderer CheckTimingPanel = null;
    public GameObject PFBall = null;
    public Transform BallStartPoint = null;
    public Transform BallEndPoint = null;
    public Transform BallPerpectPoint = null;
    private GameObject CurrentBall = null;

    protected override void BeforeInitialize()
    {
    }

    protected override void Initialize()
    {
    }

    public void OnEveryBeat(CSequencePlayer tSeqPlayer, CSequenceData tData)
    {
        BeatPanel.material.DOColor(Color.white, tSeqPlayer.BPS * 0.5f).From();
        if (tData.SoundCode != -1)
        {
            mAudioSource.PlayOneShot(SEList[tData.SoundCode]);
        }
        if (tData.ActionCode != -1)
        {
            if (CurrentBall == null)
            {
                CurrentBall = Instantiate(PFBall, BallStartPoint.position, Quaternion.identity);
            }

            CurrentBall.transform.position = BallStartPoint.position;
            CurrentBall.SetActive(true);
            CurrentBall.transform.DOJump(BallEndPoint.position, 2, 1, tSeqPlayer.BPS)
                .SetEase(Ease.Linear);
        }
    }



    public void OnInputResult(CSequencePlayer tSeqPlayer, InputResult tResult)
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
}
