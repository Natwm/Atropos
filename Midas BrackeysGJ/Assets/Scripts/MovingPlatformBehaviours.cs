using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehaviours : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private Transform[] listOfPosition;
    [SerializeField] private float speed;
    [SerializeField] private int incremente = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (platform.transform.position == listOfPosition[incremente].position)
            incremente++;
        else
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, listOfPosition[incremente].position, speed *Time.deltaTime);
        }

        if(incremente == listOfPosition.Length)
        {
            incremente = 0;
        }
    }

}
