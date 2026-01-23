using ToolBox.Interfaces;
using ToolBox.Messaging;


namespace ToolBox.Performance.Fps
{
    public class TargetFrameRateControllerEventHandler : IEventHandler
    {
        private readonly TargetFrameRateController _targetFrameRateController;
        
        public TargetFrameRateControllerEventHandler(TargetFrameRateController targetFrameRateController)
        {
            _targetFrameRateController = targetFrameRateController;
        }

        public void Subscribe()
        {
            MessageBus.AddListener( TargetFrameRateControllerMessages.SetMaxFrameRate, _targetFrameRateController.SetMaxFrameRate );
            MessageBus.AddListener( TargetFrameRateControllerMessages.SetMidFrameRate, _targetFrameRateController.SetMidFrameRate );
            MessageBus.AddListener( TargetFrameRateControllerMessages.SetMinFrameRate, _targetFrameRateController.SetMinFrameRate );
            MessageBus.AddListener<int>( TargetFrameRateControllerMessages.SetCustomFrameRate, _targetFrameRateController.SetCustomFrameRate );
            MessageBus.AddListener<float>( TargetFrameRateControllerMessages.TriggerFpsBoost, _targetFrameRateController.TriggerTimedFpsBoost );
        }

        public void UnSubscribe()
        {
            MessageBus.RemoveListener( TargetFrameRateControllerMessages.SetMaxFrameRate, _targetFrameRateController.SetMaxFrameRate );
            MessageBus.RemoveListener( TargetFrameRateControllerMessages.SetMidFrameRate, _targetFrameRateController.SetMidFrameRate );
            MessageBus.RemoveListener( TargetFrameRateControllerMessages.SetMinFrameRate, _targetFrameRateController.SetMinFrameRate );
            MessageBus.RemoveListener<int>( TargetFrameRateControllerMessages.SetCustomFrameRate, _targetFrameRateController.SetCustomFrameRate );
            MessageBus.RemoveListener<float>( TargetFrameRateControllerMessages.TriggerFpsBoost, _targetFrameRateController.TriggerTimedFpsBoost );
        }
    }
}