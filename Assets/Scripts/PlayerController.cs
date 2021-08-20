using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;

    [Header("Properties")]
    [SerializeField]
    private float mouseVelocity = 1.0f;
    [SerializeField]
    private Vector2 minBounds;
    [SerializeField]
    private Vector2 maxBounds;
    [SerializeField]
    private RotationWeapon currentState;
    [SerializeField]
    private GameObject rotationObject;
    [SerializeField]
    private float rotationTime;

    [Header("References")]
    [SerializeField]
    private MeshRenderer PinkyObject;
    [SerializeField]
    private MeshRenderer FucsiaObject;
    [SerializeField]
    private MeshRenderer GialloObject;
    
    private bool _canRotate;

    private void OnDestroy()
    {
        playerInput.actions["RotateLeft"].performed -= RotateLeft;
        playerInput.actions["RotateRight"].performed -= RotateRight;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentState = GetGivenRotation(transform.rotation);
        _canRotate = true;
        
        playerInput.actions["RotateLeft"].performed += RotateLeft;
        playerInput.actions["RotateRight"].performed += RotateRight;
        
        ChangeWeaponTransparency(currentState, 1.0f);
    }

    public void OnLook(InputValue value)
    {
        var mousePosition = value.Get<Vector2>() * mouseVelocity;
        mousePosition.x *= -1.0f;
        var selfTransform = transform;
        var newPosition = selfTransform.position + new Vector3(mousePosition.x, mousePosition.y, 0.0f);
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
        selfTransform.position = newPosition;
    }

    private void RotateLeft(InputAction.CallbackContext callback)
    {
        if (_canRotate)
        {
            _canRotate = false;
            ChangeWeaponTransparency(currentState, 0.0f);
            currentState = GetNextLeft(currentState);
            PerformRotation();
        }
    }

    private void RotateRight(InputAction.CallbackContext callback)
    {
        if (_canRotate)
        {
            _canRotate = false;
            ChangeWeaponTransparency(currentState, 0.0f);
            currentState = GetNextRight(currentState);
            PerformRotation();
        }
    }
    
    private void PerformRotation()
    {
        var rotationY = GetYRotation(currentState);
        var rotation = rotationObject.transform.rotation.eulerAngles;
        rotation.y = rotationY;
        rotationObject.transform.DORotate(rotation, rotationTime).OnComplete(() => { _canRotate = true; });
        ChangeWeaponTransparency(currentState, 1.0f);
    }

    private void ChangeWeaponTransparency(RotationWeapon current, float alpha)
    {
        var weaponObject = GetWeaponMeshRenderer(current);
        foreach (var weaponObjectMaterial in weaponObject.materials)
        {
            var color = weaponObjectMaterial.color;
            color.a = alpha;
            weaponObjectMaterial.DOColor(color, rotationTime);
        }
    }

    private RotationWeapon GetGivenRotation(Quaternion rotation)
    {
        var euler = rotation.eulerAngles;
        switch (euler.y)
        {
            case 0: return RotationWeapon.FUCSIA;
            case 90: return RotationWeapon.GIALLO;
            case 270: return RotationWeapon.PINKY;
            default: goto case 0;
        }
    }

    private RotationWeapon GetNextLeft(RotationWeapon current)
    {
        switch (current)
        {
            case RotationWeapon.GIALLO: return RotationWeapon.PINKY;
            case RotationWeapon.FUCSIA: return RotationWeapon.GIALLO;
            case RotationWeapon.PINKY: return RotationWeapon.FUCSIA;
            default: goto case RotationWeapon.FUCSIA;
        }
    }
    
    private RotationWeapon GetNextRight(RotationWeapon current)
    {
        switch (current)
        {
            case RotationWeapon.GIALLO: return RotationWeapon.FUCSIA;
            case RotationWeapon.FUCSIA: return RotationWeapon.PINKY;
            case RotationWeapon.PINKY: return RotationWeapon.GIALLO;
            default: goto case RotationWeapon.FUCSIA;
        }
    }

    private float GetYRotation(RotationWeapon current)
    {
        switch (current)
        {
            case RotationWeapon.GIALLO: return 90.0f;
            case RotationWeapon.FUCSIA: return 0.0f;
            case RotationWeapon.PINKY: return 270.0f;
            default: goto case RotationWeapon.FUCSIA;
        }
    }

    private MeshRenderer GetWeaponMeshRenderer(RotationWeapon current)
    {
        switch (current)
        {
            case RotationWeapon.GIALLO: return GialloObject;
            case RotationWeapon.FUCSIA: return FucsiaObject;
            case RotationWeapon.PINKY: return PinkyObject;
            default: goto case RotationWeapon.FUCSIA;
        }
    }
}
