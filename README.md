# Лабораторные работы — «Иммерсивные технологии»

Unity-проект с лабораторными работами (ЛР1, ЛР2, ЛР3, ЛР4, ЛР5, ЛР6)
по методичкам из папки `Downloads/MU`.

## Что внутри

| Лаба | Файл сцены | Что реализовано |
|------|-----------|-----------------|
| ЛР1 | `Assets/Labs/Scenes/Lab1_Setup.unity` | Настроен XR-проект (XR Plug-in Management + OpenXR), собрано помещение (комната), импортирована 3D-модель из Blender (FBX), размещён игрок (XR Origin) |
| ЛР2 | `Assets/Labs/Scenes/Lab2_Locomotion.unity` | Перемещение (Continuous/Dynamic Move + Teleportation), 4 Teleport Anchor, изменён цвет луча интерактора, кнопка показывает информационное сообщение |
| ЛР3 | `Assets/Labs/Scenes/Lab3_Interaction.unity` | XR Grab Interactable (Movement Type = Velocity Tracking), кнопка создаёт куб, дверь на Hinge Joint + ручка с Fixed Joint |
| ЛР4 | `Assets/Labs/Scenes/Lab4_UI.unity` | VR-интерфейс: World Space Canvas + `Tracked Device Graphic Raycaster`, кнопки меняют цвет объекта, Slider меняет оттенок, Toggle включает свет, звук при нажатии кнопок |
| ЛР4 | `Assets/Labs/Scenes/Lab4_Menu.unity` | Сцена главного меню: кнопки «Запустить» (грузит Lab4_UI), «Опции» (панель с Toggle звука и Slider громкости), «Выход»; звук интерфейса |
| ЛР5 | `Assets/Labs/Scenes/Lab5_Scripting.unity` | 4 C#-скрипта: цвет при захвате/отпускании, звук при взаимодействии, GetComponent (изменение массы/силы), масштаб при наведении |
| ЛР6 | `Assets/Labs/Scenes/Lab6_Trainer.unity` | Тренажёр: панель управления с кнопками и индикаторами, цепочка действий (последовательность нажатий), зона-триггер (Collider + Trigger) |

## Требования

- **Unity 6000.4.5f1** (Unity 6). В методичке указана 2022.3.36f — проект сделан на установленной версии Unity 6;
  XR Interaction Toolkit здесь версии **3.x** (в методичке — 2.5), поэтому часть названий компонентов обновилась
  (см. раздел «Отличия XRI 3.x»).
- Пакеты (уже в `Packages/manifest.json`):
  - `com.unity.xr.interaction.toolkit` 3.7.0
  - `com.unity.xr.management` 4.7.0
  - `com.unity.xr.openxr` 1.19.0
  - `com.unity.inputsystem` 1.20.1
- **Blender** — только для ЛР1 (модель уже импортирована, `Assets/Labs/Models/BlenderTable.fbx`).

## Как открыть

1. Unity Hub → *Add* → выбрать папку `ImmersiveTech_Labs`.
2. Открыть проект, дождаться импорта.
3. Сцены уже добавлены в *Build Settings* (File → Build Settings), порядок: Lab1, Lab2, Lab3, Lab4_UI, Lab4_Menu, Lab5, Lab6.
4. Открыть нужную сцену из `Assets/Labs/Scenes/` и нажать **Play**.

> Папку `Library/` можно удалить — Unity пересоздаст её при открытии (сэкономит ~420 МБ).

## Как тестировать без шлема

- В каждую сцену добавлен объект **XR Device Simulator** (эмуляция контроллеров клавиатурой и мышью).
- Управление симулятором: `WASD` + мышь — перемещение взгляда, `Tab` — переключение между контроллерами,
  `G` — захват, левая/правая кнопки мыши — select/activate. Полный список — в окне `XR Device Simulator` при Play.
- Если Play не запускается из-за OpenXR: на компьютере без VR-рантайма откройте
  *Edit → Project Settings → XR Plug-in Management* и снимите галочку с **OpenXR** (симулятор при этом продолжает работать),
  либо установите OpenXR-рантайм (SteamVR / Monado / Windows Mixed Reality).

## Структура проекта

