using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using BallisticSim.Core.Controllers;
using BallisticSim.Core.View;
using BallisticSim.Core.Spawner;
using BallisticSim.Core.Projectile;
using BallisticSim.Core.Camera;

namespace BallisticSim.Editor
{
    public static class SceneSetup
    {
        private const string PREFAB_PATH = "Assets/_Project/Prefabs";
        private const string MATERIAL_PATH = "Assets/_Project/Materials";

        [MenuItem("BallisticSim/Setup Full Scene")]
        public static void SetupFullScene()
        {
            CreateFolders();
            Material projectileMat = CreateMaterial("M_Projectile", Color.red);
            Material boxMat = CreateMaterial("M_Box", new Color(0.8f, 0.7f, 0.5f));
            Material weaponMat = CreateMaterial("M_Weapon", Color.gray);
            Material groundMat = CreateMaterial("M_Ground", new Color(0.3f, 0.35f, 0.3f));

            GameObject projectilePrefab = CreateProjectilePrefab(projectileMat);
            GameObject boxPrefab = CreateBoxPrefab(boxMat);
            GameObject weapon = CreateWeapon(weaponMat);
            GameObject gameManager = CreateGameManager();
            GameObject canvas = CreateCanvas();
            GameObject cameraRig = CreateCameraRig();
            GameObject ground = CreateGround(groundMat);

            WireReferences(gameManager, canvas, weapon, cameraRig, projectilePrefab, boxPrefab);

            EditorUtility.SetDirty(gameManager);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            Debug.Log("[SceneSetup] Full scene setup complete. Save the scene.");
        }

        private static void CreateFolders()
        {
            if (!AssetDatabase.IsValidFolder(PREFAB_PATH))
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            if (!AssetDatabase.IsValidFolder(MATERIAL_PATH))
                AssetDatabase.CreateFolder("Assets/_Project", "Materials");
        }

        private static Material CreateMaterial(string name, Color color)
        {
            string path = $"{MATERIAL_PATH}/{name}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            string[] guids = AssetDatabase.FindAssets("Lit t:Shader", new[] { "Packages/com.unity.render-pipelines.universal" });
            Shader shader = null;
            foreach (string guid in guids)
            {
                Shader candidate = AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(guid));
                if (candidate != null && candidate.name == "Universal Render Pipeline/Lit")
                {
                    shader = candidate;
                    break;
                }
            }
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        private static GameObject CreateProjectilePrefab(Material mat)
        {
            string path = $"{PREFAB_PATH}/Projectile.prefab";
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Projectile";
            sphere.transform.localScale = Vector3.one * 0.2f;

            sphere.GetComponent<Renderer>().sharedMaterial = mat;

            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            sphere.AddComponent<ProjectileSetup>();
            sphere.AddComponent<ProjectileTracking>();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(sphere, path);
            Object.DestroyImmediate(sphere);
            return prefab;
        }

        private static GameObject CreateBoxPrefab(Material mat)
        {
            string path = $"{PREFAB_PATH}/TargetBox.prefab";
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "TargetBox";

            cube.GetComponent<Renderer>().sharedMaterial = mat;

            Rigidbody rb = cube.AddComponent<Rigidbody>();
            rb.mass = 2f;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cube, path);
            Object.DestroyImmediate(cube);
            return prefab;
        }

        private static GameObject CreateWeapon(Material mat)
        {
            GameObject existing = GameObject.Find("Weapon");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject weapon = new GameObject("Weapon");
            weapon.transform.position = Vector3.zero;

            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.name = "Barrel";
            barrel.transform.SetParent(weapon.transform);
            barrel.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            barrel.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            barrel.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(barrel.GetComponent<Collider>());

            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.SetParent(weapon.transform);
            spawnPoint.transform.localPosition = new Vector3(0f, 1.05f, 0f);

            WeaponController wc = weapon.AddComponent<WeaponController>();
            SerializedObject so = new SerializedObject(wc);
            so.FindProperty("_spawnPoint").objectReferenceValue = spawnPoint.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            return weapon;
        }

        private static GameObject CreateGameManager()
        {
            GameObject existing = GameObject.Find("GameManager");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject gm = new GameObject("GameManager");
            gm.AddComponent<BallisticController>();
            gm.AddComponent<TargetSpawner>();
            return gm;
        }

        private static GameObject CreateGround(Material mat)
        {
            GameObject existing = GameObject.Find("Ground");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(50f, 1f, 50f);
            ground.GetComponent<Renderer>().sharedMaterial = mat;
            ground.isStatic = true;
            return ground;
        }

