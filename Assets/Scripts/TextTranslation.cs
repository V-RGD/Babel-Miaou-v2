using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextTranslation : MonoBehaviour
{
    private TMP_Text text;
    public List<TMP_FontAsset> fonts;
    public float interval;


    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(TranslateText());
        }
    }

    public IEnumerator TranslateText()
    {
        for (int i = 0; i < fonts.Count; i++)
        {
            text.font = fonts[i];
            yield return new WaitForSeconds(interval);
        }
    }
}
