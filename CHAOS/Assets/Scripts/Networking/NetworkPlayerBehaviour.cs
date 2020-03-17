using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer2State : IState, IDisposable
{
    float AngularSpeed { get; set; }
    float Speed { get; set; }
    float Shot { get; }
    bool PlayerInSight { get; set; }
    float AimWeight { get; }
    NetworkTransform Transform { get; }
}

public class NetworkPlayerBehaviour : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
