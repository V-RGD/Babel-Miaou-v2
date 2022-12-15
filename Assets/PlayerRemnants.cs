using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRemnants : MonoBehaviour
{
    private float _duration;
    private float _interval;
    [SerializeField]private float _amount;
    [SerializeField]private SpriteRenderer _remnantFx;
    [SerializeField]private SpriteRenderer playerSprite;
    private List<SpriteRenderer> _remnants = new List<SpriteRenderer>();
    private Transform _player;
    private void Start()
    {
        _duration = PlayerController.instance.dashLenght + 0.2f;
        _interval = _duration / _amount;
        _player = GameObject.Find("Player").transform;
        
        for (int i = 0; i < _amount; i++)
        {
            SpriteRenderer newRemnant = Instantiate(_remnantFx, GameManager.instance.transform);
            _remnants.Add(newRemnant);
        }
    }

    public IEnumerator DashRemnants()
    {
        //creates remnant
        for (int i = 0; i < _amount; i++)
        {
            _remnants[i].sprite = playerSprite.sprite;
            _remnants[i].flipX = playerSprite.flipX;
            _remnants[i].transform.position = _player.position + Vector3.up;
            yield return new WaitForSeconds(_interval);
        }

        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < _amount; i++)
        {
            _remnants[i].transform.position = Vector3.right * 1000;
        }
    }
}
