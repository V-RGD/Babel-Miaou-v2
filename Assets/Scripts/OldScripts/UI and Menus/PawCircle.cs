using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PawCircle : MonoBehaviour
{
    public Image[] paws;
    public List<float> timers;
    public float speed;
    public float fadeAwaySpeed;
    private float _interval;
    private bool _canTurnAround;
    
    private void Start()
    {
        
        for (int i = 0; i < paws.Length; i++)
        {
            timers.Add(0);
        }

        _canTurnAround = true;
    }

    private void Update()
    {
        _interval = speed / paws.Length;

        for (var i = 0; i < timers.Count; i++)
        {
            timers[i] -= Time.deltaTime * fadeAwaySpeed;
            paws[i].color = new Color(paws[i].color.r, paws[i].color.g, paws[i].color.b,
                timers[i]);
        }

        if (_canTurnAround)
        {
            _canTurnAround = false;
            StartCoroutine(DelayTimers());
        }
    }

    IEnumerator DelayTimers()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            timers[i] = 1;
            yield return new WaitForSeconds(_interval);
        }

        _canTurnAround = true;
    }
}
