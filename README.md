# Into the Unknown Test

# ENGLISH:

# Pathfinding Demo - A* Algorithm

This project is a demonstration of the A* pathfinding algorithm implemented in the Unity engine. The architecture was designed with flexibility, performance, and scalability in mind.

## 1. Chosen Algorithm: A* (A-star)

The **A*** algorithm was chosen for the pathfinding system implementation. It is an industry standard for grid-based games, and its selection was based on three key features:

*   **Optimality:** Guarantees finding the shortest possible path.
*   **Performance:** Thanks to an intelligent heuristic, it searches a much smaller area of the map than other algorithms, which drastically speeds up calculations.
*   **Flexibility:** It works perfectly on grids, handling different movement costs and dynamically changing obstacles.

### Modifications and Architectural Improvements

I have enhanced the standard A* implementation with the following elements:

*   **Priority queue:** Instead of a classic `OpenSet` list, a `PriorityQueue` is used, which significantly optimizes the algorithm's performance.
*   **Integer-based movement costs:** Using values of `10` (straight movement) and `14` (diagonal movement) instead of floating-point numbers (`1.0` and `1.41`) speeds up calculations and eliminates potential precision errors. Diagonal movement is disabled by default but can be enabled in `GameSettings` -> `UseDiagonalMoveCalculations`.
*   **Flexible tile interaction condition:** The algorithm itself doesn't know what an obstacle is. This logic is provided externally (e.g., `IsWalkable` for movement, `IsAttackableThrough` for attacks), making the system universal and reusable.

---

## 2. Project Configuration

Most of the project's configuration files are stored as `ScriptableObject` assets in the folder:
`Assets/Resources/`

Thanks to this approach, modifying game parameters (e.g., adding new units, changing map tile properties, adjusting the random generator) is simple and does not require code changes.

**Exception:** The camera settings configuration (movement speed, zoom) is located directly in the `Main` scene, on the `MainCamera` object, within the `CameraController` component.

---

## 3. User Guide

Camera controls are handled with the **WSAD keys** (movement) and the **mouse scroll** (zoom).

1.  Open the `Main` scene located in the `Assets/Scenes/` folder.
2.  After running the scene in the Unity editor, four buttons will appear in the top-left corner of the screen:

    a. **EDIT MODE**
*  Allows you to freely place elements on the map, such as obstacles, cover, the player unit, and an enemy unit.
*  *Note:* By default, only one player unit and one enemy can be placed. To change this, you must uncheck the `IsUniqueOnMap` flag in the respective unit's configuration file (`Assets/Resources/Units/`).

    b. **GAME MODE**
*  The main game mode, which allows you to interact with the map, find paths, and perform actions (move, attack). A legend at the top of the screen explains the color-coding of the tiles.

    c. **FILL GRID RANDOM**
*  Generates a random map based on predefined patterns.
*  The grid size can be changed in the `Assets/Resources/GridSettings` file.
*  The amount and type of generated elements can be configured in `Assets/Resources/Libraries/GridFillPatternLibrary`.

    d. **REBUILD GRID**
*  Clears the entire map of all placed objects.
*  Use this button after changing the grid size in `GridSettings` to rebuild the map with the new dimensions.

# POLISH

## 1. Wybrany Algorytm: A* (A-star)

Do implementacji systemu wyszukiwania ścieżek wybrano algorytm **A***. Jest on standardem branżowym dla gier opartych na siatce, a jego wybór został podyktowany trzema kluczowymi cechami:

*   **Optymalność:** Gwarantuje znalezienie najkrótszej ścieżki.
*   **Wydajność:** Dzięki inteligentnej heurystyce przeszukuje znacznie mniejszy obszar mapy niż inne algorytmy, co drastycznie przyspiesza obliczenia.
*   **Elastyczność:** Doskonale sprawdza się na siatkach, obsługując różne koszty ruchu i dynamicznie zmieniające się przeszkody.

### Modyfikacje i Ulepszenia Architektury

Standardową implementację A* rozbudowałem o następujące elementy:

*   **Kolejka priorytetowa (`PriorityQueue`):** Zamiast klasycznej listy `OpenSet`, co znacząco optymalizuje wydajność algorytmu.
*   **Koszty ruchu na liczbach całkowitych:** Użycie wartości `10` (ruch prosty) i `14` (ruch po skosie) zamiast liczb zmiennoprzecinkowych (`1.0` i `1.41`) przyspiesza obliczenia i eliminuje potencjalne błędy precyzji. Domyślnie ruch po skosie jest wyłączony, ale można go włączyć w `GameSettings` -> `UseDiagonalMoveCalculations`.
*   **Elastyczny warunek interakcji z kafelkiem:** Sam algorytm nie wie, co jest przeszkodą. Logika ta jest dostarczana z zewnątrz (np. `IsWalkable` dla ruchu, `IsAttackableThrough` dla ataku), co czyni system uniwersalnym i reużywalnym.

---

## 2. Konfiguracja Projektu

Większość plików konfiguracyjnych projektu znajduje się w formie `ScriptableObject` w folderze:
`Assets/Resources/`

Dzięki temu podejściu modyfikacja parametrów gry (np. dodawanie nowych jednostek, zmiana właściwości pól mapy, dostosowanie losowego generatora) jest prosta i nie wymaga zmian w kodzie.

**Wyjątek:** Konfiguracja ustawień kamery (prędkość ruchu, zoom) znajduje się bezpośrednio na scenie `Main` w obiekcie `MainCamera` w komponencie `CameraController`.

---

## 3. Instrukcja Obsługi

Sterowanie kamerą odbywa się za pomocą klawiszy **WSAD** (poruszanie) oraz **kółka myszy** (zoom).

1.  Otwórz scenę `Main` znajdującą się w folderze `Assets/Scenes/`.
2.  Po uruchomieniu sceny w edytorze Unity, w lewym górnym rogu pojawią się 4 przyciski:

    a. **EDIT MODE**
        - Pozwala na swobodne umieszczanie na mapie elementów takich jak przeszkody, osłony, jednostka gracza i przeciwnik.
        - *Uwaga:* Domyślnie można umieścić tylko jedną jednostkę gracza i jednego przeciwnika. Aby to zmienić, należy w pliku konfiguracyjnym danej jednostki (`Assets/Resources/Units/`) odznaczyć flagę `IsUniqueOnMap`.

    b. **GAME MODE**
        - Główny tryb gry, który pozwala na interakcję z mapą, wyznaczanie ścieżek i wykonywanie akcji (ruch, atak). Legenda na górze ekranu wyjaśnia oznaczenia kolorystyczne na kafelkach.

    c. **FILL GRID RANDOM**
        - Generuje losową mapę na podstawie zdefiniowanych wzorców.
        - Rozmiar siatki można zmienić w pliku `Assets/Resources/GridSettings`.
        - Ilość i typ generowanych elementów można skonfigurować w `Assets/Resources/Libraries/GridFillPatternLibrary`.

    d. **REBUILD GRID**
        - Czyści całą mapę ze wszystkich umieszczonych obiektów.
        - Użyj tego przycisku po zmianie rozmiaru siatki w `GridSettings`, aby mapa została przebudowana z nowymi wymiarami.
