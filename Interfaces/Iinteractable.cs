using Microsoft.Xna.Framework;

public enum InteractType { Hit, Talk }

public interface IInteractable
{
    Rectangle GetBounds();
    void OnInteract(Player player, InteractType type);
}