        private static GameObject CreateCanvas()
        {
            GameObject existing = GameObject.Find("UICanvas");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject canvasGo = new GameObject("UICanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            GameObject hudPanel = CreatePanel(canvasGo.transform, "HUDPanel",
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(10f, -10f), new Vector2(300f, 0f),
                new Color(0f, 0f, 0f, 0.5f));

            float yOffset = -10f;
            Slider angleSlider = CreateSlider(hudPanel.transform, "AngleSlider", ref yOffset, 0f, 90f, 45f);
            TMP_Text angleLabel = CreateLabel(hudPanel.transform, "AngleLabel", "Angle: 45.0°", ref yOffset);
            Slider forceSlider = CreateSlider(hudPanel.transform, "ForceSlider", ref yOffset, 10f, 100f, 20f);
            TMP_Text forceLabel = CreateLabel(hudPanel.transform, "ForceLabel", "Force: 20N", ref yOffset);
            Slider massSlider = CreateSlider(hudPanel.transform, "MassSlider", ref yOffset, 0.1f, 10f, 1f);
            TMP_Text massLabel = CreateLabel(hudPanel.transform, "MassLabel", "Mass: 1.00kg", ref yOffset);
            Slider sizeSlider = CreateSlider(hudPanel.transform, "BulletSizeSlider", ref yOffset, 0.05f, 1f, 0.2f);
            TMP_Text sizeLabel = CreateLabel(hudPanel.transform, "BulletSizeLabel", "Size: 0.20m", ref yOffset);
            Slider distanceSlider = CreateSlider(hudPanel.transform, "DistanceSlider", ref yOffset, 10f, 100f, 30f);
            TMP_Text distanceLabel = CreateLabel(hudPanel.transform, "DistanceLabel", "Distance: 30m", ref yOffset);
            Slider boxMassSlider = CreateSlider(hudPanel.transform, "BoxMassSlider", ref yOffset, 0.1f, 10f, 2f);
            TMP_Text boxMassLabel = CreateLabel(hudPanel.transform, "BoxMassLabel", "Box Mass: 2.0kg", ref yOffset);
            Slider jointForceSlider = CreateSlider(hudPanel.transform, "JointForceSlider", ref yOffset, 10f, 2000f, 200f);
            TMP_Text jointForceLabel = CreateLabel(hudPanel.transform, "JointForceLabel", "Joint Force: 200N", ref yOffset);

            Button fireButton = CreateButton(hudPanel.transform, "FireButton", "FIRE", ref yOffset, new Color(0.8f, 0.2f, 0.2f));

            GameObject telemetryPanel = CreatePanel(canvasGo.transform, "TelemetryPanel",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-10f, -10f), new Vector2(300f, 0f),
                new Color(0f, 0f, 0f, 0.5f));

            float telY = -5f;
            TMP_Text velocityText = CreateLabel(telemetryPanel.transform, "VelocityText", "Speed: ---", ref telY);
            TMP_Text positionText = CreateLabel(telemetryPanel.transform, "PositionText", "X:---  Y:---  Z:---", ref telY);

            GameObject reportPanel = CreatePanel(canvasGo.transform, "ReportPanel",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(400f, 0f),
                new Color(0.1f, 0.1f, 0.1f, 0.9f));

            float repY = -10f;
            TMP_Text titleText = CreateLabel(reportPanel.transform, "ReportTitle", "SHOT REPORT", ref repY);
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            TMP_Text repDistance = CreateLabel(reportPanel.transform, "ReportDistance", "Distance: ---", ref repY);
            TMP_Text repFlightTime = CreateLabel(reportPanel.transform, "ReportFlightTime", "Flight Time: ---", ref repY);
            TMP_Text repImpact = CreateLabel(reportPanel.transform, "ReportImpactPoint", "Impact: ---", ref repY);
            TMP_Text repVelocity = CreateLabel(reportPanel.transform, "ReportVelocity", "Rel. Velocity: ---", ref repY);
            TMP_Text repImpulse = CreateLabel(reportPanel.transform, "ReportImpulse", "Impulse: ---", ref repY);
            TMP_Text repBroken = CreateLabel(reportPanel.transform, "ReportBrokenJoints", "Broken Joints: ---", ref repY);
            TMP_Text repScore = CreateLabel(reportPanel.transform, "ReportScore", "Score: ---", ref repY);
            repScore.fontSize = 22;
            Button cleanButton = CreateButton(hudPanel.transform, "CleanSceneButton", "CLEAN SCENE", ref yOffset, new Color(0.3f, 0.5f, 0.3f));
            cleanButton.gameObject.SetActive(false);

