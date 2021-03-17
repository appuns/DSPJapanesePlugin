using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using TranslationCommon.SimpleJSON;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



namespace DSPJapanesePlugin
{
    [BepInPlugin("Appun.plugins.dspmod.DSPJapanesePlugin", "DSPJapanesePlugin", "1.0.0.1")]

    public class DSPJapaneseMod : BaseUnityPlugin
    {
        public static AssetBundle FontAssetBundle { get; set; }

        public static Font newFont { get; set; }

        public static Dictionary<string, string> JPDictionary { get; set; }

        public static StringProtoSet _strings;

        public static bool BeltCheckSignUpdated;

        public static bool JpaneseAddedInComboBox = false;


        private static ConfigEntry<bool> enableAutoUpdate;
        private static ConfigEntry<bool> enableShowUnTranslatedStrings;
        private static ConfigEntry<bool> enableNewWordExport;
        //private static ConfigEntry<bool> enableNewWordUpload;

        public static string PluginPath = System.IO.Path.GetDirectoryName(
           System.Reflection.Assembly.GetExecutingAssembly().Location);

        public void Awake()
        {
            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            enableAutoUpdate = Config.Bind("General", "enableAutoUpdate", true, "起動時に辞書ファイルを自動でアップデートするかどうか");
            // enableShowUnTranslatedStrings = Config.Bind("General", "enableShowUnTranslatedStrings", true, "未登録文字列を表示するかどうか");
           //enableNewWordExport = Config.Bind("General", "enableNewWordExport", true, "未登録文字列のjson書き出し");
            //enableNewWordUpload = Config.Bind("General", "enableNewWordUpload", true, "未登録文字列のアップロード");


            LogManager.Logger.LogInfo("DSPJapanesePlugin awake");
            //プラグインのパスチェック
            //string FontFilePathCheck1 = Path.Combine(Utils.PluginPath, "newJPFont");


            foreach (var token in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                LogManager.Logger.LogInfo("resouce path=" + token);
            }






            //DSPJPTranslationUpdaterの読み込み
            //var updater = new DSPJPTranslationUpdater.TranslationUpdater(Path.Combine(BepInEx.Paths.PluginPath, @"DSPJapanesePlugin"));
            if (enableAutoUpdate.Value)
            {
                var updater = new DSPJPTranslationUpdater.TranslationUpdater(Utils.PluginPath);
                updater.Update();
                LogManager.Logger.LogInfo("translation dictionary updated from web.");
            }
            LogManager.Logger.LogInfo("translation dictionary load start.");


            //翻訳用ファイルの読み込み
            string jsonFilePath = Path.Combine(Utils.PluginPath, "Translation\\日本語\\translation_DysonSphereProgram.json");
            LogManager.Logger.LogInfo("target path " + jsonFilePath);
            if (!File.Exists(jsonFilePath))
            {
                LogManager.Logger.LogInfo("File not found" + jsonFilePath);
            }
            JPDictionary = JSON.FromJson<Dictionary<string, string>>(File.ReadAllText(jsonFilePath));
            LogManager.Logger.LogInfo("translation dictionary load finish.");

            LogManager.Logger.LogInfo("mod font load start.");

            LoadFont();
            LogManager.Logger.LogInfo("mod font load finish.");
            Localization.language = Language.frFR;


            //未登録単語の書き出し
            string newWord = Path.Combine(Utils.PluginPath, "Translation\\日本語\\translation_DysonSphereProgram.json");

        }


