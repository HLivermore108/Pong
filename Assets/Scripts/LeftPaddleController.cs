using UnityEngine;

public class LeftPaddleController : PaddleController
{
    protected override string GetAxisName()
    {
        return "LeftPaddle";
    }
}
