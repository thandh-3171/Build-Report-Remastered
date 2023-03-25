using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace BuildReportRemastered.Window.Screen
{
    public class Options : BaseScreen
    {
        public override string Name
        {
            get { return Labels.OPTIONS_CATEGORY_LABEL; }
        }

        string[] _saveTypeLabels;

        /// <summary>
        /// 0: always use configured file filters <br/>
        /// 1: use file filters embedded in opened build report, if available
        /// </summary>
        static readonly string[] FileFilterToUseType =
            {Labels.FILTER_GROUP_TO_USE_CONFIGURED_LABEL, Labels.FILTER_GROUP_TO_USE_EMBEDDED_LABEL};

        /// <summary>
        /// 0: mouse is hovering over icon <br/>
        /// 1: mouse is hovering over icon or label
        /// </summary>
        static readonly string[] ShowThumbnailOnHoverTypeLabels =
            {"Mouse is hovering over asset's icon", "Mouse is hovering over asset's icon or label"};

        /// <summary>
        /// 0: dedicated ping button before each asset <br/>
        /// 1: double-click on asset label will ping
        /// </summary>
        static readonly string[] AssetPingTypeLabels =
            {"Dedicated ping button before each asset", "Double-clicking on asset will ping"};

        /// <summary>
        /// 0: verbose <br/>
        /// 1: standard <br/>
        /// 2: minimal
        /// </summary>
        static readonly string[] AssetUsageLabelTypeLabels =
        {
            "Verbose\n(use words only)",
            "Standard\n(use arrows when possible, show any extra info with words)",
            "Minimal\n(use arrows only, don't show any extra info even if available)"
        };

        static readonly string[] SearchTypeLabels =
        {
            "Basic",
            "Regex",
            "Fuzzy Search"
        };

        string OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL
        {
            get
            {
                if (BuildReportRemastered.Util.IsInWinOS)
                    return Labels.OPEN_IN_FILE_BROWSER_WIN_LABEL;
                if (BuildReportRemastered.Util.IsInMacOS)
                    return Labels.OPEN_IN_FILE_BROWSER_MAC_LABEL;

                return Labels.OPEN_IN_FILE_BROWSER_DEFAULT_LABEL;
            }
        }

        string SAVE_PATH_TYPE_PERSONAL_OS_SPECIFIC_LABEL
        {
            get
            {
                if (BuildReportRemastered.Util.IsInWinOS)
                    return Labels.SAVE_PATH_TYPE_PERSONAL_WIN_LABEL;
                if (BuildReportRemastered.Util.IsInMacOS)
                    return Labels.SAVE_PATH_TYPE_PERSONAL_MAC_LABEL;

                return Labels.SAVE_PATH_TYPE_PERSONAL_DEFAULT_LABEL;
            }
        }


        static readonly string[] CalculationTypeLabels =
        {
            Labels.CALCULATION_LEVEL_FULL_NAME,
            Labels.CALCULATION_LEVEL_NO_PREFAB_NAME,
            Labels.CALCULATION_LEVEL_NO_UNUSED_NAME,
            Labels.CALCULATION_LEVEL_MINIMAL_NAME
        };

        int _selectedCalculationLevelIdx;

        string CalculationLevelDescription
        {
            get
            {
                switch (_selectedCalculationLevelIdx)
                {
                    case 0:
                        return Labels.CALCULATION_LEVEL_FULL_DESC;
                    case 1:
                        return Labels.CALCULATION_LEVEL_NO_PREFAB_DESC;
                    case 2:
                        return Labels.CALCULATION_LEVEL_NO_UNUSED_DESC;
                    case 3:
                        return Labels.CALCULATION_LEVEL_MINIMAL_DESC;
                }

                return "";
            }
        }

        int GetCalculationLevelGuiIdxFromOptions()
        {
            if (BuildReportRemastered.Options.IsCurrentCalculationLevelAtFull)
            {
                return 0;
            }

            if (BuildReportRemastered.Options.IsCurrentCalculationLevelAtNoUnusedPrefabs)
            {
                return 1;
            }

            if (BuildReportRemastered.Options.IsCurrentCalculationLevelAtNoUnusedAssets)
            {
                return 2;
            }

            if (BuildReportRemastered.Options.IsCurrentCalculationLevelAtOverviewOnly)
            {
                return 3;
            }

            return 0;
        }

        void SetCalculationLevelFromGuiIdx(int selectedIdx)
        {
            switch (selectedIdx)
            {
                case 0:
                    BuildReportRemastered.Options.SetCalculationLevelToFull();
                    break;
                case 1:
                    BuildReportRemastered.Options.SetCalculationLevelToNoUnusedPrefabs();
                    break;
                case 2:
                    BuildReportRemastered.Options.SetCalculationLevelToNoUnusedAssets();
                    break;
                case 3:
                    BuildReportRemastered.Options.SetCalculationLevelToOverviewOnly();
                    break;
            }
        }


        Vector2 _assetListScrollPos;


        public override void RefreshData(BuildInfo buildReport, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData, UnityBuildReport unityBuildReport)
        {
            if (_saveTypeLabels == null)
            {
                _saveTypeLabels = new[] { SAVE_PATH_TYPE_PERSONAL_OS_SPECIFIC_LABEL, Labels.SAVE_PATH_TYPE_PROJECT_LABEL };
            }

            _selectedCalculationLevelIdx = GetCalculationLevelGuiIdxFromOptions();
        }

        GUIStyle _linkStyle;
        GUIStyle _textBesideLinkStyle;

        static readonly GUILayoutOption[] LayoutMinWidth100 = new[] { GUILayout.MinWidth(100) };
        static readonly GUILayoutOption[] LayoutMinWidth200 = new[] { GUILayout.MinWidth(200) };
        static readonly GUILayoutOption[] LayoutMinWidth250 = new[] { GUILayout.MinWidth(250) };
        static readonly GUILayoutOption[] LayoutMaxWidth525 = new[] { GUILayout.MaxWidth(525) };
        static readonly GUILayoutOption[] LayoutMaxWidth593 = new[] { GUILayout.MaxWidth(593) };
        static readonly GUILayoutOption[] LayoutMaxWidth848 = new[] { GUILayout.MaxWidth(848) };
        static readonly GUILayoutOption[] LayoutMaxWidth500MinHeight75 = new[] { GUILayout.MaxWidth(500), GUILayout.MinHeight(75) };

        static readonly GUILayoutOption[] LayoutWidth300 = new[] { GUILayout.Width(300) };
        static readonly GUILayoutOption[] LayoutHeight26 = new[] { GUILayout.Height(26) };

        static readonly GUILayoutOption[] LayoutNoExpandWidth = new[] { GUILayout.ExpandWidth(false) };

        string _hoveredControlTooltipText;
        readonly GUIContent _tooltipLabel = new GUIContent();

        ReorderableList _ignorePatternList;
        readonly GUIContent _basicSearchRadioLabel = new GUIContent("Basic");
        readonly GUIContent _regexSearchRadioLabel = new GUIContent("Regex");

        Texture2D _iconValid;
        Texture2D _iconInvalid;

        public override void DrawGUI(Rect position,
            BuildInfo buildReportToDisplay, AssetDependencies assetDependencies, TextureData textureData, MeshData meshData,
            UnityBuildReport unityBuildReport,
            out bool requestRepaint
        )
        {
            if (Event.current.type == EventType.Repaint)
            {
                _hoveredControlTooltipText = null;
            }

            var validityStyle = GUI.skin.FindStyle("IconValidity");
            if (validityStyle != null)
            {
                _iconValid = validityStyle.normal.background;
                _iconInvalid = validityStyle.hover.background;
            }

            requestRepaint = true;

            if (!BRT_BuildReportWindow.MouseMovedNow && !BRT_BuildReportWindow.LastMouseMoved)
            {
                // mouse hasn't moved
                // no need to repaint because nothing has changed
                // set requestRepaint to false to help lessen cpu usage
                requestRepaint = false;
            }

            var boxedLabelStyle = GUI.skin.FindStyle(BuildReportRemastered.Window.Settings.BOXED_LABEL_STYLE_NAME);
            if (boxedLabelStyle == null)
            {
                boxedLabelStyle = GUI.skin.box;
            }

            var header1Style = GUI.skin.FindStyle(BuildReportRemastered.Window.Settings.INFO_TITLE_STYLE_NAME);
            if (header1Style == null)
            {
                header1Style = GUI.skin.label;
            }

            var header2Style = GUI.skin.FindStyle(BuildReportRemastered.Window.Settings.SUB_TITLE_STYLE_NAME);
            if (header2Style == null)
            {
                header2Style = GUI.skin.label;
            }

            var prevEnabled = GUI.enabled;

            GUILayout.Space(10); // extra top padding


            _assetListScrollPos = GUILayout.BeginScrollView(_assetListScrollPos, BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20); // extra left padding
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            if (!string.IsNullOrEmpty(BuildReportRemastered.Options.FoundPathForSavedOptions))
            {
                GUILayout.BeginHorizontal(boxedLabelStyle, BRT_BuildReportWindow.LayoutNone);
                GUILayout.Label(string.Format("Using options file in: {0}",
                    BuildReportRemastered.Options.FoundPathForSavedOptions), BRT_BuildReportWindow.LayoutNone);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reload", BRT_BuildReportWindow.LayoutNone))
                {
                    BuildReportRemastered.Options.RefreshOptions();
                }

                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }

            // === Main Options ===

            GUILayout.Label("Main Options", header1Style, BRT_BuildReportWindow.LayoutNone);


            BuildReportRemastered.Options.CollectBuildInfo = GUILayout.Toggle(BuildReportRemastered.Options.CollectBuildInfo,
                "Automatically generate and save a Build Report file after building (does not include batchmode builds)", BRT_BuildReportWindow.LayoutNone);
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);
            GUILayout.Label(
                "Note: For batchmode builds, to create build reports, call <b>BuildReportTool.ReportGenerator.CreateReport()</b> after <b>BuildPipeline.BuildPlayer()</b> in your build scripts.\n\nAlso call <b>BuildReportTool.ReportGenerator.OnPreBuild()</b> in your <b>OnPreprocessBuild()</b> methods so the build time can be recorded properly.\n\nThe Build Report is automatically saved as an XML file.",
                boxedLabelStyle, LayoutMaxWidth593);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            BuildReportRemastered.Options.AutoShowWindowAfterNormalBuild = GUILayout.Toggle(
                BuildReportRemastered.Options.AutoShowWindowAfterNormalBuild,
                "Automatically show Build Report Window after building (if it is not open yet)", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.AutoResortAssetsWhenUnityEditorRegainsFocus = GUILayout.Toggle(
                BuildReportRemastered.Options.AutoResortAssetsWhenUnityEditorRegainsFocus,
                "Re-sort assets automatically whenever the Unity Editor regains focus", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.AllowDeletingOfUsedAssets = GUILayout.Toggle(
                BuildReportRemastered.Options.AllowDeletingOfUsedAssets,
                "Allow deleting of Used Assets (practice caution!)", BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(20);

            BuildReportRemastered.Options.UseThreadedReportGeneration = GUILayout.Toggle(
                BuildReportRemastered.Options.UseThreadedReportGeneration,
                "When generating Build Reports, use a separate thread", BRT_BuildReportWindow.LayoutNone);
            GUILayout.BeginHorizontal(LayoutNoExpandWidth);
            GUILayout.Space(20);
            GUILayout.Label(
                "Note: For batchmode builds, report generation with <b>BuildReportTool.ReportGenerator.CreateReport()</b> is always non-threaded.",
                boxedLabelStyle, LayoutMaxWidth593);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            BuildReportRemastered.Options.UseThreadedFileLoading = GUILayout.Toggle(
                BuildReportRemastered.Options.UseThreadedFileLoading,
                "When loading Build Report files, use a separate thread", BRT_BuildReportWindow.LayoutNone);

            //GUILayout.Space(20);

            GUILayout.Space(BuildReportRemastered.Window.Settings.CATEGORY_VERTICAL_SPACING);

            // === Data to include in the Build Report ===

            GUILayout.Label("Data to include in the Build Report", header1Style, BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(5);

            #region Calculation Level
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Calculation Level: ", BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);
            int newSelectedCalculationLevelIdx = EditorGUILayout.Popup(_selectedCalculationLevelIdx, CalculationTypeLabels,
                "Popup", LayoutWidth300);
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);
            GUILayout.Label(CalculationLevelDescription, LayoutMaxWidth500MinHeight75);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (newSelectedCalculationLevelIdx != _selectedCalculationLevelIdx)
            {
                _selectedCalculationLevelIdx = newSelectedCalculationLevelIdx;
                SetCalculationLevelFromGuiIdx(newSelectedCalculationLevelIdx);
            }
            #endregion

            GUILayout.Space(10);
            GUILayout.Label("Sizes", header2Style, BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.IncludeBuildSizeInReportCreation = GUILayout.Toggle(
                BuildReportRemastered.Options.IncludeBuildSizeInReportCreation,
                "Get build's file size upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(10);

            //BuildReportTool.Options.GetImportedSizesForUsedAssets = GUILayout.Toggle(BuildReportTool.Options.GetImportedSizesForUsedAssets,
            //	"Get imported sizes of Used Assets upon creation of a build report");

            BuildReportRemastered.Options.GetImportedSizesForUnusedAssets = GUILayout.Toggle(
                BuildReportRemastered.Options.GetImportedSizesForUnusedAssets,
                "Get imported sizes of Unused Assets upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.GetSizeBeforeBuildForUsedAssets = GUILayout.Toggle(
                BuildReportRemastered.Options.GetSizeBeforeBuildForUsedAssets,
                "Get size-before-build of Used Assets upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            #region ShowCalcSizesForUsed
            BuildReportRemastered.Options.ShowImportedSizeForUsedAssets = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowImportedSizeForUsedAssets,
                "Show calculated sizes of Used Assets instead of reported sizes", BRT_BuildReportWindow.LayoutNone);

            if (_linkStyle == null)
            {
                _linkStyle = new GUIStyle(GUI.skin.label);
                _linkStyle.normal.textColor = new Color(0.266f, 0.533f, 1);
                _linkStyle.hover.textColor = new Color(0.118f, 0.396f, 1);
                _linkStyle.stretchWidth = false;
                _linkStyle.margin.bottom = 0;
                _linkStyle.padding.bottom = 0;
            }

            if (_textBesideLinkStyle == null)
            {
                _textBesideLinkStyle = new GUIStyle(GUI.skin.label);
                _textBesideLinkStyle.stretchWidth = false;
                _textBesideLinkStyle.margin.right = 0;
                _textBesideLinkStyle.padding.right = 0;
                _textBesideLinkStyle.margin.bottom = 0;
                _textBesideLinkStyle.padding.bottom = 0;
            }

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Note: This option is a workaround for the ", _textBesideLinkStyle, BRT_BuildReportWindow.LayoutNone);
            if (GUILayout.Button("Unity bug with Issue ID 885258", _linkStyle, BRT_BuildReportWindow.LayoutNone))
            {
                Application.OpenURL(
                    "https://issuetracker.unity3d.com/issues/unity-reports-incorrect-textures-size-in-the-editor-dot-log-after-building-on-standalone");
            }

            GUILayout.EndHorizontal();
            GUILayout.Label(
                "This bug has already been fixed in Unity 2017.1, 5.5.3p1 and 5.6.0p1. Only enable this if you are affected by the bug.", BRT_BuildReportWindow.LayoutNone);
            #endregion

            GUILayout.Space(10);
            GUILayout.Label("In Unused Assets List", header2Style, BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.IncludeSvnInUnused =
                GUILayout.Toggle(BuildReportRemastered.Options.IncludeSvnInUnused, Labels.INCLUDE_SVN_LABEL, BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.IncludeGitInUnused =
                GUILayout.Toggle(BuildReportRemastered.Options.IncludeGitInUnused, Labels.INCLUDE_GIT_LABEL, BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.IncludeBuildReportToolAssetsInUnused =
                GUILayout.Toggle(BuildReportRemastered.Options.IncludeBuildReportToolAssetsInUnused, Labels.INCLUDE_BRT_LABEL, BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(10);

            // -------------------------------

            #region Ignore Patterns
            if (_ignorePatternList == null)
            {
                _ignorePatternList = new ReorderableList(BuildReportRemastered.Options.IgnorePatternsForUnused, typeof(SavedOptions.IgnorePattern));
                _ignorePatternList.onAddCallback = OnAddPattern;
                _ignorePatternList.drawHeaderCallback = rect => GUI.Label(rect, "Ignore Patterns for Unused Assets");
                _ignorePatternList.elementHeight = 25;
                _ignorePatternList.drawElementCallback =
                    (elementRect, index, isActive, isFocused) =>
                    {
                        var element = BuildReportRemastered.Options.IgnorePatternsForUnused[index];

                        var radioLeftStyle = GUI.skin.FindStyle("RadioLeft");
                        if (radioLeftStyle == null)
                        {
                            radioLeftStyle = GUI.skin.toggle;
                        }
                        var radioRightStyle = GUI.skin.FindStyle("RadioRight");
                        if (radioRightStyle == null)
                        {
                            radioRightStyle = GUI.skin.toggle;
                        }

                        var basicSearchSize = radioLeftStyle.CalcSize(_basicSearchRadioLabel);
                        var regexSearchSize = radioRightStyle.CalcSize(_regexSearchRadioLabel);

                        int spacing = 3;

                        Rect textFieldRect = new Rect(elementRect);
                        textFieldRect.y += 4;
                        textFieldRect.height = GUI.skin.textField.lineHeight + GUI.skin.textField.padding.vertical + 1;
                        textFieldRect.width -= basicSearchSize.x + regexSearchSize.x + spacing;


                        if (element.SearchType == SavedOptions.SEARCH_METHOD_REGEX)
                        {
                            if (_iconValid != null && _iconInvalid != null)
                            {
                                spacing += 18;
                                textFieldRect.width -= spacing;
                                EditorGUI.DrawTextureTransparent(new Rect(textFieldRect.xMax + 3, elementRect.y + 5, 16, 16),
                                    BuildReportRemastered.Util.IsRegexValid(element.Pattern) ? _iconValid : _iconInvalid);
                            }
                            else
                            {
                                spacing += 50;
                                textFieldRect.width -= spacing;
                                GUI.Label(new Rect(textFieldRect.xMax + 3, elementRect.y + 5, 50, 16),
                                    BuildReportRemastered.Util.IsRegexValid(element.Pattern) ? "Valid" : "Invalid");
                            }
                        }

                        element.Pattern = GUI.TextField(textFieldRect, element.Pattern);
                        var patternChanged = element.Pattern != BuildReportRemastered.Options.IgnorePatternsForUnused[index].Pattern;

                        Rect basicToggleRect = new Rect(textFieldRect.xMax + spacing, elementRect.y + 2, basicSearchSize.x, basicSearchSize.y);
                        var pressedBasic = GUI.Toggle(basicToggleRect,
                            element.SearchType == SavedOptions.SEARCH_METHOD_BASIC, _basicSearchRadioLabel, radioLeftStyle);
                        var basicChanged = pressedBasic && element.SearchType != SavedOptions.SEARCH_METHOD_BASIC;
                        if (basicChanged)
                        {
                            element.SearchType = SavedOptions.SEARCH_METHOD_BASIC;
                        }

                        Rect regexToggleRect = new Rect(textFieldRect.xMax + spacing + basicSearchSize.x, elementRect.y + 2, regexSearchSize.x, regexSearchSize.y);
                        var pressedRegex = GUI.Toggle(regexToggleRect,
                            element.SearchType == SavedOptions.SEARCH_METHOD_REGEX, _regexSearchRadioLabel, radioRightStyle);
                        var regexChanged = pressedRegex && element.SearchType != SavedOptions.SEARCH_METHOD_REGEX;
                        if (regexChanged)
                        {
                            element.SearchType = SavedOptions.SEARCH_METHOD_REGEX;
                        }

                        if (patternChanged || basicChanged || regexChanged)
                        {
                            BuildReportRemastered.Options.IgnorePatternsForUnused[index] = element;
                            BuildReportRemastered.Options.SaveOptions();
                        }
                    };
                _ignorePatternList.onChangedCallback = OnIgnorePatternChanged;
            }

            GUILayout.BeginVertical(LayoutMaxWidth848);
            _ignorePatternList.DoLayoutList();
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.Label("Assets that match these search patterns will not be included in the Unused Assets list. The search will be performed on the asset's relative path, starting from the top \"Assets\" folder.",
                boxedLabelStyle, LayoutMaxWidth848);
            #endregion

            // -------------------------------

            GUILayout.Space(15);
            GUILayout.Label("Extra Data to include", header2Style, BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.GetProjectSettings = GUILayout.Toggle(BuildReportRemastered.Options.GetProjectSettings,
                "Get Unity project settings upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(10);

            BuildReportRemastered.Options.CalculateAssetDependencies = GUILayout.Toggle(
                BuildReportRemastered.Options.CalculateAssetDependencies,
                "Calculate Asset Dependencies upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.CalculateAssetDependenciesOnUnusedToo = GUILayout.Toggle(
                BuildReportRemastered.Options.CalculateAssetDependenciesOnUnusedToo,
                "Include Unused Assets in Asset Dependency calculations", BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(10);

            BuildReportRemastered.Options.CollectTextureImportSettings = GUILayout.Toggle(
                BuildReportRemastered.Options.CollectTextureImportSettings,
                "Collect Texture Import Settings upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.CollectTextureImportSettingsOnUnusedToo = GUILayout.Toggle(
                BuildReportRemastered.Options.CollectTextureImportSettingsOnUnusedToo,
                "Include Unused Assets in Texture Import Settings collecting", BRT_BuildReportWindow.LayoutNone);

            GUILayout.Space(10);

            BuildReportRemastered.Options.CollectMeshData = GUILayout.Toggle(
                BuildReportRemastered.Options.CollectMeshData,
                "Collect Mesh Data upon creation of a build report", BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.CollectMeshDataOnUnusedToo = GUILayout.Toggle(
                BuildReportRemastered.Options.CollectMeshDataOnUnusedToo,
                "Include Unused Assets in Mesh Data collecting", BRT_BuildReportWindow.LayoutNone);


            GUILayout.Space(BuildReportRemastered.Window.Settings.CATEGORY_VERTICAL_SPACING);
            // === Editor Log File ===

            GUILayout.Label("Editor Log File", header1Style, BRT_BuildReportWindow.LayoutNone);

            // which Editor.log is used
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(string.Format("{0}{1}: {2}", Labels.EDITOR_LOG_LABEL, BuildReportRemastered.Util.EditorLogPathOverrideMessage, BuildReportRemastered.Util.UsedEditorLogPath),
                BRT_BuildReportWindow.LayoutNone);
            if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL, BRT_BuildReportWindow.LayoutNone) && BuildReportRemastered.Util.UsedEditorLogExists)
            {
                BuildReportRemastered.Util.OpenInFileBrowser(BuildReportRemastered.Util.UsedEditorLogPath);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (!BuildReportRemastered.Util.UsedEditorLogExists)
            {
                if (BuildReportRemastered.Util.IsDefaultEditorLogPathOverridden)
                {
                    GUILayout.Label(Labels.OVERRIDE_LOG_NOT_FOUND_MSG, BRT_BuildReportWindow.LayoutNone);
                }
                else
                {
                    GUILayout.Label(Labels.DEFAULT_EDITOR_LOG_NOT_FOUND_MSG, BRT_BuildReportWindow.LayoutNone);
                }
            }

            // override which log is opened
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            if (GUILayout.Button(Labels.SET_OVERRIDE_LOG_LABEL, BRT_BuildReportWindow.LayoutNone))
            {
                string filepath = EditorUtility.OpenFilePanel(
                    "", // title
                    "", // default path
                    ""); // file type (only one type allowed?)

                if (!string.IsNullOrEmpty(filepath))
                {
                    BuildReportRemastered.Options.EditorLogOverridePath = filepath;
                }
            }

            if (GUILayout.Button(Labels.CLEAR_OVERRIDE_LOG_LABEL, BRT_BuildReportWindow.LayoutNone))
            {
                BuildReportRemastered.Options.EditorLogOverridePath = "";
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(BuildReportRemastered.Window.Settings.CATEGORY_VERTICAL_SPACING);


            // === Asset Lists ===

            GUILayout.Label("Asset Lists", header1Style, BRT_BuildReportWindow.LayoutNone);


            // top largest used
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Number of Top Largest Used Assets to display in Overview Tab:", BRT_BuildReportWindow.LayoutNone);
            string numberOfTopUsedInput =
                GUILayout.TextField(BuildReportRemastered.Options.NumberOfTopLargestUsedAssetsToShow.ToString(), LayoutMinWidth100);
            numberOfTopUsedInput =
                Regex.Replace(numberOfTopUsedInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(numberOfTopUsedInput))
            {
                numberOfTopUsedInput = "0";
            }

            BuildReportRemastered.Options.NumberOfTopLargestUsedAssetsToShow = int.Parse(numberOfTopUsedInput);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            // top largest unused
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Number of Top Largest Unused Assets to display in Overview Tab:", BRT_BuildReportWindow.LayoutNone);
            string numberOfTopUnusedInput =
                GUILayout.TextField(BuildReportRemastered.Options.NumberOfTopLargestUnusedAssetsToShow.ToString(), LayoutMinWidth100);
            numberOfTopUnusedInput =
                Regex.Replace(numberOfTopUnusedInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(numberOfTopUnusedInput))
            {
                numberOfTopUnusedInput = "0";
            }

            BuildReportRemastered.Options.NumberOfTopLargestUnusedAssetsToShow = int.Parse(numberOfTopUnusedInput);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);
            GUILayout.Label(
                "Note: To disable the display of Top Largest Assets, use a value of 0.",
                boxedLabelStyle, LayoutMaxWidth525);
            GUILayout.EndHorizontal();

            // --------------------------------------------

            GUILayout.Space(10);
            GUILayout.Label("Texture Data", header2Style, BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Name of File Filter where Texture Import Settings will be shown:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.FileFilterNameForTextureData =
                GUILayout.TextField(BuildReportRemastered.Options.FileFilterNameForTextureData, LayoutMinWidth200);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.Label("Texture Import Settings To Show in Asset Lists:", BRT_BuildReportWindow.LayoutNone);
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(10);

            #region Column 1
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowTextureColumnTextureType = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnTextureType, "Texture Type", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.TextureType);
                requestRepaint = true;
            }

            BuildReportRemastered.Options.ShowTextureColumnIsSRGB = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnIsSRGB, "Is sRGB (Color Texture)", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.IsSRGB);
            }

            BuildReportRemastered.Options.ShowTextureColumnAlphaSource = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnAlphaSource, "Alpha Source", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.AlphaSource);
            }

            BuildReportRemastered.Options.ShowTextureColumnAlphaIsTransparency = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnAlphaIsTransparency, "Alpha Is Transparency", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.AlphaIsTransparency);
            }

            BuildReportRemastered.Options.ShowTextureColumnIgnorePngGamma = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnIgnorePngGamma, "Ignore PNG Gamma", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.IgnorePngGamma);
            }

            BuildReportRemastered.Options.ShowTextureColumnNPotScale = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnNPotScale, "Non-Power of 2 Scale", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.NPotScale);
            }

            BuildReportRemastered.Options.ShowTextureColumnIsReadable = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnIsReadable, "Read/Write Enabled", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.IsReadable);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(30);

            #region Column 2
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowTextureColumnMipMapGenerated = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnMipMapGenerated, "Mip Map Generated", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.MipMapGenerated);
            }

            BuildReportRemastered.Options.ShowTextureColumnMipMapFilter = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnMipMapFilter, "Mip Map Filter", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.MipMapFilter);
            }

            BuildReportRemastered.Options.ShowTextureColumnStreamingMipMaps = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnStreamingMipMaps, "Streaming Mip Maps", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.StreamingMipMaps);
            }

            BuildReportRemastered.Options.ShowTextureColumnBorderMipMaps = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnBorderMipMaps, "Border Mip Maps", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.BorderMipMaps);
            }

            BuildReportRemastered.Options.ShowTextureColumnPreserveCoverageMipMaps = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnPreserveCoverageMipMaps, "Preserve Coverage Mip Maps", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.PreserveCoverageMipMaps);
            }

            BuildReportRemastered.Options.ShowTextureColumnFadeOutMipMaps = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnFadeOutMipMaps, "Fade Mip Maps", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.FadeOutMipMaps);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(30);

            #region Column 3
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowTextureColumnSpriteImportMode = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnSpriteImportMode, "Sprite Mode", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.SpriteImportMode);
            }

            BuildReportRemastered.Options.ShowTextureColumnSpritePackingTag = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnSpritePackingTag, "Sprite Packing Tag", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.SpritePackingTag);
            }

            BuildReportRemastered.Options.ShowTextureColumnSpritePixelsPerUnit = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnSpritePixelsPerUnit, "Sprite Pixels Per Unit", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.SpritePixelsPerUnit);
            }

            BuildReportRemastered.Options.ShowTextureColumnQualifiesForSpritePacking = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnQualifiesForSpritePacking, "Qualifies for Sprite Packing", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.QualifiesForSpritePacking);
            }

            BuildReportRemastered.Options.ShowTextureColumnWrapMode = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnWrapMode, "Wrap Mode", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.WrapMode);
            }

            BuildReportRemastered.Options.ShowTextureColumnWrapModeU = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnWrapModeU, "Wrap Mode U", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.WrapModeU);
            }

            BuildReportRemastered.Options.ShowTextureColumnWrapModeV = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnWrapModeV, "Wrap Mode V", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.WrapModeV);
            }

            BuildReportRemastered.Options.ShowTextureColumnWrapModeW = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnWrapModeW, "Wrap Mode W", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.WrapModeW);
            }

            BuildReportRemastered.Options.ShowTextureColumnFilterMode = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnFilterMode, "Filter Mode", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.FilterMode);
            }

            BuildReportRemastered.Options.ShowTextureColumnAnisoLevel = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnAnisoLevel, "Anisotropic Filtering Level", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.AnisoLevel);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(30);

            #region Column 4
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowTextureColumnMaxTextureSize = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnMaxTextureSize, "Max Texture Size", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.MaxTextureSize);
            }

            BuildReportRemastered.Options.ShowTextureColumnResizeAlgorithm = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnResizeAlgorithm, "Resize Algorithm", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.TextureResizeAlgorithm);
            }

            BuildReportRemastered.Options.ShowTextureColumnTextureFormat = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnTextureFormat, "Texture Format", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.TextureFormat);
            }

            BuildReportRemastered.Options.ShowTextureColumnCompressionType = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnCompressionType, "Compression Type", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.CompressionType);
            }

            BuildReportRemastered.Options.ShowTextureColumnCompressionIsCrunched = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnCompressionIsCrunched, "Compression Crunched", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.CompressionIsCrunched);
            }

            BuildReportRemastered.Options.ShowTextureColumnCompressionQuality = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnCompressionQuality, "Compression Quality", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.CompressionQuality);
            }

            BuildReportRemastered.Options.ShowTextureColumnImportedWidthAndHeight = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnImportedWidthAndHeight, "Imported Width & Height", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.ImportedWidthAndHeight);
            }

            BuildReportRemastered.Options.ShowTextureColumnRealWidthAndHeight = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTextureColumnRealWidthAndHeight, "Source Width & Height", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.TextureData.GetTooltipTextFromId(BuildReportRemastered.TextureData.DataId.RealWidthAndHeight);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            // --------------------------------------------

            GUILayout.Space(10);
            GUILayout.Label("Mesh Data", header2Style, BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Name of File Filter where Mesh Data will be shown:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.FileFilterNameForMeshData =
                GUILayout.TextField(BuildReportRemastered.Options.FileFilterNameForMeshData, LayoutMinWidth200);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUILayout.Label("Texture Import Settings To Show in Asset Lists:", BRT_BuildReportWindow.LayoutNone);
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(10);

            #region Column 1
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowMeshColumnMeshFilterCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnMeshFilterCount, "Non-Skinned Mesh Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.MeshFilterCount);
            }

            BuildReportRemastered.Options.ShowMeshColumnSkinnedMeshRendererCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnSkinnedMeshRendererCount, "Skinned Mesh Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.SkinnedMeshRendererCount);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(30);

            #region Column 2
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowMeshColumnSubMeshCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnSubMeshCount, "Sub-mesh Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.SubMeshCount);
            }

            BuildReportRemastered.Options.ShowMeshColumnVertexCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnVertexCount, "Vertex Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.VertexCount);
            }

            BuildReportRemastered.Options.ShowMeshColumnTriangleCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnTriangleCount, "Face Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.TriangleCount);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(30);

            #region Column 2
            GUILayout.BeginVertical(BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowMeshColumnAnimationType = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnAnimationType, "Animation Type", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.AnimationType);
            }

            BuildReportRemastered.Options.ShowMeshColumnAnimationClipCount = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowMeshColumnAnimationClipCount, "Animation Clip Count", BRT_BuildReportWindow.LayoutNone);
            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                _hoveredControlTooltipText = BuildReportRemastered.MeshData.GetTooltipTextFromId(BuildReportRemastered.MeshData.DataId.AnimationClipCount);
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            // --------------------------------------------

            GUILayout.Space(10);
            GUILayout.Label("List Pagination", header2Style, BRT_BuildReportWindow.LayoutNone);

            // pagination length
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("View assets per groups of:", BRT_BuildReportWindow.LayoutNone);
            string pageInput = GUILayout.TextField(BuildReportRemastered.Options.AssetListPaginationLength.ToString(), LayoutMinWidth100);
            pageInput = Regex.Replace(pageInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(pageInput))
            {
                pageInput = "0";
            }

            BuildReportRemastered.Options.AssetListPaginationLength = int.Parse(pageInput);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // unused assets entries per batch
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Process unused assets per batches of:", BRT_BuildReportWindow.LayoutNone);
            string entriesPerBatchInput =
                GUILayout.TextField(BuildReportRemastered.Options.UnusedAssetsEntriesPerBatch.ToString(), LayoutMinWidth100);
            entriesPerBatchInput =
                Regex.Replace(entriesPerBatchInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(entriesPerBatchInput))
            {
                entriesPerBatchInput = "0";
            }

            BuildReportRemastered.Options.UnusedAssetsEntriesPerBatch = int.Parse(entriesPerBatchInput);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // log messages
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Log Messages per page:", BRT_BuildReportWindow.LayoutNone);
            string logMessagesInput =
                GUILayout.TextField(BuildReportRemastered.Options.LogMessagePaginationLength.ToString(), LayoutMinWidth100);
            logMessagesInput =
                Regex.Replace(logMessagesInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(logMessagesInput))
            {
                logMessagesInput = "0";
            }

            BuildReportRemastered.Options.LogMessagePaginationLength = int.Parse(logMessagesInput);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Asset Search", header2Style, BRT_BuildReportWindow.LayoutNone);


            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Search Method:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.SearchTypeInt = GUILayout.SelectionGrid(
                BuildReportRemastered.Options.SearchTypeInt, SearchTypeLabels, 3, BRT_BuildReportWindow.LayoutNone);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            BuildReportRemastered.Options.SearchFilenameOnly = GUILayout.Toggle(
                BuildReportRemastered.Options.SearchFilenameOnly,
                "Search through filenames only (ignore path when searching)", BRT_BuildReportWindow.LayoutNone);

            var usingFuzzy = BuildReportRemastered.Options.SearchType == SearchType.Fuzzy;
            var caseSensitiveLabel = usingFuzzy ? "Case Sensitive Search (Not applicable to Fuzzy Search. Fuzzy Search is always Case Insensitive.)" : "Case Sensitive Search";
            GUI.enabled = prevEnabled && !usingFuzzy;
            BuildReportRemastered.Options.SearchCaseSensitive = GUILayout.Toggle(
                BuildReportRemastered.Options.SearchCaseSensitive,
                caseSensitiveLabel, BRT_BuildReportWindow.LayoutNone);
            GUI.enabled = prevEnabled;

            GUILayout.Space(10);
            GUILayout.Label("File Filters", header2Style, BRT_BuildReportWindow.LayoutNone);

            // choose which file filter group to use
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(Labels.FILTER_GROUP_TO_USE_LABEL, BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.FilterToUseInt = GUILayout.SelectionGrid(BuildReportRemastered.Options.FilterToUseInt,
                FileFilterToUseType, FileFilterToUseType.Length, BRT_BuildReportWindow.LayoutNone);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // display which file filter group is used
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(string.Format("{0}{1}", Labels.FILTER_GROUP_FILE_PATH_LABEL, BuildReportRemastered.FiltersUsed.GetProperFileFilterGroupToUseFilePath()),
                BRT_BuildReportWindow.LayoutNone); // display path to used file filter
            if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL, BRT_BuildReportWindow.LayoutNone))
            {
                BuildReportRemastered.Util.OpenInFileBrowser(BuildReportRemastered.FiltersUsed.GetProperFileFilterGroupToUseFilePath());
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Asset Pinging", header2Style, BRT_BuildReportWindow.LayoutNone);


            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Asset Ping method:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.DoubleClickOnAssetWillPing = GUILayout.SelectionGrid(
                                                                     BuildReportRemastered.Options.DoubleClickOnAssetWillPing
                                                                         ? 1
                                                                         : 0,
                                                                     AssetPingTypeLabels, 2, LayoutHeight26) == 1;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);

            GUILayout.Label(
                BuildReportRemastered.Options.DoubleClickOnAssetWillPing
                    ? "Note: To ping multiple assets, select the assets, and hold Alt while double-clicking one of them."
                    : "Note: To ping multiple assets, select the assets, and hold Alt while pressing one of their Ping buttons.",
                boxedLabelStyle, LayoutMaxWidth593);

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            //AssetUsageLabelTypeLabels
            GUILayout.Label("Asset Usages/Dependencies", header2Style, BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Asset usage labels:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.AssetUsageLabelType = GUILayout.SelectionGrid(
                BuildReportRemastered.Options.AssetUsageLabelType, AssetUsageLabelTypeLabels, 1, BRT_BuildReportWindow.LayoutNone);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            BuildReportRemastered.Options.ShowAssetPrimaryUsersInTooltipIfAvailable = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowAssetPrimaryUsersInTooltipIfAvailable,
                "Show end users in asset tooltip (if available)", BRT_BuildReportWindow.LayoutNone);

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);
            GUILayout.Label(
                "Note: \"End users\" are the scenes (or Resources assets) that use a given asset (directly or indirectly), they are the main reason why that asset got included in the build.",
                boxedLabelStyle, LayoutMaxWidth525);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label("Thumbnails", header2Style, BRT_BuildReportWindow.LayoutNone);

            BuildReportRemastered.Options.ShowTooltipThumbnail = GUILayout.Toggle(
                BuildReportRemastered.Options.ShowTooltipThumbnail,
                "Show thumbnail in asset tooltip", BRT_BuildReportWindow.LayoutNone);

            GUI.enabled = prevEnabled && BuildReportRemastered.Options.ShowTooltipThumbnail;

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label("Show thumbnail when:", BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.ShowThumbnailOnHoverType = GUILayout.SelectionGrid(
                BuildReportRemastered.Options.ShowThumbnailOnHoverType, ShowThumbnailOnHoverTypeLabels,
                ShowThumbnailOnHoverTypeLabels.Length, LayoutHeight26);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

            GUILayout.Label("Thumbnail Tooltip Width:", BRT_BuildReportWindow.LayoutNone);
            string tooltipThumbnailWidthInput =
                GUILayout.TextField(BuildReportRemastered.Options.TooltipThumbnailWidth.ToString(), LayoutMinWidth100);
            tooltipThumbnailWidthInput =
                Regex.Replace(tooltipThumbnailWidthInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(tooltipThumbnailWidthInput))
            {
                tooltipThumbnailWidthInput = "0";
            }

            BuildReportRemastered.Options.TooltipThumbnailWidth = int.Parse(tooltipThumbnailWidthInput);

            GUILayout.Space(3);

            GUILayout.Label("Height:", BRT_BuildReportWindow.LayoutNone);
            string tooltipThumbnailHeightInput =
                GUILayout.TextField(BuildReportRemastered.Options.TooltipThumbnailHeight.ToString(), LayoutMinWidth100);
            tooltipThumbnailHeightInput =
                Regex.Replace(tooltipThumbnailHeightInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(tooltipThumbnailHeightInput))
            {
                tooltipThumbnailHeightInput = "0";
            }

            BuildReportRemastered.Options.TooltipThumbnailHeight = int.Parse(tooltipThumbnailHeightInput);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);

            GUILayout.Label("Thumbnail Tooltip Zoomed-in Width:", BRT_BuildReportWindow.LayoutNone);
            string tooltipThumbnailZoomedInWidthInput =
                GUILayout.TextField(BuildReportRemastered.Options.TooltipThumbnailZoomedInWidth.ToString(), LayoutMinWidth100);
            tooltipThumbnailZoomedInWidthInput =
                Regex.Replace(tooltipThumbnailZoomedInWidthInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(tooltipThumbnailZoomedInWidthInput))
            {
                tooltipThumbnailZoomedInWidthInput = "0";
            }

            BuildReportRemastered.Options.TooltipThumbnailZoomedInWidth = int.Parse(tooltipThumbnailZoomedInWidthInput);

            GUILayout.Space(3);

            GUILayout.Label("Height:", BRT_BuildReportWindow.LayoutNone);
            string tooltipThumbnailZoomedInHeightInput =
                GUILayout.TextField(BuildReportRemastered.Options.TooltipThumbnailZoomedInHeight.ToString(), LayoutMinWidth100);
            tooltipThumbnailZoomedInHeightInput =
                Regex.Replace(tooltipThumbnailZoomedInHeightInput, @"[^0-9]", ""); // positive numbers only, no fractions
            if (string.IsNullOrEmpty(tooltipThumbnailZoomedInHeightInput))
            {
                tooltipThumbnailZoomedInHeightInput = "0";
            }

            BuildReportRemastered.Options.TooltipThumbnailZoomedInHeight = int.Parse(tooltipThumbnailZoomedInHeightInput);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.enabled = prevEnabled;

            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Space(20);
            GUILayout.Label(
                "Note: Hold Ctrl while a thumbnail tooltip is shown to zoom-in.",
                boxedLabelStyle, LayoutMaxWidth525);
            GUILayout.EndHorizontal();


            GUILayout.Space(BuildReportRemastered.Window.Settings.CATEGORY_VERTICAL_SPACING);


            // === Build Report Files ===

            GUILayout.Label("Build Report Files", header1Style, BRT_BuildReportWindow.LayoutNone);

            // build report files save path
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(string.Format("{0}{1}", Labels.SAVE_PATH_LABEL, BuildReportRemastered.Options.BuildReportSavePath), BRT_BuildReportWindow.LayoutNone);
            if (GUILayout.Button(OPEN_IN_FILE_BROWSER_OS_SPECIFIC_LABEL, BRT_BuildReportWindow.LayoutNone))
            {
                BuildReportRemastered.Util.OpenInFileBrowser(BuildReportRemastered.Options.BuildReportSavePath);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // change name of build reports folder
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(Labels.SAVE_FOLDER_NAME_LABEL, BRT_BuildReportWindow.LayoutNone);
            BuildReportRemastered.Options.BuildReportFolderName =
                GUILayout.TextField(BuildReportRemastered.Options.BuildReportFolderName, LayoutMinWidth250);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // where to save build reports (my docs/home, or beside project)
            GUILayout.BeginHorizontal(BRT_BuildReportWindow.LayoutNone);
            GUILayout.Label(Labels.SAVE_PATH_TYPE_LABEL, BRT_BuildReportWindow.LayoutNone);

            if (_saveTypeLabels == null)
            {
                _saveTypeLabels = new[]
                    {SAVE_PATH_TYPE_PERSONAL_OS_SPECIFIC_LABEL, Labels.SAVE_PATH_TYPE_PROJECT_LABEL};
            }

            BuildReportRemastered.Options.SaveType = GUILayout.SelectionGrid(BuildReportRemastered.Options.SaveType, _saveTypeLabels,
                _saveTypeLabels.Length, BRT_BuildReportWindow.LayoutNone);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(BuildReportRemastered.Window.Settings.CATEGORY_VERTICAL_SPACING);


            GUILayout.EndVertical();
            GUILayout.Space(20); // extra right padding
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            //if (BuildReportTool.Options.SaveType == BuildReportTool.Options.SAVE_TYPE_PERSONAL)
            //{
            // changed to user's personal folder
            //BuildReportTool.ReportGenerator.ChangeSavePathToUserPersonalFolder();
            //}
            //else if (BuildReportTool.Options.SaveType == BuildReportTool.Options.SAVE_TYPE_PROJECT)
            //{
            // changed to project folder
            //BuildReportTool.ReportGenerator.ChangeSavePathToProjectFolder();
            //}

            if (Event.current.type == EventType.Repaint && !string.IsNullOrEmpty(_hoveredControlTooltipText))
            {
                _tooltipLabel.text = _hoveredControlTooltipText;
                var tooltipTextStyle = GUI.skin.FindStyle("TooltipText");
                if (tooltipTextStyle == null)
                {
                    tooltipTextStyle = GUI.skin.label;
                }

                const int MAX_TOOLTIP_WIDTH = 240;
                var tooltipSize = tooltipTextStyle.CalcSize(_tooltipLabel);
                if (tooltipSize.x > MAX_TOOLTIP_WIDTH)
                {
                    tooltipSize.x = MAX_TOOLTIP_WIDTH;
                    tooltipSize.y = tooltipTextStyle.CalcHeight(_tooltipLabel, tooltipSize.x);
                }

                var tooltipRect = BRT_BuildReportWindow.DrawTooltip(position, tooltipSize.x, tooltipSize.y, 5);
                GUI.Label(tooltipRect, _tooltipLabel, tooltipTextStyle);
            }
        }

        static void OnAddPattern(ReorderableList list)
        {
            SavedOptions.IgnorePattern newEntry;
            newEntry.Pattern = "";
            newEntry.SearchType = SavedOptions.SEARCH_METHOD_BASIC;
            BuildReportRemastered.Options.IgnorePatternsForUnused.Add(newEntry);
        }

        static void OnIgnorePatternChanged(ReorderableList list)
        {
            BuildReportRemastered.Options.SaveOptions();
        }
    }
}