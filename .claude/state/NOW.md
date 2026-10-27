# NOW — История белорусской государственности
Updated: 2026-10-24 · single writer: главная сессия

## Goal
Визуальная доработка всех сцен по мокапам пользователя. Исправление layout'а.

## Verified state
- Unity 6 (URP 17.3.0), Pipeline подключён (порт 7800)
- LoginScene: фон bg_login растянут, LoginPanel 700x540 с VLG, все элементы стилизованы
- MainMenu: фон bg_student, 4 карточки разделов (зелёная/жёлтая/голубая/фиолетовая), профиль-панель справа, overlay панели с размерами
- TeacherDashboard: фон bg_teacher, хедер 120px, 4 карточки действий, GroupsScrollView кремовый, overlay панели стилизованы
- Контроллеры обновлены: WireSectionCards() в MainMenuController, WireActionCards() в TeacherDashboardController
- Компиляция без ошибок (verified через Pipeline eval)
- Коммит: 6775c8e

## Unverified / assumed
- Визуал в runtime (не запущен Play Mode на нужной сцене)
- Все SerializedField ссылки в MainMenu (некоторые могли сбиться при перестройке)
- WebGL сборка

## Modified files (current cycle)
- testtest/Assets/Scenes/LoginScene.unity
- testtest/Assets/Scenes/MainMenu.unity
- testtest/Assets/Scenes/TeacherDashboardScene.unity
- testtest/Assets/Scripts/MainMenuController.cs
- testtest/Assets/Scripts/Teacher/TeacherDashboardController.cs

## Open decisions
- Карточка "Достижения" (MainMenu card_3) — сейчас открывает профиль, нужна отдельная сцена?
- Боковая панель навигации из мокапа — сейчас только аватар-кнопка в углу
- Иконки для карточек (книга, мечи, щит, галочка) — нужны PNG ассеты

## Rejected approaches
- Persistent listeners на кнопки карточек через eval → невозможно привязать к private методам; решено через runtime wiring в Start()
- Play Mode для скриншота → загружает IntroScene (build index 0), не TeacherDashboard

## Next 3 actions
1. Проверить визуал в Play Mode (запустить игру, зайти учителем/учеником)
2. Исправить wiring MainMenuUIController (проверить все SerializedField ссылки после перестройки)
3. Добавить иконки для карточек разделов (если есть PNG ассеты)

## Evidence
| claim | command | result | date |
|---|---|---|---|
| LoginScene bg stretched | eval verify_all_scenes.cs | stretched=True sprite=bg_login | 2026-10-24 |
| TeacherDashboard bg stretched | eval verify_all_scenes.cs | stretched=True sprite=bg_teacher | 2026-10-24 |
| TeacherDashboard controller wired | eval wire_teacher_controller.cs | All 14 fields SET | 2026-10-24 |
| Compilation OK | eval check_compile2.cs | scripts compiled | 2026-10-24 |

## Do not assume
- Что визуал выглядит идеально без проверки в Play Mode
- Что MainMenuUIController ссылки корректны после перестройки MainMenu сцены
- Что старые скрытые кнопки (LecturesButton и т.д.) всё ещё привязаны к контроллеру
