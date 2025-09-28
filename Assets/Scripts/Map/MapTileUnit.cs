using DG.Tweening;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTileUnit : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _unitSpriteRenderer;
        
        public MapTile CurrentTile { get; private set; }
        public BaseUnitData UnitData { get; private set; }

        private const float _damageAnimationDuration = 0.1f;
        private const float _fadeAnimationDuration = 0.5f;
        
        public void Setup(BaseUnitData unitData, MapTile mapTile)
        {
            CurrentTile = mapTile;
            UnitData  = unitData;
            _unitSpriteRenderer.sprite = unitData.MapElementSprite;
            transform.position = mapTile.transform.position;
        }
        
        public void SetCurrentTile(MapTile newTile)
        {
            CurrentTile = newTile;
        }

        public void TakeDamage()
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(_unitSpriteRenderer.DOColor(Color.red, _damageAnimationDuration));
            seq.Append(_unitSpriteRenderer.DOColor(Color.white, _damageAnimationDuration));
            seq.Append(_unitSpriteRenderer.DOFade(0, _fadeAnimationDuration)).SetId(gameObject);

            seq.OnComplete(() =>
            {
                _unitSpriteRenderer.DOFade(1, 0);
                UnitManager.Instance.DespawnUnit(this);
            });
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameObject);
        }
    }
}
