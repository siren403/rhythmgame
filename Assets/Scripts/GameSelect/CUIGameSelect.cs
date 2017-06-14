using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;
using DG.Tweening;

public class CUIGameSelect : UIBase
{
    public void DoSelectGame(string tSceneName)
    {
        if (string.IsNullOrEmpty(tSceneName) == false)
        {
            DoFade(1).OnComplete(() => 
                {
                    NavigationService.NavigateAsync("ScenePlayGame", tSceneName).Subscribe();
                });
        }
    }

}
