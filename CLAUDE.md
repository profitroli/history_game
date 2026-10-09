# История белорусской государственности — образовательное Unity-приложение

Unity 6 (URP 17.3), C#. Лекции (14 видео), мини-игры (35 шт.), тесты (14 шт.),
главное меню, настройки, подготовка к экзамену. Компания: Profitroli.
Корень Unity-проекта: `testtest/`. Скрипты: `testtest/Assets/Scripts/`.

## Session boot
1. `.claude/state/NOW.md` — текущая цель, verified/unverified, next actions.
2. Если git: `git fetch` + `git status` — не работай на устаревшей ветке.
3. Topic-файлы auto-memory — по задаче, через индекс.
Read-состояние файлов не переживает компакт и смену модели: перечитай файл перед Edit.

## Invariants
- Диск канонический; summary компакта — информация, не доказательство.
- «Verified» требует команду и её вывод в том же ходу; иначе observed/assumed/planned.
- Новый вывод правит старый в точке залегания (SUPERSEDED-пометка), не припиской снизу.
- Одно правило — один дом; ротирующиеся значения — указателем, не копией.
- Числа меряются командой, не вспоминаются.
- Секреты не читать и не выводить; механическое принуждение — в `.claude/settings.json`.
- Субагенту критичные правила дублируются в брифе дословно (память он не наследует).
- Temp-файлы → `D:\CLAUDE PROJECT\Temp\`, НИКОГДА на C: или scratchpad.
- Документы (.md, .txt, .docx) редактировать скриптами, НЕ через Edit tool.
- Глобальные правила из ~/.claude/CLAUDE.md применяются. Специфика проекта ниже.
- Результаты работы сохранять в рабочую папку проекта на D:.

## Unity-specific
- `.unity` (сцены) и `.prefab` — YAML Unity: редактировать осторожно, предпочитать скрипты.
- `.meta` файлы НИКОГДА не удалять отдельно от ассетов — Unity потеряет привязки.
- `testtest/Library/` — автогенерируемая, не редактировать, не коммитить.
- `testtest/ProjectSettings/` — YAML настроек, редактировать через скрипты или аккуратно Edit.
- C# скрипты — основной рабочий материал. Namespace не используется (legacy).
- Все UI-скрипты используют UnityEngine.UI (не UI Toolkit).
- AudioManager / GlobalAudioManager — синглтоны, DontDestroyOnLoad.
- Имена сцен: `Lecture N`, `MiniGameN`, `TestN`, `MainMenu`, `GamesScene`, и т.д.
- Кодировка C# файлов: UTF-8 BOM (стандарт Unity). Комментарии на русском (кириллица).

## State layer
- `.claude/state/NOW.md` — ПЕРЕЗАПИСЬ, ≤150 строк.
- `.claude/state/history/NNN-<тема>.md` — append-only, файл на завершённый этап.
- Auto-memory — уроки и долговременные факты.

## Compact instructions
When compacting, preserve: current goal; modified files; the verified vs unverified split;
open decisions; rejected approaches with reasons; exact next 3 actions; the paths
`.claude/state/NOW.md` and `.claude/state/history/`.
Compaction summaries are informational, not evidence; NOW.md on disk is authoritative.

## Working mode (владелец: Igor)
- Автономно, AI-First: обратимое — без вопросов; необратимое/внешнее/платное — спросить.
- Модель: claude-opus-4-6[1m]. Не использовать Sonnet или Haiku.
- Перед /compact обнови NOW.md. Тупик → /rewind, не /compact.

## Do not
- Не переименовывай и не переноси корень проекта.
- Не заводи второй дом для правила; не дублируй сюда user-level правила.
- Не создавай файлов «для памяти», которые ничто не загружает.
- Не используй scratchpad на C: для сохранения файлов.
- Не удаляй .meta файлы без удаления соответствующего ассета.
- Не редактируй бинарные ассеты (.png, .mp3, .mp4, .ttf) напрямую.
