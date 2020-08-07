using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviours : MonoBehaviour
{
    [SerializeField] private TypeofTrap m_TrapeType;

    [SerializeField] private GameObject spikeGO;
    [SerializeField] private Transform spikePos;

    [SerializeField] private float timeBtwSpawn;
    [SerializeField] private float startTimeBtwSpawn;

    public enum TypeofTrap
    {
        Shooter,
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_TrapeType)
        {
            case TypeofTrap.Shooter:
                Shoot();
                break;
            default:
                break;
        }
    }

    void Shoot()
    {
        if(timeBtwSpawn <= 0)
        {
            Instantiate(spikeGO, spikePos.position, Quaternion.identity);
            timeBtwSpawn = startTimeBtwSpawn;
        }
        else
        {
            timeBtwSpawn -= Time.deltaTime;
        }
    }
}
