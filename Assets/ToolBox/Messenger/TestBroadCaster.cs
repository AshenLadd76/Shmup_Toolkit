using System.Collections;
using UnityEngine;


namespace ToolBox.Messenger
{
    public class TestBroadCaster : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            
            MessageBus.Instance.Broadcast( "One" );
            
            yield return new WaitForSeconds(2f);
            
            MessageBus.Instance.Broadcast( "Two" );
            
            yield return new WaitForSeconds(3f);
            
            MessageBus.Instance.Broadcast( "Three", 99 );
            
            MessageBus.Instance.Broadcast( "ThreeParams", 99, 2000 , 2332 );
            
            MessageBus.Instance.Broadcast( "Doesnt Exist", 99, 2000 , 2332 );
        }
    }
}
