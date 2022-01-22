using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using TranslationCommon.SimpleJSON;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace DSPJapanesePlugin
{
    [BepInPlugin("Appun.DSP.plugin.JapanesePlugin", "DSPJapanesePlugin", "1.1.5")]

    public class Main : BaseUnityPlugin
    {
        public static AssetBundle FontAssetBundle { get; set; }

        public static Font newFont { get; set; }
        public static Dictionary<string, string> JPDictionary { get; set; }

        public static StringProtoSet _strings;


        public static ConfigEntry<bool> EnableFixUI;
        public static ConfigEntry<bool> EnableAutoUpdate;
        public static ConfigEntry<bool> ImportSheet;
        public static ConfigEntry<bool> exportNewStrings;
        public static ConfigEntry<string> DictionaryGAS;
        public static ConfigEntry<string> SsheetGAS;

        //private static ConfigEntry<bool> enableShowUnTranslatedStrings;
        //private static ConfigEntry<bool> enableNewWordExport;
        //private static ConfigEntry<bool> enableNewWordUpload;

        public static string PluginPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string jsonFilePath = Path.Combine(PluginPath, "translation_DysonSphereProgram.json");
        public static string newStringsFilePath = Path.Combine(PluginPath, "newStrings.tsv");


        public void Awake()
        {
            LogManager.Logger = Logger;
            //LogManager.Logger.LogInfo("DSPJapanesePlugin awake");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            EnableFixUI = Config.Bind("表示の修正：アップデートでエラーが出る場合はfalseにすると解消できる可能性があります。", "EnableFixUI", true, "日本語化に伴い発生する表示の問題を修正するか");
            EnableAutoUpdate = Config.Bind("辞書自動アップデート：起動時に日本語辞書ファイルを自動でダウンロードすることができます。", "EnableAutoUpdate", true, "起動時に日本語辞書ファイルを自動でアップデートするかどうか");
            ImportSheet = Config.Bind("翻訳者、開発者向けの設定：基本的に変更しないでください。", "ImportSheet", false, "翻訳作業所のシートのデータを取り込んで辞書ファイルを作るかどうか");
            DictionaryGAS = Config.Bind("翻訳者、開発者向けの設定：基本的に変更しないでください。", "DictionaryGAS", "https://script.google.com/macros/s/AKfycbwRjiRA6PUeh02MOQ6ccWfbhkQ3wW_qxM6MEl_UXcltGHnU59GLhIOcNNoM35NS7N7_/exec", "日本語辞書ファイル取得のスクリプトアドレス");
            SsheetGAS = Config.Bind("翻訳者、開発者向けの設定：基本的に変更しないでください。", "SsheetGAS", "https://script.google.com/macros/s/AKfycbxOATSa3MHENWQfWc8Ti6XLK-yx-HjzvoLMnO7S2u2nKuZYrRrD3Luh2NLA6jehgf1RUQ/exec", "翻訳作業所のシート取得のスクリプトアドレス");
            exportNewStrings = Config.Bind("翻訳者、開発者向けの設定：基本的に変更しないでください。", "exportNewStrings", false, "バージョンアップ時に新規文字列を翻訳作業所用に書き出すかどうか。");

            //辞書ファイルのダウンロード
            if (EnableAutoUpdate.Value)
            {
                if (!ImportSheet.Value) //Jsonを直接ダウンロード
                {
                    LogManager.Logger.LogInfo("完成済みの辞書をダウンロードします");
                    IEnumerator coroutine = GASAccess.CheckAndDownload(DictionaryGAS.Value, jsonFilePath);
                    ////IEnumerator coroutine = DownloadAndSave(DictionaryGAS.Value, jsonFilePath);
                    coroutine.MoveNext();
                }
                else //スプレッドシートからjson作成
                {
                    LogManager.Logger.LogInfo("辞書を作業所スプレッドシートから作成します");
                    IEnumerator coroutine = GASAccess.MakeFromSheet(SsheetGAS.Value, jsonFilePath);
                    coroutine.MoveNext();
                }
            }
            else
            {
                LogManager.Logger.LogInfo("辞書を既存のファイルから読み込みます");
                //LogManager.Logger.LogInfo("target path " + jsonFilePath);
                if (!File.Exists(jsonFilePath))
                {
                    LogManager.Logger.LogInfo("File not found" + jsonFilePath);
                }
                JPDictionary = JSON.FromJson<Dictionary<string, string>>(File.ReadAllText(jsonFilePath));



            }

            //フォントの読み込み
            try
            {
                var assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DSPJapanesePlugin.newjpfont"));
                if (assetBundle == null)
                {
                    LogManager.Logger.LogInfo("Asset Bundle not loaded.");
                }
                else
                {
                    FontAssetBundle = assetBundle;
                    newFont = FontAssetBundle.LoadAsset<Font>("MPMK85");
                    LogManager.Logger.LogInfo("フォントを読み込みました : " + newFont);
                }
            }
            catch (Exception e)
            {
                LogManager.Logger.LogInfo("e.Message " + e.Message);
                LogManager.Logger.LogInfo("e.StackTrace " + e.StackTrace);
            }
            //言語の設定
            Localization.language = Language.frFR;

            //UIの修正
            //fixUI();
        }


        //コンボボックスへ「日本語」を追加
        [HarmonyPatch(typeof(UIOptionWindow), "TempOptionToUI")]
        public static class UIOptionWindow_TempOptionToUI_Harmony
        {
            [HarmonyPrefix]
            public static void Prefix(UIOptionWindow __instance)
            {
                if (!__instance.languageComp.Items.Contains("日本語"))
                {
                    __instance.languageComp.Items.Add("日本語");
                    __instance.languageComp.itemIndex = 2;
                }
            }
        }

        //翻訳メイン
        [HarmonyPatch(typeof(StringTranslate), "Translate", typeof(string))]
        public static class StringTranslate_Translate_Prefix
        {
            [HarmonyPrefix]
            public static bool Prefix(ref string __result, string s)
            {
                if (Localization.language == Language.frFR)
                {
                    if (s == null)
                    {
                        return true;
                    }

                    if (JPDictionary.ContainsKey(s))
                    {
                        __result = JPDictionary[s];
                        return false;
                    }
                }
                return true;
            }
        }

        //リソース全体のTextのフォントを変更   //新規文字列のチェック
        [HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        public static class VFPreload_PreloadThread_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                LogManager.Logger.LogInfo("フォントを変更しました");
                var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
                foreach (var text in texts)
                {
                    text.font = newFont;

                    if (JPDictionary.ContainsKey(text.text))
                    {
                        text.text = JPDictionary[text.text];
                    }
                }

                //新規文字列のチェック
                if (exportNewStrings.Value)
                {
                    LogManager.Logger.LogInfo("新規文字列をチェックします");
                    string path = LDB.protoResDir + typeof(StringProtoSet).Name;
                    StringProtoSet strings = (Resources.Load(path) as StringProtoSet);
                    StringProtoSet stringProtoSet = Localization.strings;
                    var tsvText = new StringBuilder();

                    for (int i = 0; i < strings.Length; i++)
                    {

                        if (!JPDictionary.ContainsKey(strings[i].Name))
                        {
                            StringProto stringProto = strings[strings[i].Name];
                            string enUS = stringProto.ENUS.Replace("\n", "[LF]").Replace("\r\n", "[CRLF]");
                            string zhCN = stringProto.ZHCN.Replace("\n", "[LF]").Replace("\r\n", "[CRLF]");
                            string frFR = stringProto.FRFR.Replace("\n", "[LF]").Replace("\r\n", "[CRLF]");

                            tsvText.Append($"\t\t{strings[i].Name}\t=googletranslate(\"{enUS}\",\"en\",\"ja\")\tnew\t\t{enUS}\t{zhCN}\t{frFR}\r\n");
                            LogManager.Logger.LogInfo($"新規文字列 {i} : {strings[i].Name} : {enUS}");
                        }
                    }
                    if (tsvText.Length == 0)
                    {
                        LogManager.Logger.LogInfo("新規文字列はありません");
                    }
                    else
                    {
                        LogManager.Logger.LogInfo($"新規文字列がありましたので、{newStringsFilePath}に書き出しました。");
                    }

                    File.WriteAllText(newStringsFilePath, tsvText.ToString());
                }
            }
        }

        //未翻訳のMODアイテム名と説明分、MOD技術名と説明文の翻訳  新規文字列チェック
        [HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        public static class VFPreload_InvokeOnLoadWorkEnded_Patch
        {
            [HarmonyPostfix]
            [HarmonyPriority(1)]
            public static void Postfix()
            {
                //未翻訳のMODアイテム名と説明分、MOD技術名と説明文の翻訳
                if (Localization.language == Language.frFR)
                {
                    foreach (var item in LDB.items.dataArray)
                    {
                        if (item == null || item.name == null || item.description == null)
                            continue;

                        if (JPDictionary.ContainsKey(item.name))
                        {
                            item.name = JPDictionary[item.name];
                        }
                        if (JPDictionary.ContainsKey(item.description))
                        {
                            item.description = JPDictionary[item.description];
                        }
                    }

                    foreach (var tech in LDB.techs.dataArray)
                    {
                        if (tech == null || tech.name == null || tech.description == null)
                            continue;

                        if (JPDictionary.ContainsKey(tech.name))
                        {
                            tech.name = JPDictionary[tech.name];
                        }
                        if (JPDictionary.ContainsKey(tech.description))
                        {
                            tech.description = JPDictionary[tech.description];
                        }
                    }
                    LogManager.Logger.LogInfo("MODを翻訳しました");

                }
            }
        }

    }



    public class LogManager
    {
        public static ManualLogSource Logger;
    }
}