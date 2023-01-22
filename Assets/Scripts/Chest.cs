using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    public bool isPlayerInRange;
    private float _openDist = 5;
    private PlayerControls _playerControls;
    private InputAction _collect;
    private GameObject _player;
    public GameObject messagePrompt;
    public Animator animator;

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _player = GameObject.Find("Player");
        _collect = _playerControls.Player.Interact;
    }

    private void Start()
    {
        ObjectsManager.instance = ObjectsManager.instance;
    }

    void Collect(InputAction.CallbackContext context)
    {
        if (isPlayerInRange)
        {
            StartCoroutine(ChestLoot());
        }
    }

    IEnumerator ChestLoot()
    {
        Vector3 lootDropPos = transform.position;
        //spawns a random item between items, spells, or loot
        int randLoot = Random.Range(0, 100);
        if (randLoot <= 15)
        {
            GameObject item = Instantiate(ObjectsManager.instance.objectTemplate, transform.position, Quaternion.identity);
            //checks which items are already equipped and remove them from the possible items
            item.GetComponent<Item>().objectID = ObjectsManager.instance.itemList[Random.Range(0, ObjectsManager.instance.itemList.Count)];
            item.SetActive(true);
            float randDir = Random.Range(0, 1f);
            Vector3 pushDir = Vector3.up * 10 + new Vector3(0.5f - randDir, 0, 0.5f + randDir)* 5;
            item.GetComponent<Rigidbody>().AddForce(pushDir.normalized * 10);
        }
        else
        {
            //heal,eyes
            GameObject heal = Instantiate(ObjectsManager.instance.healItem, lootDropPos + Vector3.left,
                quaternion.identity);
            heal.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            GameObject eyes = Instantiate(ObjectsManager.instance.eyeToken, lootDropPos + Vector3.right,
                quaternion.identity);
            eyes.SetActive(true);
        }
        
        animator.CrossFade(Animator.StringToHash("Open"), 0, 0);
    }

    private void Update()
    {
        if ((_player.transform.position - transform.position).magnitude <= _openDist)
        {
            //show message prompt
            isPlayerInRange = true;
            messagePrompt.SetActive(true);
        }
        else
        {
            //disable message prompt
            isPlayerInRange = false;
            messagePrompt.SetActive(false);
        }
    }

    #region InputSystemRequirements
    private void OnEnable()
    
    {
        _collect.Enable();
        _collect.performed += Collect;
    }

    private void OnDisable()
    {
        _collect.Disable();
    }
    #endregion
}
