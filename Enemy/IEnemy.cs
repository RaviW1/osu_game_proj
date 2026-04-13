using Microsoft.Xna.Framework;
using System.Collections.Generic;

public interface IEnemy : ISprite
{
    bool IsDead { get; }
    Rectangle GetBounds();
    void TakeDamage();
    float GetVelocityX();
    float GetVelocityY();
    void ResolveCollisions(List<CollisionResult> results);
}