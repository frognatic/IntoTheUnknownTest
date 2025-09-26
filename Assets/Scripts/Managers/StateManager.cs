
namespace IntoTheUnknownTest.Managers
{
    public class StateManager : Singleton<StateManager>
    {
        public GameState _gameState = GameState.Menu;
        
        public bool IsMenu => _gameState == GameState.Menu;
        public bool IsEditMode => _gameState == GameState.Edit;
        public bool IsPlayMode => _gameState == GameState.Play;

        public void SetGameStateToMenu()
        {
            _gameState = GameState.Menu;
        }

        public void SetGameStateToEdit() => ChangeStateTo(GameState.Edit);
        public void SetGameStateToPlay() => ChangeStateTo(GameState.Play);

        private void ChangeStateTo(GameState newState)
        {
            _gameState = newState;
            PlayerInputManager.Instance.ChangeState(_gameState);
        }
    }

    public enum GameState
    {
        Edit,
        Play,
        Menu
    }
}
