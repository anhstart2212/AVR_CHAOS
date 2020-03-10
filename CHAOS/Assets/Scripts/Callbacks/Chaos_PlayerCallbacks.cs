using UnityEngine;
using System.Collections;
using System.Text;
using Gamekit3D;

namespace Bolt.AdvancedTutorial
{
	[BoltGlobalBehaviour("Test")]
	public class Chaos_PlayerCallbacks : Bolt.GlobalEventListener
	{
		public override void SceneLoadLocalDone(string scene)
		{
            // ui
            GameUI.Instantiate();

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
