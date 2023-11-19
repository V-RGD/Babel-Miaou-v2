using System.Collections;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
    private ObjectsManager objectsManager;
    IEnumerator Start()
    {
        objectsManager = ObjectsManager.instance;
        yield return new WaitForSeconds(objectsManager.gameVariables.poisonLenght);
        Destroy(gameObject);
    }
}
