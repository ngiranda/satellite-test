using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField]
    [Range(0, 360)]
    public float viewAngle;
    [SerializeField]
    private List<DetectionTypes> detecableTypes = new List<DetectionTypes>();

    IEnumerator sendDetectedTargetMessage(float frequencyInSeconds)
    {
        while(true)
        {
            yield return new WaitForSeconds (frequencyInSeconds);
            List<Detectable> detectedTargets = getDetectedTargets();
            Debug.LogWarning("Number of detected targets: " + detectedTargets.Count);
        }
    }
    private List<Detectable> getDetectedTargets()
    {
        List<Detectable> allPossibleDetectables = FindObjectsOfType<Detectable>().ToList<Detectable>();
        List<Detectable> detectedTargets = new List<Detectable>();
        foreach (var detectable in allPossibleDetectables)
        {
            Transform target = detectable.gameObject.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward - transform.position, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, distanceToTarget))
                {
                    if (detectable.waysToDetect.Any(detecableTypes.Contains))
                    {
                        detectedTargets.Add(detectable);
                    }
                }
            }
        }
        return detectedTargets;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(sendDetectedTargetMessage(.2f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
