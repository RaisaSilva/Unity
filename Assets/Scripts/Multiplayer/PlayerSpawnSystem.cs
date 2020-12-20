﻿using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    //public GameObject playerPrefab = null;

    public GameObject[] playerPrefab = new GameObject[4];


    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
        public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

        public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

        /*public override void OnStartClient()
        {
            InputManager.Add(ActionMapNames.Player);
            InputManager.Controls.Player.Look.Enable();
        }*/

        [ServerCallback]
        private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        if (nextIndex <= 3)
        {
            Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

            if (spawnPoint == null)
            {
                Debug.LogError($"Missing spawn point for player {nextIndex}");
                return;
            }

            GameObject playerInstance = Instantiate(playerPrefab[nextIndex], spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
            NetworkServer.Spawn(playerInstance, conn);

            nextIndex++;
        }
    }
}

