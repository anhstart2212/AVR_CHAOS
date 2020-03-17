using UnityEngine;
using System.Collections;
using Gamekit3D;
using Bolt.AdvancedTutorial;

public class Chaos_WeaponRifle : Chaos_WeaponBase
{
    public override void OnOwner(ChaosPlayerCommand cmd, BoltEntity entity)
    {
        if (entity.IsOwner)
        {
            //ChaosPlayerCommand state = entity.GetState<ChaosPlayerCommand> ();
            Player player = entity.GetComponent<Player>();

            Vector3 pos;
            Quaternion rot;
            IChaos_PlayerState state = entity.GetState<IChaos_PlayerState>();

            // this calculate the looking angle for this specific entity
            CameraSettings.instance.CalculateDummyCameraTransform(state, out pos, out rot);

            // display debug
            //Debug.DrawRay(pos, rot * Vector3.forward * 100, Color.blue);

            using (var hits = BoltNetwork.RaycastAll(new Ray(pos, rot * Vector3.forward), cmd.ServerFrame))
            {
                for (int i = 0; i < hits.count; ++i)
                {
                    var hit = hits.GetHit(i);
                    var serializer = hit.body.GetComponent<Player>();

                    if ((serializer != null) && (serializer.state != state))
                    {
                        serializer.ApplyDamage(player.ActiveWeapon.damagePerBullet);
                    }
                }
            }
        }
    }

    public override void Fx(BoltEntity entity)
    {
        Vector3 pos;
        Quaternion rot;
        IChaos_PlayerState state = entity.GetState<IChaos_PlayerState>();

        // this calculate the looking angle for this specific entity
        CameraSettings.instance.CalculateDummyCameraTransform(state, out pos, out rot);

        Ray r = new Ray(pos, rot * Vector3.forward);
        RaycastHit rh;

        if (Physics.Raycast(r, out rh) && impactPrefab)
        {
            var en = rh.transform.GetComponent<BoltEntity>();
            var hit = GameObject.Instantiate(impactPrefab, rh.point, Quaternion.LookRotation(rh.normal)) as GameObject;

            if (en)
            {
                hit.GetComponent<RandomSound>().enabled = false;
            }

            if (trailPrefab)
            {
                var trailGo = GameObject.Instantiate(trailPrefab, muzzleFlash.position, Quaternion.identity) as GameObject;
                var trail = trailGo.GetComponent<LineRenderer>();

                trail.SetPosition(0, muzzleFlash.position);
                trail.SetPosition(1, rh.point);
            }
        }

        GameObject go = (GameObject)GameObject.Instantiate(shellPrefab, shellEjector.position, shellEjector.rotation);
        go.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 2, ForceMode.VelocityChange);
        go.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-32f, +32f), Random.Range(-32f, +32f), Random.Range(-32f, +32f)), ForceMode.VelocityChange);

        // show flash
        muzzleFlash.gameObject.SetActive(true);
    }
}
