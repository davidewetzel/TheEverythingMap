using BepInEx;
using HarmonyLib;
using Photon.Pun;
using System.Linq;
using TheEverythingMap.Helpers;
using TheEverythingMap.Settings;
using Unity.VisualScripting;

namespace TheEverythingMap.Mod;

[BepInPlugin(MOD_GUID, nameof(TheEverythingMap), VERSION_NUMBER)]
public class TheEverythingMap : BaseUnityPlugin
{
    // TODO: need to rename everything to be readable, a lot of it was taken from decompiled code 
    private const string MOD_GUID = "Nubez.TheEverythingMap";
    private const string VERSION_NUMBER = "1.0.7";
    private const string SPECTATE_CAMERA_NAME = "Dirt Finder Map Camera";

    internal static TheEverythingMap Instance { get; private set; } = null!;

    internal Harmony? Harmony { get; set; }

    internal const float GOAL_AND_HAUL_Y_OFFSET = -95f;

    private Camera? _camera;

    private RenderTexture? _renderTexture;

    private float defaultCameraZoom = -1f;

    private Vector2 _initialHaulShowPosition = Vector2.zero;

    private Vector2 targetHaulShowPosition = Vector2.zero;

    private Vector2 _initialGoalShowPosition = Vector2.zero;

    private Vector2 targetGoalShowPosition = Vector2.zero;

    private float targetSetHeight = -1f;

    private static Sprite? spriteCircle = SpriteHelper.CreateCircleSprite();

    //private static Sprite? enemySpriteTriangle;

    private void Start()
    {
        SettingsMenu.Initialize();
    }

