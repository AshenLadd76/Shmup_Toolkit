namespace CodeBase.Services.Audio
{
    public enum AudioCommand
    {
        OneShot,
        OneShotAtPosition,
        LoopingAudio,
        LoopingAudioAtPosition,
        StopAudioLoop,
        Music,
        MusicAtPosition,
        StopMusic,
        CrossFade,
    }
}