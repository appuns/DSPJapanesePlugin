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
    [HarmonyPatch]
    internal class TranslatePatches
    {
        ////コンボボックスへ「日本語」を追加
        //[HarmonyPrefix, HarmonyPatch(typeof(UIOptionWindow), "TempOptionToUI")]
        //public static void UIOptionWindow_TempOptionToUI_Harmony(UIOptionWindow __instance)
        //{
        //    if (!__instance.languageComp.Items.Contains("日本語"))
        //    {
        //        __instance.languageComp.Items.Add("日本語");
        //        __instance.languageComp.itemIndex = 2;
        //    }
        //}

        //翻訳メイン
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "Translate", typeof(string))]
        public static bool Localization_Translate_Prefix(ref string __result, string s)
        {

            if (s == null)
            {
                    __result = "";
                    return false;
            }
            if (Localization.namesIndexer == null || Localization.currentStrings == null)
            {
                __result = s;
                    return false;
            }
            if (Main.JPDictionary.ContainsKey(s))
            {
                __result = Main.JPDictionary[s];

                return false;
            }
            if (!Localization.namesIndexer.ContainsKey(s))
            {
                __result = s;
                return false;
            }


            __result = Localization.currentStrings[Localization.namesIndexer[s]];
            return false;
        }

        //[HarmonyPrefix, HarmonyPatch(typeof(Localization), "Load", typeof(string))]
        public static bool Localization_Load_Prefix(ref string __result, string path)
        {

            if (Localization.Loaded)
            {
                Localization.Unload();
            }
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            Localization.ResourcesPath = path.Replace('\\', '/');
            if (Localization.ResourcesPath[Localization.ResourcesPath.Length - 1] != '/')
            {
                Localization.ResourcesPath += "/";
            }
            if (!Directory.Exists(Localization.ResourcesPath))
            {
                Localization.ResourcesPath = null;
                return false;
            }
            if (Localization.LoadSettings())
            {
                int num = 0;
                int languageCount = Localization.LanguageCount;
                for (int i = 0; i < languageCount; i++)
                {
                    if (Localization.Languages[i].lcId == Localization.preSelectLanguageLCID)
                    {
                        num = i;
                        break;
                    }
                }
                Localization.currentLanguageIndex = num;
                if (Localization.Languages.Length > num && Localization.LoadLanguage(num))
                {
                    Localization.currentStrings = Localization.strings[num];
                    Localization.currentFloats = Localization.floats[num];
                    Localization.NotifyLanguageChange();
                    return true;
                }
            }
            Localization.Unload();
            return true;
        }


        ////リソース全体のTextのフォントを変更   //新規文字列のチェック
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        public static void VFPreload_PreloadThread_Patch()
        {
            var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
            foreach (var text in texts)
            {
                //フォント
                if (text.font != null && text.font.name != "DIN")
                {
                    text.font = Main.newFont;
                }

                if (Main.JPDictionary.ContainsKey(text.text))
                {
                    text.text = Main.JPDictionary[text.text];
                }
            }
            LogManager.Logger.LogInfo("フォントを変更しました");




            NewStrings.Check();
        }




    }
}
