using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFade : MonoBehaviour
{
    [SerializeField] private Graphic[] UiElements;

    public float fadeTime;

    public void FadeOut()
    {
        StartCoroutine(Fade());
    } 

    private IEnumerator Fade()
    {        
        Color newColor = Color.white;
        
        while (newColor.a > 0)
        {
            newColor.a -= Time.deltaTime / fadeTime;

            foreach (var element in UiElements)
            {
                element.color = newColor;
            }            

            yield return null;            
        }
    }
}
