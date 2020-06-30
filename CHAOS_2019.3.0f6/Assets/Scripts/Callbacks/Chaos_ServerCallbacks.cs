using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UdpKit;
using Gamekit3D;

namespace Bolt.AdvancedTutorial
{
	[BoltGlobalBehaviour(BoltNetworkModes.Server, "CityScene")]
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
                if (p.entity && p.State.Dead && p.State.RespawnFrame <= BoltNetwork.ServerFrame)
                {
                    p.Spawn();
                }
            }
        }

        public override void Disconnected(BoltConnection connection)
        {
            BoltConsole.Write("Disconnected", Color.red);
            base.Disconnected(connection);
        }

        public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
        {
            BoltConsole.Write("ConnectFailed", Color.red);
            base.ConnectFailed(endpoint, token);
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

        public override void SceneLoadLocalBegin(string scene)
        {
            foreach (Chaos_PlayerObject p in Chaos_PlayerObject.AllPlayers)
            {
                p.entity = null;
            }
        }
    }
}
