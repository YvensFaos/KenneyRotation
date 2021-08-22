using System.Collections;
using DG.Tweening;
using Lean.Pool;
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
    [SerializeField]
    private float shootTime;
    [SerializeField]
    private float shootForce;

    [Header("References")]
    [SerializeField]
    private MeshRenderer pinkyObject;
    [SerializeField]
    private MeshRenderer fucsiaObject;
    [SerializeField]
    private MeshRenderer gialloObject;

    [Header("Bullet & Cannons References")]
    [SerializeField]
    private GameObject pinkyCannon;
    [SerializeField]
    private Rigidbody pinkyBullet;
    [SerializeField]
    private GameObject fucsiaCannon;
    [SerializeField]
    private Rigidbody fucsiaBullet;
    [SerializeField]
    private GameObject gialloCannon;
    [SerializeField]
    private Rigidbody gialloBullet;
    
    private bool _canRotate;
    private bool _canShoot;

    private void OnDestroy()
    {
        playerInput.actions["RotateLeft"].performed -= RotateLeft;
        playerInput.actions["RotateRight"].performed -= RotateRight;
        playerInput.actions["Fire"].performed -= Fire;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentState = GetGivenRotation(transform.rotation);
        _canRotate = true;
        _canShoot = true;
        
        playerInput.actions["RotateLeft"].performed += RotateLeft;
        playerInput.actions["RotateRight"].performed += RotateRight;
        playerInput.actions["Fire"].performed += Fire;

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

    private void Fire(InputAction.CallbackContext callback)
    {
        if (_canRotate && _canShoot)
        {
            GameObject cannon;
            Rigidbody bullet;

            switch (currentState)
            {
                case RotationWeapon.PINKY:
                    cannon = pinkyCannon;
                    bullet = pinkyBullet;
                    break;
                case RotationWeapon.FUCSIA:
                    cannon = fucsiaCannon;
                    bullet = fucsiaBullet;
                    break;
                case RotationWeapon.GIALLO:
                    cannon = gialloCannon;
                    bullet = gialloBullet;
                    break;
                default:
                    goto case RotationWeapon.FUCSIA;
            }

            var bulletRigidbody = LeanPool.Spawn(bullet, cannon.transform.position, Quaternion.identity);
            bulletRigidbody.AddForce(Vector3.back * shootForce);
            _canShoot = false;
            StartCoroutine(ShootTimer());
        }
    }

    private IEnumerator ShootTimer()
    {
        yield return new WaitForSeconds(shootTime);
        _canShoot = true;
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
            case RotationWeapon.GIALLO: return gialloObject;
            case RotationWeapon.FUCSIA: return fucsiaObject;
            case RotationWeapon.PINKY: return pinkyObject;
            default: goto case RotationWeapon.FUCSIA;
        }
    }
}