    private void Awake()
    {
        Instance = this;
        Patch();
        gameObject.transform.parent = null;
        gameObject.hideFlags = (HideFlags)61;
        ConfigValues.Configure(this);
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        Bindings.Update();
        SettingsMenu.Update();

        if (!StateUtils.IsInLevel())
        {
            if (_camera != null || _renderTexture != null)
            {
                _camera = null;
                _renderTexture = null;
            }

            return;
        }

        if (_camera == null
            && FindObjectsOfType<Camera>(includeInactive: true).Any(cam => cam.name == SPECTATE_CAMERA_NAME))
        {
            _camera = FindObjectsOfType<Camera>(includeInactive: true).First(cam => cam.name == SPECTATE_CAMERA_NAME);
        }

        if (targetHaulShowPosition == Vector2.zero || targetGoalShowPosition == Vector2.zero)
        {
            _initialHaulShowPosition = HaulUI.instance.showPosition;
            _initialGoalShowPosition = GoalUI.instance.showPosition;
            float num = ConfigValues.WidthHeight.Value / 300f;
            targetHaulShowPosition = new Vector2(_initialHaulShowPosition.x, _initialHaulShowPosition.y + -95f * num);
            targetGoalShowPosition = new Vector2(_initialGoalShowPosition.x, _initialGoalShowPosition.y + -95f * num);
        }

        if (targetSetHeight != ConfigValues.WidthHeight.Value)
        {
            float num2 = ConfigValues.WidthHeight.Value / 300f;
            targetHaulShowPosition.y = _initialHaulShowPosition.y + -95f * num2;
            targetGoalShowPosition.y = _initialGoalShowPosition.y + -95f * num2;
            targetSetHeight = ConfigValues.WidthHeight.Value;
        }

        if (ConfigValues.Preset.Value == MinimapPosition.TopRight && (HaulUI.instance.showPosition != targetHaulShowPosition || GoalUI.instance.showPosition != targetGoalShowPosition))
        {
            HaulUI.instance.showPosition = targetHaulShowPosition;
            GoalUI.instance.showPosition = targetGoalShowPosition;
        }
        else if (ConfigValues.Preset.Value != 0 && (HaulUI.instance.showPosition != _initialHaulShowPosition || GoalUI.instance.showPosition != _initialGoalShowPosition))
        {
            HaulUI.instance.showPosition = _initialHaulShowPosition;
            GoalUI.instance.showPosition = _initialGoalShowPosition;
        }

        if (Map.Instance != null && !Map.Instance.Active)
        {
            Map.Instance.ActiveSet(true);
        }

        if (_camera != null && (_renderTexture == null || _camera.activeTexture != null && _camera.activeTexture != _renderTexture))
        {
            _renderTexture = _camera.activeTexture;
        }

        if (_camera != null)
        {
            if (defaultCameraZoom == -1f)
            {
                defaultCameraZoom = _camera.orthographicSize;
            }

            float cameraZoom = !StateUtils.HasLocalMapToolActive() ? ConfigValues.Zoom.Value : defaultCameraZoom;
            if (_camera.orthographicSize != cameraZoom)
            {
                _camera.orthographicSize = cameraZoom;
            }
        }

        if (PlayerAvatar.instance != null && PlayerAvatar.instance.spectating
            && SpectateCamera.instance != null && SpectateCamera.instance.currentState == SpectateCamera.State.Normal)
        {
            Transform transform = SpectateCamera.instance.transform;
            PlayerAvatar player = SpectateCamera.instance.player;
            Quaternion rotation;

            if (player != null)
            {
                DirtFinderMapPlayer.Instance.PlayerTransform.position = player.transform.position;
                Transform playerTransform = DirtFinderMapPlayer.Instance.PlayerTransform;
                rotation = player.transform.rotation;
                playerTransform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
                PlayerController.instance.playerAvatarScript.LastNavmeshPosition = player.LastNavmeshPosition;
            }
            else if (transform != null)
            {
                DirtFinderMapPlayer.Instance.PlayerTransform.position = transform.position;
                Transform playerTransform = DirtFinderMapPlayer.Instance.PlayerTransform;
                rotation = transform.rotation;
                playerTransform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, rotation.eulerAngles.z);
            }
        }
    }

    private void OnGUI()
    {
        if (!StateUtils.IsInLevel() || _renderTexture == null || StateUtils.HasLocalMapToolActive())
        {
            return;
        }

        int xCoordinate;
        int yCoordinate;

        switch (ConfigValues.Preset.Value)
        {
            case MinimapPosition.TopLeft:
                xCoordinate = ConfigValues.Buffer.Value;
                yCoordinate = ConfigValues.Buffer.Value * 2;
                break;
            case MinimapPosition.MiddleRight:
                xCoordinate = Screen.width - ConfigValues.WidthHeight.Value - ConfigValues.Buffer.Value;
                yCoordinate = (Screen.height - ConfigValues.WidthHeight.Value) / 2;
                break;
            case MinimapPosition.TopRight:
                xCoordinate = Screen.width - ConfigValues.WidthHeight.Value - ConfigValues.Buffer.Value;
                yCoordinate = ConfigValues.Buffer.Value * 2;
                break;
            case MinimapPosition.BottomRight:
                xCoordinate = Screen.width - ConfigValues.WidthHeight.Value - ConfigValues.Buffer.Value;
                yCoordinate = Screen.height - ConfigValues.WidthHeight.Value - ConfigValues.Buffer.Value;
                break;
            case MinimapPosition.MiddleLeft:
                xCoordinate = ConfigValues.Buffer.Value;
                yCoordinate = (Screen.height - ConfigValues.WidthHeight.Value) / 2;
                break;
            case MinimapPosition.BottomLeft:
            default:
                xCoordinate = ConfigValues.Buffer.Value;
                yCoordinate = Screen.height - ConfigValues.WidthHeight.Value - ConfigValues.Buffer.Value;
                break;
        }

        Color color = GUI.color;
        if (ConfigValues.Opacity.Value != 1f)
        {
            GUI.color = new Color(1f, 1f, 1f, ConfigValues.Opacity.Value);
        }

        GUI.DrawTexture(new Rect(xCoordinate, yCoordinate, ConfigValues.WidthHeight.Value, ConfigValues.WidthHeight.Value), _renderTexture, ScaleMode.StretchToFill, false);

        if (ConfigValues.Opacity.Value != 1f)
        {
            GUI.color = color;
        }
    }

    [HarmonyPatch(typeof(PlayerAvatar))]
    public static class PlayerAvatarRPCPatch
    {
        [HarmonyPatch(nameof(ReviveRPC))]
        [HarmonyPostfix]
        public static void ReviveRPC(bool _revivedByTruck, PhotonMessageInfo _info = default)
        {
            ShowActivePlayersOnMap();
        }

        [HarmonyPatch(nameof(PlayerDeathRPC))]
        [HarmonyPostfix]
        public static void PlayerDeathRPC(int enemyIndex, PhotonMessageInfo _info = default)
        {
            ShowActivePlayersOnMap();
        }

        [HarmonyPatch(nameof(PlayerDeathDone))]
        [HarmonyPostfix]
        private static void PlayerDeathDone()
        {
            if (GameDirector.instance != null
                && GameDirector.instance.PlayerList != null && GameDirector.instance.PlayerList.Any(player => player != null && player.gameObject != null))
            {
                foreach (PlayerDeathHead playerDeathHead in FindObjectsOfType<PlayerDeathHead>())
                {
                    playerDeathHead.SeenSetRPC(true);
                }
            }
        }
    }

    [HarmonyPatch(typeof(LevelGenerator), nameof(LevelGenerator.GenerateDone))]
    public static class LevelGeneratorGenerateDoneHookPatch
    {
        private static void Postfix(PhotonMessageInfo _info = default)
        {
            ShowActivePlayersOnMap();
            ShowItems();
            ExploreMap();
        }
    }

    public static void ShowItems()
    {
        if (ConfigValues.ShowItems.Value
            && FindObjectsOfType<ValuableDiscoverCustom>().Any())
        {
            foreach (ValuableObject item in FindObjectsOfType<ValuableObject>())
            {
                item.discovered = true;
                item.Discover(ValuableDiscoverGraphic.State.Discover);
                Map.Instance.AddValuable(item);
            }

            foreach (MapValuable mapComponent in FindObjectsOfType<MapValuable>())
            {
                mapComponent.spriteRenderer.color = ConfigValues.ItemColor.Value.ToColor();

            }

            foreach (ValuableDiscoverCustom item in FindObjectsOfType<ValuableDiscoverCustom>())
            {
                item.Discover();
            }
        }
    }

    public static void ExploreMap()
    {
        if (ConfigValues.ExploreMap.Value && Map.Instance != null)
        {
            foreach (RoomVolume item in Object.FindObjectsOfType<RoomVolume>())
            {
                item.SetExplored();
            }
        }
    }


    public static void ShowActivePlayersOnMap()
    {
        if (GameDirector.instance != null
            && GameDirector.instance.PlayerList != null && GameDirector.instance.PlayerList.Any(player => player != null && player.gameObject != null))
        {
            foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList.Where(player => player != null && player.gameObject != null))
            {
                MapCustom playerMapCustom = playerAvatar.GetComponent<MapCustom>();

                if (playerMapCustom != null)
                {
                    Destroy(playerAvatar.GetComponent<MapCustom>());
                }

                playerAvatar.AddComponent<MapCustom>();

                playerMapCustom = playerAvatar.GetComponent<MapCustom>();
                playerMapCustom.sprite = spriteCircle;

                // TODO: if i want to get color of player heads, it should be something like this, but not working, maybe pass in true to method
                //item.playerCosmetics.SetupColors(false);
                // MetaManager.instance.colors[item.playerCosmetics.colorsEquipped[5]].color;

                Color spriteColor = ConfigValues.TeammateColor.Value.ToColor();
                playerMapCustom.color = spriteColor;
                playerMapCustom.Add();

            }

            foreach (PlayerDeathHead playerDeathHead in FindObjectsOfType<PlayerDeathHead>())
            {
                playerDeathHead.mapCustom.color = ConfigValues.DeadTeammateColor.Value.ToColor();
            }
        }
    }

    [HarmonyPatch(typeof(EnemyParent), "SpawnRPC")]
    public static class EnemySpawnRPCHookPatch
    {
        private static void Postfix(EnemyParent __instance, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            ShowAllEnemies();
        }
    }

    [HarmonyPatch(typeof(EnemyParent), "DespawnRPC")]
    public static class EnemyDeSpawnRPCHookPatch
    {
        private static void Postfix(EnemyParent __instance, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            ShowAllEnemies();
        }
    }

    [HarmonyPatch(typeof(EnemyHealth), "DeathRPC")]
    public static class EnemyDeathRPCHookPatch
    {
        private static void Postfix(EnemyHealth __instance, Vector3 _deathDirection, PhotonMessageInfo _info = default(PhotonMessageInfo))
        {
            ShowAllEnemies();
        }
    }

    private static void ShowAllEnemies()
    {
        if (ConfigValues.ShowEnemies.Value
            && FindObjectsOfType<EnemyParent>().Any(enemyParent => enemyParent != null && enemyParent.gameObject != null))
        {
            foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
            {
                if (enemyParent != null
                    && enemyParent.isActiveAndEnabled
                    && enemyParent.Enemy.Health != null
                    && !enemyParent.Enemy.Health.dead
                    && enemyParent.Enemy.CurrentState != EnemyState.Despawn
                    && enemyParent.Enemy.CurrentState != EnemyState.None)
                {
                    MapCustom mapCustom = enemyParent.Enemy.GetComponent<MapCustom>();
                    if (mapCustom == null)
                    {
                        mapCustom = enemyParent.Enemy.AddComponent<MapCustom>();
                        mapCustom.autoAdd = false;
                        mapCustom.sprite = spriteCircle;
                        mapCustom.color = ConfigValues.EnemyColor.Value.ToColor();
                        mapCustom.transform.localScale = Vector3.one * enemyParent.Enemy.Type.ToScale();
                        mapCustom.Add();
                    }
                }
                else if (enemyParent != null && enemyParent.gameObject != null)
                {
                    MapCustom mapCustom = enemyParent!.Enemy.GetComponent<MapCustom>();

                    if (mapCustom != null)
                    {
                        mapCustom.mapCustomEntity.spriteRenderer.sprite = SpriteHelper.CreateClearSprite();
                        mapCustom.Hide();
                        Destroy(mapCustom);
                    }
                }
            }
        }
    }
}