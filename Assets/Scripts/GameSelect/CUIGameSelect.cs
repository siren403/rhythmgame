using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScenePresenter;
using UniRx;
using DG.Tweening;

public class CUIGameSelect : UIBase
{
    public List<CBtnGame> InstBtnGames = new List<CBtnGame>();

    private void Awake()
    {
        foreach(var btn in InstBtnGames)
        {
            btn.SetUIParent(this);
        }
    }

    public void DoStartGame(string tSceneName)
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
