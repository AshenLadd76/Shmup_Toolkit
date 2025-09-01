namespace ToolBox.Performance.Fps
{
    public interface ITimedFrameRateBooster
    {
        void TempBoostFps( int frameRate, int defaultFrameRate, float duration);
    }
}