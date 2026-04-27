Контекст для другой нейронки

Unity 6 HDRP хоррор игра (клон Phasmophobia), соло разраб, базовые знания Unity, C#, Input Handling: Both.
Сделано:

Комната 10x10x3 из кубов (Floor, Ceiling, Wall_1-3)
Point Light (#FF9966, ~800lm), Directional Light выключен
Global Volume: Exposure Fixed ~12, Fog attenuation 30, Bloom/Vignette/Film Grain/Chromatic Aberration
FPS контроллер на Rigidbody (WASD + мышь), Escape = курсор, ЛКМ = управление обратно
Фонарик на F (Spot Light в CameraHolder, 3000lm, Range 12, Angle 45, тени включены)
Звук шагов через AudioSource на Player (один файл, play/stop по движению) — в PlayerController.cs
AI призрак: NavMesh Surface на Floor, GhostAI.cs (бродит + преследует при chaseDistance 8), модель из Asset Store "Cute Monster Ghost Free" (конвертирована в HDRP)
Смерть игрока: тег Ghost на призраке, PlayerHealth.cs на Player (OnTriggerEnter → перезагрузка сцены)
Проект на GitHub: github.com/qwewerqwewer1/my-first-horror

Следующий шаг: EMF сенсор + система типов призраков (3 типа, разное поведение, разные улики → журнал → победа).


Бесплатная замена
Google Gemini 2.0 Flash — быстрый, бесплатный, длинный контекст. Лучший вариант прямо сейчас.
gemini.google.com 
