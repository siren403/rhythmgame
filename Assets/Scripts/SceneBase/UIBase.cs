using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBase : MonoBehaviour
{
    [SerializeField]
    private Image mInstImgFade = null;
    protected Image ImgFade
    {
        get
        {
            return mInstImgFade;
        }
    }


    public Tween DoFade(float value, float duration = 0.3f)
    {
        mInstImgFade.raycastTarget = true;
        return mInstImgFade.DOFade(value, duration)
            .OnComplete(()=> 
            {
                if(value == 0)
                {
                    mInstImgFade.raycastTarget = false;
                }
            });
    }

}
