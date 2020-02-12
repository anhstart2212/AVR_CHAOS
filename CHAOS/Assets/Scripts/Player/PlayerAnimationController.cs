using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class PlayerAnimationController : MonoBehaviour
{
    // Token: 0x060001F1 RID: 497 RVA: 0x0000F693 File Offset: 0x0000D893
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        this.anim = base.GetComponent<Animator>();
        this.rb = base.GetComponent<Rigidbody>();
        this.landingVelocity = Vector3.zero;
    }

    // Token: 0x060001F2 RID: 498 RVA: 0x0000F6C4 File Offset: 0x0000D8C4
    private void Update()
    {
        if (this.anim.GetBool("IsHooked") && this.player.IsAnimating)
        {
            this.ToggleAnimatingState("end");
        }
    }

    // Token: 0x060001F3 RID: 499 RVA: 0x0000F6F8 File Offset: 0x0000D8F8
    private void LateUpdate()
    {
        if (!this.player.IsGrounded)
        {
            this.landingVelocity = this.rb.velocity;
            this.anim.SetFloat("ImpactVertVelocity", this.landingVelocity.y);
            Animator animator = this.anim;
            string name = "ImpactHorzVelocity";
            Vector3 vector = new Vector3(this.landingVelocity.x, 0f, this.landingVelocity.z);
            animator.SetFloat(name, vector.magnitude);
        }
        this.anim.SetBool("IsGrounded", this.player.IsGrounded);
        this.anim.SetFloat("VertVelocity", this.player.VelocityYSigned);
        this.anim.SetFloat("HorzVelocity", this.player.VelocityMagnitudeXZ);
        if (this.player.IsAnimating && PlayerAnimation.InIdleRunOrAir(this.anim, true) && !this.anim.IsInTransition(0))
        {
            this.ToggleAnimatingState("end");
        }
        this.SetHeight();
        this.SetGrabbed();
        this.SetJumped();
        this.SetSalute();
        this.SetRunning();
        this.SetAirborne();
        this.SetSlide();
        this.SetHooks();
        this.SetAcrobatics();
        this.SetGasBurst();
        this.SetWallMovement();
        this.SetGrabbed();
        if (this.anim.GetBool("IsGrounded"))
        {
            base.Invoke("ResetImpactVelocity", 1f);
        }
    }

    // Token: 0x060001F4 RID: 500 RVA: 0x0000F875 File Offset: 0x0000DA75
    private void ResetImpactVelocity()
    {
        this.anim.SetFloat("ImpactVertVelocity", 0f);
        this.anim.SetFloat("ImpactHorzVelocity", 0f);
    }

    // Token: 0x060001F5 RID: 501 RVA: 0x0000F8A1 File Offset: 0x0000DAA1
    private void JumpComplete()
    {
        this.anim.SetBool("IsJumping", false);
    }

    // Token: 0x060001F6 RID: 502 RVA: 0x0000F8B4 File Offset: 0x0000DAB4
    private void DropBlades(string time)
    {
        if (time == "start")
        {
            this.player.SoundScript.SwordSound("eject");
            if (this.reloadSuccessful)
            {
                foreach (GameObject gameObject in new GameObject[]
                {
                    UnityEngine.Object.Instantiate<GameObject>(this.blades[0], this.blades[0].transform.position, this.blades[0].transform.rotation),
                    UnityEngine.Object.Instantiate<GameObject>(this.blades[1], this.blades[1].transform.position, this.blades[1].transform.rotation)
                })
                {
                    gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    gameObject.GetComponent<Rigidbody>().velocity = this.rb.velocity;
                    gameObject.GetComponent<BoxCollider>().enabled = true;
                    //gameObject.AddComponent<DestroyByTime>();
                    //gameObject.GetComponent<DestroyByTime>().lifeTime = 5f;
                }
            }
            this.blades[0].SetActive(false);
            this.blades[1].SetActive(false);
            this.reloadSuccessful = false;
        }
        else
        {
            this.player.SoundScript.SwordSound("draw");
            this.blades[0].SetActive(true);
            this.blades[1].SetActive(true);
            this.reloadSuccessful = true;
        }
    }

    // Token: 0x060001F7 RID: 503 RVA: 0x0000FA1F File Offset: 0x0000DC1F
    public void ToggleAnimatingState(string time)
    {
        this.player.IsAnimating = time.Equals("start");
    }

    // Token: 0x060001F8 RID: 504 RVA: 0x0000FA38 File Offset: 0x0000DC38
    private void SetHeight()
    {
        Ray ray = new Ray(base.transform.position, -Vector3.up);
        if (!this.player.IsEitherHooked)
        {
            this.anim.SetFloat("Height", this.player.Height);
        }
        else if (Physics.Raycast(ray, out this.contact, 500f, Common.layerOGT))
        {
            this.anim.SetFloat("Height", Mathf.Round((base.transform.position.y - this.contact.point.y) * 100f) * 0.01f);
        }
    }

    // Token: 0x060001F9 RID: 505 RVA: 0x0000FAF4 File Offset: 0x0000DCF4
    private void SetGrabbed()
    {
        this.anim.SetBool("IsGrabbed", this.player.IsGrabbed);
        this.anim.SetBool("IsGrabbedAnim", Common.CurrentOrNextStateTag(this.anim, 0, PlayerAnimation.TAGS.grabbed));
    }

    // Token: 0x060001FA RID: 506 RVA: 0x0000FB42 File Offset: 0x0000DD42
    private void SetJumped()
    {
        if (this.player.IsJumping)
        {
            this.anim.SetBool("IsJumping", true);
            base.Invoke("JumpComplete", 0.1f);
        }
    }

    // Token: 0x060001FB RID: 507 RVA: 0x0000FB78 File Offset: 0x0000DD78
    private void SetSalute()
    {
        if (Input.GetButtonDown(this.player.axisName.salute) && Mathf.Round(this.rb.velocity.magnitude * 1000f) / 1000f == 0f)
        {
            if (this.anim.GetBool("IsSaluting"))
            {
                this.anim.SetBool("IsSaluting", false);
            }
            else
            {
                this.anim.SetBool("IsSaluting", true);
            }
        }
        if (this.anim.GetBool("IsSaluting") && (Input.GetAxis(this.player.axisName.moveLeftRight) != 0f || Input.GetAxis(this.player.axisName.moveFrontBack) != 0f))
        {
            this.anim.SetBool("IsSaluting", false);
        }
    }

    // Token: 0x060001FC RID: 508 RVA: 0x0000FC70 File Offset: 0x0000DE70
    private void SetRunning()
    {
        if (Input.GetAxisRaw(this.player.axisName.moveLeftRight) != 0f || Input.GetAxisRaw(this.player.axisName.moveFrontBack) != 0f)
        {
            this.anim.SetBool("IsRunning", true);
        }
        else
        {
            this.anim.SetBool("IsRunning", false);
        }
    }

    // Token: 0x060001FD RID: 509 RVA: 0x0000FCE4 File Offset: 0x0000DEE4
    private void SetAirborne()
    {
        if (!this.player.IsGrounded)
        {
            this.anim.SetFloat("AirborneAngle", Vector3.Angle(this.rb.velocity, Vector3.up));
            this.anim.SetFloat("VelocityAngle", Vector3.Angle(this.player.VelocityDirectionXZ, base.transform.forward));
            this.anim.SetFloat("CrossAirborne", Vector3.Cross(this.player.VelocityDirectionXZ, base.transform.forward).y);
        }
    }

    // Token: 0x060001FE RID: 510 RVA: 0x0000FD84 File Offset: 0x0000DF84
    private void SetSlide()
    {
        this.anim.SetBool("IsSliding", this.player.IsSliding);
        this.anim.SetInteger("SlideState", this.player.SlideState);
        if (Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.slide))
        {
            this.anim.SetBool("IsSlideAnim", true);
        }
        else
        {
            this.anim.SetBool("IsSlideAnim", false);
        }
    }

    // Token: 0x060001FF RID: 511 RVA: 0x0000FE0C File Offset: 0x0000E00C
    private void SetHooks()
    {
        Vector3 v = Vector3.zero;
        Vector3 v2 = Vector3.zero;
        if (this.player.IsRightHooked)
        {
            v = this.player.CurrentRightHook.GetComponent<DrawHookCable>().target - this.player.PositionNextFrame;
            v = Common.RemoveYComponent(v);
            this.anim.SetBool("IsHooked", true);
        }
        if (this.player.IsLeftHooked)
        {
            v2 = this.player.CurrentLeftHook.GetComponent<DrawHookCable>().target - this.player.PositionNextFrame;
            v2 = Common.RemoveYComponent(v2);
            this.anim.SetBool("IsHooked", true);
        }
        Vector3 lhs = Vector3.zero;
        if (this.player.JumpReelKeyDown)
        {
            this.anim.SetFloat("ReelingIn", Mathf.Lerp(this.anim.GetFloat("ReelingIn"), 1f, 0.15f));
        }
        else
        {
            this.anim.SetFloat("ReelingIn", Mathf.Lerp(this.anim.GetFloat("ReelingIn"), 0f, 0.1f));
        }
        if (this.player.IsDoubleHooked)
        {
            lhs = (v.normalized + v2.normalized).normalized;
        }
        else if (this.player.IsRightHooked)
        {
            lhs = v.normalized;
        }
        else if (this.player.IsLeftHooked)
        {
            lhs = v2.normalized;
        }
        if (this.player.IsEitherHooked)
        {
            this.anim.SetFloat("CrossHook", Mathf.Lerp(this.anim.GetFloat("CrossHook"), Vector3.Cross(lhs, base.transform.forward).y, 0.05f));
        }
        else
        {
            this.anim.SetFloat("CrossHook", Mathf.Lerp(this.anim.GetFloat("CrossHook"), 0f, 0.05f));
        }
        this.anim.SetFloat("HookAngle", Mathf.Lerp(this.anim.GetFloat("HookAngle"), Vector3.Angle(Vector3.up, this.player.HookDirection), 0.05f));
        float height = this.player.Height;
        float num;
        if (height >= 2.5f)
        {
            num = 0f;
        }
        else if (height <= 1.5f)
        {
            num = 1f;
        }
        else
        {
            num = 1f - (height - 1.5f);
        }
        if (height >= 1.5f && this.player.VelocityMagnitudeXZ <= 30f)
        {
            num = 0f;
        }
        float num2 = Mathf.Lerp(this.anim.GetFloat("HookLowPass"), num, 0.1f);
        if ((num == 0f && num2 <= 0.05f) || (num == 1f && num2 >= 0.95f))
        {
            num2 = num;
        }
        this.anim.SetFloat("HookLowPass", num2);
        if (!this.player.IsEitherHooked)
        {
            this.anim.SetBool("IsHooked", false);
        }
        if (this.player.IsEitherHooked && !this.player.JumpReelKeyDown && height <= 2.5f && !this.player.IsGrounded)
        {
            this.player.RigidBody.velocity = this.player.RigidBody.velocity * 0.99f;
        }
        if (Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.hookAnimation) || Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.gasBurst) || Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.attackRelease) || Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.attackPrime))
        {
            this.anim.SetBool("IsHookAnim", true);
        }
        else
        {
            this.anim.SetBool("IsHookAnim", false);
        }
        this.anim.SetInteger("ReelState", this.player.ReeledState);
    }

    // Token: 0x06000200 RID: 512 RVA: 0x0001027C File Offset: 0x0000E47C
    private void SetAcrobatics()
    {
        bool flag = this.anim.GetFloat("CrossHook") >= -0.2f && this.anim.GetFloat("CrossHook") <= 0.2f && PlayerAnimation.MinimumVelocities(this.anim, this.player.limit.maxVertical * 0.2f, this.player.limit.maxHorizontal * 0.33f);
        bool flag2 = (this.anim.GetFloat("CrossHook") < -0.2f || this.anim.GetFloat("CrossHook") > 0.2f) && PlayerAnimation.MinimumVelocities(this.anim, 0f, this.player.limit.maxHorizontal * 0.33f);
        if ((flag || flag2) && UnityEngine.Random.value <= 0.75f)
        {
            this.anim.SetBool("Acrobatics", true);
        }
        else
        {
            this.anim.SetBool("Acrobatics", false);
        }
        if (this.anim.GetFloat("Height") <= 5f)
        {
            this.anim.SetBool("Acrobatics", false);
        }
        if (Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.acrobatics) || Common.NextStateTag(this.anim, 0, PlayerAnimation.TAGS.acrobatics))
        {
            this.anim.SetBool("IsAcrobaticAnim", true);
        }
        else
        {
            this.anim.SetBool("IsAcrobaticAnim", false);
        }
    }

    // Token: 0x06000201 RID: 513 RVA: 0x00010448 File Offset: 0x0000E648
    private void SetReload()
    {
        if (this.anim.GetBool("Reload"))
        {
            this.anim.ResetTrigger("Reload");
        }
        if (Input.GetButtonDown(this.player.axisName.reload) && PlayerAnimation.InIdleRunOrAir(this.anim, false) && !this.anim.GetBool("Reload"))
        {
            this.anim.SetTrigger("Reload");
        }
    }

    // Token: 0x06000202 RID: 514 RVA: 0x000104CC File Offset: 0x0000E6CC
    private void SetGasBurst()
    {
        if (this.player.BurstForceIsRunning || this.player.BurstTurnIsRunning)
        {
            this.anim.SetBool("IsBursting", true);
            if (!this.burstTriggerCheck && this.player.BurstForceIsRunning)
            {
                this.anim.SetTrigger("GasBurst");
                this.burstTriggerCheck = true;
            }
        }
        else
        {
            this.burstTriggerCheck = false;
            this.anim.SetBool("IsBursting", false);
        }
    }

    // Token: 0x06000203 RID: 515 RVA: 0x0001055C File Offset: 0x0000E75C
    private void SetWallMovement()
    {
        PlayerAnimation.SetWallMovementState(this.anim, this.player.WalledState);
        if (Common.CurrentStateTag(this.anim, 0, PlayerAnimation.TAGS.wallMovement))
        {
            this.anim.SetBool("IsWallMoveAnim", true);
        }
        else
        {
            this.anim.SetBool("IsWallMoveAnim", false);
        }
    }

    // Token: 0x04000220 RID: 544
    private Player player;

    // Token: 0x04000221 RID: 545
    private Animator anim;

    // Token: 0x04000222 RID: 546
    private Rigidbody rb;

    // Token: 0x04000223 RID: 547
    public GameObject[] blades = new GameObject[2];

    // Token: 0x04000224 RID: 548
    public GameObject bladeFallSFX;

    // Token: 0x04000225 RID: 549
    private Vector3 landingVelocity;

    // Token: 0x04000226 RID: 550
    private bool reloadSuccessful = true;

    // Token: 0x04000227 RID: 551
    private RaycastHit contact;

    // Token: 0x04000228 RID: 552
    private bool burstTriggerCheck;
}
