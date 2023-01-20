using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TraductionEffect : MonoBehaviour
{
    public static void TraductionEffectFunction(TextMeshProUGUI textToTranslate, TextMeshProUGUI textTranslated, string translationEndText, float delayBetweenEachCharacter)
    {
        string spaceGenerated = textToTranslate.text;
        textTranslated.text = "";

        for(int i = 0; i < textToTranslate.text.Length; i++)
        {
            char x = textToTranslate.text[i];
            char y = " "[0];

            spaceGenerated = spaceGenerated.Replace(x, y);
        }

        textToTranslate.DOTMPText(spaceGenerated, delayBetweenEachCharacter * translationEndText.Length);
        textTranslated.DOTMPText(translationEndText, delayBetweenEachCharacter * translationEndText.Length);
    }
}
