using UnityEngine;
using UnityEditor;

namespace OccaSoftware.SuperSimpleSkybox.Editor
{
    public class StartMenu : EditorWindow
    {
        // Source for UUID: https://shortunique.id/
        private static string modalId = "ShowSuperSimpleSkyboxModal=J4aU7C";
        private Texture2D logo;
        private GUIStyle header,
            button,
            contentSection;
        private GUILayoutOption[] contentLayoutOptions;
        private static bool listenToEditorUpdates;
        private static StartMenu startMenu;

        [MenuItem("OccaSoftware/Start Menu (SuperSimpleSkybox)")]
        public static void SetupMenu()
        {
            startMenu = CreateWindow();
            CenterWindowInEditor(startMenu);
            LoadLogo(startMenu);
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            RegisterModal();
        }

        void OnGUI()
        {
            SetupHeaderStyle(startMenu);
            SetupButtonStyle(startMenu);
            SetupContentSectionStyle(startMenu);

            DrawHeader();
            DrawReviewRequest();
            DrawHelpLinks();
            DrawUpgradeLinks();
        }

        #region Setup
        private static StartMenu CreateWindow()
        {
            StartMenu startMenu = (StartMenu)GetWindow(typeof(StartMenu));
            startMenu.position = new Rect(0, 0, 270, 480);
            return startMenu;
        }

        private static void CenterWindowInEditor(EditorWindow startMenu)
        {
            Rect mainWindow = EditorGUIUtility.GetMainWindowPosition();
            Rect currentWindowPosition = startMenu.position;
            float centerX = (mainWindow.width - currentWindowPosition.width) * 0.5f;
            float centerY = (mainWindow.height - currentWindowPosition.height) * 0.5f;
            currentWindowPosition.x = mainWindow.x + centerX;
            currentWindowPosition.y = mainWindow.y + centerY;
            startMenu.position = currentWindowPosition;
        }

        private static void LoadLogo(StartMenu startMenu)
        {
            startMenu.logo = (Texture2D)
                AssetDatabase.LoadAssetAtPath("Packages/com.occasoftware.super-simple-skybox/Editor/Textures/Logo.png", typeof(Texture2D));
        }

        private static void SetupHeaderStyle(StartMenu startMenu)
        {
            startMenu.header = new GUIStyle(EditorStyles.boldLabel);
            startMenu.header.fontSize = 18;
            startMenu.header.wordWrap = true;
            startMenu.header.padding = new RectOffset(0, 0, 0, 0);
        }

        private static void SetupButtonStyle(StartMenu startMenu)
        {
            startMenu.button = new GUIStyle("button");
            startMenu.button.fontSize = 18;
            startMenu.button.fontStyle = FontStyle.Bold;
            startMenu.button.fixedHeight = 40;
        }

        private static void SetupContentSectionStyle(StartMenu startMenu)
        {
            startMenu.contentSection = new GUIStyle("label");
            startMenu.contentSection.margin = new RectOffset(20, 20, 20, 20);
            startMenu.contentSection.padding = new RectOffset(0, 0, 0, 0);
            startMenu.contentLayoutOptions = new GUILayoutOption[] { GUILayout.MinWidth(230) };
        }
        #endregion


        #region Modal Handler
        private static void RegisterModal()
        {
            if (!listenToEditorUpdates && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                listenToEditorUpdates = true;
                EditorApplication.update += PopModal;
            }
        }

        private static void PopModal()
        {
            EditorApplication.update -= PopModal;

            bool showModal = EditorPrefs.GetBool(modalId, true);
            if (showModal)
            {
                EditorPrefs.SetBool(modalId, false);
                SetupMenu();
            }
        }
        #endregion



        #region UI Drawer
        private void DrawHeader()
        {
            GUILayout.BeginVertical(contentSection, contentLayoutOptions);
            GUIStyle logoStyle = new GUIStyle("label");
            GUILayoutOption[] logoOptions = new GUILayoutOption[] { GUILayout.Width(230) };
            logoStyle.padding = new RectOffset(0, 0, 0, 0);
            logoStyle.margin = new RectOffset(0, 0, 0, 0);
            logoStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(logo, logoStyle, logoOptions);
            GUILayout.EndVertical();
        }

        static string reviewButtonText = "Leave a review";
        static string reviewButtonHeader = "What do you think about Super Simple Skybox?";
        static string assetURL = "https://assetstore.unity.com/packages/slug/210177";

        static string helpSectionHeader = "Get support";

        static ButtonData[] helpSectionButtons =
        {
            new ButtonData("Website", "https://www.occasoftware.com/"),
            new ButtonData("Manual", "https://www.occasoftware.com/manual/super-simple-skybox"),
            new ButtonData("Discord", "https://www.occasoftware.com/discord")
        };

        static string upgradeSectionHeader = "Make your game a success";
        static ButtonData[] upgradeSectionButtons =
        {
            new ButtonData("Join the Newsletter", "https://www.occasoftware.com/newsletter"),
            new ButtonData("Get the Membership Bundle", "https://www.occasoftware.com/occasoftware-membership-bundle")
        };

        struct ButtonData
        {
            public string text;
            public string url;

            public ButtonData(string buttonText, string buttonUrl)
            {
                text = buttonText;
                url = buttonUrl;
            }
        }

        private void DrawReviewRequest()
        {
            GUILayout.BeginVertical(contentSection, contentLayoutOptions);
            GUILayout.Label(reviewButtonHeader, header);

            if (GUILayout.Button(reviewButtonText, button, new GUILayoutOption[] { GUILayout.MaxWidth(300) }))
            {
                Application.OpenURL(assetURL);
            }
            GUILayout.EndVertical();
        }

        private void DrawHelpLinks()
        {
            GUILayout.BeginVertical(contentSection, contentLayoutOptions);

            GUILayout.Label(helpSectionHeader, header);
            foreach (var button in helpSectionButtons)
            {
                if (EditorGUILayout.LinkButton(button.text))
                {
                    Application.OpenURL(button.url);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawUpgradeLinks()
        {
            GUILayout.BeginVertical(contentSection, contentLayoutOptions);

            GUILayout.Label(upgradeSectionHeader, header);
            foreach (var button in upgradeSectionButtons)
            {
                if (EditorGUILayout.LinkButton(button.text))
                {
                    Application.OpenURL(button.url);
                }
            }
            EditorGUILayout.EndVertical();
        }
        #endregion
    }
}
