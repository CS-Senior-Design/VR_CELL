using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject player;

    public void SpawnItem()
    {
        var newItem = Instantiate(Prefab, new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z),Quaternion.identity);
    }
}
