using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviours : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int amountOfDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed/10); 
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bi");
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerStatus>().GetDamage(amountOfDamage);
            Destroy(this.gameObject);
        }
    }
}
