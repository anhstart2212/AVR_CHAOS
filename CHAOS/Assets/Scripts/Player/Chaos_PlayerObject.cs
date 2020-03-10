using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Chaos_PlayerObject : MonoBehaviour {

    public BoltEntity character;
    public BoltConnection connection;

    public bool IsServer
    {
        get { return connection == null; }
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
    public static Chaos_PlayerObject GetChaosPlayer(BoltConnection connection)
    {
        if (connection == null)
        {
            return ServerPlayer;
        }

        return (Chaos_PlayerObject)connection.UserData;
    }

    public void Spawn()
    {
        if (!character)
        {
            character = BoltNetwork.Instantiate(BoltPrefabs.Chaos_Player, RandomPosition(), Quaternion.identity);

            if (IsServer)
            {
                character.TakeControl();
            }
            else
            {
                character.AssignControl(connection);
            }
        }

        // teleport entity to a random spawn position
        character.transform.position = RandomPosition();
    }

    Vector3 RandomPosition()
    {
        float x = Random.Range(-5f, 5f);
        float z = Random.Range(-5f, 5f);
        return new Vector3(x, 2f, z);
    }
}
