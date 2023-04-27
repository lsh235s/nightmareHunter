using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingControl : MonoBehaviour
{
            //페이드 아웃
    public IEnumerator FadeInStart()
    {
        gameObject.SetActive(true);
        for (float f = 1f; f > 0; f -= 0.005f)
        {
            Color c = gameObject.GetComponent<Image>().color;
            c.a = f;
            gameObject.GetComponent<Image>().color = c;
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}
