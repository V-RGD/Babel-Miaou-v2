using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    public Camera cam;
    private Vector3 castPoint;
    public GameObject player;
    public LayerMask groundLayerMask;

    public PlayerController playerController;

    void Start()
    {
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        groundLayerMask = LayerMask.GetMask("Ground");
        player = GameObject.Find("Player");
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit, 10000, groundLayerMask) && hit.collider)
        {
            if (playerController.currentState != playerController.PlayerStates.Attack)
            {
                transform.LookAt(new Vector3(hit.point.x, player.transform.position.y, hit.point.z));
            }
        }*/
    }
}
