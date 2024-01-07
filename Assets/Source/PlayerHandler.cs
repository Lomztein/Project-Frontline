using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UI;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerHandler : MonoBehaviour
{
    public enum InputType { MouseAndKeyboard, Gamepad }
    private const string PLAYER_RESOURCE_PATH = "Input/Player";
    public static List<PlayerHandler> Handlers = new List<PlayerHandler>();

    public int InputDeviceId;
    public InputType PlayerInputType;
    public PlayerInput PlayerInput;
    public Canvas PlayerCanvas;
    public CameraSelector CameraSelector;
    public CameraControl CameraControl;
    public Camera UICamera;
    public Rect CameraViewport;
    public Commander PlayerCommander;
    public UnitPurchaseMenu PurchaseMenu;
    public Tooltip Tooltip;

    public bool IsObserver => PlayerCommander == null;
    public bool IsDefault => name == "DefaultPlayerHandler";

    public static PlayerHandler Get(InputType type, int id)
        => Handlers.FirstOrDefault(x => x.PlayerInputType == type && x.InputDeviceId == id);

    public static PlayerHandler Get(Commander commander)
        => Handlers.FirstOrDefault(x => x.PlayerCommander == commander);

    public void Assign(Commander commander, InputType inputType, int inputId, Rect viewportRect)
    {
        CameraViewport = viewportRect;
        PlayerCommander = commander;
        PlayerInputType = inputType;
        InputDeviceId = inputId;
    }

    private void Awake()
    {
        PlayerInput.actions = Instantiate(PlayerInput.actions);
        PlayerInput.actions.Enable();
    }

    public static PlayerHandler CreateNewPlayer()
    {
        GameObject prefab = Resources.Load<GameObject>(PLAYER_RESOURCE_PATH);
        GameObject handlerObject = Instantiate(prefab, GameObject.Find("Players").transform);
        PlayerHandler handler = handlerObject.GetComponent<PlayerHandler>();
        return handler;
    }

    private void Start()
    {
        Initialize();
        Handlers.Add(this);
    }

    private void OnDestroy()
    {
        Handlers.Remove(this);
        Destroy(PlayerInput.actions);
    }

    public Vector2 GetPointerScreenPosition()
    {
        if (PlayerInputType == InputType.MouseAndKeyboard)
        {
            return Mouse.current.position.ReadValue();
        }
        return new Vector2(
            (CameraViewport.xMin + CameraViewport.width / 2f) * Screen.width,
            (CameraViewport.yMin + CameraViewport.height / 2f) * Screen.height
            );
    }

    public Vector2 GetPointerViewportPosition()
        => CameraSelector.CurrentCamera.ScreenToViewportPoint(GetPointerScreenPosition());

    public Ray PointerToWorldRay()
        => CameraSelector.CurrentCamera.ScreenPointToRay(GetPointerScreenPosition());

    public Vector2 ScreenPointToPlayerScreenPoint(Vector3 point)
    {
        point = CameraSelector.CurrentCamera.ScreenToViewportPoint(point);
        Rect playerScreenRect = ViewportRectToScreenSpace();
        return point * new Vector2(playerScreenRect.width, playerScreenRect.height);
    }

    public Rect ViewportRectToScreenSpace()
    {
        return new Rect(
            CameraViewport.x * Screen.width,
            CameraViewport.y * Screen.height,
            CameraViewport.width * Screen.width,
            CameraViewport.height * Screen.height
            );
    }

    private void Update()
    {
        if (Physics.Raycast(PointerToWorldRay(), out RaycastHit hit, Mathf.Infinity))
        {
            Debug.DrawRay(hit.point, hit.normal, Color.red, 0.1f);
        }
    }

    private void Initialize()
    {
        if (PlayerInputType == InputType.MouseAndKeyboard)
        {
            SetDevices(InputSystem.GetDevice("Keyboard"), InputSystem.GetDevice("Mouse"));
        }
        if (PlayerInputType == InputType.Gamepad)
        {
            Debug.Log(PlayerCommander.Name + ": " + InputDeviceId);
            SetDevices(Gamepad.all.First(x => x.deviceId == InputDeviceId));
            PurchaseMenu.gameObject.AddComponent<UnitPurchaseMenuGamepadAdapter>().Assign(PurchaseMenu, this, GetComponentInChildren<UnitPlacement>());
        }

        CameraSelector.SetViewport(CameraViewport);
        UICamera.rect = CameraViewport;
        if (!IsDefault)
        {
            GetComponentInChildren<AudioListener>().enabled = false;
        }

        if (!IsObserver)
        {
            AttachPlayerToPurchaseMenu();
            CameraControl.MoveToHQ();
        }
    }

    public void SetDevices(params InputDevice[] devices)
    {
        PlayerInput.actions.devices = devices;
    }

    private void AttachPlayerToPurchaseMenu()
    {
        UnitPurchaseMenu menu = transform.Find("PlayerCanvas/PlayerCommanderUI/PurchaseMenu").GetComponent<UnitPurchaseMenu>();
        menu.Commander = PlayerCommander;
        menu.UpdateActive();
        FindObjectOfType<CreditsDisplay>().Commander = PlayerCommander;
        transform.Find("PlayerCanvas/PlayerCommanderUI/PurchaseMenu/Credits/Credits").GetComponent<CreditsDisplay>().Commander = PlayerCommander;
    }

    public void OnDeviceLost()
    {
        Debug.LogWarning($"PlayerHandler {PlayerInputType}-{InputDeviceId} device lost!", this);
    }

    public void OnDeviceRegained()
    {
        Debug.LogWarning($"PlayerHandler {PlayerInputType}-{InputDeviceId} device regained.", this);
    }
}
