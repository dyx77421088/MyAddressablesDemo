using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public Transform playerBirthPosition;
    private GameObject player;
    void Start()
    {
        StartCoroutine(InitGO("Player", go =>
        {
            player = go;
            player.transform.position = playerBirthPosition.position;
            player.AddComponent<PlayerController>();
            CameraController.Instance.SetPlayer(player.transform);
        }));
    }

    IEnumerator InitGO(string gameName, Action<GameObject> rgo)
    {
        var t = Addressables.InstantiateAsync(gameName);
        yield return t;
        rgo(t.Result);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
