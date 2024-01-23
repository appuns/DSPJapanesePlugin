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
    public class NewStrings
    {
        public static void Check()
        {
            //新規文字列のチェック
            //LogManager.Logger.LogInfo(Localization.namesIndexer.Count);
            var tsvText = new StringBuilder();

            foreach (KeyValuePair<string, int> keyValuePair in Localization.namesIndexer)
            {
                //LogManager.Logger.LogInfo(keyValuePair.Key + ":" + keyValuePair.Value + ":" + Localization.currentStrings[keyValuePair.Value]);
                if (!Main.JPDictionary.ContainsKey(keyValuePair.Key))
                {
                    //記号を置換
                    string ENUSStringReplaced = Localization.currentStrings[keyValuePair.Value].Replace("#", "[SHARP]").Replace("\r\n", "[CRLF]").Replace("\n", "[LF]");


                    //IEnumerator coroutine = GASAccess.TranslateString(ENUSStringReplaced);
                    //coroutine.MoveNext();
                    //記号を戻す
                    string JpString = ""; //coroutine.Current.ToString().Replace("<color = ", "<color=").Replace("</ color>", "</color>").Replace("[SHARP] ", "#").Replace("[SHARP]", "#").Replace("<size = ", "<size=").Replace("</ size>", "</size>");//.Replace(" [FORMATTEDNUM] ", "{0}").Replace("[FORMATTEDNUM]", "{0}");
                    //LogManager.Logger.LogInfo($"{keyValuePair.Key}\t{JpString}\t\t\tnew\t\t{ENUSStringReplaced}\t\t\r\n");
                    tsvText.Append($"{keyValuePair.Key}\t{JpString}\t\tnew\t{ENUSStringReplaced}\t\r\n");
                }
                else {
                    Main.JPDictionary[keyValuePair.Key] = Main.JPDictionary[keyValuePair.Key].Replace(" ", "\u00a0");



                }


            }

            if (tsvText.Length == 0)
            {
                LogManager.Logger.LogInfo("新規文字列はありません");
            }
            else
            {
                File.WriteAllText(Main.newStringsFilePath, tsvText.ToString());
                LogManager.Logger.LogInfo($"新規文字列がありましたので、{Main.newStringsFilePath}に書き出しました。");
            }


        }

    }
}
