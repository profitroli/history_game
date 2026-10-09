# NOW — История белорусской государственности
Updated: 2026-10-09 · single writer: главная сессия (субагенты сюда не пишут)

## Goal
Проект каркасирован. Готов к рабочим задачам.

## Verified state
- Unity 6 (URP 17.3.0), C#, Input System 1.16.0
- 80+ C# скриптов в testtest/Assets/Scripts/
- 35 мини-игр (MiniGame1-35), 14 лекций, 14 тестов
- Сцены: MainMenu, IntroScene, LecturesScene, GamesScene, SettingsScene, PreparationScene,
  ContentPage1-3, Lecture 1-14, MiniGame1-35, Test1-14
- AudioManager и GlobalAudioManager — синглтоны с DontDestroyOnLoad
- Git: branch main, компания Profitroli

## Unverified / assumed
- Все мини-игры функционируют корректно (не проверено)
- Build Settings содержит все сцены (не проверено)

## Modified files (current cycle)
- CLAUDE.md (создан)
- .claude/settings.json (создан)
- .claude/hooks/session-anchor.ps1 (создан)
- .claude/state/NOW.md (создан)

## Open decisions
—

## Rejected approaches
—

## Next 3 actions
1. Ожидание рабочей задачи от владельца
2. —
3. —

## Evidence (в рамках цикла)
| claim | command | result | date |
|---|---|---|---|
| Unity 6 / URP 17.3 | Read manifest.json | com.unity.render-pipelines.universal: 17.3.0 | 2026-10-09 |
| 80+ скриптов | Glob testtest/Assets/Scripts/**/*.cs | 80 файлов | 2026-10-09 |
| Git branch main | git status (session start) | On branch main | 2026-10-09 |

## Do not assume
- Что все сцены добавлены в Build Settings
- Что все мини-игры работают без багов
- Что проект собирается без ошибок
