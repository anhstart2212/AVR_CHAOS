using Bolt;
using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkHandler : Bolt.EntityEventListener<IChaos_PlayerState>
{
    [SerializeField]
    [Tooltip("Player Component")]
    private Player player; //Reference to component Player

    [SerializeField]
    [Tooltip("Animator Component")]
    private Animator animator;

    [SerializeField]
    [Tooltip("PlayerMovement Component")]
    private PlayerMovement playerMovement; //Reference to component PlayerMovement

    /// <summary>
    /// Initialize values
    /// </summary>
    private void Awake()
    {
        player = this.GetComponent<Player>();
        animator = this.GetComponent<Animator>();
        playerMovement = this.GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Bolt equivalent of Start()
    /// </summary>
    public override void Attached()
    {
        if (player && animator && playerMovement)
        {
            // This couples the Transform property of the State with the GameObject Transform
            state.SetTransforms(state.Transform, player.transform);
            state.SetAnimator(animator);

            // setting layerweights 
            //state.Animator.SetLayerWeight(0, 1);
            //state.Animator.SetLayerWeight(1, 1);

            state.OnPlayerHook += player.OnPlayerHook;

            //state.AddCallback("WeaponId", player.WeaponChanged);
            //state.OnFire += player.OnFire;
            // setup rifle weapon
            //player.WeaponChanged();

            // setup melee weapon
            //state.OnBasicAttack += player.OnBasicAttack;
            //state.OnHeavyAttack += player.OnHeavyAttack;
            //state.OnParry += player.OnParry;

            //state.AddCallback("SlotIndex", player.SlotWeaponChanged);
            // setup slot weapon
            //player.SlotWeaponChanged();

            // attack
            state.OnFire += player.OnFire;

            //state.AddCallback("IsBasicAttack", player.PlayCombo);
            //state.AddCallback("IsHeavyAttack", player.PlayCombo);
        }
        else
        {
            Debug.LogError("Exception! Null Player Or Animator");
        }
    }

    /// <summary>
    /// Bolt equivalent of update()
    /// Only runs on local
    /// </summary>
    public override void SimulateController()
    {
        player.PollKeys();

        player.OwnerController();

        IChaosPlayerCommandInput input = ChaosPlayerCommand.Create();

        input.MouseX = player.MouseXaxis;
        input.MouseY = player.MouseYaxis;
        input.MovementX = player.MovementX;
        input.MovementY = player.MovementY;
        input.Jump = player.JumpKeyDown;
        input.FireCenterHook = player.CenterHookKeyDown;
        input.FireLeftHook = player.FireLeftHook;
        input.FireRightHook = player.FireRightHook;
        input.JumpReel = player.JumpReelKeyDown;
        input.IsFastSpeed = player.FastSpeed;
        input.IsInventory = player.InventoryKey;

        if (CameraSettings.instance != null)
        {
            input.CamPos = CameraSettings.instance.CinemachineBrain.position;
            input.CamRot = CameraSettings.instance.CinemachineBrain.rotation;
        }

        //input.CurrentItemId = player.CurrentItemId;
        input.CurrentFPSId = player.CurrentFPSId;
        input.IsCanAction = player.IsCanAction;
        input.IsMouseLocked = player.IsMouseLocked;
        input.HandEquipId = player.HandEquipId;
        input.Gun1EquipId = player.Gun1EquipId;
        input.Gun2EquipId = player.Gun2EquipId;
        input.BackpackEquipId = player.BackpackEquipId;
        input.ArmorEquipId = player.ArmorEquipId;
        input.MeleeEquipId = player.MeleeEquipId;
        input.HandIndex = player.HandIndex;
        input.Gun1Index = player.Gun1Index;
        input.Gun2Index = player.Gun2Index;
        input.BackpackIndex = player.BackpackIndex;
        input.ArmorIndex = player.ArmorIndex;
        input.MeleeIndex = player.MeleeIndex;
        input.Gun1InUse = player.Gun1InUse;
        input.Gun2InUse = player.Gun2InUse;
        input.MeleeInUse = player.MeleeInUse;
        input.IsFire = player.FireContinuouslyKey;
        input.IsAim = player.AimKey;
        input.MoveSpeed = playerMovement.MoveSpeed;
        input.IsShooter = player.IsShooter;
        input.IsStrafing = player.IsStrafing;

        entity.QueueInput(input);
    }

    /// <summary>
    /// Execute command receives from Bolt
    /// think of this as Execute() from Command Pattern
    /// </summary>
    /// <param name="command"></param>
    /// <param name="resetState"></param>
    public override void ExecuteCommand(Command command, bool resetState)
    {
        if (state.Dead)
        {
            return;
        }

        ChaosPlayerCommand cmd = (ChaosPlayerCommand)command;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            player.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            // apply movement(this runs on both server and client)
            State result = player.Apply(cmd.Input.MovementX, cmd.Input.MovementY);

            // copy the state to the commands result (this gets sent back to the client)
            cmd.Result.Position = result.position;
            cmd.Result.Rotation = result.rotation;

            if (cmd.IsFirstExecution)
            {
                // animation run
                player.AnimatePlayer(cmd);

                state.MovementXKey = cmd.Input.MovementX;
                state.MovementYKey = cmd.Input.MovementY;
                state.MouseXKey = cmd.Input.MouseX;
                state.MouseYKey = cmd.Input.MouseY;
                state.IsFireCenterHookKey = cmd.Input.FireCenterHook;
                state.IsFireLeftHookKey = cmd.Input.FireLeftHook;
                state.IsFireRightHookKey = cmd.Input.FireRightHook;
                state.IsJumpReelKey = cmd.Input.JumpReel;
                state.IsJumpKey = cmd.Input.Jump;
                state.FastSpeedKey = cmd.Input.IsFastSpeed;
                state.IsJump = player.IsJumping;
                state.IsGrounded = player.IsGrounded;
                state.CamPos = cmd.Input.CamPos;
                state.CamRot = cmd.Input.CamRot;
                //state.CurrentItemId = cmd.Input.CurrentItemId;
                state.CurrentFPSId = cmd.Input.CurrentFPSId;
                state.IsShowInventory = cmd.Input.IsInventory;
                state.IsCanAction = cmd.Input.IsCanAction;
                state.IsMouseLocked = cmd.Input.IsMouseLocked;
                state.HandEquipId = cmd.Input.HandEquipId;
                state.Gun1EquipId = cmd.Input.Gun1EquipId;
                state.Gun2EquipId = cmd.Input.Gun2EquipId;
                state.BackpackEquipId = cmd.Input.BackpackEquipId;
                state.ArmorEquipId = cmd.Input.ArmorEquipId;
                state.MeleeEquipId = cmd.Input.MeleeEquipId;
                state.HandIndex = cmd.Input.HandIndex;
                state.Gun1Index = cmd.Input.Gun1Index;
                state.Gun2Index = cmd.Input.Gun2Index;
                state.BackpackIndex = cmd.Input.BackpackIndex;
                state.ArmorIndex = cmd.Input.ArmorIndex;
                state.MeleeIndex = cmd.Input.MeleeIndex;
                state.Gun1InUse = cmd.Input.Gun1InUse;
                state.Gun2InUse = cmd.Input.Gun2InUse;
                state.MeleeInUse = cmd.Input.MeleeInUse;
                state.IsFire = cmd.Input.IsFire;
                state.IsAim = cmd.Input.IsAim;
                state.MoveSpeed = cmd.Input.MoveSpeed;
                state.IsShooter = cmd.Input.IsShooter;
                state.IsStrafing = cmd.Input.IsStrafing;

                if ((cmd.Input.FireCenterHook || cmd.Input.FireLeftHook || cmd.Input.FireRightHook))
                {
                    player.PlayerHook();
                }

                player.Fire(cmd);

                player.Aim(cmd);
            }
        }
    }
}
