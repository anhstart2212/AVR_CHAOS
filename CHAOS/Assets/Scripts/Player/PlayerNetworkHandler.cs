using Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkHandler : Bolt.EntityBehaviour<IChaos_PlayerState>
{
    [SerializeField]
    [Tooltip("Player Component")]
    private Player player; //Reference to component Player

    [SerializeField]
    [Tooltip("Player Component")]
    private Animator animator;

    private State m_State; // State 

    /// <summary>
    /// Initialize values
    /// </summary>
    private void Awake()
    {
        player = this.GetComponent<Player>();
        animator = this.GetComponent<Animator>();

        m_State = new State();
        m_State.position = transform.position;
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

        input.MouseX = player.m_MouseXaxis;
        input.MouseY = player.m_MouseYaxis;
        input.MovementX = player.movementX;
        input.MovementY = player.movementY;
        input.Jump = player.jumpKeyDown;
        input.CenterHook = player.centerHookKeyDown;
        input.FireLeftHook = player.fireLeftHook;
        input.FireRightHook = player.fireRightHook;
        input.JumpReel = player.jumpReelKeyDown;

        input.CamPos = player.cameraSettings.CinemachineBrain.position;
        input.CamRot = player.transforms.playerCamera.rotation;

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
            SetState(cmd.Result.Position, cmd.Result.Rotation);
        }
        else
        {
            // apply movement(this runs on both server and client)
            State result = player.Apply(cmd.Input.MovementX, cmd.Input.MovementY, cmd.Input.MouseX, cmd.Input.Jump, cmd.Input.FireLeftHook, cmd.Input.FireRightHook, cmd.Input.JumpReel);

            // copy the state to the commands result (this gets sent back to the client)
            cmd.Result.Position = result.position;
            cmd.Result.Rotation = result.rotation;

            state.CamPos = cmd.Input.CamPos;
            state.CamRot = cmd.Input.CamRot;

            if(cmd.IsFirstExecution)
            {
                // animation run
                player.AnimateRun(cmd);

                //FindHookTarget();
                state.IsCenterHook = cmd.Input.CenterHook;
                state.IsLeftHook = cmd.Input.FireLeftHook;
                state.IsRightHook = cmd.Input.FireRightHook;
                state.IsJump = player.IsJumping;

                if (cmd.Input.CenterHook || cmd.Input.FireLeftHook || cmd.Input.FireRightHook)
                {
                    player.PlayerHook();
                }

                player.AnimateHook();
            }
        }
    }

    public void SetState(Vector3 position, Quaternion rotation)
    {
        // assign new state
        m_State.position = position;
        m_State.rotation = rotation;

        // assign local transform
        transform.position = Vector3.Lerp(transform.position, m_State.position, 3f * Time.deltaTime);
        transform.rotation = m_State.rotation;
    }
}
