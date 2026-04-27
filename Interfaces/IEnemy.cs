using Microsoft.Xna.Framework;
using System.Collections.Generic;

public interface IEnemy : ISprite
{
    bool IsDead { get; }
    bool IsPhased { get; }
    // Bosses keep this false so their death sequence stays on screen.
    // Regular enemies report true after their post-death timer expires
    // so EnemyGenerator can cull them from the active list.
    bool ShouldBeRemoved { get; }
    Rectangle GetBounds();
    void TakeDamage();
    void BounceX();
    void BounceY();
    float GetVelocityX();
    float GetVelocityY();
    void ResolveCollisions(List<CollisionResult> results);
}