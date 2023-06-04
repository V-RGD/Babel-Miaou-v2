using UnityEngine;

public class BossManagement : MonoBehaviour
{
    public bool canActivateBossFight;
    public GameObject player;
    public Transform replacePlayerPoint;
    public Animator bossDoor;
    private void Start()
    {
        canActivateBossFight = true;
        player = GameObject.Find("Player");
        player.transform.position =
            new Vector3(replacePlayerPoint.position.x, player.transform.position.y, replacePlayerPoint.position.z);
        bossDoor.transform.parent.transform.parent.gameObject.SetActive(false);
    }

    //when entering the room, closes the door

    private void OnTriggerEnter(Collider other)
    {
        if (canActivateBossFight && other.CompareTag("Player"))
        {
            Debug.Log("activated boss");
            bossDoor.transform.parent.transform.parent.gameObject.SetActive(true);
            canActivateBossFight = false;
            StartCoroutine(FinalBossIA.instance.BossApparitionSequence());
            bossDoor.CrossFade(Animator.StringToHash("Close"), 0);
        }
    }
}