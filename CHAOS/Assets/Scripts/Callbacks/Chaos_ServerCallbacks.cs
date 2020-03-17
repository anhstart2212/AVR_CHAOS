using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UdpKit;
using Gamekit3D;

namespace Bolt.AdvancedTutorial
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "Test")]
	public class Chaos_ServerCallbacks : Bolt.GlobalEventListener
	{
		public static bool ListenServer = true;

		void Awake()
		{
			if (ListenServer)
			{
                Chaos_PlayerObject.CreateServerPlayer();
            }
		}

        void FixedUpdate()
        {
            foreach (Chaos_PlayerObject p in Chaos_PlayerObject.AllPlayers)
            {
                // if we have an entity, it's dead but our spawn frame has passed
                if (p.entity && p.state.Dead && p.state.RespawnFrame <= BoltNetwork.ServerFrame)
                {
                    p.Spawn();
                }
            }
        }

        public override void Connected(BoltConnection connection)
        {
            Chaos_PlayerObject.CreateClientPlayer(connection);
        }

        public override void SceneLoadLocalDone(string scene)
        {
            Chaos_PlayerObject.ServerPlayer.Spawn();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
		{
            connection.GetChaosPlayer().Spawn();
        }

		
	}
}
