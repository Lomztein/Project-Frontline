using UnityEngine;

public class GameObjectIconOverride : MonoBehaviour, IIconOverride
{
    public GameObject Model;

    public Sprite GetIcon()
    {
        return Util.Iconography.GenerateSprite(Model);
    }
}
