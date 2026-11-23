using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Unity.XR.CoreUtils;

public static class LabBuilder
{
    const string SampleRoot = "Assets/Samples/XR Interaction Toolkit/3.7.0";
    const string XrOriginPrefab = SampleRoot + "/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
    const string SimulatorPrefab = SampleRoot + "/XR Device Simulator/XR Device Simulator.prefab";
    const string TeleportAnchorPrefab = SampleRoot + "/Starter Assets/DemoAssets/Prefabs/Teleport/Teleport Anchor.prefab";

    const string LabsRoot = "Assets/Labs";
    const string ScenesDir = LabsRoot + "/Scenes";
    const string MaterialsDir = LabsRoot + "/Materials";
    const string ModelsDir = LabsRoot + "/Models";

    static readonly Dictionary<string, Material> s_Materials = new Dictionary<string, Material>();

    [MenuItem("Tools/Rebuild Lab Scenes")]
    public static void BuildAll()
    {
        EnsureFolders();
        XRConfigurator.Configure();

        var scenes = new List<string>
        {
            BuildLab1(),
            BuildLab2(),
            BuildLab3(),
            BuildLab4(),
            BuildLab4Menu(),
            BuildLab5(),
            BuildLab6(),
        };

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
        var list = new List<EditorBuildSettingsScene>();
        foreach (var s in scenes)
            list.Add(new EditorBuildSettingsScene(s, true));
        EditorBuildSettings.scenes = list.ToArray();

        Debug.Log("[LabBuilder] Done. Scenes: " + string.Join(", ", scenes));
    }

    [MenuItem("Tools/Rebuild Lab4 UI Scenes")]
    public static void BuildLab4Only()
    {
        EnsureFolders();
        XRConfigurator.Configure();

        var l4 = BuildLab4();
        var l4Menu = BuildLab4Menu();

        AddSceneToBuildSettings(l4);
        AddSceneToBuildSettings(l4Menu);

        Debug.Log("[LabBuilder] Lab4 scenes built: " + l4 + ", " + l4Menu);
    }

    static void AddSceneToBuildSettings(string path)
    {
        var list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (var s in list)
        {
            if (s.path == path)
                return;
        }
        list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();
    }

    static void EnsureFolders()
    {
        Directory.CreateDirectory(ScenesDir);
        Directory.CreateDirectory(MaterialsDir);
        Directory.CreateDirectory(ModelsDir);
        AssetDatabase.Refresh();
    }

    // ---------------------------------------------------------------------
    // Shared scene scaffolding
    // ---------------------------------------------------------------------

    static Scene CreateBaseScene(string sceneName, bool addFloor = true, bool addSimulator = true)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = sceneName;

        CreateDirectionalLight();
        if (addFloor)
            CreateFloor();
        CreateXrOrigin();
        CreateInteractionManager();
        CreateEventSystem();
        if (addSimulator && AssetDatabase.LoadAssetAtPath<GameObject>(SimulatorPrefab) != null)
            InstantiatePrefab(SimulatorPrefab);

