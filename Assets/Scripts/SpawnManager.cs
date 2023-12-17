using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints_redside, spawnPoints_blueside;

    public static SpawnManager instance;

    [SerializeField] GameObject spawnCharacter_blue,spawnCharacter_red;
    [SerializeField] Material blue;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonTeamsManager.Instance.TryGetTeamByCode(1,out var blue);
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam()== blue)
        {
        var player = PhotonNetwork.Instantiate(spawnCharacter_blue.name,GetSpawnPoints().position,Quaternion.identity);
        player.name = PhotonNetwork.NickName;
        }
        else
        {
         var player = PhotonNetwork.Instantiate(spawnCharacter_red.name,GetSpawnPoints().position,Quaternion.identity);
           player.name = PhotonNetwork.NickName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetSpawnPoints()
    {
         PhotonTeamsManager.Instance.TryGetTeamByCode(1,out var blue);
        if(PhotonNetwork.LocalPlayer.GetPhotonTeam()== blue)
        {
           return spawnPoints_blueside[Random.Range(0, spawnPoints_blueside.Length)];
        }
        else
        {
            return spawnPoints_redside[Random.Range(0,spawnPoints_redside.Length)];
        }
    }

    public Transform GetSpawnPoints_2(bool isBlue)
    {
       if(isBlue)
       {
          return spawnPoints_blueside[Random.Range(0, spawnPoints_blueside.Length)];
        }
        else
        {
            return spawnPoints_redside[Random.Range(0,spawnPoints_redside.Length)];
        }
    }
}
