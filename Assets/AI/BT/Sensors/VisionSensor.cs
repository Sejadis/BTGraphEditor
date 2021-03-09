using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AI.BT.Sensors
{
    public class VisionSensor : BTSensor
    {
        public LayerMask visionMask;
        public float range;
        public float angle;
        public float detectionFrequency = 5;
        public bool debug;
        private List<Transform> hits = new List<Transform>();

        private float nextScan;
        
        private void Update()
        {
            if (nextScan < Time.time)
            {
                nextScan = Time.time +  1 / detectionFrequency;
                Scan();
            }
        }

        private void Scan()
        {
            var raycastHits = Physics.OverlapSphere(transform.position, range, visionMask);
            hits.Clear();
            foreach (var hit in raycastHits)
            {
                Vector3 hitDir = (hit.transform.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, hitDir) < angle / 2)
                {
                    hits.Add(hit.transform);
                }
            }
            //ToList() to create a copy, so we can clear it here next time without affecting the blackboard
            WriteToBlackBoard(hits.Count > 0 ? hits[0] : null);
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;
            var trans = transform;
            var position = trans.position;
            var up = trans.up;
            var arcDir =  Quaternion.AngleAxis(-angle / 2, up) * trans.forward;
            Handles.color = new Color(1, 1, 1, 0.3f);
            Handles.DrawWireDisc(position,up,range);
            Handles.DrawSolidArc(position,up,arcDir,angle,range);
            Handles.color = Color.red;
            foreach (var hit in hits)
            {
                Handles.DrawSolidDisc(hit.position,hit.transform.up,0.5f);
            }
        }
        
    }
}