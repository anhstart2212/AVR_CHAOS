using System;
using UnityEngine;
using Gamekit3D;
using Bolt.AdvancedTutorial;
using Bolt;
using System.Collections;

public struct State
{
    public Vector3 position;
    public Quaternion rotation;
}

// Token: 0x02000030 RID: 48
public class Player : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    // Token: 0x06000128 RID: 296 RVA: 0x0000C7CC File Offset: 0x0000A9CC
    private void Awake()
    {
        this.rigidBody = base.GetComponent<Rigidbody>();
        this.animator = base.GetComponent<Animator>();
        this.InitColliders();
        this.moveScript = base.GetComponent<PlayerMovement>();
        this.rotScript = base.GetComponent<PlayerRotations>();
        this.burstScript = base.GetComponent<PlayerBursts>();
        this.actionScript = base.GetComponent<PlayerActions>();
        this.wallScript = base.GetComponent<WallContactInteractions>();
        this.gasScript = base.GetComponent<PlayerGasManagement>();
        this.animScript = base.GetComponent<PlayerAnimationController>();
        this.soundScript = base.GetComponent<PlayerSoundController>();
        this.fxScript = base.GetComponentInChildren<PlayerParticleEffects>();
        this.hudScript = base.GetComponent<DrawPlayerHUD>();
        //this.camScript = this.transforms.playerCamera.GetComponent<CameraControl>();
        cameraSettings = FindObjectOfType<CameraSettings>();
        ////this.gameScript = GameObject.FindWithTag("GameController").GetComponent<GameBase>();
        if (cameraSettings != null)
        {
            transforms.playerCamera = cameraSettings.CinemachineBrain;
            transforms.hookTargetNull = cameraSettings.HookTargetNull;
        }

        m_State = new State();
        m_State.position = transform.position;

        m_LauncherControls = GetComponentsInChildren<LauncherControl>();
    }

    // Token: 0x0600012A RID: 298 RVA: 0x0000C8A4 File Offset: 0x0000AAA4
    private void InitColliders()
    {
        this.capCollider = base.GetComponent<CapsuleCollider>();
        Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
        int num = 0;
        foreach (Transform transform in componentsInChildren)
        {
            if (this.ValidateColliders(transform.name))
            {
                this.lowerColliders[num++] = transform.gameObject.GetComponent<CapsuleCollider>();
            }
        }
        if (num != 4)
        {
            Debug.LogError("Could not find all lower leg and feet colliders on player!");
        }
    }

    // Token: 0x0600012B RID: 299 RVA: 0x0000C920 File Offset: 0x0000AB20
    private bool ValidateColliders(string name)
    {
        string value = "Collider(Toe_L)";
        string value2 = "Collider(Toe_R)";
        string value3 = "Collider(LowLeg_L)";
        string value4 = "Collider(LowLeg_R)";
        return name.Equals(value) || name.Equals(value2) || name.Equals(value3) || name.Equals(value4);
    }

    // Token: 0x0600012C RID: 300 RVA: 0x0000C974 File Offset: 0x0000AB74
    private void Update()
    {
        //this.jumpReelKeyDown = (Input.GetAxisRaw(this.axisName.jumpReel) != 0f);
        //this.jumpReelKeyDown = (Input.GetButton(this.axisName.jumpReel));
        //this.centerHookKeyDown = (Input.GetAxisRaw(this.axisName.centerHook) != 0f);
        //this.fireLeftHook = Input.GetButton(this.axisName.fireLeftHook);
        //this.fireRightHook = Input.GetButton(this.axisName.fireRightHook);
        //jumpKeyDown = Input.GetButtonDown(this.axisName.jump);
        //attackKeyDown = Input.GetButtonDown(this.axisName.attack);
        //this.movementX = Input.GetAxis(this.axisName.moveLeftRight);
        //this.movementY = Input.GetAxis(this.axisName.moveFrontBack);

        //// Chaos Added
        //this.fastSpeed = (Input.GetButton(this.axisName.fastSpeed));
        //// Chaos Added

        //this.mouseAxisX = Input.GetAxis(this.axisName.mouseX);
        //this.mouseAxisY = Input.GetAxis(this.axisName.mouseY) * (float)((PlayerPrefs.GetInt(PlayerPrefKeys.invertYAxis) != 0) ? -1 : 1);

        this.CheckIfGrounded();
        this.CheckIfStationary();
        this.CheckSliding();
        //this.CheckIfJumping();
        this.CheckForGrabRelease();
        this.SetColliders();
        this.SetPhysicsState();
        this.SetAndResetHookDirection();
        this.ResetImpulse();
        this.ResetAnchors();

    }

    // Token: 0x0600012D RID: 301 RVA: 0x0000CAA7 File Offset: 0x0000ACA7
    private void FixedUpdate()
    {
        this.VelocityProjection();
        this.CacheVelocityVariables();
    }

    // Token: 0x0600012E RID: 302 RVA: 0x0000CAB5 File Offset: 0x0000ACB5
    private void LateUpdate()
    {
        this.EnforceMaxSpeeds();
        this.SetPositionNextFrame();
        this.SetVelocityLastFrame();
    }

    public void PollKeys()
    {
        if (cameraSettings != null)
        {
            m_MouseXaxis = cameraSettings.Current.m_XAxis.Value;
            m_MouseYaxis = cameraSettings.Current.m_YAxis.Value;
        }

        movementX = Input.GetAxis(axisName.moveLeftRight);
        movementY = Input.GetAxis(axisName.moveFrontBack);
        jumpKeyDown = Input.GetButtonDown(axisName.jump);
        centerHookKeyDown = (Input.GetButton(axisName.centerHook));
        fireLeftHook = Input.GetButton(axisName.fireLeftHook);
        fireRightHook = Input.GetButton(axisName.fireRightHook);
        jumpReelKeyDown = (Input.GetButton(axisName.jumpReel));

        //titanAimLock = Input.GetButton(axisName.titanLock);
    }

    public void AnimateRun(ChaosPlayerCommand cmd)
    {
        AnimationScript.SetRunning(cmd);
    }

    public void AnimateHook()
    {
        //AnimationScript.SetHooks(entity);
    }

    //void FindHookTarget()
    //{
    //    foreach (var launcherControl in m_LauncherControls)
    //    {
    //        launcherControl.HookTarget(entity);
    //    }
    //}

    public void PlayerHook()
    {
        state.PlayerHook();
    }

    public void OnPlayerHook()
    {
        MovementScript.StartHookActions(entity);
    }

    public void DrawOwnerHookTarget()
    {
        hudScript.DrawOwnerHookTarget(true);
    }

    public State Apply(float movementX, float movementY, float mouseX, bool jumpKey, bool fireLeftHook, bool fireRightHook, bool jumpReelKey)
    {
        PlayerMoveInput(movementX, movementY);

        PlayerHookInput(fireLeftHook, fireRightHook, jumpReelKey);

        RotationScript.PlayerRotation(movementX, movementY, mouseX);

        // jump
        CheckIfJumping(jumpKey);

        // update transform
        m_State.position = transform.position;
        m_State.rotation = transform.rotation;

        // done
        return m_State;
    }

    private void PlayerMoveInput(float movementX, float movementY)
    {
        MovementX = movementX;
        MovementY = movementY;
    }

    private void PlayerHookInput(bool fireLeftHook, bool fireRightHook, bool jumpReelKey)
    {
        FireLeftHook = fireLeftHook;
        FireRightHook = fireRightHook;
        JumpReelKeyDown = jumpReelKey;
    }


    private void CheckIfGrounded()
    {
        bool flag = false;
        for (int i = 0; i < this.rayHits.Length; i++)
        {
            this.rayHits[i] = false;
        }
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
        {
            this.SetControlGroundState(false, false);
            flag = true;
        }
        if (this.IsEitherHooked && this.groundTakeOff)
        {
            if (this.animator.GetFloat("Height") > 2f || !this.JumpReelKeyDown)
            {
                base.Invoke("ResetGroundTakeOff", 0.25f);
            }
            this.SetControlGroundState(false, false);
            flag = true;
        }
        if (this.IsEitherHooked && this.jumpReelKeyDown && this.animator.GetFloat("HookLowPass") > 0f)
        {
            this.SetControlGroundState(false, false);
            flag = true;
        }
        bool flag2 = false;
        this.rayOrigins[0] = base.transform.position;
        this.groundRays[0] = new Ray(this.rayOrigins[0], Vector3.down);
        RaycastHit raycastHit;
        this.rayHits[0] = Physics.Raycast(this.groundRays[0], out raycastHit, 500f, Common.layerObstacle | Common.layerGround | Common.layerNoHook);
        if (this.rayHits[0])
        {
            this.height = this.rayOrigins[0].y - raycastHit.point.y;
            if (flag)
            {
                return;
            }
            if (this.height <= 1.1f)
            {
                this.SetControlGroundState(true, false);
            }
            else if (this.height <= 1.5f)
            {
                flag2 = true;
            }
            else
            {
                this.SetControlGroundState(false, false);
            }
        }
        else
        {
            Debug.LogWarning("RAYCAST ERROR: Player is too high to detect surface! Cast length set to 500 units.");
        }
        if (flag2)
        {
            this.rayOrigins[1] = base.transform.position + 0.25f * base.transform.forward;
            this.rayOrigins[2] = base.transform.position + 0.25f * -base.transform.forward;
            this.rayOrigins[3] = base.transform.position + 0.25f * -base.transform.right;
            this.rayOrigins[4] = base.transform.position + 0.25f * base.transform.right;
            for (int j = 1; j < this.groundRays.Length; j++)
            {
                this.groundRays[j] = new Ray(this.rayOrigins[j], Vector3.down);
                this.rayHits[j] = Physics.Raycast(this.groundRays[j], 1.2f, Common.layerObstacle | Common.layerGround | Common.layerNoHook);
                if (this.rayHits[j])
                {
                    this.SetControlGroundState(true, true);
                    break;
                }
                if (j == this.rayHits.Length - 1)
                {
                    this.SetControlGroundState(false, false);
                }
            }
        }
        if (this.isGrounded)
        {
            this.ApplyHeightCorrection();
        }
    }

    // Token: 0x06000130 RID: 304 RVA: 0x0000CE6C File Offset: 0x0000B06C
    private void ApplyHeightCorrection()
    {
        float num = this.height - 1f;
        Vector3 position = base.transform.position;
        position.y = position.y - num + 0.025f;
        base.transform.position = position;
    }

    // Token: 0x06000131 RID: 305 RVA: 0x0000CEB4 File Offset: 0x0000B0B4
    private void SetControlGroundState(bool grounded, bool slopedTerrain)
    {
        this.isGrounded = grounded;
        this.slopedTerrain = slopedTerrain;
    }

    // Token: 0x06000132 RID: 306 RVA: 0x0000CEC4 File Offset: 0x0000B0C4
    private void CheckIfJumping(bool jumpKey)
    {
        if (jumpKey && this.isGrounded && !this.jumping && !this.IsEitherHooked)
        {
            this.jumping = true;
        }
        else if (this.JumpReelKeyDown && this.isGrounded && this.IsEitherHooked)
        {
            this.groundTakeOff = true;
        }
        else if (!this.isGrounded)
        {
            this.jumping = false;
        }
    }

    // Token: 0x06000133 RID: 307 RVA: 0x0000CF52 File Offset: 0x0000B152
    private void ResetGroundTakeOff()
    {
        this.groundTakeOff = false;
    }

    // Token: 0x06000134 RID: 308 RVA: 0x0000CF5C File Offset: 0x0000B15C
    private void CheckIfStationary()
    {
        if (Mathf.Round(this.VelocityMagnitude) <= 3f)
        {
            if (this.stationaryCount >= 10)
            {
                this.stationary = true;
            }
            this.stationaryCount++;
        }
        else
        {
            this.stationary = false;
            this.stationaryCount = 0;
        }
    }

    // Token: 0x06000135 RID: 309 RVA: 0x0000CFB3 File Offset: 0x0000B1B3
    private void CheckSliding()
    {
        if (!this.isGrounded)
        {
            this.sliding = false;
        }
    }

    // Token: 0x06000136 RID: 310 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
    public void ResetImpulse()
    {
        if (this.leftImpulse && this.rightImpulse && !this.resetImpulse)
        {
            this.resetImpulse = true;
            return;
        }
        if (this.resetImpulse)
        {
            this.leftImpulse = false;
            this.rightImpulse = false;
            this.resetImpulse = false;
        }
        if (this.leftImpulse && !this.resetImpulse)
        {
            this.resetImpulse = true;
            return;
        }
        if (this.resetImpulse)
        {
            this.leftImpulse = false;
            this.resetImpulse = false;
        }
        if (this.rightImpulse && !this.resetImpulse)
        {
            this.resetImpulse = true;
            return;
        }
        if (this.resetImpulse)
        {
            this.rightImpulse = false;
            this.resetImpulse = false;
        }
    }

    // Token: 0x06000137 RID: 311 RVA: 0x0000D08C File Offset: 0x0000B28C
    private void CheckForGrabRelease()
    {
        if (this.grabbed)
        {
            base.transform.localPosition = Vector3.zero;
            base.transform.localRotation = Quaternion.identity;
            base.transform.localScale = Vector3.one;
        }
    }

    // Token: 0x06000138 RID: 312 RVA: 0x0000D0CC File Offset: 0x0000B2CC
    private void SetColliders()
    {
        this.capCollider.enabled = this.isGrounded;
        for (int i = 0; i < this.lowerColliders.Length; i++)
        {
            this.lowerColliders[i].enabled = !this.isGrounded;
        }
    }

    // Token: 0x06000139 RID: 313 RVA: 0x0000D11C File Offset: 0x0000B31C
    private void SetPhysicsState()
    {
        if (this.isGrounded)
        {
            this.physicsState = 0;
        }
        else if (!this.IsEitherHooked && this.walledState == 0)
        {
            if (!this.IsMoving)
            {
                this.physicsState = 1;
            }
            else
            {
                this.physicsState = 2;
            }
        }
        else if (this.IsEitherHooked && this.walledState == 0)
        {
            if (!this.jumpReelKeyDown)
            {
                this.physicsState = 3;
            }
            else
            {
                this.physicsState = 4;
            }
        }
        else
        {
            this.physicsState = 5;
        }
        this.AssignPhysicsValues();
    }

    // Token: 0x0600013A RID: 314 RVA: 0x0000D1C0 File Offset: 0x0000B3C0
    private void AssignPhysicsValues()
    {
        switch (this.PhysicsState)
        {
            case 0:
                if (this.height > 1.1f)
                {
                    this.SetPhysicsVariables(true, 0f);
                }
                else
                {
                    this.SetPhysicsVariables(false, 0f);
                }
                break;
            case 1:
                this.SetPhysicsVariables(true, 0f);
                break;
            case 2:
                this.SetPhysicsVariables(true, 0f);
                break;
            case 3:
                if (this.reeledState == 0)
                {
                    this.SetPhysicsVariables(false, 0f);
                }
                else
                {
                    this.SetPhysicsVariables(true, 0.5f);
                }
                break;
            case 4:
                if (this.IsDoubleHooked)
                {
                    this.SetPhysicsVariables(false, 0f);
                }
                else
                {
                    this.SetPhysicsVariables(false, 0f);
                }
                break;
            case 5:
                this.SetPhysicsVariables(false, 0f);
                break;
        }
    }

    // Token: 0x0600013B RID: 315 RVA: 0x0000D2B6 File Offset: 0x0000B4B6
    private void SetPhysicsVariables(bool useGrav, float drag)
    {
        this.rigidBody.useGravity = useGrav;
        this.rigidBody.drag = drag;
    }

    // Token: 0x0600013C RID: 316 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
    private void SetPositionNextFrame()
    {
        this.positionNextFrame = base.transform.position + this.rigidBody.velocity * Time.fixedDeltaTime;
    }

    // Token: 0x0600013D RID: 317 RVA: 0x0000D2FD File Offset: 0x0000B4FD
    private void SetVelocityLastFrame()
    {
        this.velocityLastFrame = this.rigidBody.velocity;
    }

    // Token: 0x0600013E RID: 318 RVA: 0x0000D310 File Offset: 0x0000B510
    private void SetAndResetHookDirection()
    {
        if (this.leftHooked && this.rightHooked)
        {
            //this.hookDirection = (this.leftAnchorPosition - base.transform.position + (this.rightAnchorPosition - base.transform.position)).normalized;
            this.hookDirection = ((this.transforms.playerCamera.transform.forward * this.speed.cameraForwardDouble) + (this.leftAnchorPosition - base.transform.position) + (this.rightAnchorPosition - base.transform.position)).normalized;
        }
        else if (this.leftHooked)
        {
            //this.hookDirection = (this.leftAnchorPosition - base.transform.position).normalized;
            this.hookDirection = ((this.transforms.playerCamera.transform.forward * this.speed.cameraForwardSingle) + (this.leftAnchorPosition - base.transform.position)).normalized;
        }
        else if (this.rightHooked)
        {
            //this.hookDirection = (this.rightAnchorPosition - base.transform.position).normalized;
            this.hookDirection = ((this.transforms.playerCamera.transform.forward * this.speed.cameraForwardSingle) + (this.rightAnchorPosition - base.transform.position)).normalized;
        }
        else
        {
            this.hookDirection = Vector3.zero;
        }
    }

    // Token: 0x0600013F RID: 319 RVA: 0x0000D3EC File Offset: 0x0000B5EC
    private void ResetAnchors()
    {
        if (this.leftAnchorPosition != Vector3.zero && !this.leftHooked)
        {
            this.leftAnchorPosition = Vector3.zero;
        }
        if (this.rightAnchorPosition != Vector3.zero && !this.rightHooked)
        {
            this.rightAnchorPosition = Vector3.zero;
        }
    }

    // Token: 0x06000140 RID: 320 RVA: 0x0000D450 File Offset: 0x0000B650
    private void CacheVelocityVariables()
    {
        Vector3 velocity = this.rigidBody.velocity;
        Vector3 a = Common.RemoveYComponent(velocity);
        this.velocityMagnitude = velocity.magnitude;
        this.velocityMagnitudeXZ = a.magnitude;
        if (this.velocityMagnitude == 0f)
        {
            this.velocityDirection = Vector3.zero;
        }
        else
        {
            this.velocityDirection = velocity / this.velocityMagnitude;
        }
        if (this.velocityMagnitudeXZ == 0f)
        {
            this.velocityDirectionXZ = Vector3.zero;
        }
        else
        {
            this.velocityDirectionXZ = a / this.velocityMagnitudeXZ;
        }
        this.velocityY = velocity.y;
        this.velocityYAbs = Mathf.Abs(velocity.y);
    }

    // Token: 0x06000141 RID: 321 RVA: 0x0000D510 File Offset: 0x0000B710
    public void Jump(string type)
    {
        Vector3 a = Vector3.up;
        float d = this.speed.runJump;
        if (type == "idle")
        {
            d = this.speed.idleJump;
            if (this.IsEitherHooked)
            {
                return;
            }
        }
        a = a.normalized;
        this.rigidBody.AddForce(a * 10f * d * Time.fixedDeltaTime, ForceMode.VelocityChange);
        this.jumping = false;
    }

    // Token: 0x06000142 RID: 322 RVA: 0x0000D590 File Offset: 0x0000B790
    public Vector3 HookedSteerDirection()
    {
        Vector3 a = Vector3.zero;
        float num = this.MovementX;
        if (num > 0f)
        {
            a = base.transform.right;
        }
        else if (num < 0f)
        {
            a = -base.transform.right;
        }
        if (this.IsDoubleHooked)
        {
            return (0.15f * a + this.HookDirection).normalized;
        }
        return (0.21f * a + this.HookDirection).normalized;
    }

    // Token: 0x06000143 RID: 323 RVA: 0x0000D62C File Offset: 0x0000B82C
    public void InstantGasConsumption(int type)
    {
        float amount = 0f;
        if (type == 0)
        {
            amount = this.gas.costHookFire;
        }
        if (type == 1)
        {
            amount = this.gas.costHorzBurst;
        }
        if (type == 2)
        {
            amount = this.gas.costHorzBurst * 2f;
        }
        this.gasScript.ExpendGas(amount);
    }

    // Token: 0x06000144 RID: 324 RVA: 0x0000D689 File Offset: 0x0000B889
    public void ResetScripts()
    {
        this.DisableScripts();
        this.EnableScripts();
    }

    // Token: 0x06000145 RID: 325 RVA: 0x0000D698 File Offset: 0x0000B898
    public void DisableScripts()
    {
        this.moveScript.Disable();
        this.moveScript.enabled = false;
        this.rotScript.Disable();
        this.rotScript.enabled = false;
        this.burstScript.Disable();
        this.burstScript.enabled = false;
        this.actionScript.Disable();
        this.actionScript.enabled = false;
        this.wallScript.Disable();
        this.wallScript.enabled = false;
        this.hudScript.Disable();
        this.hudScript.enabled = false;
    }

    // Token: 0x06000146 RID: 326 RVA: 0x0000D730 File Offset: 0x0000B930
    public void EnableScripts()
    {
        this.moveScript.Enable();
        this.moveScript.enabled = true;
        this.rotScript.Enable();
        this.rotScript.enabled = true;
        this.burstScript.Enable();
        this.burstScript.enabled = true;
        this.actionScript.Enable();
        this.actionScript.enabled = true;
        this.wallScript.Enable();
        this.wallScript.enabled = true;
        this.hudScript.Enable();
        this.hudScript.enabled = true;
    }

    // Token: 0x06000147 RID: 327 RVA: 0x0000D7C8 File Offset: 0x0000B9C8
    public void GrabPlayer(Transform grabHand)
    {
        GameObject gameObject = Common.GetRootObject(grabHand).gameObject;
        //TitanMain component = gameObject.GetComponent<TitanMain>();
        //component.GrabSuccess = true;
        //component.GrabbedPlayer = base.gameObject;
        base.transform.parent = grabHand;
        base.transform.localPosition = Vector3.zero;
        base.transform.localRotation = Quaternion.identity;
        base.transform.localScale = Vector3.one;
        this.rigidBody.velocity = Vector3.zero;
        this.rigidBody.isKinematic = true;
        this.soundScript.SwordSound("stop");
        this.DisableScripts();
        this.grabbed = true;
        this.actionScript.StartDamageEffect();
        this.actionScript.StartGrabEscapeAttempt();
        //this.camScript.StartGrabZoom();
    }

    // Token: 0x06000148 RID: 328 RVA: 0x0000D894 File Offset: 0x0000BA94
    public void ReleasePlayer()
    {
        GameObject gameObject = Common.GetRootObject(base.transform.parent).gameObject;
        //gameObject.GetComponent<TitanMain>().ReleaseGrabbedPlayer();
        base.transform.parent = null;
        this.rigidBody.isKinematic = false;
        //this.camScript.ResetFollowDistance();
        this.EnableScripts();
        this.actionScript.StopDamageEffect();
        this.grabbed = false;
    }

    // Token: 0x06000149 RID: 329 RVA: 0x0000D900 File Offset: 0x0000BB00
    public void KillPlayer(bool useVFX, Vector3 direction, Vector3 origin = default(Vector3))
    {
        if (useVFX)
        {
            Vector3 position = (!(origin == Vector3.zero)) ? origin : base.transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            UnityEngine.Object.Instantiate<ParticleSystem>(this.particles.deathVFX, position, rotation);
        }
        //GameObject.FindWithTag("GameController").GetComponent<GameBase>().RemovePlayer(base.gameObject);
        UnityEngine.Object.Destroy(this.transforms.playerUI.gameObject);
        UnityEngine.Object.Destroy(this.transforms.playerCamera.gameObject.GetComponent<CameraControl>());
        //FreeCameraControl freeCameraControl = this.transforms.playerCamera.gameObject.AddComponent<FreeCameraControl>();
        //freeCameraControl.axisNames = this.axisName;
        UnityEngine.Object.Destroy(base.gameObject);
    }

    // Token: 0x0600014A RID: 330 RVA: 0x0000D9C5 File Offset: 0x0000BBC5
    public void ReleaseAttack()
    {
        this.IsAttacking = false;
        this.animator.SetBool("AttackReleased", true);
        this.animator.SetBool("AttackPrimed", false);
    }

    // Token: 0x0600014B RID: 331 RVA: 0x0000D9F0 File Offset: 0x0000BBF0
    public void AttackSuccess()
    {
    }

    // Token: 0x0600014C RID: 332 RVA: 0x0000D9F4 File Offset: 0x0000BBF4
    private void EnforceMaxSpeeds()
    {
        if (this.VelocityYAbsolute > this.limit.maxVertical)
        {
            if (this.VelocityYSigned < 0f)
            {
                this.rigidBody.velocity = new Vector3(this.rigidBody.velocity.x, -this.limit.maxVertical, this.rigidBody.velocity.z);
            }
            else
            {
                this.rigidBody.velocity = new Vector3(this.rigidBody.velocity.x, this.limit.maxVertical, this.rigidBody.velocity.z);
            }
        }
        if (this.VelocityMagnitudeXZ > this.limit.maxHorizontal)
        {
            Vector3 b = new Vector3(0f, this.rigidBody.velocity.y, 0f);
            this.rigidBody.velocity = this.VelocityDirectionXZ * this.limit.maxHorizontal + b;
        }
    }

    // Token: 0x0600014D RID: 333 RVA: 0x0000DB14 File Offset: 0x0000BD14
    private void VelocityProjection()
    {
        if (this.BurstForceIsRunning)
        {
            return;
        }
        if (this.IsEitherHooked && this.JumpReelKeyDown)
        {
            Vector3 onNormal = this.HookedSteerDirection();
            float num = Vector3.Angle(new Vector3(this.rigidBody.velocity.x, 0f, this.rigidBody.velocity.z), new Vector3(onNormal.x, 0f, onNormal.z));
            if (num < 90f)
            {
                num = 90f;
            }
            num -= 90f;
            float num2 = num / 90f;
            Vector3 vector = Vector3.Project(this.rigidBody.velocity, onNormal);
            Vector3 vector2 = this.rigidBody.velocity - vector;
            float num3;
            float num4;
            if (this.IsDoubleHooked)
            {
                num3 = 0.92f + 0.07999998f * num2;
                num4 = 0.925f;
            }
            else
            {
                num3 = 0.97f + 0.0299999714f * num2;
                num4 = 0.97f;
            }
            vector2 = new Vector3(vector2.x * num3, vector2.y * num4, vector2.z * num3);
            vector2 = Vector3.RotateTowards(vector2, vector, 0.00196349551f, 0f);
            Vector3 velocity = vector2 + vector;
            this.rigidBody.velocity = velocity;
        }
    }

    // Token: 0x17000016 RID: 22
    // (get) Token: 0x0600014E RID: 334 RVA: 0x0000DC6D File Offset: 0x0000BE6D
    public bool JumpReelKeyDown
    {
        get
        {
            return this.jumpReelKeyDown;
        }
        set
        {
            this.jumpReelKeyDown = value;
        }
    }

    public bool JumpKeyDown
    {
        get
        {
            return this.jumpKeyDown;
        }
    }
    public bool AttackKeyDown
    {
        get
        {
            return this.attackKeyDown;
        }
    }


    public bool FastSpeed
    {
        get
        {
            return this.fastSpeed;
        }
    }

    // Token: 0x17000017 RID: 23
    // (get) Token: 0x0600014F RID: 335 RVA: 0x0000DC75 File Offset: 0x0000BE75
    public bool CenterHookKeyDown
    {
        get
        {
            return this.centerHookKeyDown;
        }
        set
        {
            this.centerHookKeyDown = value;
        }
    }

    // Token: 0x17000018 RID: 24
    // (get) Token: 0x06000150 RID: 336 RVA: 0x0000DC7D File Offset: 0x0000BE7D
    public bool FireLeftHook
    {
        get
        {
            return this.fireLeftHook;
        }
        set
        {
            this.fireLeftHook = value;
        }
    }

    // Token: 0x17000019 RID: 25
    // (get) Token: 0x06000151 RID: 337 RVA: 0x0000DC85 File Offset: 0x0000BE85
    public bool FireRightHook
    {
        get
        {
            return this.fireRightHook;
        }
        set
        {
            this.fireRightHook = value;
        }
    }

    // Token: 0x1700001A RID: 26
    // (get) Token: 0x06000152 RID: 338 RVA: 0x0000DC8D File Offset: 0x0000BE8D
    public float MovementX
    {
        get
        {
            return this.movementX;
        }
        set
        {
            this.movementX = value;
        }

    }

    // Token: 0x1700001B RID: 27
    // (get) Token: 0x06000153 RID: 339 RVA: 0x0000DC95 File Offset: 0x0000BE95
    public float MovementY
    {
        get
        {
            return this.movementY;
        }
        set
        {
            this.movementY = value;
        }
    }

    // Token: 0x1700001C RID: 28
    // (get) Token: 0x06000154 RID: 340 RVA: 0x0000DC9D File Offset: 0x0000BE9D
    //public float MouseAxisX
    //{
    //    get
    //    {
    //        return this.mouseAxisX;
    //    }
    //}

    //// Token: 0x1700001D RID: 29
    //// (get) Token: 0x06000155 RID: 341 RVA: 0x0000DCA5 File Offset: 0x0000BEA5
    //public float MouseAxisY
    //{
    //    get
    //    {
    //        return this.mouseAxisY;
    //    }
    //}

    // Token: 0x1700001E RID: 30
    // (get) Token: 0x06000156 RID: 342 RVA: 0x0000DCAD File Offset: 0x0000BEAD
    public bool IsGrounded
    {
        get
        {
            return this.isGrounded;
        }
    }

    // Token: 0x1700001F RID: 31
    // (get) Token: 0x06000157 RID: 343 RVA: 0x0000DCB5 File Offset: 0x0000BEB5
    public bool IsSlopedTerrain
    {
        get
        {
            return this.slopedTerrain;
        }
    }

    // Token: 0x17000020 RID: 32
    // (get) Token: 0x06000158 RID: 344 RVA: 0x0000DCBD File Offset: 0x0000BEBD
    public bool IsJumping
    {
        get
        {
            return this.jumping;
        }
    }

    // Token: 0x17000021 RID: 33
    // (get) Token: 0x06000159 RID: 345 RVA: 0x0000DCC5 File Offset: 0x0000BEC5
    public bool IsGroundTakeOff
    {
        get
        {
            return this.groundTakeOff;
        }
    }

    // Token: 0x17000022 RID: 34
    // (get) Token: 0x0600015A RID: 346 RVA: 0x0000DCCD File Offset: 0x0000BECD
    public bool IsMoving
    {
        get
        {
            return this.movementX != 0f || this.movementY != 0f;
        }
    }

    // Token: 0x17000023 RID: 35
    // (get) Token: 0x0600015B RID: 347 RVA: 0x0000DCF2 File Offset: 0x0000BEF2
    // (set) Token: 0x0600015C RID: 348 RVA: 0x0000DCFA File Offset: 0x0000BEFA
    public bool IsSliding
    {
        get
        {
            return this.sliding;
        }
        set
        {
            this.sliding = value;
        }
    }

    // Token: 0x17000024 RID: 36
    // (get) Token: 0x0600015D RID: 349 RVA: 0x0000DD03 File Offset: 0x0000BF03
    // (set) Token: 0x0600015E RID: 350 RVA: 0x0000DD0B File Offset: 0x0000BF0B
    public bool IsAttacking
    {
        get
        {
            return this.attacking;
        }
        set
        {
            this.attacking = value;
        }
    }

    // Token: 0x17000025 RID: 37
    // (get) Token: 0x0600015F RID: 351 RVA: 0x0000DD14 File Offset: 0x0000BF14
    // (set) Token: 0x06000160 RID: 352 RVA: 0x0000DD1C File Offset: 0x0000BF1C
    public bool IsAttackReadied
    {
        get
        {
            return this.attackReadied;
        }
        set
        {
            this.attackReadied = value;
        }
    }

    // Token: 0x17000026 RID: 38
    // (get) Token: 0x06000161 RID: 353 RVA: 0x0000DD25 File Offset: 0x0000BF25
    public bool IsGrabbed
    {
        get
        {
            return this.grabbed;
        }
    }

    // Token: 0x17000027 RID: 39
    // (get) Token: 0x06000162 RID: 354 RVA: 0x0000DD2D File Offset: 0x0000BF2D
    // (set) Token: 0x06000163 RID: 355 RVA: 0x0000DD35 File Offset: 0x0000BF35
    public bool IsEscapingGrab
    {
        get
        {
            return this.escaping;
        }
        set
        {
            this.escaping = value;
        }
    }

    // Token: 0x17000028 RID: 40
    // (get) Token: 0x06000164 RID: 356 RVA: 0x0000DD3E File Offset: 0x0000BF3E
    // (set) Token: 0x06000165 RID: 357 RVA: 0x0000DD46 File Offset: 0x0000BF46
    public bool IsGasVFX
    {
        get
        {
            return this.gasEffectActive;
        }
        set
        {
            this.gasEffectActive = value;
        }
    }

    // Token: 0x17000029 RID: 41
    // (get) Token: 0x06000166 RID: 358 RVA: 0x0000DD4F File Offset: 0x0000BF4F
    public bool IsStationary
    {
        get
        {
            return this.stationary;
        }
    }

    // Token: 0x1700002A RID: 42
    // (get) Token: 0x06000167 RID: 359 RVA: 0x0000DD57 File Offset: 0x0000BF57
    // (set) Token: 0x06000168 RID: 360 RVA: 0x0000DD5F File Offset: 0x0000BF5F
    public bool IsAnimating
    {
        get
        {
            return this.animating;
        }
        set
        {
            this.animating = value;
        }
    }

    // Token: 0x1700002B RID: 43
    // (get) Token: 0x06000169 RID: 361 RVA: 0x0000DD68 File Offset: 0x0000BF68
    // (set) Token: 0x0600016A RID: 362 RVA: 0x0000DD70 File Offset: 0x0000BF70
    public bool IsLeftHooked
    {
        get
        {
            return this.leftHooked && !isLeftHookTargetNull;
        }
        set
        {
            this.leftHooked = value;
        }
    }

    // Token: 0x1700002C RID: 44
    // (get) Token: 0x0600016B RID: 363 RVA: 0x0000DD79 File Offset: 0x0000BF79
    // (set) Token: 0x0600016C RID: 364 RVA: 0x0000DD81 File Offset: 0x0000BF81
    public bool IsRightHooked
    {
        get
        {
            return this.rightHooked && !isRightHookTargetNull;
        }
        set
        {
            this.rightHooked = value;
        }
    }

    // Token: 0x1700002D RID: 45
    // (get) Token: 0x0600016D RID: 365 RVA: 0x0000DD8A File Offset: 0x0000BF8A
    public bool IsDoubleHooked
    {
        get
        {
            return this.IsLeftHooked && this.IsRightHooked && !isLeftHookTargetNull && !isRightHookTargetNull;
        }
    }

    // Token: 0x1700002E RID: 46
    // (get) Token: 0x0600016E RID: 366 RVA: 0x0000DDA0 File Offset: 0x0000BFA0
    public bool IsEitherHooked
    {
        get
        {
            return (this.IsLeftHooked || this.IsRightHooked) && (!isLeftHookTargetNull || !isRightHookTargetNull);
        }
    }

    // Token: 0x1700002F RID: 47
    // (get) Token: 0x0600016F RID: 367 RVA: 0x0000DDB6 File Offset: 0x0000BFB6
    // (set) Token: 0x06000170 RID: 368 RVA: 0x0000DDBE File Offset: 0x0000BFBE
    public bool IsLeftImpulse
    {
        get
        {
            return this.leftImpulse;
        }
        set
        {
            this.leftImpulse = value;
        }
    }

    // Token: 0x17000030 RID: 48
    // (get) Token: 0x06000171 RID: 369 RVA: 0x0000DDC7 File Offset: 0x0000BFC7
    // (set) Token: 0x06000172 RID: 370 RVA: 0x0000DDCF File Offset: 0x0000BFCF
    public bool IsRightImpulse
    {
        get
        {
            return this.rightImpulse;
        }
        set
        {
            this.rightImpulse = value;
        }
    }

    // Token: 0x17000031 RID: 49
    // (get) Token: 0x06000173 RID: 371 RVA: 0x0000DDD8 File Offset: 0x0000BFD8
    // (set) Token: 0x06000174 RID: 372 RVA: 0x0000DDE0 File Offset: 0x0000BFE0
    public bool IsLeftTargetSet
    {
        get
        {
            return this.leftTargetSet;
        }
        set
        {
            this.leftTargetSet = value;
        }
    }

    // Token: 0x17000032 RID: 50
    // (get) Token: 0x06000175 RID: 373 RVA: 0x0000DDE9 File Offset: 0x0000BFE9
    // (set) Token: 0x06000176 RID: 374 RVA: 0x0000DDF1 File Offset: 0x0000BFF1
    public bool IsRightTargetSet
    {
        get
        {
            return this.rightTargetSet;
        }
        set
        {
            this.rightTargetSet = value;
        }
    }

    // Token: 0x17000033 RID: 51
    // (get) Token: 0x06000177 RID: 375 RVA: 0x0000DDFA File Offset: 0x0000BFFA
    public int PhysicsState
    {
        get
        {
            return this.physicsState;
        }
    }

    // Token: 0x17000034 RID: 52
    // (get) Token: 0x06000178 RID: 376 RVA: 0x0000DE02 File Offset: 0x0000C002
    // (set) Token: 0x06000179 RID: 377 RVA: 0x0000DE0A File Offset: 0x0000C00A
    public int ReeledState
    {
        get
        {
            return this.reeledState;
        }
        set
        {
            this.reeledState = value;
        }
    }

    // Token: 0x17000035 RID: 53
    // (get) Token: 0x0600017A RID: 378 RVA: 0x0000DE13 File Offset: 0x0000C013
    // (set) Token: 0x0600017B RID: 379 RVA: 0x0000DE1B File Offset: 0x0000C01B
    public int WalledState
    {
        get
        {
            return this.walledState;
        }
        set
        {
            this.walledState = value;
        }
    }

    // Token: 0x17000036 RID: 54
    // (get) Token: 0x0600017C RID: 380 RVA: 0x0000DE24 File Offset: 0x0000C024
    // (set) Token: 0x0600017D RID: 381 RVA: 0x0000DE2C File Offset: 0x0000C02C
    public int SlideState
    {
        get
        {
            return this.slideState;
        }
        set
        {
            this.slideState = value;
        }
    }

    // Token: 0x17000037 RID: 55
    // (get) Token: 0x0600017E RID: 382 RVA: 0x0000DE35 File Offset: 0x0000C035
    public float Height
    {
        get
        {
            return Mathf.Round(this.height * 100f) / 100f;
        }
    }

    // Token: 0x17000038 RID: 56
    // (get) Token: 0x0600017F RID: 383 RVA: 0x0000DE4E File Offset: 0x0000C04E
    public Vector3 PositionNextFrame
    {
        get
        {
            return this.positionNextFrame;
        }
    }

    // Token: 0x17000039 RID: 57
    // (get) Token: 0x06000180 RID: 384 RVA: 0x0000DE56 File Offset: 0x0000C056
    public Vector3 VelocityLastFrame
    {
        get
        {
            return this.velocityLastFrame;
        }
    }

    // Token: 0x1700003A RID: 58
    // (get) Token: 0x06000181 RID: 385 RVA: 0x0000DE60 File Offset: 0x0000C060
    public float MaxHookDistance
    {
        get
        {
            Vector3 vector = new Vector3(this.rigidBody.velocity.x, 0f, this.rigidBody.velocity.z);
            Vector3 onNormal = new Vector3(this.transforms.playerCamera.forward.x, 0f, this.transforms.playerCamera.forward.z);
            float magnitude = Vector3.Project(vector, onNormal).magnitude;
            return this.limit.originalHookRange + magnitude / 2f;
        }
    }

    // Token: 0x1700003B RID: 59
    // (get) Token: 0x06000182 RID: 386 RVA: 0x0000DF03 File Offset: 0x0000C103
    // (set) Token: 0x06000183 RID: 387 RVA: 0x0000DF0B File Offset: 0x0000C10B
    public float MaxLengthLeft
    {
        get
        {
            return this.maxLengthLeft;
        }
        set
        {
            this.maxLengthLeft = value;
        }
    }

    // Token: 0x1700003C RID: 60
    // (get) Token: 0x06000184 RID: 388 RVA: 0x0000DF14 File Offset: 0x0000C114
    // (set) Token: 0x06000185 RID: 389 RVA: 0x0000DF1C File Offset: 0x0000C11C
    public float MaxLengthRight
    {
        get
        {
            return this.maxLengthRight;
        }
        set
        {
            this.maxLengthRight = value;
        }
    }

    // Token: 0x1700003D RID: 61
    // (get) Token: 0x06000186 RID: 390 RVA: 0x0000DF25 File Offset: 0x0000C125
    // (set) Token: 0x06000187 RID: 391 RVA: 0x0000DF2D File Offset: 0x0000C12D
    public float CurrentLengthLeft
    {
        get
        {
            return this.currentLengthLeft;
        }
        set
        {
            this.currentLengthLeft = value;
        }
    }

    // Token: 0x1700003E RID: 62
    // (get) Token: 0x06000188 RID: 392 RVA: 0x0000DF36 File Offset: 0x0000C136
    // (set) Token: 0x06000189 RID: 393 RVA: 0x0000DF3E File Offset: 0x0000C13E
    public float CurrentLengthRight
    {
        get
        {
            return this.currentLengthRight;
        }
        set
        {
            this.currentLengthRight = value;
        }
    }

    // Token: 0x1700003F RID: 63
    // (get) Token: 0x0600018A RID: 394 RVA: 0x0000DF47 File Offset: 0x0000C147
    public Vector3 HookDirection
    {
        get
        {
            return this.hookDirection;
        }
    }

    // Token: 0x17000040 RID: 64
    // (get) Token: 0x0600018B RID: 395 RVA: 0x0000DF4F File Offset: 0x0000C14F
    // (set) Token: 0x0600018C RID: 396 RVA: 0x0000DF57 File Offset: 0x0000C157
    public Vector3 LeftAnchorPosition
    {
        get
        {
            return this.leftAnchorPosition;
        }
        set
        {
            this.leftAnchorPosition = value;
        }
    }

    // Token: 0x17000041 RID: 65
    // (get) Token: 0x0600018D RID: 397 RVA: 0x0000DF60 File Offset: 0x0000C160
    // (set) Token: 0x0600018E RID: 398 RVA: 0x0000DF68 File Offset: 0x0000C168
    public Vector3 RightAnchorPosition
    {
        get
        {
            return this.rightAnchorPosition;
        }
        set
        {
            this.rightAnchorPosition = value;
        }
    }

    // Token: 0x17000042 RID: 66
    // (get) Token: 0x0600018F RID: 399 RVA: 0x0000DF71 File Offset: 0x0000C171
    // (set) Token: 0x06000190 RID: 400 RVA: 0x0000DF79 File Offset: 0x0000C179
    public Vector3 LeftTargetPosition
    {
        get
        {
            return this.leftTargetPosition;
        }
        set
        {
            this.leftTargetPosition = value;
        }
    }

    // Token: 0x17000043 RID: 67
    // (get) Token: 0x06000191 RID: 401 RVA: 0x0000DF82 File Offset: 0x0000C182
    // (set) Token: 0x06000192 RID: 402 RVA: 0x0000DF8A File Offset: 0x0000C18A
    public Vector3 RightTargetPosition
    {
        get
        {
            return this.rightTargetPosition;
        }
        set
        {
            this.rightTargetPosition = value;
        }
    }

    // Token: 0x17000044 RID: 68
    // (get) Token: 0x06000193 RID: 403 RVA: 0x0000DF93 File Offset: 0x0000C193
    // (set) Token: 0x06000194 RID: 404 RVA: 0x0000DF9B File Offset: 0x0000C19B
    public GameObject CurrentLeftHook
    {
        get
        {
            return this.currentLeftHook;
        }
        set
        {
            this.currentLeftHook = value;
        }
    }

    // Token: 0x17000045 RID: 69
    // (get) Token: 0x06000195 RID: 405 RVA: 0x0000DFA4 File Offset: 0x0000C1A4
    // (set) Token: 0x06000196 RID: 406 RVA: 0x0000DFAC File Offset: 0x0000C1AC
    public GameObject CurrentRightHook
    {
        get
        {
            return this.currentRightHook;
        }
        set
        {
            this.currentRightHook = value;
        }
    }

    // Token: 0x17000046 RID: 70
    // (get) Token: 0x06000197 RID: 407 RVA: 0x0000DFB5 File Offset: 0x0000C1B5
    // (set) Token: 0x06000198 RID: 408 RVA: 0x0000DFBD File Offset: 0x0000C1BD
    public GameObject LeftHookedTitan
    {
        get
        {
            return this.leftHookedTitan;
        }
        set
        {
            this.leftHookedTitan = value;
        }
    }

    // Token: 0x17000047 RID: 71
    // (get) Token: 0x06000199 RID: 409 RVA: 0x0000DFC6 File Offset: 0x0000C1C6
    // (set) Token: 0x0600019A RID: 410 RVA: 0x0000DFCE File Offset: 0x0000C1CE
    public GameObject RightHookedTitan
    {
        get
        {
            return this.rightHookedTitan;
        }
        set
        {
            this.rightHookedTitan = value;
        }
    }

    // Token: 0x17000048 RID: 72
    // (get) Token: 0x0600019B RID: 411 RVA: 0x0000DFD7 File Offset: 0x0000C1D7
    // (set) Token: 0x0600019C RID: 412 RVA: 0x0000DFDF File Offset: 0x0000C1DF
    public bool TitanAimLock
    {
        get
        {
            return this.titanAimLock;
        }
        set
        {
            this.titanAimLock = value;
        }
    }

    // Token: 0x17000049 RID: 73
    // (get) Token: 0x0600019D RID: 413 RVA: 0x0000DFE8 File Offset: 0x0000C1E8
    // (set) Token: 0x0600019E RID: 414 RVA: 0x0000DFF0 File Offset: 0x0000C1F0
    public Quaternion BurstStartRotation
    {
        get
        {
            return this.burstStartRotation;
        }
        set
        {
            this.burstStartRotation = value;
        }
    }

    // Token: 0x1700004A RID: 74
    // (get) Token: 0x0600019F RID: 415 RVA: 0x0000DFF9 File Offset: 0x0000C1F9
    // (set) Token: 0x060001A0 RID: 416 RVA: 0x0000E001 File Offset: 0x0000C201
    public Quaternion BurstEndRotation
    {
        get
        {
            return this.burstEndRotation;
        }
        set
        {
            this.burstEndRotation = value;
        }
    }

    // Token: 0x1700004B RID: 75
    // (get) Token: 0x060001A1 RID: 417 RVA: 0x0000E00A File Offset: 0x0000C20A
    // (set) Token: 0x060001A2 RID: 418 RVA: 0x0000E012 File Offset: 0x0000C212
    public bool BurstTurnIsRunning
    {
        get
        {
            return this.burstTurnIsRunning;
        }
        set
        {
            this.burstTurnIsRunning = value;
        }
    }

    // Token: 0x1700004C RID: 76
    // (get) Token: 0x060001A3 RID: 419 RVA: 0x0000E01B File Offset: 0x0000C21B
    // (set) Token: 0x060001A4 RID: 420 RVA: 0x0000E023 File Offset: 0x0000C223
    public bool BurstForceIsRunning
    {
        get
        {
            return this.burstForceIsRunning;
        }
        set
        {
            this.burstForceIsRunning = value;
        }
    }

    // Token: 0x1700004D RID: 77
    // (get) Token: 0x060001A5 RID: 421 RVA: 0x0000E02C File Offset: 0x0000C22C
    // (set) Token: 0x060001A6 RID: 422 RVA: 0x0000E034 File Offset: 0x0000C234
    public float BurstStartTime
    {
        get
        {
            return this.burstStartTime;
        }
        set
        {
            this.burstStartTime = value;
        }
    }

    // Token: 0x1700004E RID: 78
    // (get) Token: 0x060001A7 RID: 423 RVA: 0x0000E03D File Offset: 0x0000C23D
    // (set) Token: 0x060001A8 RID: 424 RVA: 0x0000E045 File Offset: 0x0000C245
    public float CurrentBurstRate
    {
        get
        {
            return this.currentBurstRate;
        }
        set
        {
            this.currentBurstRate = value;
        }
    }

    // Token: 0x1700004F RID: 79
    // (get) Token: 0x060001A9 RID: 425 RVA: 0x0000E04E File Offset: 0x0000C24E
    // (set) Token: 0x060001AA RID: 426 RVA: 0x0000E056 File Offset: 0x0000C256
    public float BurstTankLevel
    {
        get
        {
            return this.burstTankLevel;
        }
        set
        {
            this.burstTankLevel = value;
        }
    }

    // Token: 0x17000050 RID: 80
    // (get) Token: 0x060001AB RID: 427 RVA: 0x0000E05F File Offset: 0x0000C25F
    public float VelocityMagnitude
    {
        get
        {
            return this.velocityMagnitude;
        }
    }

    // Token: 0x17000051 RID: 81
    // (get) Token: 0x060001AC RID: 428 RVA: 0x0000E067 File Offset: 0x0000C267
    public float VelocityMagnitudeXZ
    {
        get
        {
            return this.velocityMagnitudeXZ;
        }
    }

    // Token: 0x17000052 RID: 82
    // (get) Token: 0x060001AD RID: 429 RVA: 0x0000E06F File Offset: 0x0000C26F
    public float VelocityYSigned
    {
        get
        {
            return this.velocityY;
        }
    }

    // Token: 0x17000053 RID: 83
    // (get) Token: 0x060001AE RID: 430 RVA: 0x0000E077 File Offset: 0x0000C277
    public float VelocityYAbsolute
    {
        get
        {
            return this.velocityYAbs;
        }
    }

    // Token: 0x17000054 RID: 84
    // (get) Token: 0x060001AF RID: 431 RVA: 0x0000E07F File Offset: 0x0000C27F
    public Vector3 VelocityDirection
    {
        get
        {
            return this.velocityDirection;
        }
    }

    // Token: 0x17000055 RID: 85
    // (get) Token: 0x060001B0 RID: 432 RVA: 0x0000E087 File Offset: 0x0000C287
    public Vector3 VelocityDirectionXZ
    {
        get
        {
            return this.velocityDirectionXZ;
        }
    }

    // Token: 0x17000056 RID: 86
    // (get) Token: 0x060001B1 RID: 433 RVA: 0x0000E08F File Offset: 0x0000C28F
    public Rigidbody RigidBody
    {
        get
        {
            return this.rigidBody;
        }
    }

    // Token: 0x17000057 RID: 87
    // (get) Token: 0x060001B2 RID: 434 RVA: 0x0000E097 File Offset: 0x0000C297
    public Animator Animator
    {
        get
        {
            return this.animator;
        }
    }

    // Token: 0x17000058 RID: 88
    // (get) Token: 0x060001B3 RID: 435 RVA: 0x0000E09F File Offset: 0x0000C29F
    // (set) Token: 0x060001B4 RID: 436 RVA: 0x0000E0A7 File Offset: 0x0000C2A7
    public GameObject LeftHookTitanObject
    {
        get
        {
            return this.leftTitanObject;
        }
        set
        {
            this.leftTitanObject = value;
        }
    }

    // Token: 0x17000059 RID: 89
    // (get) Token: 0x060001B5 RID: 437 RVA: 0x0000E0B0 File Offset: 0x0000C2B0
    // (set) Token: 0x060001B6 RID: 438 RVA: 0x0000E0B8 File Offset: 0x0000C2B8
    public GameObject RightHookTitanObject
    {
        get
        {
            return this.rightTitanObject;
        }
        set
        {
            this.rightTitanObject = value;
        }
    }

    // Token: 0x1700005A RID: 90
    // (get) Token: 0x060001B7 RID: 439 RVA: 0x0000E0C1 File Offset: 0x0000C2C1
    // (set) Token: 0x060001B8 RID: 440 RVA: 0x0000E0C9 File Offset: 0x0000C2C9
    public string LeftHookTargetTag
    {
        get
        {
            return this.leftHookTargetTag;
        }
        set
        {
            this.leftHookTargetTag = value;
        }
    }

    // Token: 0x1700005B RID: 91
    // (get) Token: 0x060001B9 RID: 441 RVA: 0x0000E0D2 File Offset: 0x0000C2D2
    // (set) Token: 0x060001BA RID: 442 RVA: 0x0000E0DA File Offset: 0x0000C2DA
    public string RightHookTargetTag
    {
        get
        {
            return this.rightHookTargetTag;
        }
        set
        {
            this.rightHookTargetTag = value;
        }
    }

    // Token: 0x1700005C RID: 92
    // (get) Token: 0x060001BB RID: 443 RVA: 0x0000E0E3 File Offset: 0x0000C2E3
    public PlayerMovement MovementScript
    {
        get
        {
            return this.moveScript;
        }
    }

    // Token: 0x1700005D RID: 93
    // (get) Token: 0x060001BC RID: 444 RVA: 0x0000E0EB File Offset: 0x0000C2EB
    public PlayerRotations RotationScript
    {
        get
        {
            return this.rotScript;
        }
    }

    // Token: 0x1700005E RID: 94
    // (get) Token: 0x060001BD RID: 445 RVA: 0x0000E0F3 File Offset: 0x0000C2F3
    public PlayerBursts BurstScript
    {
        get
        {
            return this.burstScript;
        }
    }

    // Token: 0x1700005F RID: 95
    // (get) Token: 0x060001BE RID: 446 RVA: 0x0000E0FB File Offset: 0x0000C2FB
    public PlayerActions ActionScript
    {
        get
        {
            return this.actionScript;
        }
    }

    // Token: 0x17000060 RID: 96
    // (get) Token: 0x060001BF RID: 447 RVA: 0x0000E103 File Offset: 0x0000C303
    public WallContactInteractions WallScript
    {
        get
        {
            return this.wallScript;
        }
    }

    // Token: 0x17000061 RID: 97
    // (get) Token: 0x060001C0 RID: 448 RVA: 0x0000E10B File Offset: 0x0000C30B
    public PlayerAnimationController AnimationScript
    {
        get
        {
            return this.animScript;
        }
    }

    // Token: 0x17000062 RID: 98
    // (get) Token: 0x060001C1 RID: 449 RVA: 0x0000E113 File Offset: 0x0000C313
    public PlayerSoundController SoundScript
    {
        get
        {
            return this.soundScript;
        }
    }

    // Token: 0x17000063 RID: 99
    // (get) Token: 0x060001C2 RID: 450 RVA: 0x0000E11B File Offset: 0x0000C31B
    public PlayerParticleEffects EffectsScript
    {
        get
        {
            return this.fxScript;
        }
    }

    // Token: 0x17000064 RID: 100
    // (get) Token: 0x060001C3 RID: 451 RVA: 0x0000E123 File Offset: 0x0000C323
    public DrawPlayerHUD HUDScript
    {
        get
        {
            return this.hudScript;
        }
    }

    // Token: 0x17000065 RID: 101
    // (get) Token: 0x060001C4 RID: 452 RVA: 0x0000E12B File Offset: 0x0000C32B
    //public CameraControl CamScript
    //{
    //    get
    //    {
    //        return this.camScript;
    //    }
    //}

    // Token: 0x17000066 RID: 102
    // (get) Token: 0x060001C5 RID: 453 RVA: 0x0000E133 File Offset: 0x0000C333
    public PlayerGasManagement GasScript
    {
        get
        {
            return this.gasScript;
        }
    }

    public bool IsLeftHookTargetNull
    {
        get
        {
            return this.isLeftHookTargetNull;
        }
        set
        {
            this.isLeftHookTargetNull = value;
        }
    }

    public bool IsRightHookTargetNull
    {
        get
        {
            return this.isRightHookTargetNull;
        }
        set
        {
            this.isRightHookTargetNull = value;
        }
    }

    //public GameBase GameScript
    //{
    //    get
    //    {
    //        return this.gameScript;
    //    }
    //}


    //public HitMessageManager HitMessageScript
    //{
    //    get
    //    {
    //        return this.uiMessageScript;
    //    }
    //}

    private bool isLeftHookTargetNull;

    private bool isRightHookTargetNull;

    // Token: 0x0400018C RID: 396
    public bool jumpReelKeyDown;

    public bool jumpKeyDown;

    private bool attackKeyDown;

    // Token: 0x0400018D RID: 397
    public bool centerHookKeyDown;

    // Token: 0x0400018E RID: 398
    public bool fireLeftHook;

    // Token: 0x0400018F RID: 399
    public bool fireRightHook;

    // Token: 0x04000190 RID: 400
    public float movementX;

    // Token: 0x04000191 RID: 401
    public float movementY;

    //Token: 0x04000192 RID: 402
    public float mouseAxisX;

    //Token: 0x04000193 RID: 403
    public float mouseAxisY;

    private bool fastSpeed;

    // Token: 0x04000194 RID: 404
    public bool isGrounded;

    // Token: 0x04000195 RID: 405
    private bool slopedTerrain;

    // Token: 0x04000196 RID: 406
    private bool jumping;

    // Token: 0x04000197 RID: 407
    private bool groundTakeOff;

    // Token: 0x04000198 RID: 408
    private bool animating;

    // Token: 0x04000199 RID: 409
    private bool stationary;

    // Token: 0x0400019A RID: 410
    private bool sliding;

    // Token: 0x0400019B RID: 411
    private bool attackReadied;

    // Token: 0x0400019C RID: 412
    private bool attacking;

    // Token: 0x0400019D RID: 413
    private bool grabbed;

    // Token: 0x0400019E RID: 414
    private bool escaping;

    // Token: 0x0400019F RID: 415
    private bool gasEffectActive;

    // Token: 0x040001A0 RID: 416
    private bool leftHooked;

    // Token: 0x040001A1 RID: 417
    private bool rightHooked;

    // Token: 0x040001A2 RID: 418
    private bool leftImpulse;

    // Token: 0x040001A3 RID: 419
    private bool rightImpulse;

    // Token: 0x040001A4 RID: 420
    private bool leftTargetSet;

    // Token: 0x040001A5 RID: 421
    private bool rightTargetSet;

    // Token: 0x040001A6 RID: 422
    private int physicsState;

    // Token: 0x040001A7 RID: 423
    private int reeledState;

    // Token: 0x040001A8 RID: 424
    private int walledState;

    // Token: 0x040001A9 RID: 425
    private int slideState;

    // Token: 0x040001AA RID: 426
    private Vector3 positionNextFrame;

    // Token: 0x040001AB RID: 427
    private Vector3 velocityLastFrame;

    // Token: 0x040001AC RID: 428
    private float height;

    // Token: 0x040001AD RID: 429
    private Vector3 hookDirection;

    // Token: 0x040001AE RID: 430
    private Vector3 leftAnchorPosition;

    // Token: 0x040001AF RID: 431
    private Vector3 rightAnchorPosition;

    // Token: 0x040001B0 RID: 432
    private Vector3 leftTargetPosition;

    // Token: 0x040001B1 RID: 433
    private Vector3 rightTargetPosition;

    // Token: 0x040001B2 RID: 434
    private GameObject currentLeftHook;

    // Token: 0x040001B3 RID: 435
    private GameObject currentRightHook;

    // Token: 0x040001B4 RID: 436
    private GameObject leftHookedTitan;

    // Token: 0x040001B5 RID: 437
    private GameObject rightHookedTitan;

    // Token: 0x040001B6 RID: 438
    private float maxLengthLeft;

    // Token: 0x040001B7 RID: 439
    private float maxLengthRight;

    // Token: 0x040001B8 RID: 440
    private float currentLengthLeft;

    // Token: 0x040001B9 RID: 441
    private float currentLengthRight;

    // Token: 0x040001BA RID: 442
    private bool titanAimLock;

    // Token: 0x040001BB RID: 443
    private float velocityMagnitude;

    // Token: 0x040001BC RID: 444
    private float velocityMagnitudeXZ;

    // Token: 0x040001BD RID: 445
    private float velocityY;

    // Token: 0x040001BE RID: 446
    private float velocityYAbs;

    // Token: 0x040001BF RID: 447
    private Vector3 velocityDirection;

    // Token: 0x040001C0 RID: 448
    private Vector3 velocityDirectionXZ;

    // Token: 0x040001C1 RID: 449
    private Quaternion burstStartRotation;

    // Token: 0x040001C2 RID: 450
    private Quaternion burstEndRotation;

    // Token: 0x040001C3 RID: 451
    private bool burstTurnIsRunning;

    // Token: 0x040001C4 RID: 452
    private bool burstForceIsRunning;

    // Token: 0x040001C5 RID: 453
    private float burstStartTime;

    // Token: 0x040001C6 RID: 454
    private float currentBurstRate;

    // Token: 0x040001C7 RID: 455
    private float burstTankLevel;

    // Token: 0x040001C8 RID: 456
    private GameObject leftTitanObject;

    // Token: 0x040001C9 RID: 457
    private GameObject rightTitanObject;

    // Token: 0x040001CA RID: 458
    private string leftHookTargetTag;

    // Token: 0x040001CB RID: 459
    private string rightHookTargetTag;

    // Token: 0x040001CC RID: 460
    private Rigidbody rigidBody;

    // Token: 0x040001CD RID: 461
    private Animator animator;

    // Token: 0x040001CE RID: 462
    private CapsuleCollider capCollider;

    // Token: 0x040001CF RID: 463
    private CapsuleCollider[] lowerColliders = new CapsuleCollider[4];

    // Token: 0x040001D0 RID: 464
    private PlayerMovement moveScript;

    // Token: 0x040001D1 RID: 465
    private PlayerRotations rotScript;

    // Token: 0x040001D2 RID: 466
    private PlayerBursts burstScript;

    // Token: 0x040001D3 RID: 467
    private PlayerActions actionScript;

    // Token: 0x040001D4 RID: 468
    private WallContactInteractions wallScript;

    // Token: 0x040001D5 RID: 469
    private PlayerGasManagement gasScript;

    // Token: 0x040001D6 RID: 470
    private PlayerAnimationController animScript;

    // Token: 0x040001D7 RID: 471
    private PlayerSoundController soundScript;

    // Token: 0x040001D8 RID: 472
    private PlayerParticleEffects fxScript;

    // Token: 0x040001D9 RID: 473
    private DrawPlayerHUD hudScript;

    // Token: 0x040001DA RID: 474
    private CameraControl camScript;

    // Token: 0x040001DB RID: 475
    //private GameBase gameScript;

    // Token: 0x040001DC RID: 476
    //private HitMessageManager uiMessageScript;

    // Token: 0x040001DD RID: 477
    public InputAxisNames axisName;

    // Token: 0x040001DE RID: 478
    public PlayerSpeeds speed;

    // Token: 0x040001DF RID: 479
    public PlayerLimits limit;

    // Token: 0x040001E0 RID: 480
    public PlayerGas gas;

    // Token: 0x040001E1 RID: 481
    public PlayerTransforms transforms;

    // Token: 0x040001E2 RID: 482
    public PlayerParticles particles;

    // Token: 0x040001E3 RID: 483
    private bool resetImpulse;

    // Token: 0x040001E4 RID: 484
    private const int STATIONARY_LIMIT = 10;

    // Token: 0x040001E5 RID: 485
    private int stationaryCount;

    // Token: 0x040001E6 RID: 486
    private Vector3[] rayOrigins = new Vector3[5];

    // Token: 0x040001E7 RID: 487
    private Ray[] groundRays = new Ray[5];

    // Token: 0x040001E8 RID: 488
    private bool[] rayHits = new bool[5];

    public CameraSettings cameraSettings;

    private State m_State;

    public float m_MouseXaxis;

    public float m_MouseYaxis;

    private LauncherControl[] m_LauncherControls;
}
