using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class Boss : ISprite, IEnemy
{
    private Texture2D texture;
    public Vector2 position { get; private set; }
    private bool isDead;
    private int currentFrame;
    public Vector2 velocity { get; set; }
    public bool facingLeft { get; set; }

    private IBossState currentState;

    public bool IsDead => isDead;
    public Rectangle sourceRectangle;

    public Boss(Texture2D texture, Vector2 startPos)
    {
        this.texture = texture;
        this.position = startPos;
        this.isDead = false;
        this.currentFrame = 0;
        this.facingLeft = false;
        currentState = new BossIdleState();
        currentState.OnEnter(this);
    }
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        position += velocity * dt;
        currentState.Update(this, gameTime);
    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startPos)
    {
        Vector2 origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height);
        var direction = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        currentState.Draw(this, spriteBatch);
        spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f, origin, 0.4f, direction, 0f);
    }
    // TODO: finish these, currently placeholders
    // TODO: maybe get bounds depends on the state
    // had AI generate some quick placeholder code (maybe we can keep)
    public Rectangle GetBounds()
    {
        return currentState.GetBounds(this);
    }
    public void TakeDamage()
    {
        isDead = true;
        velocity = Vector2.Zero;
    }
    // I copied these bounce methods from the enemy class but I haven't found a use for them yet
    public void BounceX()
    {
        Vector2 currentVelocity = velocity;
        currentVelocity.X *= -1;
        velocity = currentVelocity;
        facingLeft = (velocity.X < 0);
        //facingLeft = !facingLeft;
    }
    public void BounceY()
    {
        Vector2 currentVelocity = velocity;
        currentVelocity.Y *= -1;
        velocity = currentVelocity;
    }
    public float GetVelocityX() => velocity.X;
    public float GetVelocityY() => velocity.Y;
    public void ResolveCollisions(List<CollisionResult> results)
    {
        foreach (var result in results)
        {
            if (result.IsHarmful)
            {
                TakeDamage();
                continue;
            }

            if (!result.IsCollideable) continue;

            switch (result.Direction)
            {
                case CollisionDirection.Left:
                case CollisionDirection.Right:
                    BounceX();
                    break;
                case CollisionDirection.Up:
                case CollisionDirection.Down:
                    BounceY();
                    break;
            }
        }
    }
    public void ChangeState(IBossState newState)
    {
        currentState = newState;
        newState.OnEnter(this);
    }
    public void OffsetPosition(Vector2 offset)
    {
        Vector2 currentPos = position;
        currentPos.X = currentPos.X + offset.X;
        currentPos.Y = currentPos.Y + offset.Y;
        position = currentPos;

    }

}
