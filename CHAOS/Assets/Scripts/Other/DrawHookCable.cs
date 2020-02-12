using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class DrawHookCable : MonoBehaviour
{
    // Token: 0x0600010F RID: 271 RVA: 0x0000B109 File Offset: 0x00009309
    private void Awake()
    {
        this.lineRenderer = base.GetComponent<LineRenderer>();
        this.lineRenderer.SetWidth(this.lineWidth, this.lineWidth);
    }

    // Token: 0x06000110 RID: 272 RVA: 0x0000B130 File Offset: 0x00009330
    private void Start()
    {
        this.player = this.playerObject.GetComponent<Player>();
        this.lineRenderer.enabled = true;
        this.trailInstance = UnityEngine.Object.Instantiate<GameObject>(this.trailVFX, base.transform.position, base.transform.rotation);
        Transform hookOriginLeft = this.player.transforms.hookOriginLeft;
        Transform hookOriginRight = this.player.transforms.hookOriginRight;
        if (this.isLeft)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.launchVFX, hookOriginLeft.position, hookOriginLeft.rotation);
            gameObject.transform.parent = hookOriginLeft;
        }
        else
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.launchVFX, hookOriginRight.position, hookOriginRight.rotation);
            gameObject.transform.parent = hookOriginRight;
        }
        //if (this.titanObject != null)
        //{
        //    this.titanAnchor = new GameObject("Titan Anchor").transform;
        //    this.titanAnchor.position = this.target;
        //    TitanMain component = this.titanObject.GetComponent<TitanMain>();
        //    Transform[] bones;
        //    if (component == null)
        //    {
        //        TitanRagdollController component2 = this.titanObject.GetComponent<TitanRagdollController>();
        //        bones = component2.bones;
        //    }
        //    else
        //    {
        //        bones = component.BoneAll;
        //    }
        //    this.titanAnchor.parent = TitanBones.FindNearestBone(bones, this.target);
        //}
        this.lookAtPosition = (this.target - base.transform.position).normalized;
        this.positionLastFrame = base.transform.position;
    }

    // Token: 0x06000111 RID: 273 RVA: 0x0000B2BC File Offset: 0x000094BC
    private void LateUpdate()
    {
        if (this.playerObject != null)
        {
            Vector3 b = this.player.PositionNextFrame - this.playerObject.transform.position;
            Transform transform = (!this.isLeft) ? this.player.transforms.hookOriginRight : this.player.transforms.hookOriginLeft;
            base.transform.position = transform.position + b;
        }
        else
        {
            this.DestroyAll();
        }
    }

    // Token: 0x06000112 RID: 274 RVA: 0x0000B350 File Offset: 0x00009550
    private void FixedUpdate()
    {
        this.lineRenderer.SetPosition(0, base.transform.position);
        if (this.titanAnchor != null)
        {
            this.target = this.titanAnchor.position;
        }
        this.dist = Vector3.Distance(base.transform.position, this.target);
        this.lineRenderer.material.mainTextureScale = new Vector2(this.dist * 2f, 1f);
        if (!this.isRetracting)
        {
            if (!this.reachedTarget)
            {
                Vector3 vector = base.transform.position - this.positionLastFrame;
                Vector3 vector2 = Vector3.Normalize(this.target - base.transform.position);
                Vector3 from = Vector3.Project(vector, vector2);
                float num = from.magnitude;
                if (Vector3.Angle(from, vector2) < 90f)
                {
                    num *= -1f;
                }
                this.incDist += Time.fixedDeltaTime * this.lineDrawSpeed * 10f + num;
                if (this.incDist >= this.dist)
                {
                    this.incDist = this.dist;
                }
                Vector3 vector3 = this.incDist * vector2 + base.transform.position;
                if (this.trailInstance != null)
                {
                    this.trailInstance.transform.position = vector3;
                }
                this.lineRenderer.SetPosition(1, vector3);
                if (vector3 == this.target)
                {
                    this.reachedTarget = true;
                    this.SendHookedInfo(true);
                    this.jointInstance = UnityEngine.Object.Instantiate<GameObject>(this.joint, this.target, Quaternion.identity);
                    this.jointInstance.name = "Hook Anchor";
                    this.jointInstance.transform.LookAt(base.transform.position + this.lookAtPosition);
                    string text = (!this.isLeft) ? this.player.RightHookTargetTag : this.player.LeftHookTargetTag;
                    GameObject original;
                    if (text != null)
                    {
                        if (text == "Wood")
                        {
                            original = this.impactWoodFX;
                            goto IL_269;
                        }
                        if (text == "Titan")
                        {
                            original = this.impactFleshFX;
                            goto IL_269;
                        }
                    }
                    original = this.impactConcreteFX;
                    IL_269:
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, this.target, this.jointInstance.transform.rotation);
                    gameObject.name = "ImpactFX";
                    this.CreateJoint();
                    if (this.isLeft)
                    {
                        this.player.MaxLengthLeft = this.dist;
                        this.player.IsLeftImpulse = true;
                    }
                    else
                    {
                        this.player.MaxLengthRight = this.dist;
                        this.player.IsRightImpulse = true;
                    }
                }
            }
            else if (this.titanObject != null)
            {
                this.jointInstance.transform.position = this.target;
                this.lineRenderer.SetPosition(1, this.target);
                if (this.isLeft)
                {
                    this.player.LeftAnchorPosition = this.target;
                }
                else
                {
                    this.player.RightAnchorPosition = this.target;
                }
            }
            this.positionLastFrame = base.transform.position;
        }
        else
        {
            this.SendHookedInfo(false);
            if (this.jointInstance != null)
            {
                UnityEngine.Object.Destroy(this.jointInstance);
                UnityEngine.Object.Destroy(this.hookJoint);
            }
            if (!this.fullyRetracted)
            {
                if (this.firstRetractFlag)
                {
                    this.incDist = Vector3.Magnitude(this.target - base.transform.position);
                    this.firstRetractFlag = false;
                }
                this.incDist -= Time.fixedDeltaTime * this.lineDrawSpeed * 10f;
                if (this.incDist <= 0f)
                {
                    this.incDist = 0f;
                }
                Vector3 vector4 = this.incDist * Vector3.Normalize(this.target - base.transform.position) + base.transform.position;
                this.lineRenderer.SetPosition(1, vector4);
                if (vector4 == base.transform.position)
                {
                    this.fullyRetracted = true;
                }
            }
            else
            {
                if (this.titanAnchor != null)
                {
                    UnityEngine.Object.Destroy(this.titanAnchor.gameObject);
                }
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
    }

    // Token: 0x06000113 RID: 275 RVA: 0x0000B804 File Offset: 0x00009A04
    private void SendHookedInfo(bool isNotRetracting)
    {
        if (this.isLeft)
        {
            this.player.IsLeftHooked = isNotRetracting;
            this.player.LeftAnchorPosition = this.target;
        }
        else
        {
            this.player.IsRightHooked = isNotRetracting;
            this.player.RightAnchorPosition = this.target;
        }
    }

    // Token: 0x06000114 RID: 276 RVA: 0x0000B85C File Offset: 0x00009A5C
    private void CreateJoint()
    {
        this.hookJoint = this.playerObject.AddComponent<ConfigurableJoint>();
        SoftJointLimit linearLimit = this.hookJoint.linearLimit;
        linearLimit.limit = this.dist;
        linearLimit.bounciness = 0f;
        linearLimit.contactDistance = 0f;
        this.hookJoint.connectedBody = this.jointInstance.GetComponent<Rigidbody>();
        this.hookJoint.linearLimit = linearLimit;
        this.hookJoint.xMotion = ConfigurableJointMotion.Limited;
        this.hookJoint.yMotion = ConfigurableJointMotion.Limited;
        this.hookJoint.zMotion = ConfigurableJointMotion.Limited;
        this.hookJoint.enableCollision = false;
        this.hookJoint.autoConfigureConnectedAnchor = false;
        this.hookJoint.rotationDriveMode = RotationDriveMode.Slerp;
        JointDrive slerpDrive = this.hookJoint.slerpDrive;
        slerpDrive.mode = JointDriveMode.PositionAndVelocity;
        slerpDrive.positionSpring = 20f;
        slerpDrive.positionDamper = 5f;
        this.hookJoint.slerpDrive = slerpDrive;
        this.hookJoint.connectedAnchor = this.hookJoint.connectedBody.transform.InverseTransformPoint(this.jointInstance.transform.position);
    }

    // Token: 0x06000115 RID: 277 RVA: 0x0000B980 File Offset: 0x00009B80
    private void DestroyAll()
    {
        UnityEngine.Object.Destroy(this.jointInstance);
        UnityEngine.Object.Destroy(this.hookJoint);
        if (this.titanAnchor != null)
        {
            UnityEngine.Object.Destroy(this.titanAnchor.gameObject);
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    // Token: 0x04000149 RID: 329
    private LineRenderer lineRenderer;

    // Token: 0x0400014A RID: 330
    public GameObject playerObject;

    // Token: 0x0400014B RID: 331
    private Player player;

    // Token: 0x0400014C RID: 332
    public bool isLeft;

    // Token: 0x0400014D RID: 333
    public GameObject trailVFX;

    // Token: 0x0400014E RID: 334
    private GameObject trailInstance;

    // Token: 0x0400014F RID: 335
    public GameObject launchVFX;

    // Token: 0x04000150 RID: 336
    public GameObject impactConcreteFX;

    // Token: 0x04000151 RID: 337
    public GameObject impactWoodFX;

    // Token: 0x04000152 RID: 338
    public GameObject impactFleshFX;

    // Token: 0x04000153 RID: 339
    public GameObject joint;

    // Token: 0x04000154 RID: 340
    private GameObject jointInstance;

    // Token: 0x04000155 RID: 341
    [Range(0f, 0.05f)]
    public float lineWidth;

    // Token: 0x04000156 RID: 342
    [Range(15f, 40f)]
    public float lineDrawSpeed = 25f;

    // Token: 0x04000157 RID: 343
    private float incDist;

    // Token: 0x04000158 RID: 344
    private bool firstRetractFlag = true;

    // Token: 0x04000159 RID: 345
    private Vector3 positionLastFrame;

    // Token: 0x0400015A RID: 346
    private float dist;

    // Token: 0x0400015B RID: 347
    private Vector3 lookAtPosition;

    // Token: 0x0400015C RID: 348
    private Transform titanAnchor;

    // Token: 0x0400015D RID: 349
    [HideInInspector]
    public Vector3 target;

    // Token: 0x0400015E RID: 350
    [HideInInspector]
    public GameObject titanObject;

    // Token: 0x0400015F RID: 351
    [HideInInspector]
    public bool isRetracting;

    // Token: 0x04000160 RID: 352
    [HideInInspector]
    public bool reachedTarget;

    // Token: 0x04000161 RID: 353
    [HideInInspector]
    public bool fullyRetracted;

    // Token: 0x04000162 RID: 354
    [HideInInspector]
    public ConfigurableJoint hookJoint;
}
