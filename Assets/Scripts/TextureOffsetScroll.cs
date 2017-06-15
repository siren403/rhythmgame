using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TextureOffsetScroll : MonoBehaviour
{
    public float Duration = 10.0f;
    public int LoopCount = -1;
    public Ease EaseType = Ease.Linear;
    public Vector2 Offset = new Vector2(-10.0f, -10.0f);

    private void Awake()
    {
        var tRenderer = GetComponent<Renderer>();
        string tPropName = "_MainTex";
        DOTween.To(() => tRenderer.material.GetTextureOffset(tPropName),
            (v) => tRenderer.material.SetTextureOffset(tPropName, v),
            Offset,
            Duration)
            .SetEase(EaseType)
            .SetLoops(LoopCount, LoopType.Restart);
    }
}
