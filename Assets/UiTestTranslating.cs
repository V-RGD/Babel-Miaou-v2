using TMPro;
using UnityEngine;

public class UiTestTranslating : MonoBehaviour
{
    public TextMeshProUGUI textToTranslate;
    public TextMeshProUGUI textTranslated;

    private void Update()
    {
        
    }

    public void TranslateText()
    {
        TraductionEffect.TraductionEffectFunction(textToTranslate, textTranslated, textTranslated.text, 0.1f);
    }
}
