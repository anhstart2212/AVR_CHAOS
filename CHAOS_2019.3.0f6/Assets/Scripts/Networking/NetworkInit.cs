using Bolt.Matchmaking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInit : Bolt.GlobalEventListener
{

    enum State
    {
        SelectMode,
        SelectMap,
        SelectRoom,
        StartServer,
        StartClient,
        Started,
    }

    [SerializeField]
    private State state;

    [SerializeField]
    private string map  =  "Hello";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// Call by unity button event
    /// </summary>
    public void State_StartServer()
    {
        BoltLauncher.StartServer();
        state = State.Started;
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            var id = Guid.NewGuid().ToString().Split('-')[0];
            var matchName = string.Format("{0} - {1}", id, map);

            BoltMatchmaking.CreateSession(
                sessionID: matchName,
                sceneToLoad: map
            );

            // BoltNetwork.SetServerInfo(matchName, null);
             BoltNetwork.LoadScene(map);
        }
    }
}
