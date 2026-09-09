using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class InputReader : MonoBehaviour
    {
        private int _inverted = 1;
        public void Move(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                Vector2 input = ctx.ReadValue<Vector2>();
                //Debug.Log($"Move Input: {input}");
            }
        }
        
        public void ActionPressed(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                //Debug.Log("Attack Input");
            }
        }

        public void SetInverted(bool inverted)
        {
            _inverted = inverted ? -1 : 1;
        }
    }
}
