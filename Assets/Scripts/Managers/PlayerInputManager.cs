using System.Collections.Generic;
using IntoTheUnknownTest.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IntoTheUnknownTest.Managers
{
    public class PlayerInputManager : Singleton<PlayerInputManager>
    {
        private Camera _camera;
        private IInputState _currentState;
        
        private PlayModeInputState _playModeInputState;
        private EditModeInputState _editModeInputState;
        
        private readonly Dictionary<GameState, IInputState> _inputStatesDictionary =  new Dictionary<GameState, IInputState>();

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;

            _playModeInputState = new PlayModeInputState();
            _editModeInputState = new EditModeInputState();
            
            _inputStatesDictionary.Add(GameState.Play, _playModeInputState);
            _inputStatesDictionary.Add(GameState.Edit, _editModeInputState);
        }

        public void ChangeState(GameState newState)
        {
            _currentState?.OnExit();

            _inputStatesDictionary.TryGetValue(newState, out var state);
            _currentState = state;

            _currentState?.OnEnter();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            if (_currentState == null) return;

            Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
            
            if (hit.collider != null)
            {
                _currentState.HandleRaycastClick(hit);
            }
        }
    }
}
