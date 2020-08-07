using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PlayerStatus player;
    [SerializeField] GameObject playerGO;
    [SerializeField] Transform playerSpawner;
    [SerializeField] int nbMaxDeath = 5;
    [SerializeField] CinemachineVirtualCamera m_VC;
    [SerializeField] int sizeOfOffset;

    void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.Health = 0;
        }

        float newX = Mathf.Lerp(0, sizeOfOffset, 0.05f );

        if (Input.GetAxis("Horizontal")>0)
        {
            m_VC.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.Lerp(m_VC.GetComponent<CinemachineCameraOffset>().m_Offset, new Vector3(sizeOfOffset, m_VC.GetComponent<CinemachineCameraOffset>().m_Offset.y, m_VC.GetComponent<CinemachineCameraOffset>().m_Offset.z), Time.deltaTime);
        }else if(Input.GetAxis("Horizontal") < 0)
        {
            m_VC.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.Lerp(m_VC.GetComponent<CinemachineCameraOffset>().m_Offset, new Vector3(0-(sizeOfOffset/1.5f), m_VC.GetComponent<CinemachineCameraOffset>().m_Offset.y, m_VC.GetComponent<CinemachineCameraOffset>().m_Offset.z), Time.deltaTime);
        }
        else
        {
            m_VC.GetComponent<CinemachineCameraOffset>().m_Offset = Vector3.Lerp(m_VC.GetComponent<CinemachineCameraOffset>().m_Offset, Vector3.zero, 7*Time.deltaTime);
        }


        if (player.Health <= 0  )
        {
            DestroyPlayer();
            m_VC.LookAt = player.transform.GetChild(0).transform;
            m_VC.Follow = player.transform.GetChild(0).transform;
        }
    }

    void DestroyPlayer()
    {
        player.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        player.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        player.gameObject.layer = 8;
        player.gameObject.GetComponent<PlayerController>().enabled = false;
        player.gameObject.GetComponent<PlayerInput>().enabled = false;
        player.gameObject.GetComponent<PlayerStatus>().enabled = true;
        player.gameObject.GetComponent<PlayerStatus>().IsAlive = false;

        switch (player.gameObject.GetComponent<PlayerStatus>().Power)
        {
            case PlayerStatus.PlayerPower.Normal:
                break;
            case PlayerStatus.PlayerPower.Poisoned:
                player.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                break;
            case PlayerStatus.PlayerPower.Midas:
                player.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                break;
            default:
                break;
        }

        if (GameObject.FindObjectsOfType<PlayerStatus>().Length < nbMaxDeath)
            player = Instantiate(playerGO, playerSpawner.position, Quaternion.identity).GetComponent<PlayerStatus>();
    }
}