            Button exportButton = CreateButton(reportPanel.transform, "ExportDataButton", "EXPORT DATA", ref repY, new Color(0.3f, 0.3f, 0.6f));

            reportPanel.SetActive(false);

            BallisticView view = canvasGo.AddComponent<BallisticView>();
            SerializedObject so = new SerializedObject(view);
            so.FindProperty("_angleSlider").objectReferenceValue = angleSlider;
            so.FindProperty("_forceSlider").objectReferenceValue = forceSlider;
            so.FindProperty("_massSlider").objectReferenceValue = massSlider;
            so.FindProperty("_bulletSizeSlider").objectReferenceValue = sizeSlider;
            so.FindProperty("_distanceSlider").objectReferenceValue = distanceSlider;
            so.FindProperty("_boxMassSlider").objectReferenceValue = boxMassSlider;
            so.FindProperty("_jointBreakForceSlider").objectReferenceValue = jointForceSlider;
            so.FindProperty("_angleLabelText").objectReferenceValue = angleLabel;
            so.FindProperty("_forceLabelText").objectReferenceValue = forceLabel;
            so.FindProperty("_massLabelText").objectReferenceValue = massLabel;
            so.FindProperty("_bulletSizeLabelText").objectReferenceValue = sizeLabel;
            so.FindProperty("_distanceLabelText").objectReferenceValue = distanceLabel;
            so.FindProperty("_boxMassLabelText").objectReferenceValue = boxMassLabel;
            so.FindProperty("_jointBreakForceLabelText").objectReferenceValue = jointForceLabel;
            so.FindProperty("_fireButton").objectReferenceValue = fireButton;
            so.FindProperty("_velocityText").objectReferenceValue = velocityText;
            so.FindProperty("_positionText").objectReferenceValue = positionText;
            so.FindProperty("_reportPanel").objectReferenceValue = reportPanel;
            so.FindProperty("_reportDistanceText").objectReferenceValue = repDistance;
            so.FindProperty("_reportFlightTimeText").objectReferenceValue = repFlightTime;
            so.FindProperty("_reportImpactPointText").objectReferenceValue = repImpact;
            so.FindProperty("_reportVelocityText").objectReferenceValue = repVelocity;
            so.FindProperty("_reportImpulseText").objectReferenceValue = repImpulse;
            so.FindProperty("_reportBrokenJointsText").objectReferenceValue = repBroken;
            so.FindProperty("_reportScoreText").objectReferenceValue = repScore;
            so.FindProperty("_cleanSceneButton").objectReferenceValue = cleanButton;
            so.FindProperty("_exportDataButton").objectReferenceValue = exportButton;
            so.ApplyModifiedPropertiesWithoutUndo();

