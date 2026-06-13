using BepInEx.Configuration;
using MenuLib.MonoBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using TheEverythingMap.Helpers;
using TMPro;
using static MenuLib.MenuAPI;
using static TheEverythingMap.Settings.ConfigValues;

namespace TheEverythingMap.Settings;
//??
internal class SettingsMenu
{
    private static readonly string[] COLOR_DICTIONARY_KEYS = Extensions.VALID_COLORS.Keys.ToArray();

    private static readonly Dictionary<string, MinimapPosition> OPTION_TO_PRESET = new()
    {
        {
            nameof(MinimapPosition.TopRight),
            MinimapPosition.TopRight
        },
        {
            nameof(MinimapPosition.TopLeft),
            MinimapPosition.TopLeft
        },
        {
            nameof(MinimapPosition.MiddleRight),
            MinimapPosition.MiddleRight
        },
        {
            nameof(MinimapPosition.MiddleLeft),
            MinimapPosition.MiddleLeft
        },
        {
            nameof(MinimapPosition.BottomRight),
            MinimapPosition.BottomRight
        },
        {
            nameof(MinimapPosition.BottomLeft),
            MinimapPosition.BottomLeft
        }
    };

    private static List<ConfigEntry<KeyCode>> BINDABLE_KEYS =
    [
        SettingsKey,
        ZoomInKey,
        ZoomOutKey
    ];

    private static List<KeyCode> DISALLOWED_BINDINGS =
        [
            KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Escape
        ];

    private static REPOPopupPage? pageInstance;

    private static bool isSelectingKey = false;

    private static REPOButton? activatedButton;

    private static ConfigEntry<KeyCode>? keyToSet;

    internal static void Initialize()
    {
        AddElementToSettingsMenu(parent =>
        {
            CreateREPOButton("Nubez Minimap", View, parent, new Vector2(225f, 252f));
        });
    }

