using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehaviours : MonoBehaviour
{
    public enum PowerUpStatus
    {
        Coins,
        Poison,
        Midas
    }
    [SerializeField] private PowerUpStatus m_Status;

    public PowerUpStatus Status { get => m_Status; set => m_Status = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
