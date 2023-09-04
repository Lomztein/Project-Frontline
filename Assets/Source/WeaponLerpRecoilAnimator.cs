using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLerpRecoilAnimator : MonoBehaviour
{
    private Vector3 _baseLocalPosition;
    private Quaternion _baseLocalRotation;

    public Vector3 RecoilForceMin;
    public Vector3 RecoilForceMax;
    public float RecoilMultiplier;
    public Vector3 RecoilAngularForceMin;
    public Vector3 RecoilAngularForceMax;
    public float RecoilAngularMultiplier;

    public float RecoilLerp;
    public float RecoilAngularLerp;

    public GameObject WeaponObject;
    private IWeapon _weapon;

    public StarterAssetsInputs Inputs;
    public bool Primary;

    // Start is called before the first frame update
    void Start()
    {
        _baseLocalPosition = transform.localPosition;
        _baseLocalRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        if (!WeaponObject)
        {
            IWeapon child = GetComponentInChildren<IWeapon>();
            if (child != null)
            {
                WeaponObject = (child as Component).gameObject;

            }
        }
        else
        {
            if (_weapon == null)
            {
                _weapon = WeaponObject.GetComponent<IWeapon>();
                _weapon.OnFire += OnFire;
            }
        }
    }

    private void OnFire(IWeapon obj)
    {
        Vector3 force = new Vector3(
            Random.Range(RecoilForceMin.x, RecoilForceMax.x),
            Random.Range(RecoilForceMin.y, RecoilForceMax.y),
            Random.Range(RecoilForceMin.z, RecoilForceMax.z)) * RecoilMultiplier;

        Vector3 angularForce = new Vector3(
            Random.Range(RecoilAngularForceMin.x, RecoilAngularForceMax.x),
            Random.Range(RecoilAngularForceMin.y, RecoilAngularForceMax.y),
            Random.Range(RecoilAngularForceMin.z, RecoilAngularForceMax.z)) * RecoilAngularMultiplier;

        transform.localPosition += force;
        transform.localRotation *= Quaternion.Euler(angularForce);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, _baseLocalPosition, RecoilLerp * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _baseLocalRotation, RecoilAngularLerp * Time.deltaTime);
    }
}
