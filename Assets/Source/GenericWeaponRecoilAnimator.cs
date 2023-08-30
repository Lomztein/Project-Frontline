using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericWeaponRecoilAnimator : MonoBehaviour
{
    public GameObject WeaponObject;
    private IWeapon _weapon;

    public RecoilPart[] Parts;
    
    private float _animationTime;
    private bool _animating;
    private float _maxAnimTime;

    private void Start()
    {
        _weapon = WeaponObject.GetComponent<IWeapon>();
        _weapon.OnFire += OnFire;
        _maxAnimTime = Parts.Max(x => x.RecoilTime);
        foreach (RecoilPart part in Parts)
        {
            part.PartBasePosition = part.PartTransform.localPosition;
        }
    }

    private void OnFire(IWeapon obj)
    {
        Recoil();
    }

    [System.Serializable]
    public class RecoilPart
    {
        public Transform PartTransform;
        public Vector3 RecoilDirection = Vector3.forward;
        public float RecoilStrength;
        public float RecoilTime;
        public AnimationCurve RecoilCurve;

        public Vector3 PartBasePosition;
        
        public Vector3 ComputePosition (float time)
        {
            return PartBasePosition + RecoilCurve.Evaluate(time / RecoilTime) * RecoilStrength * RecoilDirection;
        }
    }

    public void Recoil()
    {
        _animating = true;
        _animationTime = 0;
    }

    void FixedUpdate()
    {
        if (_animating)
        {
            foreach (var part in Parts)
            {
                if (_animationTime < part.RecoilTime)
                {
                    part.PartTransform.localPosition = part.ComputePosition(_animationTime);
                }
            }

            if (_animationTime > _maxAnimTime)
            {
                _animationTime = 0;
                _animating = false;
            }
            _animationTime += Time.fixedDeltaTime;
        }
    }
}
