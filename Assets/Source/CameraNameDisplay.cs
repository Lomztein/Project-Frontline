using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraNameDisplay : MonoBehaviour
{
    public CameraSelector CameraSelector;
    public TMP_Text Text;

    // Update is called once per frame
    void Update()
    {
        if (CameraSelector.CurrentIs(out ICameraController cont))
        {
            Text.text = cont.GetName();
        }
        else
        {
            Text.text = CameraSelector.CurrentCameraObject.name;
        }
    }
}
