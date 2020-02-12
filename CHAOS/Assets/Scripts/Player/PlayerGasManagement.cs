using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000038 RID: 56
public class PlayerGasManagement : MonoBehaviour
{
    // Token: 0x0600021F RID: 543 RVA: 0x00013170 File Offset: 0x00011370
    private void Start()
    {
        this.player = base.GetComponent<Player>();
        this.SetGas(this.player.gas.maxGasAmount);
        this.InitGasCores();
    }

    // Token: 0x06000220 RID: 544 RVA: 0x0001319C File Offset: 0x0001139C
    private void InitGasCores()
    {
        this.innerCore = this.player.particles.gasCore;
        this.outerCore = this.innerCore.gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        this.inner = new PlayerGasManagement.CoreSettings(this.innerCore.main, 0.5f, 0.75f);
        this.outer = new PlayerGasManagement.CoreSettings(this.outerCore.main, 0.5f, 0.75f);
    }

    // Token: 0x06000221 RID: 545 RVA: 0x00013220 File Offset: 0x00011420
    private void Update()
    {
        this.CheckForGasEffect();
        this.GasConsumptionControl();
    }

    // Token: 0x06000222 RID: 546 RVA: 0x00013230 File Offset: 0x00011430
    private void LateUpdate()
    {
        float num = this.currentGas / this.player.gas.maxGasAmount;
        if (num > 1f)
        {
            num = 1f;
        }
        if (num < 0f)
        {
            num = 0f;
        }
        this.gasBar.fillAmount = num;
        num = Mathf.Clamp(this.player.BurstTankLevel / 100f, 0f, 1f);
        this.gasMiniBar.fillAmount = num;
    }

    // Token: 0x06000223 RID: 547 RVA: 0x000132B0 File Offset: 0x000114B0
    private void CheckForGasEffect()
    {
        if (this.player.IsGrounded || this.player.ReeledState != 0)
        {
            this.GasTrailOnOff(false, false, this.player.BurstForceIsRunning);
        }
        else if ((this.player.WalledState != 0 && !this.player.IsEitherHooked) || !this.player.JumpReelKeyDown)
        {
            this.GasTrailOnOff(false, false, this.player.BurstForceIsRunning);
        }
        else if (this.player.IsDoubleHooked)
        {
            this.GasCoreLifeAdjust(true);
            this.GasTrailOnOff(true, true, this.player.BurstForceIsRunning);
        }
        else if (this.player.IsEitherHooked)
        {
            this.GasCoreLifeAdjust(true);
            this.GasTrailOnOff(true, false, this.player.BurstForceIsRunning);
        }
        else if (this.player.IsMoving && this.player.JumpReelKeyDown)
        {
            this.GasCoreLifeAdjust(false);
            this.GasTrailOnOff(true, false, this.player.BurstForceIsRunning);
        }
        else
        {
            this.GasTrailOnOff(false, false, this.player.BurstForceIsRunning);
        }
    }

    // Token: 0x06000224 RID: 548 RVA: 0x000133F0 File Offset: 0x000115F0
    private void GasCoreLifeAdjust(bool max)
    {
        if (max)
        {
            this.inner.main.startLifetime = this.inner.lifeMax;
            this.inner.main.startSize = this.inner.sizeMax;
            this.outer.main.startLifetime = this.outer.lifeMax;
            this.outer.main.startSize = this.outer.sizeMax;
        }
        else
        {
            this.inner.main.startLifetime = this.inner.lifeMin;
            this.inner.main.startSize = this.inner.sizeMin;
            this.outer.main.startLifetime = this.outer.lifeMin;
            this.outer.main.startSize = this.outer.sizeMin;
        }
    }

    // Token: 0x06000225 RID: 549 RVA: 0x00013508 File Offset: 0x00011708
    private void GasTrailOnOff(bool core, bool fast, bool burst)
    {
        PlayerParticles particles = this.player.particles;
        if (core)
        {
            this.player.IsGasVFX = true;
        }
        else
        {
            this.player.IsGasVFX = false;
        }
        if (core)
        {
            if (!particles.gasCore.isPlaying)
            {
                particles.gasCore.Play();
            }
        }
        else if (particles.gasCore.isPlaying)
        {
            particles.gasCore.Stop();
        }
        if (fast)
        {
            if (!particles.gasFast.isPlaying)
            {
                particles.gasFast.Play();
            }
        }
        else if (particles.gasFast.isPlaying)
        {
            particles.gasFast.Stop();
        }
        if (burst)
        {
            if (!particles.gasBurst.isPlaying)
            {
                particles.gasBurst.Play();
            }
        }
        else if (particles.gasBurst.isPlaying)
        {
            particles.gasBurst.Stop();
        }
    }

