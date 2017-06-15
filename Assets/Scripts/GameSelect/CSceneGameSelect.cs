using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;

public class CSceneGameSelect : SceneBase
{
    public CUIGameSelect InstUIGameSelect = null;

    protected override void BeforeInitialize()
    {
        InstUIGameSelect.DoFade(1, 0);
    }

    protected override void Initialize()
    {
        InstUIGameSelect.DoFade(0);
        AudioManager.Inst.PlayBGM("game_select");
    }
}
