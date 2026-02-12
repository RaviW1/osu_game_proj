using Microsoft.Xna.Framework.Input;

interface IController
{
    void Update();
}

public class KeyboardController : IController
{
    private KeyboardState currentKey;
    public void Update()
    {

    }
}