        //フォントの読み込み
        private void LoadFont()
        {
            try
            {
                var assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DSPJapanesePlugin.newjpfont"));
                if (assetBundle == null)
                {
                    LogManager.Logger.LogInfo("asset bundle not loaded.");
                }
                else
                {
                    FontAssetBundle = assetBundle;
                }
                newFont = FontAssetBundle.LoadAsset<Font>("MPMK85");
                LogManager.Logger.LogInfo("ModFontLoaded. " + newFont);
                if (newFont != null)
                {
                    LogManager.Logger.LogInfo("mod font name " + newFont.name);
                    LogManager.Logger.LogInfo("mod font size " + newFont.fontSize);
                }

            }
            catch (Exception e)
            {
                LogManager.Logger.LogInfo("e.Message " + e.Message);
                LogManager.Logger.LogInfo("e.StackTrace " + e.StackTrace);
            }
        }

        private void LevelLoaded(Scene scene, LoadSceneMode mode)
        {
            Invoke(nameof(ModifyFont), 5);
        }

        //フォントを適応その１
        public static void ModifyFont()
        {
            if (newFont != null)
            {
                var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
                foreach (var text in texts)
                {
                    text.font = newFont;
                }
            }
        }

　　　　//コンボボックスへ「日本語」を追加
        [HarmonyPatch(typeof(UIOptionWindow), "TempOptionToUI")]
        public static class UIOptionWindow_TempOptionToUI_Harmony
        {
            [HarmonyPrefix]
            public static void Prefix(UIOptionWindow __instance)
            {
                if (!JpaneseAddedInComboBox)
                {
                    UIComboBox comboBox = (UIComboBox)__instance.GetType().GetField("languageComp", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(__instance);
                    comboBox.Items.Add("日本語");
                    LogManager.Logger.LogInfo("comboBox.ItemsDatae " + comboBox.ItemsData);
                    LogManager.Logger.LogInfo("comboBox.itemIndex " + comboBox.itemIndex);
                    LogManager.Logger.LogInfo("comboBox.text " + comboBox.text);
                    LogManager.Logger.LogInfo("comboBox.ItemsData " + comboBox.ItemsData);
                    JpaneseAddedInComboBox = true;
                }
            }
        }


        //未翻訳のMODアイテム名と説明分、MOD技術名と説明文の翻訳
        public static void Translate2()
        {
            foreach (var item in LDB.items.dataArray)
            {
                if (item != null)
                { 
                    if (item.name != null && item.description != null)
                    {
                        if (JPDictionary.ContainsKey(item.name))
                        {
                            item.name = JPDictionary[item.name];
                        }
                        if (JPDictionary.ContainsKey(item.description))
                        {
                            item.description = JPDictionary[item.description];
                        }
                    }
                }
            }
            foreach (var tech in LDB.techs.dataArray)
            {
                if (tech != null)
                {
                    if (tech.name != null && tech.description != null)
                    {
                        if (JPDictionary.ContainsKey(tech.name))
                        {
                            tech.name = JPDictionary[tech.name];
                        }
                        if (JPDictionary.ContainsKey(tech.description))
                        {
                            tech.description = JPDictionary[tech.description];
                        }

                    }
                }

            }

        }


        // 翻訳メイン
        [HarmonyPatch(typeof(Localization), "get_strings")]
            public static class Localization_get_strings_PrePatch
        {
            [HarmonyPrefix]

            public static bool Prefix()
            {
                if (LDB.strings.dataArray != null)
                {
                    foreach (StringProto sp in LDB.strings.dataArray)
                    {
                        if (sp != null && sp.name != null)
                        {
                            if ( sp.FRFR != null && JPDictionary.ContainsKey(sp.name))
                            {
                            //   sp.ZHCN = sp.ENUS;
                               sp.FRFR = JPDictionary[sp.name];
                            }
                            
                            
                        }
                    }
                }
                return true;
            }

        }

        //建築メニューの[☑コンベアベルト]の位置調整
        [HarmonyPatch(typeof(UIBuildMenu), "UpdateUXPanel")]
        public static class UIBuildMenu_UpdateUXPanels_PrePatch
        {
            [HarmonyPrefix]

