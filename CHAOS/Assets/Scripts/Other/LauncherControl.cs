using Gamekit3D;
using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class LauncherControl : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x06000122 RID: 290 RVA: 0x0000C250 File Offset: 0x0000A450
    private void Start()
    {
        this.player = base.GetComponentInParent<Player>();
        //this.cc = this.player.transforms.playerCamera.GetComponent<CameraControl>();
    }

    // Token: 0x06000123 RID: 291 RVA: 0x0000C27C File Offset: 0x0000A47C
    private void LateUpdate()
    {
        //if (this.player.transforms.playerCamera == null)
        //{
        //    return;
        //}

        Vector3 pos;
        Quaternion rot;
        IChaos_PlayerState state = entity.GetState<IChaos_PlayerState>();

        // this calculate the looking angle for this specific entity
        CameraSettings.instance.CalculateDummyCameraTransform(state, out pos, out rot);

        m_Forward = rot * Vector3.forward;
        m_Pos = pos;
        //this.player.TitanAimLock = Input.GetButton(this.player.axisName.titanLock);
        if (state.IsCenterHook)
        {
            CenterHookTarget(m_Pos, m_Forward);
        }
        else
        {
            SideHookTarget(m_Pos, m_Forward);
        }

        GameObject x;
        if (this.isLeft)
        {
            x = this.player.CurrentLeftHook;
        }
        else
        {
            x = this.player.CurrentRightHook;
        }

        // Chaos Added

        //Vector3 direction2 = this.targetPoint - base.transform.position;
        //this.inRange = (this.TargetIsInRange(direction2, direction2.magnitude) && x == null);
        this.inRange = true; // Always detect Hook target

        // Chaos Added

        if (this.targetAcquired && this.inRange)
        {
            this.TargetSetAssignments(true);
        }
        else
        {
            this.TargetSetAssignments(false);
        }
    }

    public void HookTarget(BoltEntity entity)
    {
        this.player = entity.GetComponent<Player>(); ;

        IChaos_PlayerState state = entity.GetState<IChaos_PlayerState>();

        Vector3 pos;
        Quaternion rot;

        // this calculate the looking angle for this specific entity
        CameraSettings.instance.CalculateDummyCameraTransform(state, out pos, out rot);

        // display debug
        //Debug.DrawRay(pos, rot * Vector3.forward * 100, Color.blue);

        //Vector3 forward = player.transforms.playerCamera.forward;
        m_Forward = rot * Vector3.forward;
        m_Pos = pos;

        if (state.IsCenterHook)
        {
            CenterHookTarget(m_Pos, m_Forward);
        }
        else
        {
            SideHookTarget(m_Pos, m_Forward);
        }
    }

    // Token: 0x06000124 RID: 292 RVA: 0x0000C55C File Offset: 0x0000A75C
    private void AssignTargetVariables()
    {
        this.targetPoint = this.contact.point;
        this.targetAcquired = true;
        Transform rootObject = Common.GetRootObject(this.contact.transform);
        this.targetTag = rootObject.tag;
        if (this.targetTag.Equals("Titan"))
        {
            this.titanObject = rootObject.gameObject;
        }
        else
        {
            this.titanObject = null;
        }
    }

    // Token: 0x06000125 RID: 293 RVA: 0x0000C5CC File Offset: 0x0000A7CC
    private bool TargetIsInRange(Vector3 direction, float distance)
    {
        Vector3 vector = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z);
        Vector3 normalized = vector.normalized;
        Vector3 vector2 = new Vector3(direction.x, 0f, direction.z);
        Vector3 normalized2 = vector2.normalized;
        float num = Vector3.Angle(normalized2, normalized);
        Vector3 vector3 = Vector3.Cross(normalized2, normalized);
        if (distance > this.player.MaxHookDistance)
        {
            return false;
        }
        if (num > 110f)
        {
            return false;
        }
        if (this.isLeft)
        {
            return vector3.y <= 1f && vector3.y >= -0.9f;
        }
        return vector3.y <= 0.9f && vector3.y >= -1f;
    }

    // Token: 0x06000126 RID: 294 RVA: 0x0000C6CC File Offset: 0x0000A8CC
    private void TargetSetAssignments(bool targetSet)
    {
        if (this.isLeft)
        {
            this.player.LeftTargetPosition = this.targetPoint;
            this.player.IsLeftTargetSet = targetSet;
            this.player.LeftHookTitanObject = this.titanObject;
            if (this.player.CurrentLeftHook == null)
            {
                this.player.LeftHookTargetTag = this.targetTag;
            }
        }
        else
        {
            this.player.RightTargetPosition = this.targetPoint;
            this.player.IsRightTargetSet = targetSet;
            this.player.RightHookTitanObject = this.titanObject;
            if (this.player.CurrentRightHook == null)
            {
                this.player.RightHookTargetTag = this.targetTag;
            }
        }
    }

    private void SideHookTarget(Vector3 pos, Vector3 forward)
    {
        float d = 1f;
        float num = 0.05f;
        if (this.isLeft)
        {
            d = -1f;
        }
        for (int i = 0; i < 15; i++)
        {
            //Vector3 direction = forward + this.player.transforms.playerCamera.right * num * d;
            //Ray ray = new Ray(this.player.transform.position, direction);

            Vector3 direction = forward + CameraSettings.instance.DummyCamera.right * num * d;

            //Chaos Added
            //Ray ray = new Ray(this.player.transforms.playerCamera.position, direction);
            Ray ray = new Ray(pos, direction);
            //Chaos Added

            //Debug.DrawRay(pos, direction * 100, Color.red);

            if (!Physics.Raycast(ray, this.player.MaxHookDistance, Common.layerNoHook))
            {
                int layerMask = (!this.player.TitanAimLock) ? Common.layerOGT : Common.layerTitan;
                bool flag = Physics.Raycast(ray, out this.contact, this.player.MaxHookDistance, layerMask);
                if (flag)
                {
                    this.AssignTargetVariables();
                    break;
                }
                this.targetAcquired = false;
            }
            else
            {
                this.targetAcquired = false;
            }
            num += 0.025f;
        }
    }

    private void CenterHookTarget(Vector3 pos, Vector3 forward)
    {
        //Ray ray2 = new Ray(player.transforms.playerCamera.transform.position, forward);
        //Debug.DrawRay(player.transforms.playerCamera.transform.position, forward * 1000, Color.blue);

        Ray ray = new Ray(pos, forward);

        if (!Physics.Raycast(ray, this.player.MaxHookDistance, Common.layerNoHook))
        {
            int layerMask2 = (!this.player.TitanAimLock) ? Common.layerOGT : Common.layerTitan;
            bool flag2 = Physics.Raycast(ray, out this.contact, this.player.MaxHookDistance, layerMask2);
            if (flag2)
            {
                this.AssignTargetVariables();
            }
            else
            {
                this.targetAcquired = false;
            }
        }
        else
        {
            this.targetAcquired = false;
        }
    }

    // Token: 0x04000183 RID: 387
    [HideInInspector]
    public bool targetAcquired;

    // Token: 0x04000184 RID: 388
    [HideInInspector]
    public Vector3 targetPoint;

    // Token: 0x04000185 RID: 389
    [HideInInspector]
    public bool inRange;

    // Token: 0x04000186 RID: 390
    private Player player;

    // Token: 0x04000187 RID: 391
    //private CameraControl cc;

    // Token: 0x04000188 RID: 392
    private RaycastHit contact;

    // Token: 0x04000189 RID: 393
    public bool isLeft;

    // Token: 0x0400018A RID: 394
    private GameObject titanObject;

    // Token: 0x0400018B RID: 395
    private string targetTag;

    private Vector3 m_Pos;

    private Vector3 m_Forward;
}
