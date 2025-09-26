
namespace IntoTheUnknownTest.Managers
{
    public class StateManager : Singleton<StateManager>
    {
        private GameState _gameState = GameState.Menu;
        
        public bool IsEditMode => _gameState == GameState.Edit;
        public bool IsPlayMode => _gameState == GameState.Play;

        public void SetGameStateToMenu()
        {
            _gameState = GameState.Menu;
        }
        
        public void SetGameStateToEdit()
        {
            _gameState = GameState.Edit;
            PlayerInputManager.Instance.ChangeState(_gameState);
        }
        public void SetGameStateToPlay()
        {
            _gameState = GameState.Play;
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
