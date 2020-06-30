using UnityEngine;
using System.Collections;
using System.Text;
using Gamekit3D;

namespace Bolt.AdvancedTutorial
{
	[BoltGlobalBehaviour("CityScene")]
	public class Chaos_PlayerCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene)
		{
            // ui
            Chaos_GameUI.Instantiate();

            // camera
            CameraSettings.Instantiate();
        }

        public override void ControlOfEntityGained(BoltEntity entity)
        {
            // this tells the player camera to look at the entity we are controlling
            CameraSettings.instance.SetTarget(entity);
        }
    }
}
