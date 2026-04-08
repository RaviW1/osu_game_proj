using Microsoft.Xna.Framework;
using System.Collections.Generic;

public static class InteractSystem
{
    // Triggered by melee attack
    public static void BroadcastHit(Rectangle hitbox, List<IInteractable> interactables, Player player)
    {
        foreach (var interactable in interactables)
            if (hitbox.Intersects(interactable.GetBounds()))
                interactable.OnInteract(player, InteractType.Hit);
    }

    // Triggered by pressing up while grounded and still
    public static void BroadcastTalk(Vector2 position, int range, List<IInteractable> interactables, Player player)
    {
        Rectangle talkArea = new Rectangle(
            (int)position.X - range,
            (int)position.Y - range,
            range * 2,
            range * 2);

        foreach (var interactable in interactables)
            if (talkArea.Intersects(interactable.GetBounds()))
                interactable.OnInteract(player, InteractType.Talk);
    }
}