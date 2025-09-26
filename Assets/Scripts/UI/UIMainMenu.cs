using IntoTheUnknownTest.Managers;
using UnityEngine;

namespace IntoTheUnknownTest.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private UIEditPanel _uiEditPanel;
        [SerializeField] private UIGamePanel _uiGamePanel;

        private void Start()
        {
            _uiEditPanel.Hide();
            _uiGamePanel.Hide();
        }

        public void OnEditModeButtonClicked()
        {
            _uiEditPanel.Show();
            _uiGamePanel.Hide();
            
            StateManager.Instance.SetGameStateToEdit();
        }

        public void OnGameModeButtonClicked()
        {
            _uiGamePanel.Show();
            _uiEditPanel.Hide();
            
            StateManager.Instance.SetGameStateToPlay();
        }

        public void OnFillRandomGridButtonClicked()
        {
            GridRandomFillManager.Instance.GenerateRandomElements();
        }

        public void OnRebuildGridButtonClicked()
        {
            MapTileManager.Instance.GenerateGridAndTiles();
        }
    }
}
