using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBodyAnimator : MonoBehaviour
{
    public PlaneBody Plane;
    public Transform BodyTransform;

    public float RollAmount;
    public float RollLerpSpeed;

    // Update is called once per frame
    void Update()
    {
        BodyTransform.rotation = Quaternion.Lerp(BodyTransform.rotation, Quaternion.Euler(0f, BodyTransform.rotation.eulerAngles.y, Plane.TurnFactor * -RollAmount), RollLerpSpeed * Time.deltaTime);
    }
}
