using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingControl : MonoBehaviour
{
    public void FadeActive() {
        gameObject.SetActive(true);
    }
            //페이드 아웃
    public IEnumerator FadeInStart()
    {
        
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
