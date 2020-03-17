using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class Chaos_GameSpawnTimer : Bolt.GlobalEventListener
	{
		BoltEntity m_Me;
		IChaos_PlayerState m_MeState;

		[SerializeField]
		TypogenicText timer;

		public override void ControlOfEntityGained (BoltEntity arg)
		{
			if (arg.GetComponent<PlayerNetworkHandler> ()) {
				m_Me = arg;
				m_MeState = m_Me.GetState<IChaos_PlayerState> ();
			}
		}

		public override void ControlOfEntityLost (BoltEntity arg)
		{
			if (arg.GetComponent<PlayerNetworkHandler> ()) {
				m_Me = null;
				m_MeState = null;
			}
		}

		void Update ()
		{
			// lock in middle of screen
			transform.position = Vector3.zero;

			// update timer
			if (m_Me && m_MeState != null) {
				if (m_MeState.Dead) {
					timer.Set (Mathf.Max (0, (m_MeState.RespawnFrame - BoltNetwork.Frame) / BoltNetwork.FramesPerSecond).ToString ());
				} else {
					timer.Set ("");
				}
			} else {
				timer.Set ("");
			}
		}
	}
}