    // Token: 0x06000226 RID: 550 RVA: 0x00013608 File Offset: 0x00011808
    private void GasConsumptionControl()
    {
        bool flag;
        if (this.player.PhysicsState == 2 && this.player.JumpReelKeyDown)
        {
            flag = true;
            this.currentContinuousGasCost = this.player.gas.costAirMove;
        }
        else if (this.player.PhysicsState == 4)
        {
            flag = true;
            this.currentContinuousGasCost = ((!this.player.IsDoubleHooked) ? this.player.gas.costSingleHook : this.player.gas.costDoubleHook);
        }
        else
        {
            flag = false;
        }
        if (!this.continuousGasRunning && flag)
        {
            this.continuousGasRunning = true;
            base.StartCoroutine(this.ContinuousGasConsumption());
        }
        else if (this.continuousGasRunning && !flag)
        {
            this.continuousGasRunning = false;
        }
    }

    // Token: 0x06000227 RID: 551 RVA: 0x000136EA File Offset: 0x000118EA
    private void SetGas(float amount)
    {
        this.currentGas = amount;
    }

    // Token: 0x06000228 RID: 552 RVA: 0x000136F3 File Offset: 0x000118F3
    public void ExpendGas(float amount)
    {
        this.currentGas -= amount;
    }

    // Token: 0x06000229 RID: 553 RVA: 0x00013703 File Offset: 0x00011903
    private void AddGas(float amount)
    {
        this.currentGas += amount;
    }

    // Token: 0x0600022A RID: 554 RVA: 0x00013714 File Offset: 0x00011914
    private IEnumerator ContinuousGasConsumption()
    {
        float maxWaitTime = 1f;
        while (this.continuousGasRunning)
        {
            float timeSinceLast = Time.time - this.timeOfLastConsumption;
            if (timeSinceLast >= maxWaitTime)
            {
                timeSinceLast = maxWaitTime;
            }
            else if (timeSinceLast <= 0f)
            {
                timeSinceLast = 0f;
            }
            float waitTime = maxWaitTime - (maxWaitTime - timeSinceLast);
            if (timeSinceLast >= maxWaitTime)
            {
                this.currentGas -= this.currentContinuousGasCost;
                this.timeOfLastConsumption = Time.time;
            }
            yield return new WaitForSeconds(waitTime);
        }
        yield break;
    }

    // Token: 0x04000246 RID: 582
    private Player player;

    // Token: 0x04000247 RID: 583
    public Image gasBar;

    // Token: 0x04000248 RID: 584
    public Image gasMiniBar;

    // Token: 0x04000249 RID: 585
    private float currentGas;

    // Token: 0x0400024A RID: 586
    private float currentContinuousGasCost;

    // Token: 0x0400024B RID: 587
    private bool continuousGasRunning;

    // Token: 0x0400024C RID: 588
    private float timeOfLastConsumption;

    // Token: 0x0400024D RID: 589
    private ParticleSystem innerCore;

    // Token: 0x0400024E RID: 590
    private ParticleSystem outerCore;

    // Token: 0x0400024F RID: 591
    private PlayerGasManagement.CoreSettings inner;

    // Token: 0x04000250 RID: 592
    private PlayerGasManagement.CoreSettings outer;

    // Token: 0x02000039 RID: 57
    private class CoreSettings
    {
        // Token: 0x0600022B RID: 555 RVA: 0x00013730 File Offset: 0x00011930
        public CoreSettings(ParticleSystem.MainModule main, float scaleLife, float scaleSize)
        {
            this.main = main;
            this.lifeMin = main.startLifetime.constant * scaleLife;
            this.lifeMax = main.startLifetime.constant;
            this.sizeMin = main.startSize.constant * scaleSize;
            this.sizeMax = main.startSize.constant;
        }

        // Token: 0x04000251 RID: 593
        public float lifeMin;

        // Token: 0x04000252 RID: 594
        public float lifeMax;

        // Token: 0x04000253 RID: 595
        public float sizeMin;

        // Token: 0x04000254 RID: 596
        public float sizeMax;

        // Token: 0x04000255 RID: 597
        public ParticleSystem.MainModule main;
    }
}
