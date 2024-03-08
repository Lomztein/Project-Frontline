using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfoList : MonoBehaviour
{
    public EquipmentInfo[] InfoList;

    private void Reset()
    {
        ResetList();
    }

    public void ResetList()
    {
        InfoList = transform.root.gameObject.GetComponentsInChildren<EquipmentInfo>();
    }
}
