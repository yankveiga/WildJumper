using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle_destroy : MonoBehaviour
{
    public Transform point;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = player.position.z - point.position.z;
        if(distance > 5){
            Destroy(gameObject);
        }
    }
}
