using Unity.VisualScripting;
using UnityEngine;

public class PlayerMouseLook : MonoBehaviour {
    public PlayerConstants playerConstants;

    float _rotationY;
    float _rotationX;
    
    private PlayerAction _playerAction;

    /// <summary>
    /// Called by the ActionManager when the mouse is moved.
    /// </summary>
    /// <param name="mouseDelta">Mouse move delta</param>
    private void OnMouseMove(Vector2 mouseDelta) {
        _rotationX = transform.localEulerAngles.y + mouseDelta.x * playerConstants.mouseSensitivityX;
        _rotationY += mouseDelta.y * playerConstants.mouseSensitivityY;
        _rotationY = Mathf.Clamp(_rotationY, playerConstants.viewMinimumY, playerConstants.viewMaximumY);
    }

    private void Start() {
        _rotationX = transform.localEulerAngles.y;
        _rotationY = -transform.localEulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;

        _playerAction = new PlayerAction();
        _playerAction.Enable();
        
        _playerAction.gameplay.MouseMove.performed += ctx => OnMouseMove(ctx.ReadValue<Vector2>());
    }

    private void OnEnable()
    { 
        _playerAction = new PlayerAction();
        _playerAction.Enable();
        _playerAction.gameplay.MouseMove.performed += ctx => OnMouseMove(ctx.ReadValue<Vector2>());
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        transform.localEulerAngles = new Vector3(-_rotationY, _rotationX, 0);
    }

    private void OnDisable() {
        _playerAction.Disable();
        Cursor.lockState = CursorLockMode.Confined;
    }
}