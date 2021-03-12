using System;
using System.Collections;
using System.Collections.Generic;
using AI.BT;
using AI.BT.Sensors;
using UnityEngine;

public class BTRunner : MonoBehaviour, IBlackboardProvider, IBehaviorTreeProvider
{
    public Blackboard Blackboard => BehaviorTree.Blackboard;
    public BehaviorTree BehaviorTree => treeClone;
    public BehaviorTree tree;
    public int frequency;

    private float nextActivation;
    private BehaviorTree treeClone;

    // Start is called before the first frame update
    void Awake()
    {
        treeClone = tree.Clone();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextActivation < Time.time)
        {
            treeClone.Run();
            nextActivation = Time.time + 1f / frequency;
        }
    }

}