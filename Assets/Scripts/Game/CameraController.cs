using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;

    private static CameraController instance;
    public static CameraController Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //player = GameObject.FindWithTag("Player").transform;
        //offset = transform.position - player.position;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
        offset = transform.position - player.position;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}
