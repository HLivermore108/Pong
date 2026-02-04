using UnityEngine;

public class LeftPaddleController : PaddleController
{
    protected override float GetMovementInput()
    {
        return Input.GetAxis("LeftPaddle");
    }
}
