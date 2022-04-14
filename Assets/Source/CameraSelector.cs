using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public GameObject[] Cameras;
    public int SelectedIndex;

    private GameObject Current => Cameras[SelectedIndex];

    private void Start()
    {
        SelectCamera(SelectedIndex);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Prev();
        if (Input.GetKeyDown(KeyCode.E)) Next();
    }

    public void SelectCamera(int index)
    {
        Vector3 pos = Current.transform.position;
        Quaternion rot = Current.transform.rotation;
        Current.SetActive(false);
        SelectedIndex = index % Cameras.Length;
        if (SelectedIndex < 0f) SelectedIndex = Cameras.Length - 1;
        Current.SetActive(true);
        Current.transform.position = pos;
        Current.transform.rotation = rot;
    }


    public void Next ()
        => SelectCamera(SelectedIndex + 1);

    public void Prev ()
        => SelectCamera(SelectedIndex - 1);
}
