using Bolt;
using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkHandler : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    [SerializeField]
    [Tooltip("Player Component")]
    private Player player; //Reference to component Player

    [SerializeField]
    [Tooltip("Animator Component")]
    private Animator animator;

    /// <summary>
    /// Initialize values
    /// </summary>
    private void Awake()
    {
        player = this.GetComponent<Player>();
        animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// Bolt equivalent of Start()
    /// </summary>
    public override void Attached()
    {
        if (player && animator)
        {
            // This couples the Transform property of the State with the GameObject Transform
            state.SetTransforms(state.Transform, player.transform);
            state.SetAnimator(animator);

            // setting layerweights 
            state.Animator.SetLayerWeight(0, 1);
            state.Animator.SetLayerWeight(1, 1);

            state.OnPlayerHook += player.OnPlayerHook;
        }else
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

        player.DrawOwnerHookTarget();

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
        input.FastSpeed = player.FastSpeed;

        if (CameraSettings.instance != null)
        {
            input.CamPos = CameraSettings.instance.CinemachineBrain.position;
            input.CamRot = CameraSettings.instance.CinemachineBrain.rotation;
        }

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
        ChaosPlayerCommand cmd = (ChaosPlayerCommand)command;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            player.SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            // apply movement(this runs on both server and client)
            State result = player.Apply(cmd.Input.FireLeftHook, cmd.Input.FireRightHook, cmd.Input.JumpReel);

            // copy the state to the commands result (this gets sent back to the client)
            cmd.Result.Position = result.position;
            cmd.Result.Rotation = result.rotation;

            state.CamPos = cmd.Input.CamPos;
            state.CamRot = cmd.Input.CamRot;

            if(cmd.IsFirstExecution)
            {
                // animation run
                player.AnimateRun(cmd);

                state.MovementXKey = cmd.Input.MovementX;
                state.MovementYKey = cmd.Input.MovementY;
                state.MouseXKey = cmd.Input.MouseX;
                state.IsFireCenterHookKey = cmd.Input.FireCenterHook;
                state.IsFireLeftHookKey = cmd.Input.FireLeftHook;
                state.IsFireRightHookKey = cmd.Input.FireRightHook;
                state.IsJumpReelKey = cmd.Input.JumpReel;
                state.IsJumpKey = cmd.Input.Jump;
                state.FastSpeedKey = cmd.Input.FastSpeed;
                state.IsJump = player.IsJumping;
                state.IsGrounded = player.IsGrounded;

                if (cmd.Input.FireCenterHook || cmd.Input.FireLeftHook || cmd.Input.FireRightHook)
                {
                    player.PlayerHook();
                }
            }
        }
    }
}
