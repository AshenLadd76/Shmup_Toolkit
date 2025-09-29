using System.Collections;

namespace ToolBox.Services.Sequencing
{
    public interface ISegment
    {
        public IEnumerator StartSegment();
        
        public void StopSegment();
    }
}