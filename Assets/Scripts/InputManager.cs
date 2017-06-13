using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum InputCode
{
    None,
    SingleDown, SingleUp,
}
public static class InputManager
{
    

    private static Dictionary<InputCode, Func<bool>> mInputFuncs = null;

    static InputManager()
    {
        mInputFuncs = new Dictionary<InputCode, Func<bool>>();

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            mInputFuncs.Add(InputCode.SingleDown, () =>
            {
                return Input.GetKeyDown(KeyCode.Space);
            });
            mInputFuncs.Add(InputCode.SingleUp, () =>
            {
                return Input.GetKeyUp(KeyCode.Space);
            });
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            mInputFuncs.Add(InputCode.SingleDown, () =>
            {
                return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began; 
            });
            mInputFuncs.Add(InputCode.SingleUp, () =>
            {
                return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
            });
        }

    }

    public static bool GetKey(InputCode tCode)
    {
        if(mInputFuncs.ContainsKey(tCode))
        {
            return mInputFuncs[tCode].Invoke();
        }
        else
        {
            return false;
        }
    }


}

