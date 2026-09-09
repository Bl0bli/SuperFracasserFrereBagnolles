using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputReader : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;

        private InputAction _move;
        private InputAction _action;

        // -1 quand les controles sont inverses
        private int _inverted = 1;
        
        public Vector2 Move => _move.ReadValue<Vector2>() * _inverted;

        public bool ActionHeld => _action.IsPressed();
        public bool IsInverted => _inverted < 0;

        public event Action ActionPressed;
        public event Action ActionReleased;

        private void Awake()
        {
            if (_playerInput == null) _playerInput = GetComponent<PlayerInput>();

            _move = _playerInput.actions.FindAction("Move", true);
            _action = _playerInput.actions.FindAction("Action", true);
        }

        private void OnEnable()
        {
            _action.performed += HandleActionPerformed;
            _action.canceled += HandleActionCanceled;
        }

        private void OnDisable()
        {
            _action.performed -= HandleActionPerformed;
            _action.canceled -= HandleActionCanceled;
        }

        public void SetInverted(bool inverted)
        {
            _inverted = inverted ? -1 : 1;
        }

        private void HandleActionPerformed(InputAction.CallbackContext ctx)
        {
            ActionPressed?.Invoke();
        }

        private void HandleActionCanceled(InputAction.CallbackContext ctx)
        {
            ActionReleased?.Invoke();
        }
    }
}
