using Game.Sim;
using UnityEngine;

namespace Game.View
{
    public class GameView: MonoBehaviour
    {
        [SerializeField] GameObject _fighter1;
        [SerializeField] GameObject _fighter2;

        public void Render(in GameState state)
        {
            _fighter1.transform.position = state.Fighters[0].Position;
            _fighter2.transform.position = state.Fighters[1].Position;
        }
    }
}