            // EventSystem
            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            return canvasGo;
        }

        private static GameObject CreateCameraRig()
        {
            GameObject existing = GameObject.Find("CameraRig");
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject rig = new GameObject("CameraRig");

            GameObject overviewGo = new GameObject("OverviewCamera");
            overviewGo.transform.SetParent(rig.transform);
            CinemachineCamera overview = overviewGo.AddComponent<CinemachineCamera>();
            overviewGo.transform.position = new Vector3(-8f, 6f, -8f);
            overviewGo.transform.rotation = Quaternion.Euler(25f, 45f, 0f);
            overview.Priority = 20;

            GameObject trackingGo = new GameObject("TrackingCamera");
            trackingGo.transform.SetParent(rig.transform);
            CinemachineCamera tracking = trackingGo.AddComponent<CinemachineCamera>();
            tracking.Priority = 10;

            var thirdPerson = trackingGo.AddComponent<CinemachineThirdPersonFollow>();
            thirdPerson.Damping = new Vector3(0.5f, 0.5f, 0.5f);
            thirdPerson.ShoulderOffset = new Vector3(0.5f, 0.3f, 0f);
            thirdPerson.CameraDistance = 3f;

            CameraController cc = rig.AddComponent<CameraController>();
            SerializedObject so = new SerializedObject(cc);
            so.FindProperty("_overviewCamera").objectReferenceValue = overview;
            so.FindProperty("_trackingCamera").objectReferenceValue = tracking;
            so.ApplyModifiedPropertiesWithoutUndo();

            return rig;
        }

        private static void WireReferences(
            GameObject gameManager,
            GameObject canvas,
            GameObject weapon,
            GameObject cameraRig,
            GameObject projectilePrefab,
            GameObject boxPrefab)
        {
            BallisticController bc = gameManager.GetComponent<BallisticController>();
            SerializedObject bcSo = new SerializedObject(bc);
            bcSo.FindProperty("_view").objectReferenceValue = canvas.GetComponent<BallisticView>();
            bcSo.FindProperty("_weapon").objectReferenceValue = weapon.GetComponent<WeaponController>();
            bcSo.FindProperty("_spawner").objectReferenceValue = gameManager.GetComponent<TargetSpawner>();
            bcSo.FindProperty("_projectilePrefab").objectReferenceValue = projectilePrefab;
            bcSo.FindProperty("_cameraController").objectReferenceValue = cameraRig.GetComponent<CameraController>();
            bcSo.ApplyModifiedPropertiesWithoutUndo();

            TargetSpawner ts = gameManager.GetComponent<TargetSpawner>();
            SerializedObject tsSo = new SerializedObject(ts);
            tsSo.FindProperty("_boxPrefab").objectReferenceValue = boxPrefab;
            tsSo.ApplyModifiedPropertiesWithoutUndo();
        }

        private static GameObject CreatePanel(
            Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPosition, Vector2 size,
            Color bgColor)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = size;

            Image img = panel.AddComponent<Image>();
            img.color = bgColor;

            VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 10, 10);
            layout.spacing = 4f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = true;

            ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return panel;
        }

        private static Slider CreateSlider(Transform parent, string name, ref float yOffset, float min, float max, float value)
        {
            GameObject sliderGo = new GameObject(name);
            sliderGo.transform.SetParent(parent, false);

            RectTransform rt = sliderGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, 20f);

            LayoutElement le = sliderGo.AddComponent<LayoutElement>();
            le.preferredHeight = 20f;
            le.minHeight = 20f;

            Slider slider = sliderGo.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;

            GameObject background = new GameObject("Background");
            background.transform.SetParent(sliderGo.transform, false);
            RectTransform bgRt = background.AddComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            Image bgImg = background.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f);

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGo.transform, false);
            RectTransform fillAreaRt = fillArea.AddComponent<RectTransform>();
            fillAreaRt.anchorMin = Vector2.zero;
            fillAreaRt.anchorMax = Vector2.one;
            fillAreaRt.offsetMin = new Vector2(5f, 0f);
            fillAreaRt.offsetMax = new Vector2(-5f, 0f);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.AddComponent<RectTransform>();
            fillRt.sizeDelta = Vector2.zero;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.9f, 0.4f, 0.1f);

            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderGo.transform, false);
            RectTransform handleAreaRt = handleArea.AddComponent<RectTransform>();
            handleAreaRt.anchorMin = Vector2.zero;
            handleAreaRt.anchorMax = Vector2.one;
            handleAreaRt.offsetMin = new Vector2(10f, 0f);
            handleAreaRt.offsetMax = new Vector2(-10f, 0f);

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform handleRt = handle.AddComponent<RectTransform>();
            handleRt.sizeDelta = new Vector2(20f, 0f);
            Image handleImg = handle.AddComponent<Image>();
            handleImg.color = Color.white;

            slider.fillRect = fillRt;
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;

            yOffset -= 28f;
            return slider;
        }

        private static TMP_Text CreateLabel(Transform parent, string name, string text, ref float yOffset)
        {
            GameObject labelGo = new GameObject(name);
            labelGo.transform.SetParent(parent, false);

            RectTransform rt = labelGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, 22f);

            LayoutElement le = labelGo.AddComponent<LayoutElement>();
            le.preferredHeight = 22f;
            le.minHeight = 22f;

            TextMeshProUGUI tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 14;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;

            yOffset -= 24f;
            return tmp;
        }

        private static Button CreateButton(Transform parent, string name, string label, ref float yOffset, Color color)
        {
            GameObject btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);

            RectTransform rt = btnGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0f, 36f);

            LayoutElement le = btnGo.AddComponent<LayoutElement>();
            le.preferredHeight = 36f;
            le.minHeight = 36f;

            Image img = btnGo.AddComponent<Image>();
            img.color = color;

            Button btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = img;

            GameObject textGo = new GameObject("Text");
            textGo.transform.SetParent(btnGo.transform, false);
            RectTransform textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            yOffset -= 40f;
            return btn;
        }
    }
}
