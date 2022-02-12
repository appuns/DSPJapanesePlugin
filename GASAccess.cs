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

namespace DSPJapanesePlugin
{
    class GASAccess
    {
        //辞書ファイルの更新チェック＆ダウンロードコルーチン
        public static IEnumerator CheckAndDownload(String Url, String dstPath)
        {
            string LastUpdate;
            if (!File.Exists(dstPath))
            {
                LastUpdate = "0";
            }
            else
            {
                LastUpdate = System.IO.File.GetLastWriteTime(dstPath).ToString("yyyyMMddHHmmss");
            }
            LogManager.Logger.LogInfo("URL : " + $"{Url}?date={LastUpdate}");
            UnityWebRequest request = UnityWebRequest.Get($"{Url}?date={LastUpdate}");
            request.timeout = 10;
            AsyncOperation checkAsync = request.SendWebRequest();
            while (!checkAsync.isDone) ;
            if (request.isNetworkError || request.isHttpError)
            {
                LogManager.Logger.LogInfo("辞書チェックエラー : " + request.error);
            }
            else
            {
                if (request.downloadHandler.text == "match")
                {
                    LogManager.Logger.LogInfo("辞書は最新です");
                    Main.JPDictionary = JSON.FromJson<Dictionary<string, string>>(File.ReadAllText(dstPath));
                }
                else if (request.downloadHandler.data.Length < 2000)
                {
                    LogManager.Logger.LogInfo("辞書のダウンロードに失敗しました　:　" + Regex.Match(request.downloadHandler.text, @"TypeError.*）"));
                }
                else
                {
                    LogManager.Logger.LogInfo("辞書をダウンロードしました");
                    Main.JPDictionary = JSON.FromJson<Dictionary<string, string>>(request.downloadHandler.text);
                    File.WriteAllText(dstPath, request.downloadHandler.text);
                }
            }
            yield return null;
        }

        //辞書ファイルをスプレッドシートから取得コルーチン
        public static IEnumerator MakeFromSheet(String Url, String dstPath)
        {
            LogManager.Logger.LogInfo("URL : " + Url);

            UnityWebRequest request = UnityWebRequest.Get($"{Url}");
            request.timeout = 10;
            AsyncOperation checkAsync = request.SendWebRequest();
            while (!checkAsync.isDone) ;

            if (request.isNetworkError || request.isHttpError)
            {
                LogManager.Logger.LogInfo("辞書チェックエラー : " + request.error);
            }
            else if (request.downloadHandler.data.Length < 2000)
            {
                LogManager.Logger.LogInfo("辞書のダウンロードに失敗しました　:　" + Regex.Match(request.downloadHandler.text, @"TypeError.*）"));
            }
            else
            {
                LogManager.Logger.LogInfo("辞書をダウンロードしました");
                var strings = request.downloadHandler.text.Replace("[LF]", @"\n").Replace("[CRLF]", @"\r\n");
                Main.JPDictionary = JSON.FromJson<Dictionary<string, string>>(strings);
                File.WriteAllText(dstPath, strings);

            }
            yield return null;
        }

        //辞書ファイルのダウンロードコルーチン
        public static IEnumerator DownloadAndSave(String Url, String dstPath)
        {
            UnityWebRequest request = UnityWebRequest.Get(Url);

            AsyncOperation checkAsync = request.SendWebRequest();

            while (!checkAsync.isDone) ;

            if (request.isNetworkError || request.isHttpError)
            {
                LogManager.Logger.LogInfo("Dictionary download error : " + request.error);
            }
            else
            {
                LogManager.Logger.LogInfo("Dictionary downloaded");
                File.WriteAllText(dstPath, request.downloadHandler.text);
                LogManager.Logger.LogInfo("Dictionary saved ");
            }
            yield return null;
        }

        //GAS翻訳コルーチン
        public static IEnumerator TranslateString(String ENUSString)
        {
            if (ENUSString == "") yield return "";
            UnityWebRequest request = UnityWebRequest.Get($"{Main.TranslateGAS.Value}?text={ENUSString}");
            request.timeout = 10;
            AsyncOperation checkAsync = request.SendWebRequest();
            while (!checkAsync.isDone) ;
            if (request.isNetworkError || request.isHttpError)
            {
                LogManager.Logger.LogInfo("GASアクセスエラー : " + request.error);
            }
            else
            {
                if (request.downloadHandler.text == "traslateFailed")
                {
                    LogManager.Logger.LogInfo("GAS翻訳に失敗しました。");
                }
                else
                {
                    LogManager.Logger.LogInfo("GAS翻訳に成功しました。: " + request.downloadHandler.text);
                }
            }

            yield return request.downloadHandler.text;
        }

    }
}
