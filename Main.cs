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
    [BepInPlugin("Appun.DSP.plugin.JapanesePlugin", "DSPJapanesePlugin", "1.2.2")]

    public class Main : BaseUnityPlugin
    {
        public static AssetBundle FontAssetBundle { get; set; }

        public static Font newFont { get; set; }
        public static Dictionary<string, string> JPDictionary { get; set; }

        //public static StringProtoSet _strings;


        public static ConfigEntry<bool> EnableFixUI;
        public static ConfigEntry<bool> EnableAutoUpdate;
        public static ConfigEntry<bool> ImportSheet;
        public static ConfigEntry<bool> exportNewStrings;
        //public static ConfigEntry<bool> firstBoot;
        public static ConfigEntry<string> DictionaryGAS;
        public static ConfigEntry<string> SsheetGAS;
        public static ConfigEntry<string> TranslateGAS;

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
            TranslateGAS = Config.Bind("翻訳者、開発者向けの設定：基本的に変更しないでください。", "TraslateGAS", "https://script.google.com/macros/s/AKfycbzaQLfuzNbo-uOO0XtLKq6xjQIgNC2_IibXbVzZEEtSRXBWKD06q8OuDMbZd_XQXHH8/exec", "google翻訳のスクリプトアドレス");
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
            //辞書をダウンロードしない設定か、ダウンロード失敗したら
            if (JPDictionary == null)
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
            //Localization.language = Language.frFR;


        }


    }



    public class LogManager
    {
        public static ManualLogSource Logger;
    }
}