using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CHoleInOne : PresenterBase
{
    protected override IPresenter[] Children
    {
        get
        {
            return EmptyChildren;
        }
    }


    protected override void BeforeInitialize()
    {
        Debug.Log("Hole In Before");
    }

    protected override void Initialize()
    {
        Debug.Log("Hole In Init");
    }
}
