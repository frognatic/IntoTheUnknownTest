using System.Collections.Generic;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class UnitManager : Singleton<UnitManager>
    {
        [SerializeField] private UnitsLibrary _unitsLibrary;
        
        public List<BaseUnitData> Units => _unitsLibrary.BaseUnits;
        
        // private IEnumerator MoveUnitAlongPath()
        // {
        //     _isMoving = true; // Zablokuj input
        //
        //     MapTile startTile = _playerUnitData.Item1;
        //     PlayerUnitData unitData = _playerUnitData.Item2;
        //     Transform unitTransform = _playerUnitData.Item3; // Pobierz Transform jednostki
        //
        //     // Wyczyść podświetlenie ścieżki przed ruchem
        //     ClearPreviousPathHighlight();
        //
        //     // Stwórz sekwencję ruchów w DOTween
        //     Sequence moveSequence = DOTween.Sequence();
        //     float moveDurationPerTile = 0.3f; // Czas na przejście jednego kafelka
        //
        //     foreach (var position in _currentHighlightedPath)
        //     {
        //         // Dodaj do sekwencji ruch do kolejnego punktu na ścieżce
        //         moveSequence.Append(unitTransform.DOMove(position, moveDurationPerTile).SetEase(Ease.Linear));
        //     }
        //
        //     // Poczekaj, aż cała animacja się zakończy
        //     yield return moveSequence.WaitForCompletion();
        //
        //     // --- LOGIKA PO ZAKOŃCZENIU RUCHU ---
        //
        //     // 1. Zaktualizuj dane
        //     // Pobierz nowy kafelek, na którym stoi jednostka
        //     PathfindingNode finalNode = PathfindingManager.Instance.GetNode(_currentHighlightedPath.Last());
        //     MapTile newTile = _mapTiles[finalNode.GridPosition];
        //
        //     // 2. Podmień stary kafelek na domyślny
        //     startTile.SetElementOnSlot(null); // Usuń jednostkę ze starego kafelka (wizualnie)
        //     // Jeśli masz osobne dane dla "pustego" tile'a, użyj `UpdateTile`
        //     // startTile.UpdateTile(DefaultMapTileData);
        //
        //     // 3. Ustaw jednostkę na nowym kafelku (logicznie)
        //     newTile.SetElementOnSlot(unitData);
        //
        //     // 4. Zaktualizuj referencję w menedżerze
        //     _playerUnitData = new Tuple<MapTile, PlayerUnitData, Transform>(newTile, unitData, unitTransform);
        //
        //     // 5. Odblokuj input
        //     _isMoving = false;
        // }
    }
}
