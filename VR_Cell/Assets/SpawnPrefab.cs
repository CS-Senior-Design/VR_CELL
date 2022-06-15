using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour
{
    public GameObject Prefab;
    public Transform player;

    void SpawnItem()
    {
        var newItem = Instantiate(Prefab, new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z),Quaternion.identity);
    }
}
