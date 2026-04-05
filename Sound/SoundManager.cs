using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

public static class SoundManager
{
    private static Dictionary<string, SoundEffect> sfx;
    private static SoundEffectInstance walkingLoop;

    private static Song background_music;

    public static void Initialize(ContentManager content)
    {
        sfx = new Dictionary<string, SoundEffect>();
        // load sfx
        sfx.Add("Jump", content.Load<SoundEffect>("hero_jump"));

        background_music = content.Load<Song>("background_music");
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = .7f;
        // load walking loop
        var walkSFX = content.Load<SoundEffect>("hero_run_footsteps_stone");
        walkingLoop = walkSFX.CreateInstance();
        walkingLoop.IsLooped = true;
    }
    public static void PlaySFX(string name)
    {
        if (sfx.ContainsKey(name))
        {
            sfx[name].Play();
        }
    }
    public static void PlayBGMusic()
    {
        if (MediaPlayer.State != MediaState.Playing)
        {
            MediaPlayer.Play(background_music);
        }
    }
    public static void StopMusic()
    {
        MediaPlayer.Stop();
    }
    public static void StartWalkingSound()
    {
        if (walkingLoop.State != SoundState.Playing)
        {
            walkingLoop.Play();
        }
    }
    public static void StopWalkingSound()
    {
        if (walkingLoop.State == SoundState.Playing)
        {
            walkingLoop.Stop();
        }
    }
}