        return scene;
    }

    static string SaveScene(Scene scene, string fileName)
    {
        var path = ScenesDir + "/" + fileName;
        EditorSceneManager.SaveScene(scene, path);
        return path;
    }

    static void CreateDirectionalLight()
    {
        var go = new GameObject("Directional Light");
        var light = go.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        light.shadows = LightShadows.Soft;
        go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    static GameObject CreateFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(12f, 0.1f, 12f);
        floor.transform.position = new Vector3(0f, -0.05f, 0f);
        floor.GetComponent<Renderer>().sharedMaterial = GetMaterial("Floor", new Color(0.55f, 0.56f, 0.6f));
        floor.AddComponent<TeleportationArea>();
        return floor;
    }

    static GameObject CreateXrOrigin()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrOriginPrefab);
        if (prefab == null)
        {
            Debug.LogError("[LabBuilder] XR Origin prefab not found: " + XrOriginPrefab);
            return null;
        }

        var go = InstantiatePrefab(XrOriginPrefab);
        go.name = "XR Origin (XR Rig)";
        return go;
    }

    static void CreateInteractionManager()
    {
        var go = new GameObject("XR Interaction Manager");
        go.AddComponent<XRInteractionManager>();
    }

    static void CreateEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<XRUIInputModule>();
    }

    static GameObject InstantiatePrefab(string path, Transform parent = null)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("[LabBuilder] Missing prefab: " + path);
            return null;
        }

        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (parent != null)
            go.transform.SetParent(parent, false);
        return go;
    }

    // ---------------------------------------------------------------------
    // Lab 1 - setup, room, imported model
    // ---------------------------------------------------------------------

    static string BuildLab1()
    {
        var scene = CreateBaseScene("Lab1", addFloor: false);

        var room = new GameObject("Room");
        var wallColor = new Color(0.85f, 0.83f, 0.78f);
        CreateBox("Room/Floor", room.transform, new Vector3(0f, -0.05f, 0f), new Vector3(6f, 0.1f, 6f), wallColor);
        CreateBox("Room/Ceiling", room.transform, new Vector3(0f, 3f, 0f), new Vector3(6f, 0.1f, 6f), new Color(0.9f, 0.9f, 0.92f));
        CreateBox("Room/Wall_North", room.transform, new Vector3(0f, 1.5f, 3f), new Vector3(6f, 3f, 0.1f), wallColor);
        CreateBox("Room/Wall_South", room.transform, new Vector3(0f, 1.5f, -3f), new Vector3(6f, 3f, 0.1f), wallColor);
        CreateBox("Room/Wall_East", room.transform, new Vector3(3f, 1.5f, 0f), new Vector3(0.1f, 3f, 6f), wallColor);
        CreateBox("Room/Wall_West", room.transform, new Vector3(-3f, 1.5f, 0f), new Vector3(0.1f, 3f, 6f), wallColor);

        var floor = GameObject.Find("Room/Floor");
        if (floor != null)
            floor.AddComponent<TeleportationArea>();

        // Modeled in Blender and exported to FBX, then imported here.
        var modelPath = ModelsDir + "/BlenderTable.fbx";
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model != null)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
            instance.name = "Blender_Model_Table";
            instance.transform.position = new Vector3(1.5f, 0f, 1.2f);
        }
        else
        {
            Debug.LogWarning("[LabBuilder] Blender model not found, using primitive stand-in.");
            CreateBox("Imported_Model_StandIn", null, new Vector3(1.5f, 0.4f, 1.2f), new Vector3(1.2f, 0.8f, 0.8f), new Color(0.6f, 0.4f, 0.25f));
        }

        TeleportAnchorAt(new Vector3(-1.8f, 0f, 0f));
        TeleportAnchorAt(new Vector3(1.8f, 0f, -1.8f));

        var origin = GameObject.Find("XR Origin (XR Rig)");
        if (origin != null)
            origin.transform.position = new Vector3(0f, 0f, 0f);

        return SaveScene(scene, "Lab1_Setup.unity");
    }

    // ---------------------------------------------------------------------
    // Lab 2 - locomotion, teleport anchors, ray color, info message
    // ---------------------------------------------------------------------

    static string BuildLab2()
    {
        var scene = CreateBaseScene("Lab2");

        TeleportAnchorAt(new Vector3(-2.5f, 0f, -2.5f));
        TeleportAnchorAt(new Vector3(2.5f, 0f, -2.5f));
        TeleportAnchorAt(new Vector3(2.5f, 0f, 2.5f));
        TeleportAnchorAt(new Vector3(-2.5f, 0f, 2.5f));

        ColorRayInteractor(new Color(0.1f, 0.9f, 1f));

        // Information message shown when the user presses a button.
        var message = CreateWorldCanvas("Info_Message", new Vector3(0f, 1.6f, 2.4f), "Нажата кнопка!\nИнформационное сообщение", 1.2f, new Color(0.1f, 0.1f, 0.15f, 0.9f));
        message.SetActive(false);
        CreatePushButton("Info_Button", new Vector3(0f, 1.0f, 2.0f), new Color(0.9f, 0.5f, 0.1f), message, null);

        return SaveScene(scene, "Lab2_Locomotion.unity");
    }

    // ---------------------------------------------------------------------
    // Lab 3 - grab interactables, spawn on press, hinge-joint door
    // ---------------------------------------------------------------------

    static string BuildLab3()
    {
        var scene = CreateBaseScene("Lab3");

        CreateBox("Table", null, new Vector3(0f, 0.4f, 1.5f), new Vector3(1.6f, 0.1f, 1.0f), new Color(0.45f, 0.3f, 0.18f));

        CreateGrabCube("Grab_Cube", new Vector3(-0.4f, 0.75f, 1.4f), new Color(0.8f, 0.2f, 0.2f));
        CreateGrabCube("Grab_Cube_2", new Vector3(0.2f, 0.75f, 1.7f), new Color(0.2f, 0.5f, 0.9f));
        CreateGrabCube("Grab_Cube_3", new Vector3(0.5f, 0.75f, 1.3f), new Color(0.3f, 0.8f, 0.3f));

        // A button that spawns a cube when selected.
        var spawner = CreatePushButton("Spawn_Button", new Vector3(-1.4f, 1.0f, 1.5f), new Color(0.85f, 0.85f, 0.2f), null, null);
        var spawn = spawner.AddComponent<SpawnOnSelect>();
        spawn.spawnPoint = spawner.transform;
        spawn.maxObjects = 8;

        CreateHingeDoor(new Vector3(1.8f, 1.0f, 1.5f));

        return SaveScene(scene, "Lab3_Interaction.unity");
    }

    // ---------------------------------------------------------------------
    // Lab 4 - VR user interface (World Space Canvas, buttons, slider, toggle)
    // ---------------------------------------------------------------------

    static string BuildLab4()
    {
        var scene = CreateBaseScene("Lab4");

        // Object whose colour is changed from the UI.
        var target = CreateBox("Color_Target", null, new Vector3(-1.3f, 0.75f, 1.6f), Vector3.one * 0.4f, new Color(0.6f, 0.6f, 0.6f));

        // A point light toggled from the UI.
        var lightGo = new GameObject("UI_Light");
        lightGo.transform.position = new Vector3(-1.3f, 2.4f, 1.6f);
        var roomLight = lightGo.AddComponent<Light>();
        roomLight.type = LightType.Point;
        roomLight.range = 6f;
        roomLight.intensity = 2f;
        roomLight.color = new Color(1f, 0.95f, 0.85f);

        // Shared audio source for UI click sounds.
        var audioGo = new GameObject("UI_Audio");
        var audioSource = audioGo.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f;
        var clickClip = CreateToneClip("UI_Click", 880f, 0.08f);

        var canvas = CreateUICanvas("Lab4_UI_Canvas", new Vector3(0f, 1.5f, 2.2f));
        CreateUIPanel(canvas.transform, new Vector2(760f, 600f), new Color(0.08f, 0.09f, 0.12f, 0.94f));

        CreateUIText(canvas.transform, "Панель управления", new Vector2(0f, 240f), new Vector2(700f, 56f), 40, TextAnchor.MiddleCenter);

        var targetRenderer = target.GetComponent<Renderer>();

        var red = CreateUIButton(canvas.transform, "Красный", new Vector2(-240f, 130f), new Vector2(150f, 64f));
        WireColorButton(red, targetRenderer, new Color(0.85f, 0.2f, 0.2f));
        var green = CreateUIButton(canvas.transform, "Зелёный", new Vector2(-80f, 130f), new Vector2(150f, 64f));
        WireColorButton(green, targetRenderer, new Color(0.2f, 0.8f, 0.3f));
        var blue = CreateUIButton(canvas.transform, "Синий", new Vector2(80f, 130f), new Vector2(150f, 64f));
        WireColorButton(blue, targetRenderer, new Color(0.25f, 0.45f, 0.95f));
        var random = CreateUIButton(canvas.transform, "Случайный", new Vector2(240f, 130f), new Vector2(150f, 64f));
        var randomEvent = random.AddComponent<UIButtonEvent>();
        randomEvent.action = UIButtonEvent.ButtonAction.RandomColor;
        randomEvent.colorTarget = targetRenderer;

        CreateUIText(canvas.transform, "Оттенок (Slider)", new Vector2(-190f, 40f), new Vector2(260f, 32f), 24, TextAnchor.MiddleCenter);
        var slider = CreateUISlider(canvas.transform, new Vector2(-190f, -10f), new Vector2(300f, 32f));
        var sliderComp = slider.AddComponent<UIColorSlider>();
        sliderComp.target = targetRenderer;

        var toggle = CreateUIToggle(canvas.transform, "Свет (Toggle)", new Vector2(210f, -10f), new Vector2(320f, 40f));
        var toggleComp = toggle.AddComponent<UILightToggle>();
        toggleComp.targetLight = roomLight;

        CreateUIText(canvas.transform, "Наводите луч контроллера на кнопки и нажимайте (Trigger)",
            new Vector2(0f, -160f), new Vector2(700f, 60f), 22, TextAnchor.MiddleCenter);

        foreach (var btn in canvas.GetComponentsInChildren<Button>(true))
        {
            var snd = btn.gameObject.AddComponent<UIButtonSound>();
            snd.clip = clickClip;
            snd.source = audioSource;
        }

        return SaveScene(scene, "Lab4_UI.unity");
    }

    static string BuildLab4Menu()
    {
        var scene = CreateBaseScene("Lab4Menu");

        var audioGo = new GameObject("UI_Audio");
        var audioSource = audioGo.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0.5f;
        var clickClip = CreateToneClip("Menu_Click", 660f, 0.08f);

        var canvas = CreateUICanvas("Main_Menu_Canvas", new Vector3(0f, 1.6f, 2.4f));
        CreateUIPanel(canvas.transform, new Vector2(780f, 640f), new Color(0.06f, 0.07f, 0.1f, 0.96f));

        CreateUIText(canvas.transform, "ИММЕРСИВНЫЕ ТЕХНОЛОГИИ", new Vector2(0f, 250f), new Vector2(720f, 56f), 40, TextAnchor.MiddleCenter);
        CreateUIText(canvas.transform, "Главное меню", new Vector2(0f, 200f), new Vector2(720f, 40f), 26, TextAnchor.MiddleCenter);

        var start = CreateUIButton(canvas.transform, "ЗАПУСТИТЬ", new Vector2(0f, 90f), new Vector2(440f, 76f));
        var startEvent = start.AddComponent<UIButtonEvent>();
        startEvent.action = UIButtonEvent.ButtonAction.LoadScene;
        startEvent.sceneName = "Lab4_UI";

        var options = CreateUIButton(canvas.transform, "ОПЦИИ", new Vector2(0f, -10f), new Vector2(440f, 76f));

        var quit = CreateUIButton(canvas.transform, "ВЫХОД", new Vector2(0f, -110f), new Vector2(440f, 76f));
        var quitEvent = quit.AddComponent<UIButtonEvent>();
        quitEvent.action = UIButtonEvent.ButtonAction.Quit;

        // Options panel (hidden by default).
        var optionsPanel = CreateUIPanel(canvas.transform, new Vector2(680f, 460f), new Color(0.1f, 0.12f, 0.16f, 0.99f));
        optionsPanel.name = "Options_Panel";
        CreateUIText(optionsPanel.transform, "ОПЦИИ", new Vector2(0f, 170f), new Vector2(620f, 50f), 34, TextAnchor.MiddleCenter);

        var soundToggle = CreateUIToggle(optionsPanel.transform, "Звук интерфейса", new Vector2(-140f, 70f), new Vector2(420f, 40f));
        var soundComp = soundToggle.AddComponent<UIAudioToggle>();
        soundComp.target = audioSource;

        CreateUIText(optionsPanel.transform, "Громкость", new Vector2(0f, 5f), new Vector2(520f, 30f), 22, TextAnchor.MiddleCenter);
        var volumeSlider = CreateUISlider(optionsPanel.transform, new Vector2(0f, -35f), new Vector2(500f, 32f));
        volumeSlider.AddComponent<UIVolumeSlider>();

        var back = CreateUIButton(optionsPanel.transform, "НАЗАД", new Vector2(0f, -150f), new Vector2(240f, 60f));
        var backEvent = back.AddComponent<UIButtonEvent>();
        backEvent.action = UIButtonEvent.ButtonAction.TogglePanel;
        backEvent.panel = optionsPanel;

        options.SetActive(true);
        var optionsEvent = options.AddComponent<UIButtonEvent>();
        optionsEvent.action = UIButtonEvent.ButtonAction.TogglePanel;
        optionsEvent.panel = optionsPanel;

        optionsPanel.SetActive(false);

        foreach (var btn in canvas.GetComponentsInChildren<Button>(true))
        {
            var snd = btn.gameObject.AddComponent<UIButtonSound>();
            snd.clip = clickClip;
            snd.source = audioSource;
        }

        return SaveScene(scene, "Lab4_Menu.unity");
    }

    // ---------------------------------------------------------------------
    // Lab 5 - C# scripting behaviours
    // ---------------------------------------------------------------------

    static string BuildLab5()
    {
        var scene = CreateBaseScene("Lab5");

        var colorCube = CreateGrabCube("Script_ColorOnGrab", new Vector3(-1.0f, 1.0f, 1.5f), new Color(0.6f, 0.6f, 0.6f));
        colorCube.AddComponent<GrabColorChange>();

        var soundCube = CreateGrabCube("Script_SoundOnGrab", new Vector3(-0.2f, 1.0f, 1.5f), new Color(0.2f, 0.7f, 0.7f));
        var sound = soundCube.AddComponent<InteractionSound>();
        sound.grabClip = CreateToneClip("Grab_Tone", 660f, 0.12f);
        sound.releaseClip = CreateToneClip("Release_Tone", 440f, 0.12f);

        var physCube = CreateGrabCube("Script_PhysicsOnGrab", new Vector3(0.6f, 1.0f, 1.5f), new Color(0.8f, 0.5f, 0.2f));
        physCube.AddComponent<PhysicsForceOnGrab>();

        var hoverCube = CreateSimpleInteractableObject("Script_HoverScale", new Vector3(1.4f, 1.0f, 1.5f), new Color(0.7f, 0.3f, 0.8f));
        hoverCube.AddComponent<HoverScale>();

        return SaveScene(scene, "Lab5_Scripting.unity");
    }

    // ---------------------------------------------------------------------
    // Lab 6 - trainer: control panel, indicators, sequence, trigger
    // ---------------------------------------------------------------------

    static string BuildLab6()
    {
        var scene = CreateBaseScene("Lab6");

        var panelRoot = new GameObject("Control_Panel");
        CreateBox("Control_Panel/Base", panelRoot.transform, new Vector3(0f, 1.0f, 1.6f), new Vector3(1.6f, 0.7f, 0.1f), new Color(0.2f, 0.22f, 0.25f));

        var buttonA = CreatePushButton("Button_A", new Vector3(-0.5f, 1.1f, 1.5f), new Color(0.85f, 0.25f, 0.25f), null, null);
        var buttonB = CreatePushButton("Button_B", new Vector3(0f, 1.1f, 1.5f), new Color(0.25f, 0.55f, 0.85f), null, null);
        var buttonC = CreatePushButton("Button_C", new Vector3(0.5f, 1.1f, 1.5f), new Color(0.85f, 0.75f, 0.2f), null, null);

        var indA = CreateIndicator("Indicator_A", new Vector3(-0.5f, 0.85f, 1.5f));
        var indB = CreateIndicator("Indicator_B", new Vector3(0f, 0.85f, 1.5f));
        var indC = CreateIndicator("Indicator_C", new Vector3(0.5f, 0.85f, 1.5f));
        var indDone = CreateIndicator("Indicator_Done", new Vector3(0f, 0.7f, 1.5f));

        var message = CreateWorldCanvas("Completion_Message", new Vector3(0f, 1.9f, 2.2f), "ЗАДАНИЕ ВЫПОЛНЕНО", 1.0f, new Color(0.05f, 0.3f, 0.1f, 0.95f));
        message.SetActive(false);

        var trainer = panelRoot.AddComponent<TrainerSequence>();
        trainer.buttonsInOrder = new[] { buttonA.GetComponent<XRSimpleInteractable>(), buttonB.GetComponent<XRSimpleInteractable>(), buttonC.GetComponent<XRSimpleInteractable>() };
        trainer.stepIndicators = new[] { indA.GetComponent<IndicatorLight>(), indB.GetComponent<IndicatorLight>(), indC.GetComponent<IndicatorLight>() };
        trainer.completionIndicator = indDone.GetComponent<IndicatorLight>();
        trainer.completionMessage = message;

        // A trigger zone: entering it lights an indicator (Collider + Trigger).
        var zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zone.name = "Trigger_Zone";
        zone.transform.position = new Vector3(-2.0f, 0.3f, 1.0f);
        zone.transform.localScale = new Vector3(0.8f, 0.6f, 0.8f);
        var zoneRenderer = zone.GetComponent<Renderer>();
        zoneRenderer.sharedMaterial = GetMaterial("TriggerZone", new Color(0.3f, 0.3f, 0.35f));
        zone.GetComponent<Collider>().isTrigger = true;
        var zoneLight = CreateIndicator("Zone_Indicator", new Vector3(-2.0f, 0.7f, 1.0f));
        var trigger = zone.AddComponent<TriggerZone>();
        trigger.indicator = zoneLight.GetComponent<IndicatorLight>();
        trigger.visual = zoneRenderer;

        return SaveScene(scene, "Lab6_Trainer.unity");
    }

    // ---------------------------------------------------------------------
    // Object helpers
    // ---------------------------------------------------------------------

    static GameObject CreateBox(string name, Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        if (parent != null)
            go.transform.SetParent(parent, true);
        go.transform.position = position;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("Mat_" + ColorKey(color), color);
        return go;
    }

    static GameObject CreateGrabCube(string name, Vector3 position, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.2f;
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("Mat_" + ColorKey(color), color);

        var body = go.AddComponent<Rigidbody>();
        body.mass = 1f;
        var grab = go.AddComponent<XRGrabInteractable>();
        grab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        return go;
    }

    static GameObject CreateSimpleInteractableObject(string name, Vector3 position, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.2f;
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("Mat_" + ColorKey(color), color);
        go.AddComponent<XRSimpleInteractable>();
        return go;
    }

    static GameObject CreatePushButton(string name, Vector3 position, Color color, GameObject messageObject, System.Action onPressed)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = new Vector3(0.12f, 0.03f, 0.12f);
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("Mat_" + ColorKey(color), color);
        go.AddComponent<XRSimpleInteractable>();

        if (messageObject != null)
        {
            var info = go.AddComponent<InfoMessageButton>();
            info.messageObject = messageObject;
        }

        return go;
    }

    static GameObject CreateIndicator(string name, Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.09f;
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("Indicator", new Color(0.25f, 0.05f, 0.05f));
        go.AddComponent<IndicatorLight>();
        return go;
    }

    static void CreateHingeDoor(Vector3 position)
    {
        var doorRoot = new GameObject("Door");
        doorRoot.transform.position = position;

        var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        panel.name = "Door_Panel";
        panel.transform.SetParent(doorRoot.transform, false);
        panel.transform.localPosition = new Vector3(0.4f, 0f, 0f);
        panel.transform.localScale = new Vector3(0.8f, 1.6f, 0.06f);
        panel.GetComponent<Renderer>().sharedMaterial = GetMaterial("Door", new Color(0.5f, 0.32f, 0.18f));

        var doorBody = doorRoot.AddComponent<Rigidbody>();
        doorBody.mass = 8f;
        doorBody.useGravity = false;
        doorBody.isKinematic = false;

        var hinge = doorRoot.AddComponent<HingeJoint>();
        hinge.axis = new Vector3(0f, 1f, 0f);
        hinge.anchor = new Vector3(0f, 0f, 0f);
        hinge.useLimits = true;
        hinge.limits = new JointLimits { min = -90f, max = 0f };

        var handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Door_Handle";
        handle.transform.SetParent(doorRoot.transform, false);
        handle.transform.localPosition = new Vector3(0.72f, 0f, 0.08f);
        handle.transform.localScale = new Vector3(0.06f, 0.18f, 0.06f);
        handle.GetComponent<Renderer>().sharedMaterial = GetMaterial("Handle", new Color(0.85f, 0.75f, 0.2f));

        var handleBody = handle.AddComponent<Rigidbody>();
        handleBody.mass = 0.5f;

        var handleGrab = handle.AddComponent<XRGrabInteractable>();
        handleGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;

        var fixedJoint = handle.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = doorBody;
    }

    static void TeleportAnchorAt(Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Teleport Anchor";
        go.transform.position = new Vector3(position.x, 0.03f, position.z);
        go.transform.localScale = new Vector3(0.6f, 0.03f, 0.6f);
        go.GetComponent<Renderer>().sharedMaterial = GetMaterial("TeleportAnchor", new Color(0.1f, 0.5f, 0.9f));
        var anchor = go.AddComponent<TeleportationAnchor>();
        var destination = new GameObject("Anchor");
        destination.transform.SetParent(go.transform, false);
        anchor.teleportAnchorTransform = destination.transform;
    }

    static void ColorRayInteractor(Color color)
    {
        var origin = GameObject.Find("XR Origin (XR Rig)");
        if (origin == null)
            return;

        var lineVisuals = origin.GetComponentsInChildren<XRInteractorLineVisual>(true);
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });

        foreach (var lv in lineVisuals)
        {
            lv.setLineColorGradient = true;
            lv.validColorGradient = gradient;
            EditorUtility.SetDirty(lv);
        }

        Debug.Log($"[LabBuilder] Recolored {lineVisuals.Length} interactor line visual(s).");
    }

    static GameObject CreateWorldCanvas(string name, Vector3 position, string text, float width, Color background)
    {
        var canvasGo = new GameObject(name);
        canvasGo.transform.position = position;
        canvasGo.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rect = canvasGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, width * 0.35f);
        canvasGo.transform.localScale = Vector3.one * 0.5f;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var bg = new GameObject("Background");
        bg.transform.SetParent(canvasGo.transform, false);
        var bgImage = bg.AddComponent<Image>();
        bgImage.color = background;
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(canvasGo.transform, false);
        var uiText = textGo.AddComponent<Text>();
        uiText.text = text;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;
        uiText.fontSize = 36;
        uiText.font = GetBuiltinFont();
        var textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 8f);
        textRect.offsetMax = new Vector2(-8f, -8f);

        return canvasGo;
    }

    // ---------------------------------------------------------------------
    // XR UI helpers (World Space canvas with Tracked Device Graphic Raycaster)
    // ---------------------------------------------------------------------

    static RectTransform NewUIElement(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static GameObject CreateUICanvas(string name, Vector3 position)
    {
        var canvasGo = new GameObject(name, typeof(RectTransform));
        canvasGo.transform.position = position;
        canvasGo.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        var rect = canvasGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(820f, 680f);
        canvasGo.transform.localScale = Vector3.one * 0.0018f;

        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();
        return canvasGo;
    }

    static GameObject CreateUIPanel(Transform parent, Vector2 size, Color color)
    {
        var rect = NewUIElement("Panel", parent);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        var image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        return rect.gameObject;
    }

    static GameObject CreateUIText(Transform parent, string text, Vector2 position, Vector2 size, int fontSize, TextAnchor anchor)
    {
        var rect = NewUIElement("Text", parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        var t = rect.gameObject.AddComponent<Text>();
        t.text = text;
        t.alignment = anchor;
        t.color = Color.white;
        t.fontSize = fontSize;
        t.font = GetBuiltinFont();
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return rect.gameObject;
    }

    static GameObject CreateUIButton(Transform parent, string label, Vector2 position, Vector2 size)
    {
        var rect = NewUIElement(label, parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.16f, 0.2f, 0.28f, 1f);

        var button = rect.gameObject.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.7f, 0.85f, 1f);
        colors.pressedColor = new Color(0.5f, 0.7f, 0.95f);
        colors.selectedColor = new Color(0.7f, 0.85f, 1f);
        button.colors = colors;
        button.targetGraphic = image;

        var textRect = NewUIElement("Text", rect);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var t = textRect.gameObject.AddComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.fontSize = 28;
        t.font = GetBuiltinFont();
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        return rect.gameObject;
    }

    static void WireColorButton(GameObject button, Renderer target, Color color)
    {
        var e = button.AddComponent<UIButtonEvent>();
        e.action = UIButtonEvent.ButtonAction.ChangeColor;
        e.color = color;
        e.colorTarget = target;
    }

    static GameObject CreateUISlider(Transform parent, Vector2 position, Vector2 size)
    {
        var rect = NewUIElement("Slider", parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var slider = rect.gameObject.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f;

        var bg = NewUIElement("Background", rect);
        bg.anchorMin = new Vector2(0f, 0.25f);
        bg.anchorMax = new Vector2(1f, 0.75f);
        bg.offsetMin = Vector2.zero;
        bg.offsetMax = Vector2.zero;
        bg.gameObject.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.12f, 0.9f);

        var fillArea = NewUIElement("Fill Area", rect);
        fillArea.anchorMin = new Vector2(0f, 0.25f);
        fillArea.anchorMax = new Vector2(1f, 0.75f);
        fillArea.offsetMin = new Vector2(5f, 0f);
        fillArea.offsetMax = new Vector2(-5f, 0f);
        var fill = NewUIElement("Fill", fillArea);
        fill.anchorMin = Vector2.zero;
        fill.anchorMax = new Vector2(1f, 1f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
        var fillImg = fill.gameObject.AddComponent<Image>();
        fillImg.color = new Color(0.25f, 0.55f, 0.95f);

        var handleArea = NewUIElement("Handle Slide Area", rect);
        handleArea.anchorMin = Vector2.zero;
        handleArea.anchorMax = Vector2.one;
        handleArea.offsetMin = new Vector2(10f, 0f);
        handleArea.offsetMax = new Vector2(-10f, 0f);
        var handle = NewUIElement("Handle", handleArea);
        handle.sizeDelta = new Vector2(20f, 0f);
        var handleImg = handle.gameObject.AddComponent<Image>();
        handleImg.color = new Color(0.9f, 0.95f, 1f);

        slider.fillRect = fill;
        slider.handleRect = handle;
        slider.targetGraphic = handleImg;

        return rect.gameObject;
    }

    static GameObject CreateUIToggle(Transform parent, string label, Vector2 position, Vector2 size)
    {
        var rect = NewUIElement("Toggle", parent);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var toggle = rect.gameObject.AddComponent<Toggle>();
        toggle.isOn = true;

        var bg = NewUIElement("Background", rect);
        bg.anchorMin = new Vector2(0f, 0.5f);
        bg.anchorMax = new Vector2(0f, 0.5f);
        bg.sizeDelta = new Vector2(36f, 36f);
        bg.anchoredPosition = new Vector2(18f, 0f);
        var bgImg = bg.gameObject.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);

        var check = NewUIElement("Checkmark", bg);
        check.anchorMin = Vector2.zero;
        check.anchorMax = Vector2.one;
        check.offsetMin = new Vector2(6f, 6f);
        check.offsetMax = new Vector2(-6f, -6f);
        var checkImg = check.gameObject.AddComponent<Image>();
        checkImg.color = new Color(0.3f, 0.9f, 0.4f);

        var textRect = NewUIElement("Label", rect);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(56f, 0f);
        textRect.offsetMax = Vector2.zero;
        var t = textRect.gameObject.AddComponent<Text>();
        t.text = label;
        t.alignment = TextAnchor.MiddleLeft;
        t.color = Color.white;
        t.fontSize = 24;
        t.font = GetBuiltinFont();
        t.horizontalOverflow = HorizontalWrapMode.Overflow;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;

        return rect.gameObject;
    }

    static Font s_Font;

    static Font GetBuiltinFont()
    {
        if (s_Font != null)
            return s_Font;
        s_Font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (s_Font == null)
            s_Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return s_Font;
    }

    static AudioClip CreateToneClip(string name, float frequency, float duration)
    {
        const int sampleRate = 44100;
        var samples = Mathf.CeilToInt(sampleRate * duration);
        var data = new float[samples];
        for (var i = 0; i < samples; i++)
        {
            var t = (float)i / sampleRate;
            var envelope = Mathf.Clamp01(1f - t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * 0.4f * envelope;
        }

        var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);

        Directory.CreateDirectory(LabsRoot + "/Audio");
        var path = LabsRoot + "/Audio/" + name + ".asset";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(clip, path);
        AssetDatabase.SaveAssets();
        return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
    }

    // ---------------------------------------------------------------------
    // Materials
    // ---------------------------------------------------------------------

    static Material GetMaterial(string name, Color color)
    {
        if (s_Materials.TryGetValue(name, out var cached))
            return cached;

        var path = MaterialsDir + "/" + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            var shader = Shader.Find("Standard");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");
            mat = new Material(shader) { color = color };
            AssetDatabase.CreateAsset(mat, path);
        }

        s_Materials[name] = mat;
        return mat;
    }

    static string ColorKey(Color c)
    {
        return Mathf.RoundToInt(c.r * 255) + "_" + Mathf.RoundToInt(c.g * 255) + "_" + Mathf.RoundToInt(c.b * 255);
    }
}
