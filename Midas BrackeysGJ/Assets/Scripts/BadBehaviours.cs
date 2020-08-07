using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadBehaviours : MonoBehaviour
{
    public enum EnemyBehaviours
    {
        WalkAround,
        WalkToThePlayer,
        FlyToThePlayer,
        Stomp
    }
    public EnemyBehaviours m_Behaviour;
    public GameObject playerGO;
    [SerializeField] private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_Behaviour)
        {
            case EnemyBehaviours.WalkAround:
                break;
            case EnemyBehaviours.WalkToThePlayer:
                break;
            case EnemyBehaviours.FlyToThePlayer:
                FlyingBeahavours();
                break;
            case EnemyBehaviours.Stomp:
                break;
            default:
                break;
        }
    }

    void FlyingBeahavours ()    
    {
        if (playerGO != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerGO.transform.position, speed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.gameObject.GetComponent<PlayerStatus>().IsAlive)
        {
            playerGO = collision.gameObject;
        }
    }
}
