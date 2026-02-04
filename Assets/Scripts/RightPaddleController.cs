using UnityEngine;

public class RightPaddleController : PaddleController
{
    protected override string GetAxisName()
    {
        return "RightPaddle";
    }
}
