using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

interface IController
{
    //void Update();

    List<ICommand> GetCommands();
}
