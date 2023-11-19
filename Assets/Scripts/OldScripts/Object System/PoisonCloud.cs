using System.Collections;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    private ObjectsManager_old objectsManager;
    IEnumerator Start()
    {
        objectsManager = ObjectsManager_old.instance;
        yield return new WaitForSeconds(objectsManager.gameVariables.poisonLenght);
        Destroy(gameObject);
    }
}
