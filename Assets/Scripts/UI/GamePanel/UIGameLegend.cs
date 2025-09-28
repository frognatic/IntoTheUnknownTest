using IntoTheUnknownTest.Libraries;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IntoTheUnknownTest.UI
{
    public class UIGameLegend : MonoBehaviour
    {
        [SerializeField] private Image _legendImage;
        [SerializeField] private TextMeshProUGUI _legendText;

        public void Init(TileSelectorConfig tileSelectorConfig)
        {
            _legendImage.color = tileSelectorConfig.Color;
            _legendText.text = tileSelectorConfig.Name;
        }
    }
}
