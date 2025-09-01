using UnityEngine;

namespace ToolBox.Messenger
{
    public class TestListener : MonoBehaviour
    {
        private void OnEnable()
        {
            MessageBus.Instance.AddListener( "One", One );
            MessageBus.Instance.AddListener( "One", Two );
            MessageBus.Instance.AddListener( "Two", Two );
            MessageBus.Instance.AddListener<int>( "Three", Three );
            MessageBus.Instance.AddListener<int,int,int>( "ThreeParams", ThreeParams );
        }

    

        private void OnDisable()
        {
            MessageBus.Instance.RemoveListener( "One", One );
            MessageBus.Instance.RemoveListener( "Two", Two );
            MessageBus.Instance.RemoveListener<int>( "Three", Three );
            MessageBus.Instance.RemoveListener<int,int,int>( "ThreeParams", ThreeParams );
        }


        private void One() => Debug.Log($"This works ok");
        
        private void Two() => Debug.Log($"This also works and demonstrate multi cast");
        
        private void Three(int i) => Debug.Log($"This is a call with param {i}");

        private void ThreeParams(int arg1, int arg2, int arg3) => Debug.Log($"Three params {arg1} {arg2} {arg3}");
        
      


    }
}
