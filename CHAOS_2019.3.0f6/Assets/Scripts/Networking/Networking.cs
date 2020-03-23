using Bolt;
using Gamekit3D;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server,"Scene1")]
public class Networking : Bolt.GlobalEventListener
{
    public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token)
    {
        BoltConsole.Write("ConnectRequest", Color.red);

        if (token != null)
        {
            BoltConsole.Write("Token Received", Color.red);
        }

        BoltNetwork.Accept(endpoint);
    }

    public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
    {
        BoltConsole.Write("ConnectAttempt", Color.red);
        base.ConnectAttempt(endpoint, token);
    }

    public override void Disconnected(BoltConnection connection)
    {
        BoltConsole.Write("Disconnected", Color.red);
        base.Disconnected(connection);
    }

    public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
    {
        BoltConsole.Write("ConnectRefused", Color.red);
        base.ConnectRefused(endpoint, token);
    }

    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        BoltConsole.Write("ConnectFailed", Color.red);
        base.ConnectFailed(endpoint, token);
    }

    public override void Connected(BoltConnection connection)
    {
        BoltConsole.Write("Connected", Color.red);
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        BoltConsole.Write("SceneLoadRemoteDone", Color.red);
    }

    public override void SceneLoadLocalDone(string scene)
    {
        BoltConsole.Write("SceneLoadLocalDone", Color.red);

        //var a = BoltNetwork.Instantiate(BoltPrefabs.Character, new NetworkToken(), new Vector3(0,2,3), Quaternion.identity);
        //a.TakeControl();

        CameraSettings.Instantiate();
    }

    public override void SceneLoadLocalBegin(string scene)
    {
        BoltConsole.Write("SceneLoadLocalBegin", Color.red);
    }

    public override void ControlOfEntityGained(BoltEntity entity)
    {
        print("ControlOfEntityGained");
        BoltConsole.Write("ControlOfEntityGained", Color.blue);
    }
}
