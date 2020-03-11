using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class PlayerParticleEffects : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x0600024B RID: 587 RVA: 0x00015360 File Offset: 0x00013560
    private void Start()
    {
        this.player = base.GetComponentInParent<Player>();
        this.spdLineMaxSpeed = this.player.limit.maxHorizontal * 0.95f;
        this.spdLineMinSpeed = this.player.limit.maxHorizontal * 0.5f;
        //this.leftMWT = this.leftHand.GetComponent<MeleeWeaponTrail>();
        //this.rightMWT = this.rightHand.GetComponent<MeleeWeaponTrail>();
        this.defaultTrailTime = this.leftTrail.time;
    }

    // Token: 0x0600024C RID: 588 RVA: 0x000153E4 File Offset: 0x000135E4
    private void Update()
    {
        if (this.escapeRunning)
        {
            return;
        }
        this.SpeedLine();
        this.SlideFX();
        this.SwordFX();
    }

    // Token: 0x0600024D RID: 589 RVA: 0x00015404 File Offset: 0x00013604
    public void AttackHit(string type)
    {
        if (type != null)
        {
            if (!(type == "evade"))
            {
                if (!(type == "nape"))
                {
                    if (!(type == "knee"))
                    {
                        if (!(type == "body"))
                        {
                            if (!(type == "training"))
                            {
                                if (type == "escape")
                                {
                                    this.attackHit.Play();
                                    this.player.SoundScript.SwordSound("hit");
                                }
                            }
                            else
                            {
                                this.attackHit.Play();
                                IEnumerator enumerator = this.attackHit.transform.GetEnumerator();
                                try
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        object obj = enumerator.Current;
                                        Transform transform = (Transform)obj;
                                        if (transform.name == "Blood")
                                        {
                                            transform.GetComponent<ParticleSystem>().Stop();
                                        }
                                    }
                                }
                                finally
                                {
                                    IDisposable disposable;
                                    if ((disposable = (enumerator as IDisposable)) != null)
                                    {
                                        disposable.Dispose();
                                    }
                                }
                                this.player.SoundScript.SwordSound("hit");
                            }
                        }
                        else
                        {
                            this.player.SoundScript.SwordSound("hit");
                        }
                    }
                    else
                    {
                        this.attackHit.Play();
                        this.player.SoundScript.SwordSound("hit");
                    }
                }
                else
                {
                    this.attackHit.Play();
                    this.player.SoundScript.SwordSound("hit");
                }
            }
            else
            {
                if (this.escapeRunning)
                {
                    return;
                }
                this.trajectory = this.player.ActionScript.VelocityAtPrime;
                this.player.DisableScripts();
                base.StartCoroutine(this.Escape());
            }
        }
    }

    // Token: 0x0600024E RID: 590 RVA: 0x000155E4 File Offset: 0x000137E4
    private IEnumerator Escape()
    {
        this.escapeRunning = true;
        //this.leftMWT.Emit = true;
        //this.rightMWT.Emit = true;
        Vector3 initialVelocity = this.trajectory;
        Vector3 finalVelocity = initialVelocity * 0.4f;
        this.player.transform.rotation = Quaternion.LookRotation(initialVelocity, Vector3.up);
        this.attackHit.Play();
        this.player.SoundScript.SwordSound("escape");
        this.player.Animator.SetTrigger("GrabEvaded");
        float t = 0f;
        float runTime = 1.49253726f;
        while (t < 1f)
        {
            t += Time.deltaTime * runTime;
            this.player.RigidBody.velocity = Vector3.Lerp(initialVelocity, finalVelocity, this.rampCurve.Evaluate(t));
            yield return null;
        }
        //this.leftMWT.Emit = false;
        //this.rightMWT.Emit = false;
        this.player.EnableScripts();
        this.escapeRunning = false;
        yield break;
    }

    // Token: 0x0600024F RID: 591 RVA: 0x00015600 File Offset: 0x00013800
    private void SlideFX()
    {
        if (this.player.Animator.GetBool("IsSlideAnim") && state.IsGrounded)
        {
            switch (this.player.SlideState)
            {
                case 0:
                    this.SetDustAndSparkVFX(true, true, true, true);
                    break;
                case 1:
                    this.SetDustAndSparkVFX(false, true, false, true);
                    break;
                case 2:
                    this.SetDustAndSparkVFX(true, false, true, false);
                    break;
                case 3:
                    this.SetDustAndSparkVFX(true, true, false, false);
                    break;
            }
        }
        else if (!state.IsGrounded && this.player.Animator.GetFloat("HookLowPass") >= 0.95f && this.player.VelocityMagnitudeXZ > 35f)
        {
            if (this.player.Animator.GetFloat("CrossHook") <= -0.2f)
            {
                this.SetDustAndSparkVFX(true, false, true, false);
            }
            else if (this.player.Animator.GetFloat("CrossHook") >= 0.2f)
            {
                this.SetDustAndSparkVFX(false, true, false, true);
            }
            else
            {
                this.SetDustAndSparkVFX(true, true, true, true);
            }
        }
        else
        {
            this.SetDustAndSparkVFX(false, false, false, false);
        }
    }

    // Token: 0x06000250 RID: 592 RVA: 0x00015758 File Offset: 0x00013958
    private void SetDustAndSparkVFX(bool dustL, bool dustR, bool sparkL, bool sparkR)
    {
        if (dustL)
        {
            if (!this.dustKickUpL.isPlaying)
            {
                this.dustKickUpL.Play();
            }
        }
        else if (this.dustKickUpL.isPlaying)
        {
            this.dustKickUpL.Stop();
        }
        if (dustR)
        {
            if (!this.dustKickUpR.isPlaying)
            {
                this.dustKickUpR.Play();
            }
        }
        else if (this.dustKickUpR.isPlaying)
        {
            this.dustKickUpR.Stop();
        }
        if (sparkL)
        {
            if (!this.scabbardSparksL.isPlaying)
            {
                this.scabbardSparksL.Play();
            }
        }
        else if (this.scabbardSparksL.isPlaying)
        {
            this.scabbardSparksL.Stop();
        }
        if (sparkR)
        {
            if (!this.scabbardSparksR.isPlaying)
            {
                this.scabbardSparksR.Play();
            }
        }
        else if (this.scabbardSparksR.isPlaying)
        {
            this.scabbardSparksR.Stop();
        }
    }

    // Token: 0x06000251 RID: 593 RVA: 0x0001586C File Offset: 0x00013A6C
    private void SpeedLine()
    {
        if (state.IsGrounded)
        {
            this.speedLine.enableEmission = false;
            return;
        }
        Vector3 velocity = this.player.RigidBody.velocity;
        Vector3 vector = new Vector3(velocity.x, 0f, velocity.z);
        float magnitude = vector.magnitude;
        this.speedLine.transform.LookAt(base.transform.position + this.player.RigidBody.velocity);
        if (magnitude >= this.spdLineMinSpeed)
        {
            if (magnitude >= this.spdLineMaxSpeed)
            {
                magnitude = this.spdLineMaxSpeed;
            }
            float num = magnitude / this.spdLineMaxSpeed;
            this.spdLineNewAlpha.a = num * 1.3333f - 0.8333f;
            this.speedLine.emissionRate = Mathf.Clamp(magnitude - this.spdLineMinSpeed, 0f, 5f);
            this.speedLine.startColor = this.spdLineNewAlpha;
            this.speedLine.enableEmission = true;
        }
        else
        {
            this.speedLine.enableEmission = false;
        }
    }

    // Token: 0x06000252 RID: 594 RVA: 0x0001598C File Offset: 0x00013B8C
    private void SwordFX()
    {
        float num = this.player.limit.maxHorizontal / 2f;
        float num2 = (Mathf.Clamp(this.player.VelocityMagnitude, 0f, this.player.limit.maxHorizontal) - num) / num;
        float b = 0f;
        if (num2 > 0f && !this.player.IsAttacking)
        {
            b = this.defaultTrailTime * num2;
        }
        float num3 = Mathf.Lerp(this.leftTrail.time, b, (!this.player.IsAttacking) ? 0.2f : 1f);
        TrailRenderer trailRenderer = this.leftTrail;
        float time = num3;
        this.rightTrail.time = time;
        trailRenderer.time = time;
        bool flag = Common.CurrentStateTag(this.player.Animator, 0, PlayerAnimation.TAGS.attackPrime) || Common.CurrentStateTag(this.player.Animator, 0, PlayerAnimation.TAGS.gasBurst) || Common.CurrentStateTag(this.player.Animator, 0, PlayerAnimation.TAGS.grabbed) || this.player.IsAttacking;
        if (flag)
        {
            //this.leftMWT.Emit = true;
            //this.rightMWT.Emit = true;
        }
        else
        {
            //this.leftMWT.Emit = false;
            //this.rightMWT.Emit = false;
        }
    }

    // Token: 0x0400026D RID: 621
    public ParticleSystem speedLine;

    // Token: 0x0400026E RID: 622
    public ParticleSystem attackHit;

    // Token: 0x0400026F RID: 623
    public ParticleSystem dustKickUpL;

    // Token: 0x04000270 RID: 624
    public ParticleSystem dustKickUpR;

    // Token: 0x04000271 RID: 625
    public ParticleSystem scabbardSparksL;

    // Token: 0x04000272 RID: 626
    public ParticleSystem scabbardSparksR;

    // Token: 0x04000273 RID: 627
    public TrailRenderer leftTrail;

    // Token: 0x04000274 RID: 628
    public TrailRenderer rightTrail;

    // Token: 0x04000275 RID: 629
    public Transform leftHand;

    // Token: 0x04000276 RID: 630
    public Transform rightHand;

    // Token: 0x04000277 RID: 631
    //private MeleeWeaponTrail leftMWT;

    // Token: 0x04000278 RID: 632
    //private MeleeWeaponTrail rightMWT;

    // Token: 0x04000279 RID: 633
    private Player player;

    // Token: 0x0400027A RID: 634
    private float spdLineMaxSpeed;

    // Token: 0x0400027B RID: 635
    private float spdLineMinSpeed;

    // Token: 0x0400027C RID: 636
    public Color spdLineNewAlpha;

    // Token: 0x0400027D RID: 637
    private float defaultTrailTime;

    // Token: 0x0400027E RID: 638
    public AnimationCurve rampCurve;

    // Token: 0x0400027F RID: 639
    private bool escapeRunning;

    // Token: 0x04000280 RID: 640
    private Vector3 trajectory = Vector3.zero;
}