    internal static void Update()
    {
        if (StateUtils.IsInLevel() && !StateUtils.IsChatActive() && !StateUtils.HasLocalMapToolActive() && Input.GetKeyDown(SettingsKey.Value))
        {
            if (pageInstance == null)
            {
                View();
            }
            else
            {
                Close();
            }
        }
        if (StateUtils.IsInLevel() && StateUtils.HasLocalMapToolActive() && pageInstance != null)
        {
            Close();
        }

        if (!isSelectingKey || !Input.anyKeyDown)
        {
            return;
        }

        foreach (KeyCode value3 in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(value3))
            {
                if (!DISALLOWED_BINDINGS.Contains(value3))
                {
                    keyToSet!.Value = value3;
                }
                TextMeshProUGUI labelTMP = activatedButton!.labelTMP;
                string key = keyToSet!.Definition.Key;
                KeyCode value2 = keyToSet.Value;
                labelTMP.text = key + ": " + value2.ToString();
                isSelectingKey = false;
                keyToSet = null;
                activatedButton = null;
                break;
            }
        }
    }

    private static void Close()
    {
        if (!(pageInstance == null))
        {
            MenuManager.instance.PageRemove(pageInstance.menuPage);
            UnityEngine.Object.Destroy(pageInstance.menuPage.gameObject);
            pageInstance.ClosePage(true);
            pageInstance = null;
            isSelectingKey = false;
            keyToSet = null;
            activatedButton = null;
        }
    }

    private static void View()
    {
        Close();
        REPOPopupPage popupPage = CreateREPOPopupPage("Nubezz Minimap", 0, false, true, 0f);

        popupPage.AddElement(parent =>
        {
            CreateREPOButton("Back", Close, parent, new Vector2(66f, 18f));
        });

        popupPage.AddElementToScrollView(parent =>
        {
            REPOLabel generalLabel = CreateREPOLabel("General", parent, new Vector2(0f, 90f));
            return generalLabel.rectTransform;
        }, 0f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOButton resetButton = CreateREPOButton("Reset to Default Settings", () =>
            {
                ResetDefaultValues();
                View();
            }, parent, new Vector2(38f, 190f));
            resetButton.overrideButtonSize = new Vector2(resetButton.GetLabelSize().x * 0.8f, resetButton.GetLabelSize().y * 0.8f);
            resetButton.rectTransform.localScale = new Vector3(resetButton.rectTransform.localScale.x * 0.8f, resetButton.rectTransform.localScale.y * 0.8f, resetButton.rectTransform.localScale.z);
            return resetButton.rectTransform;
        }, 0f, 0f);

        string defaultMinimapOption = nameof(MinimapPosition.BottomLeft);
        foreach (KeyValuePair<string, MinimapPosition> minimapPositions in OPTION_TO_PRESET)
        {
            if (minimapPositions.Value == Preset.Value)
            {
                defaultMinimapOption = minimapPositions.Key;
                break;
            }
        }

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider minimapPositionSlider = CreateREPOSlider("Position Preset", "Built-in preset positions", str =>
            {
                Preset.Value = OPTION_TO_PRESET[str];
            }, parent, OPTION_TO_PRESET.Keys.ToArray(), defaultMinimapOption, new Vector2(0f, 270f), "", "", 0);
            return minimapPositionSlider.rectTransform;
        }, 0f, 0f);

        string defaultTeammateColor = DEFAULT_TEAMMATE_COLOR;
        foreach (string color in COLOR_DICTIONARY_KEYS)
        {
            if (color == TeammateColor.Value)
            {
                defaultTeammateColor = TeammateColor.Value;
                break;
            }
        }

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider teammateColorSlider = CreateREPOSlider("Teammate Colors", "Color of teammates on map", str =>
            {
                TeammateColor.Value = COLOR_DICTIONARY_KEYS[str];
            }, parent, COLOR_DICTIONARY_KEYS, defaultTeammateColor, new Vector2(0f, 270f), "", "", 0);
            return teammateColorSlider.rectTransform;
        }, 0f, 0f);

        string defaultDeadTeammateColor = DEFAULT_DEAD_TEAMMATE_COLOR;
        foreach (string color in COLOR_DICTIONARY_KEYS)
        {
            if (color == DeadTeammateColor.Value)
            {
                defaultDeadTeammateColor = DeadTeammateColor.Value;
                break;
            }
        }

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider deadTeammateColorSlider = CreateREPOSlider("Dead Teammate Color", "Color of DEAD teammates on map", str =>
            {
                DeadTeammateColor.Value = COLOR_DICTIONARY_KEYS[str];
            }, parent, COLOR_DICTIONARY_KEYS, defaultDeadTeammateColor, new Vector2(0f, 270f), "", "", 0);
            return deadTeammateColorSlider.rectTransform;
        }, 0f, 0f);


        popupPage.AddElementToScrollView(parent =>
        {
            REPOToggle showEnemiesToggle = CreateREPOToggle("Show Enemies", toggle =>
            {
                ShowEnemies.Value = toggle;
            }, parent, Vector2.zero, "Enabled", "Disabled", defaultValue: ShowEnemies.Value);

            return showEnemiesToggle.rectTransform;
        }, 0f, 0f);

        string defaultEnemyColor = DEFAULT_ENEMY_COLOR;
        foreach (string color in COLOR_DICTIONARY_KEYS)
        {
            if (color == EnemyColor.Value)
            {
                defaultEnemyColor = EnemyColor.Value;
                break;
            }
        }

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider enemyColorSlider = CreateREPOSlider("Enemy Colors", "Color of enemies on map", str =>
            {
                EnemyColor.Value = COLOR_DICTIONARY_KEYS[str];
            }, parent, COLOR_DICTIONARY_KEYS, defaultEnemyColor, new Vector2(0f, 270f), "", "", 0);
            return enemyColorSlider.rectTransform;
        }, 0f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOToggle showItemsToggle = CreateREPOToggle("Show Items ", toggle =>
            {
                ShowItems.Value = toggle;
                if (toggle)
                {
                    Mod.TheEverythingMap.ShowItems();
                }
            }, parent, Vector2.zero, "Enabled", "Disabled", defaultValue: ShowItems.Value);

            return showItemsToggle.rectTransform;
        }, 0f, 0f);

        string defaultItemColor = DEFAULT_ITEM_COLOR;
        foreach (string color in COLOR_DICTIONARY_KEYS)
        {
            if (color == ItemColor.Value)
            {
                defaultItemColor = ItemColor.Value;
                break;
            }
        }

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider itemColorSlider = CreateREPOSlider("Item Colors", "Color of items on map", str =>
            {
                ItemColor.Value = COLOR_DICTIONARY_KEYS[str];
            }, parent, COLOR_DICTIONARY_KEYS, defaultItemColor, new Vector2(0f, 270f), "", "", 0);
            return itemColorSlider.rectTransform;
        }, 0f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOToggle exploreMapToggle = CreateREPOToggle("Auto Explore Map ", toggle =>
            {
                ExploreMap.Value = toggle;
                if (toggle)
                {
                    Mod.TheEverythingMap.ExploreMap();
                }
            }, parent, Vector2.zero, "Enabled", "Disabled", defaultValue: ExploreMap.Value);

            return exploreMapToggle.rectTransform;
        }, 0f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider val14 = CreateREPOSlider("Size", "Minimap Size Height / Width", flt =>
            {
                WidthHeight.Value = (int)flt;
            }, parent, new Vector2(0, 150), 100, 800, 0, Mathf.Max(WidthHeight.Value, WidthHeight.Value), "", "", 0);
            return val14.rectTransform;
        }, 0f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider val13 = CreateREPOSlider("Zoom Level", "Minimap Zoom Level", flt =>
            {
                Zoom.Value = flt;
            }, parent, new Vector2(0f, 150f), 1.5f, 10f, 2, Zoom.Value, "", "", 0);
            return val13.rectTransform;
        }, 15f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOSlider val12 = CreateREPOSlider("Opacity", "Minimap Opacity", flt =>
            {
                Opacity.Value = flt;
            }, parent, new Vector2(0f, 150f), 0f, 1f, 2, Opacity.Value, "", "", 0);
            return val12.rectTransform;
        }, 15f, 0f);

        popupPage.AddElementToScrollView(parent =>
        {
            REPOLabel val10 = CreateREPOLabel("Controls", parent, new Vector2(0f, 90f));
            return val10.rectTransform;
        }, 15f, 0f);

        foreach (ConfigEntry<KeyCode> item in BINDABLE_KEYS)
        {
            popupPage.AddElementToScrollView(parent =>
            {
                string key = item.Definition.Key;
                KeyCode value = item.Value;
                REPOButton element = CreateREPOButton(key + ": " + value.ToString(), () =>
                {
                }, parent, new Vector2(0f, 190f));

                element.onClick = () =>
                {
                    if (!isSelectingKey)
                    {
                        element.labelTMP.text = item.Definition.Key + ": <PRESS ANY KEY>";
                        keyToSet = item;
                        activatedButton = element;
                        isSelectingKey = true;
                    }
                    else
                    {
                        isSelectingKey = false;
                        TextMeshProUGUI labelTMP = element.labelTMP;
                        string key2 = item.Definition.Key;
                        KeyCode value2 = item.Value;
                        labelTMP.text = key2 + ": " + value2.ToString();
                    }
                };
                element.overrideButtonSize = new Vector2(element.GetLabelSize().x * 0.8f, element.GetLabelSize().y * 0.8f);
                element.rectTransform.localScale = new Vector3(element.rectTransform.localScale.x * 0.8f, element.rectTransform.localScale.y * 0.8f, element.rectTransform.localScale.z);
                return element.rectTransform;
            }, 0f, 0f);
        }

        pageInstance = popupPage;
        popupPage.OpenPage(false);
    }
}