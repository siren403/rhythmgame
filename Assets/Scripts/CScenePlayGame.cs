using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;
using BitStrap;
using DG.Tweening;

public class CScenePlayGame : SceneBase
{
    protected override IPresenter[] Children
    {
        get
        {
            return new IPresenter[] { InstSequencePlayer, InstHoleInOne };
        }
    }

    public CSequencePlayer InstSequencePlayer = null;
    public CHoleInOne InstHoleInOne = null;
    public CStageData CurrentStageData = null;
    public float SeekBeat = 0;

    public SpriteRenderer InstFadeSprite = null;

    protected override void BeforeInitialize()
    {
        InstSequencePlayer.SetStageData(CurrentStageData);
        InstSequencePlayer.SetReceiver(InstHoleInOne);
        InstSequencePlayer.OnComplete = () => 
        {
            InstFadeSprite.DOFade(1, 0.3f);
        };
        InstFadeSprite.color = new Color(0, 0, 0, 1);
    }
    protected override void Initialize()
    {
        InstFadeSprite.DOFade(0, 0.3f)
            .SetDelay(0.5f)
            .OnComplete(() => 
            {
                InstSequencePlayer.Play();
            });
    }

    [Button]
    public void OnSeek()
    {
        InstSequencePlayer.Seek(SeekBeat);
    }
}
