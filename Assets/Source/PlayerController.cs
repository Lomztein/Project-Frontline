using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject ControllableObject;
    public FollowerCamera FollowerCamera;
    public Vector2 CameraDistanceMultiplier;
    public Vector2 MinCameraDistance;
    private LayerMask _targetLayer;

    public List<ITurret> Turrets = new List<ITurret>();
    public List<IWeapon> Weapons = new List<IWeapon>();
    private IControllable _controllable;

    private void Awake()
    {
        if (ControllableObject)
        {
            Control(ControllableObject);
        }
    }

    private Bounds GetObjectVisibleBounds (GameObject obj)
    {
        Bounds result = new Bounds();
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        if (colliders.Length > 0)
        {
           result = colliders[0].bounds;
            foreach (var collider in colliders)
            {
                Bounds bounds = new Bounds(collider.bounds.center, collider.bounds.size);
                result.Encapsulate(bounds);
            }
        }

        return result;
    }

    public void Control (GameObject obj)
    {
        if (obj)
        {
            _controllable = obj.GetComponentInChildren<IControllable>();
            AIController controller = obj.GetComponentInChildren<AIController>();
            if (controller)
            {
                _targetLayer = controller.TargetLayer;
            }
            else
            {
                _targetLayer = TeamInfo.LayerAllTeams;
            }

            Bounds bounds = GetObjectVisibleBounds(obj);
            Vector3 offset = new Vector3(0f, 
                Mathf.Max(MinCameraDistance.x, bounds.size.y * CameraDistanceMultiplier.x),
                Mathf.Max(MinCameraDistance.y, -bounds.size.z * CameraDistanceMultiplier.y))
                ;

            FollowerCamera.PositionOffset = offset;
            FollowerCamera.LookPositionOffset = offset + Vector3.forward * -offset.z;
        }
    }

    public void Release()
    {
        _controllable = null;
        Turrets.Clear();
        Weapons.Clear();
    }

    void Update()
    {
        if (_controllable as Component != null)
        {
            _controllable.Accelerate(Input.GetAxis("Vertical"));
            _controllable.Turn(Input.GetAxis("Horizontal"));

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ITarget target = new PositionTarget(ray.GetPoint(1000));

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _targetLayer))
            {
                Turrets.ForEach(x => x.AimTowards(hit.point));
                target = hit.collider.gameObject.CompareTag("Terrain") ? target : new ColliderTarget(hit.collider);
            }
            else
            {
                Turrets.ForEach(x => x.AimTowards(ray.GetPoint(1000f)));
            }

            for (int i = 0; i < Mathf.Min(Weapons.Count, 2); i++)
            {
                if (Input.GetMouseButton(i))
                {
                    Weapons[i].TryFire(target);
                }
            }
        }
    }
}
