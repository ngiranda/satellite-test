using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (Detector))]
public class NewBehaviourScript : Editor
{
    private void OnSceneGUI()
    {
        Detector detector = (Detector) target;
        Vector3 viewAngleA = detector.dirFromAngle(-detector.viewAngle / 2, false);
        Vector3 viewAngleB = detector.dirFromAngle(detector.viewAngle / 2, false);

        Handles.DrawLine(detector.transform.position, detector.transform.position + viewAngleA * 1000);
        Handles.DrawLine(detector.transform.position, detector.transform.position + viewAngleB * 1000);
    }
}
