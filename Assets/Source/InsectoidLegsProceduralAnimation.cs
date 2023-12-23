using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InsectoidLegsProceduralAnimation : MonoBehaviour
{
    public Transform BaseTransform;
    public LegGroup[] LegGroups;

    public float StepSize;
    public float StepMargin = 0.1f;
    public float StepHeight;
    public float StepDuration;
    public float StepLerp;
    public AnimationCurve StepCurveY;
    public float MinTimeBetweenSteps = 0.1f;

    private Vector3 _velocity;
    private Vector3 _prevPosition;
    private Vector3 _direction;
    private float _lastStepTime;

    void Awake()
    {
        foreach (var group in LegGroups)
        {
            group.BakeBasePositions(BaseTransform);
        }
    }

    void FixedUpdate()
    {
        foreach (var group in LegGroups)
        {
            UpdateLegGroup(group);
        }

        _velocity = (BaseTransform.position - _prevPosition) / 0.02f;
        _prevPosition = BaseTransform.position;
        _direction = _velocity.normalized;
    }

    private void UpdateLegGroup(LegGroup group)
    {
        foreach (var leg in group.Legs)
        {
            if (group.CurrentSimultanious < group.MaxSimultaniousMovements && Time.time > _lastStepTime + MinTimeBetweenSteps && !leg.IsMoving)
            {
                UpdateLeg(leg, group);
            }
        }

        foreach (var leg in group.Legs)
        {
            leg.LegTarget.position = leg.CurrentWorldPosition;
        }
    }

    private void UpdateLeg (Leg leg, LegGroup group)
    {
        if (ShouldMoveLeg(leg))
        {
            _lastStepTime = Time.time;
            Vector3 targetWorldPosition = EstimateLegTargetPosition(BaseTransform.TransformPoint(leg.BaseLocalPosition));
            Debug.DrawRay(targetWorldPosition, Vector3.up, Color.red, StepDuration);
            StartCoroutine(MoveLeg(leg, group, targetWorldPosition));
        }
    }

    private bool ShouldMoveLeg(Leg leg)
        => Vector3.Distance(leg.CurrentWorldPosition, BaseTransform.TransformPoint(leg.BaseLocalPosition)) > (StepSize + StepMargin);

    private Vector3 EstimateLegTargetPosition(Vector3 baseWorldPosition)
        => baseWorldPosition + _direction * StepSize;

    private IEnumerator MoveLeg(Leg leg, LegGroup group, Vector3 targetPosition)
    {
        leg.IsMoving = true;
        group.CurrentSimultanious++;

        Vector3 startPosition = leg.CurrentWorldPosition;
        Vector3 targetLocalPosition = BaseTransform.InverseTransformPoint(targetPosition);

        int ticks = Mathf.RoundToInt(StepDuration * 50);
        for (int i = 0; i < ticks; i++)
        {
            float progress = ((float)i / ticks);
            leg.CurrentWorldPosition = ComputeLegPosition(startPosition, BaseTransform.TransformPoint(targetLocalPosition), progress);
            yield return new WaitForFixedUpdate();
        }
        leg.CurrentWorldPosition = BaseTransform.TransformPoint(targetLocalPosition);

        leg.IsMoving = false;
        group.CurrentSimultanious--;
    }

    private Vector3 ComputeLegPosition(Vector3 start, Vector3 end, float progress)
    {
        Vector3 position = Vector3.Lerp(start, end, progress);
        position.y = StepCurveY.Evaluate(progress) * StepHeight;
        return position;
    }

    [System.Serializable]
    public class LegGroup
    {
        public Leg[] Legs;

        public int MaxSimultaniousMovements = 1;
        [HideInInspector] public int CurrentSimultanious;

        public void BakeBasePositions(Transform baseTransform)
        {
            foreach (var leg in Legs)
            {
                leg.BakeBasePosition(baseTransform);
            }
        }
    }

    [System.Serializable]
    public class Leg
    {
        public Transform LegTarget;
        public Vector3 BaseLocalPosition;
        public Vector3 CurrentWorldPosition;
        public bool IsMoving;

        public void BakeBasePosition (Transform baseTransform)
        {
            BaseLocalPosition = baseTransform.InverseTransformPoint(LegTarget.transform.position);
            CurrentWorldPosition = LegTarget.transform.position;
        }
    }

    public void OnDrawGizmosSelected()
    {
        foreach (LegGroup group in LegGroups)
        {
            foreach (Leg leg in group.Legs)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(BaseTransform.TransformPoint(leg.BaseLocalPosition), 0.05f);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(BaseTransform.TransformPoint(leg.BaseLocalPosition), StepSize);
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(BaseTransform.TransformPoint(leg.BaseLocalPosition), StepSize + StepMargin);
            }
        }
    }
}
