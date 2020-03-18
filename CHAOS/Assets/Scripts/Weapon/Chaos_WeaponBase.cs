using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial
{
	public class Chaos_WeaponBase : MonoBehaviour
	{
		[SerializeField]
		public GameObject shellPrefab;

		[SerializeField]
		public GameObject impactPrefab;

		[SerializeField]
		public GameObject trailPrefab;

		[SerializeField]
		public Transform muzzleFlash;

		[SerializeField]
		public Transform shellEjector;

		[SerializeField]
		public AudioClip fireSound;

		[SerializeField]
		public AudioClip dryFireSound;

		[SerializeField]
		public AudioClip reloadSound;

		public byte damagePerBullet = 25;
		public int refireRate = 5;
		public int fireFrame;

		public virtual void OnOwner (ChaosPlayerCommand cmd, BoltEntity entity)
		{

		}

		public virtual void Fx (BoltEntity entity)
		{

		}

        public virtual void BasicAttack()
        {

        }

        public virtual void HeavyAttack()
        {

        }

        public virtual void Parry()
        {

        }
    }
}