            public static void Prefix(Image ___uxBeltCheckSign , UIButton ___uxBeltCheck)
            {
                if (!BeltCheckSignUpdated)
                {
                    Transform myTransform = ___uxBeltCheck.transform;
                    myTransform.Translate(-0.4f, 0, 0);
                    BeltCheckSignUpdated = true;
                }

            }

        }


            //セーブ＆ロード確認MessageBoxのフォント変更
            //新しく作られるのでフォントの変更
            [HarmonyPatch(typeof(UIDialog), "CreateDialog")]
        public static class UIMessageBox_Show_Patch
        {
            [HarmonyPostfix]

            public static void Postfix() //UIDialog __result)
            {
                LogManager.Logger.LogInfo("UIMessageBox_Show_Patch");
                var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
                foreach (var text in texts)
                {
                    text.font = newFont;
                }
            }

        }


        //フォントの適応
        //リソース全体のTextのフォントを変更
        [HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        public static class VFPreload_PreloadThread_Patch
        {
            [HarmonyPostfix]

            public static void Postfix()
            {
                LogManager.Logger.LogInfo("call VFPreload PreloadThread.");
                ModifyFont();
            }

        }

        //MODの翻訳メイン
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        public static class VFPreload_InvokeOnLoadWorkEnded_Patch
        {
            [HarmonyPostfix]
            [HarmonyPriority(1)]

            public static void Postfix()
            {
                LogManager.Logger.LogInfo("call VFPreload InvokeOnLoadWorkEnded.");
                Translate2();
            }

        }



        // UITechNodeのフック：テックツリーの技術名の位置調整：aki9284
        [HarmonyPatch(typeof(UITechNode), "UpdateLayoutDynamic")]
        static class UITechNodePatch
        {
            [HarmonyPostfix]

            static void Postfix(Text ___titleText2, Text ___techDescText)
            {
                ___titleText2.rectTransform.anchoredPosition = new Vector2(0, 10.0f);
            }
        }


        //UIGalaxySelectのフック：新規開始画面の文字位置調整
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        static class UpdateUIDisplayPatch
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
                foreach (var text in texts)
                {
                    if (text.name.Contains("-star"))
                    {
                        text.text = "　　　　　　　" + text.text;

                    }
                }
            }
        }


        //UIAssemblerWindowのフック：コピー＆ペーストボタンのサイズ拡大
        [HarmonyPatch(typeof(UIAssemblerWindow), "_OnOpen")]
        static class UIAssemblerWindowPatch
        {
            [HarmonyPostfix]
            static void Postfix(UIButton ___resetButton, UIButton ___copyButton, UIButton ___pasteButton)
            {
                LogManager.Logger.LogInfo("copyButton");
                Text copyText = ___copyButton.GetComponentInChildren<Text>();
                if (copyText != null)
                {
                    float width = copyText.preferredWidth;
                    float height = copyText.preferredHeight;

                    RectTransform trs = (RectTransform)___copyButton.button.transform;

                    trs.offsetMin = new Vector2(-35, trs.offsetMin.y);
                    trs.offsetMax = new Vector2(35, trs.offsetMax.y);
                }
                LogManager.Logger.LogInfo("pasteButton");
                Text pasteText = ___pasteButton.GetComponentInChildren<Text>();
                if (pasteText != null)
                {
                    RectTransform trs = (RectTransform)___pasteButton.button.transform;
                    trs.offsetMin = new Vector2(10, trs.offsetMin.y);
                    trs.offsetMax = new Vector2(80, trs.offsetMax.y);
                }
            }
        }
    }


    public class LogManager
    {
        public static ManualLogSource Logger;
    }
    public static class Utils
    {
        public const string PluginName = "DSPJapanesePlugin";
        public static string PluginPath = Path.Combine(BepInEx.Paths.PluginPath, PluginName); /*BepInEx.Paths.PluginPath*/
        public static string PluginPathForModman = Path.Combine(BepInEx.Paths.PluginPath, "appuns-" + PluginName); /*BepInEx.Paths.PluginPath*/
    }
}