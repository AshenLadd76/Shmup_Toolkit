using ToolBox.Messaging;
using UnityEngine;

namespace CodeBase.Audio
{
    public class AudioTest : MonoBehaviour
    {
        
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
                MessageBus.Broadcast<string>( AudioServiceMessages.RequestPlayOneShot, "one" );
            
        }
    }
}
