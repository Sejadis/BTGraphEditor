using System;
using System.Collections;
using System.Collections.Generic;
using AI.BT;
using AI.BT.Sensors;
using UnityEngine;

public class BTRunner : MonoBehaviour, IBlackboardProvider
{
    public BehaviorTree tree;

    public int frequency;

    private float nextActivation;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (nextActivation < Time.time)
        {
            tree.Run();
            nextActivation = Time.time + 1f / frequency * 60;
        }
    }

    public Blackboard Blackboard => tree.Blackboard;
}