using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;

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

    protected override void BeforeInitialize()
    {
        InstSequencePlayer.SetReceiver(InstHoleInOne);
    }
    protected override void Initialize()
    {
    }
}
