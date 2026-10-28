# NOW — История белорусской государственности
Updated: 2026-10-28 · single writer: главная сессия

## Goal
Визуальная доработка TeacherDashboard до эталонного качества LoginScene/MainMenu + teacher workflow.

## Verified state
- Font sizes matched to MainMenu (38 elements): Title 50, CardTitle 30, CardDesc 19, CardBtn 25, NavLabels 25, SidebarName 25, SidebarRole 20, InfoTitles 25
- 4 card icons extracted from mockup and added: icon_mydata (green person), icon_mygroups (orange group), icon_assignments (blue clipboard), icon_grading (purple checkmark)
- Card layout adjusted: Icon top (0.58-0.97), Title (0.38-0.58), Desc (0.18-0.38), Btn (0.02-0.16)
- GroupService SelfStudy fallback: 2 seed groups (7A, 8B), 5 mock students with progress
- AssignmentService SelfStudy fallback: local storage for assignments
- StudentsScrollView mask fixed: Mask+transparent Image → RectMask2D
- Students visible: Мария Иванова (10/14, 25/35, 8/14, 77%), Андрей Сидоров, Ольга Петровна
- Play Mode verified: scene=TeacherDashboardScene, user=Алексей Петров, IsTeacher=True
- Commit: 5bcd21a

## Unverified / assumed
- WebGL build not tested
- Full assignment creation flow (editor panel) not visually tested
- PreviewAsStudent (Card_3) loads MainMenu — not tested this session

## Modified files (current cycle)
- testtest/Assets/Scenes/TeacherDashboardScene.unity — fonts, layout, icons, card colors, TitleArea, mask fix
- testtest/Assets/Scripts/Groups/GroupService.cs — SelfStudy fallback + seed data
- testtest/Assets/Scripts/Services/AssignmentService.cs — SelfStudy fallback
- testtest/Assets/Scripts/Teacher/TeacherDashboardController.cs — student row rendering
- testtest/Assets/Sprites/Teacher/ — 4 icon PNGs + .meta

## Open decisions
- Action card click mapping (Card_0→ShowGroupDetail, Card_2→CreateGroup) seems reversed
- NavMyTasks, NavGrading, NavStats — need dedicated views
- Teacher avatar — still placeholder
- Notification bell — per mockup

## Rejected approaches
- Editing scene YAML directly — used eval scripts (more reliable)
- Using pipeline screenshot for UI — doesn't render Canvas
- Extracting icons from card area of mockup — too imprecise, used separate icons row instead
- Using UI Mask on ScrollView — transparent Image masked everything, replaced with RectMask2D

## Next 3 actions
1. Visual polish: info section buttons font sizes, action card shadow/rounded corners
2. Assignment editor panel visual test in Play Mode
3. Student preview (PreviewAsStudent) workflow verification

## Evidence
| claim | command | result | date |
|---|---|---|---|
| Fonts 38 elements | eval fix_fonts_to_mainmenu.cs | Fixed 38 texts to MainMenu sizes | 2026-10-28 |
| Icons extracted | Python PIL crop from mockup | 4 icons 256x256 saved | 2026-10-28 |
| Icons on cards | eval add_card_icons.cs | 4 cards with sprites | 2026-10-28 |
| Groups loaded | eval check_groups.cs | 2 groups: 7A, 8B | 2026-10-28 |
| Students visible | eval+screenshot | 3 students with progress data | 2026-10-28 |
| Mask bug | Viewport Image alpha=0 | Replaced Mask→RectMask2D | 2026-10-28 |

## Do not assume
- Что Firebase работает в SelfStudy режиме
- Что Pipeline screenshot рендерит UI
- Что UI Mask с прозрачным Image показывает содержимое