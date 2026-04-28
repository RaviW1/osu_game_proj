using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public partial class GameScene
{
    private HitstopManager _hitstop = new HitstopManager();
    private Effect _fogEffect;
    private bool _fogEnabled;
    private Texture2D _fogTexture;

    public void TriggerHitEffects(bool playerWasHit)
    {
        if (playerWasHit)
        {
            _hitstop.Trigger(4);
            _camera.Shake(6f, 14);
            _tookHit = true;
        }
        else
        {
            _hitstop.Trigger(6);
            _camera.Shake(3f, 10);
        }
    }

    private void LoadFogEffect()
    {
        _fogTexture = CreateRadialFogTexture(512);
    }

    public void UpdateFog()
    {
        _fogEnabled = levels.currentRoom.roomName == "exploration"
                   || levels.currentRoom.roomName == "ascent"
                   || levels.currentRoom.roomName == "boss";
    }

    private Texture2D CreateRadialFogTexture(int size)
    {
        Texture2D tex = new Texture2D(_graphics, size, size);
        Color[] data = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center) / (size / 2f);

                // smoothstep falloff: transparent center, black edges
                float alpha = MathHelper.Clamp((dist - 0.2f) / 0.8f, 0f, 1f);
                alpha = alpha * alpha * (3f - 2f * alpha);

                data[y * size + x] = Color.Black * alpha;
            }
        }

        tex.SetData(data);
        return tex;
    }


}