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
        Text.text = CameraSelector.CurrentCameraObject.name;
    }
}
