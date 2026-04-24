using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class Boss : ISprite, IEnemy
{
    private Texture2D texture;
    private Vector2 position;
    private bool isDead;
    private int currentFrame;
    private Vector2 velocity;
    private bool facingLeft;

    private IBossState currentState;

    public bool IsDead => isDead;
    public Rectangle sourceRectangle;

    public Boss(Texture2D texture, Vector2 startPos)
    {
        this.texture = texture;
        this.position = startPos;
        this.isDead = false;
        this.currentFrame = 0;
        this.facingLeft = true;
        currentState = new BossAttackRecoveryState();
        currentState.OnEnter(this);
    }
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        currentState.Update(this, gameTime);
        position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

    }
    public void Draw(SpriteBatch spriteBatch, Vector2 startPos)
    {
        Vector2 origin = new Vector2(sourceRectangle.Width / 2f, sourceRectangle.Height);
        var direction = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        currentState.Draw(this, spriteBatch);
        spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f, origin, 0.5f, direction, 0f);
    }
    // TODO: finish these, currently placeholders
    public Rectangle GetBounds()
    {
        return new Rectangle((int)position.X, (int)position.Y, 56, 64);
    }
    public void TakeDamage()
    {
        isDead = true;
        velocity = Vector2.Zero;
    }
    public void Run()
    {
        var direction = facingLeft ? 1 : -1;
        currentState.Run(this, direction);
    }
    public void BounceX()
    {
        velocity.X *= -1;
        facingLeft = !facingLeft;
    }
    public void BounceY()
    {
        velocity.Y *= -1;
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

}
