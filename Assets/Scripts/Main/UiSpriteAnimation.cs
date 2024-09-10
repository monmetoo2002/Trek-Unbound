using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSpriteAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] spriteArray;
    public float speed = 0.02f;
    private int indexSprite;
    Coroutine corotineAnim;
    bool isDone;

    void Start()
    {
        // Tự động bắt đầu animation khi scene được tải
        Func_PlayUIAnim();
    }

    public void Func_PlayUIAnim()
    {
        if (corotineAnim != null)
        {
            StopCoroutine(corotineAnim);
        }
        isDone = false;
        corotineAnim = StartCoroutine(Func_PlayAnimUI());
    }

    public void Func_StopUIAnim()
    {
        isDone = true;
        if (corotineAnim != null)
        {
            StopCoroutine(corotineAnim);
            corotineAnim = null;
        }
    }

    IEnumerator Func_PlayAnimUI()
    {
        while (!isDone)
        {
            yield return new WaitForSeconds(speed);
            if (indexSprite >= spriteArray.Length)
            {
                indexSprite = 0;
            }
            image.sprite = spriteArray[indexSprite];
            indexSprite += 1;
        }
    }
}
