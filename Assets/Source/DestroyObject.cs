using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public Object Target;
    public float Time;

    private void Start()
    {
        Invoke(nameof(DoDestroy), Time);
    }

    public void ResetDestroyTimer ()
    {
        CancelInvoke(nameof(DoDestroy));
        Invoke(nameof(DoDestroy), Time);
    }

    private void DoDestroy ()
    {
        Destroy(Target);
    }
}
