using System.Collections;
using System.Collections.Generic;
using AI.BT;
using UnityEngine;

public class BlackboardTester : MonoBehaviour
{
    public BehaviorTree tree;
    public float waitTime;
    public Transform target = null;
    public Blackboard Blackboard;
    // Start is called before the first frame update
    void Start()
    {
        Blackboard = tree.Blackboard;
    }

    // Update is called once per frame
    void Update()
    {
        Blackboard.SetValue("waitTime", waitTime);
        Blackboard.SetValue("target", target);
    }
}
