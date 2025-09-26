using IntoTheUnknownTest.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace IntoTheUnknownTest.UI
{
    public class UIBasePanel : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
            
            _backButton.onClick.RemoveAllListeners();
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void OnBackButtonClicked()
        {
            Hide();
            StateManager.Instance.SetGameStateToMenu();
        }
    }
}
