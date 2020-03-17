using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkToken : Bolt.IProtocolToken
{
    static int NumberCounter;
    public int Number = 0;

    public NetworkToken()
    {
        Number = ++NumberCounter;
    }

    void Bolt.IProtocolToken.Read(UdpKit.UdpPacket packet)
    {
        Number = packet.ReadInt();
    }

    void Bolt.IProtocolToken.Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(Number);
    }

    public override string ToString()
    {
        return string.Format("[NetworkToken {0}]", Number);
    }
}