```
Assets/
  Labs/
    Scenes/      — сцены лабораторных работ (Lab1, Lab2, Lab3, Lab4_UI, Lab4_Menu, Lab5, Lab6)
    Scripts/     — C#-скрипты (см. таблицу ниже)
    Materials/   — материалы сцен
    Models/      — BlenderTable.fbx (модель из Blender, ЛР1)
    Audio/       — тональные звуки (ЛР4, ЛР5)
  Samples/XR Interaction Toolkit/3.7.0/
    Starter Assets/   — XR Origin (XR Rig), UI, префабы XRI
    XR Device Simulator/ — симулятор контроллеров
  XR/            — сгенерированные настройки XR Plug-in Management + OpenXR
  _LabTools/Editor/  — скрипты генерации/проверки сцен (LabBuilder и др.)
```

### C#-скрипты (`Assets/Labs/Scripts/`)

| Скрипт | Назначение | Лаба |
|--------|-----------|------|
| `GrabColorChange` | Меняет цвет объекта при захвате, возвращает при отпускании | ЛР5 |
| `InteractionSound` | Проигрывает звук при захвате/отпускании | ЛР5 |
| `PhysicsForceOnGrab` | GetComponent: меняет массу и прикладывает силу при захвате | ЛР5 |
| `HoverScale` | Увеличивает масштаб при наведении | ЛР5 |
| `SpawnOnSelect` | По нажатию кнопки создаёт куб | ЛР3 |
| `InfoMessageButton` | Показывает информационное сообщение по нажатию | ЛР2 |
| `IndicatorLight` | Индикатор (выкл/вкл — цвет + свечение) | ЛР6 |
| `TrainerSequence` | Проверяет правильную последовательность нажатий | ЛР6 |
| `TriggerZone` | Реагирует на вход контроллера в зону (OnTriggerEnter/Exit) | ЛР6 |
| `UIButtonEvent` | Действие кнопки UI: смена цвета объекта, загрузка сцены, выход, показать/скрыть панель | ЛР4 |
| `UIButtonSound` | Звук при нажатии кнопки интерфейса | ЛР4 |
| `UIColorSlider` | Slider меняет цвет (оттенок) целевого объекта | ЛР4 |
| `UILightToggle` | Toggle включает/выключает источник света | ЛР4 |
| `UIAudioToggle` | Toggle включает/выключает звук (mute) AudioSource | ЛР4 |
| `UIVolumeSlider` | Slider регулирует общую громкость (AudioListener) | ЛР4 |

## Отличия XRI 3.x (не путать с методичкой)

Методичка написана под XR Interaction Toolkit **2.5**, а в Unity 6 используется **3.x**. Ключевые изменения:

- `Continuous Move Provider` → **Dynamic Move Provider / Continuous Move Provider** (в сцене уже есть).
- Вместо `XRController (Action based)` контроллеры используют **Controller Input Action Manager**
  (внутри префаба `XR Origin (XR Rig)` уже всё настроено: Near-Far Interactor, Teleport Interactor, Poke Interactor).
- Локомоция собрана на **Locomotion Mediator** + провайдеры (Move / Turn / Teleportation / Grab Move / Jump) —
  всё внутри `XR Origin (XR Rig)/Locomotion`.
- Провайдеры телепортации — `Teleportation Area` и `Teleportation Anchor` (те же имена).
- `XR Device Simulator` помечен устаревшим, но работает; при желании можно использовать новый `XR Interaction Simulator`.

## Перегенерация сцен

Сцены строятся скриптом (а не вручную), это документирует состав сцен:

- Меню **Tools → Rebuild Lab Scenes** — метод `LabBuilder.BuildAll()` (пересобирает все сцены, через `-executeMethod LabBuilder.BuildAll`).
- Меню **Tools → Rebuild Lab4 UI Scenes** — метод `LabBuilder.BuildLab4Only()` (собирает только `Lab4_UI` и `Lab4_Menu`, не трогая остальные сцены).
- `XRConfigurator.Configure()` — настраивает XR Plug-in Management + OpenXR.
- `SceneValidator.ValidateAll()` — проверяет сцены (нет ли «missing script»/материалов).
- `SceneDump` — выгружает иерархию сцен в текст.

> Сцены ЛР4 генерируются тем же способом. После первого открытия проекта, если сцен
> `Lab4_UI`/`Lab4_Menu` нет, выполните **Tools → Rebuild Lab4 UI Scenes**.
