using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UE = UnityEngine;

public partial class Chaos_PlayerObject : IDisposable
{

    public BoltEntity entity;
    public BoltConnection connection;

    public bool IsServer
    {
        get { return connection == null; }
    }

    public IChaos_PlayerState State
    {
        get { return entity.GetState<IChaos_PlayerState>(); }
    }


    public void Dispose()
    {
        players.Remove(this);

        // destroy
        if (entity)
        {
            BoltNetwork.Destroy(entity.gameObject);
        }

        //// while we have a team difference of more then 1 player
        //while (Mathf.Abs(redPlayers.Count() - bluePlayers.Count()) > 1)
        //{
        //    if (redPlayers.Count() < bluePlayers.Count())
        //    {
        //        var player = bluePlayers.First();
        //        player.Kill();
        //        player.state.team = TEAM_RED;
        //    }
        //    else
        //    {
        //        var player = redPlayers.First();
        //        player.Kill();
        //        player.state.team = TEAM_BLUE;
        //    }
        //}
    }

    public void Kill()
    {
        if (entity)
        {
            State.Dead = true;
            State.RespawnFrame = BoltNetwork.ServerFrame + (5 * BoltNetwork.FramesPerSecond);
        }
    }
}

partial class Chaos_PlayerObject
{
    static List<Chaos_PlayerObject> players = new List<Chaos_PlayerObject>();

    static Chaos_PlayerObject CreatePlayer(BoltConnection connection)
    {
        Chaos_PlayerObject player;

        // create a new player object, assign the connection property
        // of the object to the connection was passed in
        player = new Chaos_PlayerObject();
        player.connection = connection;

        // if we have a connection, assign this player
        // as the user data for the connection so that we
        // always have an easy way to get the player object
        // for a connection
        if (player.connection != null)
        {
            player.connection.UserData = player;
        }

        // add to list of all players
        players.Add(player);

        return player;
    }

    // this simply returns the 'players' list cast to
    // an IEnumerable<T> so that we hide the ability
    // to modify the player list from the outside.
    public static IEnumerable<Chaos_PlayerObject> AllPlayers
    {
        get { return players; }
    }

    // finds the server player by checking the
    // .IsServer property for every player object.
    public static Chaos_PlayerObject ServerPlayer
    {
        get { return players.Find(player => player.IsServer); }
    }

    // utility function which creates a server player
    public static Chaos_PlayerObject CreateServerPlayer()
    {
        return CreatePlayer(null);
    }

    // utility that creates a client player object.
    public static Chaos_PlayerObject CreateClientPlayer(BoltConnection connection)
    {
        return CreatePlayer(connection);
    }

    // utility function which lets us pass in a
    // BoltConnection object (even a null) and have
    // it return the proper player object for it.
    //public static Chaos_PlayerObject GetChaosPlayer(BoltConnection connection = null)
    //{
    //    if (connection == null)
    //    {
    //        return ServerPlayer;
    //    }

    //    return (Chaos_PlayerObject)connection.UserData;
    //}

    public void Spawn()
    {
        if (!entity)
        {
            entity = BoltNetwork.Instantiate(BoltPrefabs.Chaos_Player, RandomPosition(), Quaternion.identity);

            if (IsServer)
            {
                entity.TakeControl();
            }
            else
            {
                entity.AssignControl(connection);
            }
        }

        if (entity)
        {
            State.Dead = false;
            State.Health = 100;

            // teleport entity to a random spawn position
            entity.transform.position = RandomPosition();
        }

        
    }

    Vector3 RandomPosition()
    {
        float x = UE.Random.Range(20f, 30f);
        float y = 10f;
        float z = UE.Random.Range(20f, 30f);
       
        return new Vector3(x, y, z);
    }
}
