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
            Chaos_PlayerObject.GetChaosPlayer(connection).Spawn();
        }

		
	}
}
