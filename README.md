# Horror Game — Unity 6 HDRP

## Контекст для AI

Unity 6 HDRP хоррор игра (клон Phasmophobia), соло разраб,
базовые знания Unity, C#, Input Handling: Both.

## Сделано

- Дом построен через ProBuilder по плану (9+ комнат)
- NavMesh Surface запечён под новую карту
- HDRP атмосфера: Exposure ~12, Fog 30, Bloom, Vignette,
  Film Grain, Chromatic Aberration, Depth of Field, Color Adjustments
- FPS контроллер (WASD + мышь, Escape/ЛКМ курсор)
- Фонарик (F) — Spot Light 3000lm, тени включены
- Звук шагов — AudioSource на Player
- AI призрак — NavMesh Agent, GhostAI.cs (Wander/Chase)
- Модель призрака — Cute Monster Ghost Free (HDRP конвертирован)
- Смерть игрока — OnTriggerEnter → перезагрузка сцены
- Проект на GitHub

## Следующий шаг

- AI призрак не видит сквозь стены (Raycast)
- EMF сенсор (E) — TextMeshPro UI
- Система типов призраков (3 типа)
- Пропсы из KayKit
