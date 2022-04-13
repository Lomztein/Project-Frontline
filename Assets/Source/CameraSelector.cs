using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    public GameObject[] Cameras;
    private int _currentIndex;

    public void SelectCamera(int index)
    {
        Cameras[_currentIndex].SetActive(false);
        _currentIndex = index % Cameras.Length;
        Cameras[_currentIndex].SetActive(true);
    }

    public void Next ()
        => SelectCamera(_currentIndex + 1);

    public void Prev ()
        => SelectCamera(_currentIndex - 1);
}
