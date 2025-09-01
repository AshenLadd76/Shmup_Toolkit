using ToolBox.Interfaces;
using ToolBox.Messenger;

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
            MessageBus.Instance.AddListener( TargetFrameRateControllerMessages.SetMaxFrameRate, _targetFrameRateController.SetMaxFrameRate );
            MessageBus.Instance.AddListener( TargetFrameRateControllerMessages.SetMidFrameRate, _targetFrameRateController.SetMidFrameRate );
            MessageBus.Instance.AddListener( TargetFrameRateControllerMessages.SetMinFrameRate, _targetFrameRateController.SetMinFrameRate );
            MessageBus.Instance.AddListener<int>( TargetFrameRateControllerMessages.SetCustomFrameRate, _targetFrameRateController.SetCustomFrameRate );
            MessageBus.Instance.AddListener<float>( TargetFrameRateControllerMessages.TriggerFpsBoost, _targetFrameRateController.TriggerTimedFpsBoost );
        }

        public void UnSubscribe()
        {
            MessageBus.Instance.RemoveListener( TargetFrameRateControllerMessages.SetMaxFrameRate, _targetFrameRateController.SetMaxFrameRate );
            MessageBus.Instance.RemoveListener( TargetFrameRateControllerMessages.SetMidFrameRate, _targetFrameRateController.SetMidFrameRate );
            MessageBus.Instance.RemoveListener( TargetFrameRateControllerMessages.SetMinFrameRate, _targetFrameRateController.SetMinFrameRate );
            MessageBus.Instance.RemoveListener<int>( TargetFrameRateControllerMessages.SetCustomFrameRate, _targetFrameRateController.SetCustomFrameRate );
            MessageBus.Instance.RemoveListener<float>( TargetFrameRateControllerMessages.TriggerFpsBoost, _targetFrameRateController.TriggerTimedFpsBoost );
        }
    }
}