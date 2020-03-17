using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public static class ExtensionMethods
	{
		public static Player GetPlayer (this BoltConnection connection)
		{
			if (connection == null) {
				return Player.serverPlayer;
			}

			return (Player)connection.UserData;
		}

        public static Chaos_PlayerObject GetChaosPlayer(this BoltConnection connection)
        {
            if (connection == null)
            {
                return Chaos_PlayerObject.ServerPlayer;
            }

            return (Chaos_PlayerObject)connection.UserData;
        }
    }
}
