using System;
using System.Collections.Generic;
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
        private ManiaView[] _manias;
        private CharacterConfig[] _characters;
        
        public GameObject HealthbarPrefab;
        private GameObject[] _healthbars;
        public Canvas canvas;
        private float Zoom = 5f;

        public ManiaViewConfig Config;

        [SerializeField]
        private DJ_CameraControl CameraControl;

        public void Init(CharacterConfig[] characters)
        {
            _healthbars = new GameObject[2];

            _conductor = GetComponent<Conductor>();
            if (_conductor == null)
            {
                throw new InvalidOperationException(
                    "Conductor was null. Did you forget to assign a conductor component to the GameView?"
                );
            }
            _fighters = new FighterView[characters.Length];
            _manias = new ManiaView[characters.Length];
            _characters = characters;
            for (int i = 0; i < characters.Length; i++)
            {
                _fighters[i] = Instantiate(_characters[i].Prefab);
                _fighters[i].name = "Fighter View";
                _fighters[i].transform.SetParent(transform, true);
                _fighters[i].Init(characters[i]);

                float xPos = i - ((float)characters.Length - 1) / 2;
                GameObject maniaView = new GameObject("Mania View");
                _manias[i] = maniaView.AddComponent<ManiaView>();
                _manias[i].transform.SetParent(transform, true);
                _manias[i].Init(new Vector2(8f * xPos, 0f), Config);
            }
            _conductor.Init();

            for (int i = 0; i < _healthbars.Length; i++) {
                _healthbars[i] = Instantiate(HealthbarPrefab);
                _healthbars[i].transform.SetParent(canvas.transform);
            }
            _healthbars[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-615f, 445f);

            _healthbars[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(615f, 445f);
            _healthbars[1].GetComponent<RectTransform>().localScale = new Vector3(-1,1,1);


        }

        public void Render(in GameState state, GlobalConfig config)
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _fighters[i].Render(state.Frame, state.Fighters[i]);
                _manias[i].Render(state.Frame, state.Manias[i]);
            }
            _conductor.RequestSlice(state.Frame);

            List<Vector2> interestPoints = new List<Vector2>();
            for (int i = 0; i < _characters.Length; i++)
            {
                HealthBarScript healthbarScript = _healthbars[i].GetComponent<HealthBarScript>();
                interestPoints.Add((Vector2)state.Fighters[i].Position);

                if (healthbarScript.slider.maxValue == 1) {
                    healthbarScript.SetMaxHealth((int) state.Fighters[i].Health);
                }
                healthbarScript.SetHealth((int) state.Fighters[i].Health);
            }
            // Debug testing for zoom, remove later
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Zoom == 5f)
                {
                    Zoom = 4f;
                }
                else
                {
                    Zoom = 5f;
                }
            }
            CameraControl.UpdateCamera(interestPoints, Zoom, Time.deltaTime);
        }

        public void DeInit()
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                _fighters[i].DeInit();
                Destroy(_fighters[i].gameObject);
                _manias[i].DeInit();
                Destroy(_manias[i].gameObject);
            }
            _fighters = null;
            _characters = null;
        }
    }
}
