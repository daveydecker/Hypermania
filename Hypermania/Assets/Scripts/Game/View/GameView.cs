using System;
using Design;
using Game.Sim;
using UnityEngine;

namespace Game.View
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Conductor))]
    public class GameView : MonoBehaviour
    {
        private Conductor _conductor;
        public FighterView[] Fighters => _fighters;

        private FighterView[] _fighters;
        private CharacterConfig[] _characters;

        public void Init(CharacterConfig[] characters)
        {
            _conductor = GetComponent<Conductor>();
            if (_conductor == null)
            {
                throw new InvalidOperationException(
                    "Conductor was null. Did you forget to assign a conductor component to the GameView?"
                );
            }
            _fighters = new FighterView[characters.Length];
            _characters = characters;
            for (int i = 0; i < characters.Length; i++)
            {
                _fighters[i] = Instantiate(_characters[i].Prefab);
                _fighters[i].transform.SetParent(transform, true);
                _fighters[i].Init(characters[i]);
            }
            _conductor.Init();
        }

        public void Render(in GameState state)
        {
            for (int i = 0; i < _fighters.Length; i++)
            {
                _fighters[i].Render(state.Frame, state.Fighters[i]);
            }
            _conductor.RequestSlice(state.Frame);
        }

        public void DeInit()
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _fighters[i].DeInit();
                Destroy(_fighters[i].gameObject);
            }
            _fighters = null;
            _characters = null;
        }
    }
}
