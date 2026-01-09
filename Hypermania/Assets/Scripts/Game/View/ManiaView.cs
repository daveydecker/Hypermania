using Game.Sim;
using UnityEngine;
using Utils;

namespace Game.View
{
    public class ManiaView : MonoBehaviour
    {
        public void Render(Frame frame, in ManiaState state)
        {
            for (int i = 0; i < state.Config.NumKeys; i++)
            {
                for (int j = 0; j < state.Channels[i].Notes.Count; j++) { }
            }
        }

        private void RenderNote(Frame frame, int channel, in ManiaNote note)
        {
            GameObject noteView = GameObject.CreatePrimitive(PrimitiveType.Cube);
            noteView.transform.SetParent(transform);
        }
    }
}
