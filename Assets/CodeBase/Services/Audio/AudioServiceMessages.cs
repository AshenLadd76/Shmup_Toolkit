namespace CodeBase.Services.Audio
{
    public static class AudioServiceMessages
    {
        public const string RequestAudio = "RequestAudio";
        public const string RequestAudioAtPosition = "RequestAudioAtPosition";
        public const string StopAudio = "StopAudio";
        
        //Sfx Audio Service messages
        public const string RequestPlayOneShot = "RequestPlayOneShot";
        public const string RequestPlayOneShotAtPosition = "RequestPlayOneShotAtPosition";
       
        //Music Service messages
        public const string RequestPlayAudioLoop = "RequestPlayAudioLoop";
        public const string RequestPlayAudioLoopAtPosition = "RequestPlayAudioLoopAtPosition";
        public const string RequestStopAudioLoop = "RequestStopAudioLoop";
        
        public const string RequestPlayMusic = "RequestPlayMusic";
        public const string RequestPlayMusicAtPosition = "RequestPlayMusicAtPosition";
        public const string RequestStopMusic = "RequestStopMusic";
        
        public const string RequestAudioCrossFade = "RequestAudioCrossFade";
    }
}