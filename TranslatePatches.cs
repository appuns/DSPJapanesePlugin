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
        //コンボボックスへ「日本語」を追加
        [HarmonyPrefix, HarmonyPatch(typeof(UIOptionWindow), "TempOptionToUI")]
        public static void UIOptionWindow_TempOptionToUI_Harmony(UIOptionWindow __instance)
        {
            if (!__instance.languageComp.Items.Contains("日本語"))
            {
                __instance.languageComp.Items.Add("日本語");
                __instance.languageComp.itemIndex = 2;
            }
        }

        //翻訳メイン
        [HarmonyPrefix, HarmonyPatch(typeof(StringTranslate), "Translate", typeof(string))]
        public static bool StringTranslate_Translate_Prefix(ref string __result, string s)
        {
            if (Localization.language == Language.frFR)
            {
                if (s == null)
                {
                    __result = "";
                    return false;
                }
                if (Main.JPDictionary.ContainsKey(s))
                {
                    __result = Main.JPDictionary[s];
                    return false;
                }
                //日本語辞書に無い場合
                StringProtoSet strings = Localization.strings;
                if (strings == null)
                {
                    __result = s;
                    return false;
                }
                StringProto stringProto = strings[s];
                if (stringProto == null)
                {
                    __result = s;
                    return false;
                }
                __result = stringProto.ENUS;
                return false;
            }
            return true;
        }


        //リソース全体のTextのフォントを変更   //新規文字列のチェック
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "PreloadThread")]
        public static void VFPreload_PreloadThread_Patch()
        {
            var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
            foreach (var text in texts)
            {
                //フォント
                if (text.font.name != "DIN")
                {
                    text.font = Main.newFont;
                }

                if (Main.JPDictionary.ContainsKey(text.text))
                {
                    text.text = Main.JPDictionary[text.text];
                }
            }
            LogManager.Logger.LogInfo("フォントを変更しました");

            //新規文字列のチェック
            if (Main.exportNewStrings.Value)
            {
                LogManager.Logger.LogInfo("新規文字列をチェックします");
                NewStrings.Check();
            }
        }


        //未翻訳のMODアイテム名と説明分、MOD技術名と説明文の翻訳  新規文字列チェック
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "InvokeOnLoadWorkEnded")]
        [HarmonyPriority(1)]
        public static void VFPreload_InvokeOnLoadWorkEnded_Patch()
        {
            //未翻訳のMODアイテム名と説明分、MOD技術名と説明文の翻訳
            if (Localization.language == Language.frFR)
            {
                foreach (var item in LDB.items.dataArray)
                {
                    if (item == null || item.name == null || item.description == null)
                        continue;

                    if (Main.JPDictionary.ContainsKey(item.name))
                    {
                        item.name = Main.JPDictionary[item.name];
                    }
                    if (Main.JPDictionary.ContainsKey(item.description))
                    {
                        item.description = Main.JPDictionary[item.description];
                    }
                }

                foreach (var tech in LDB.techs.dataArray)
                {
                    if (tech == null || tech.name == null || tech.description == null)
                        continue;

                    if (Main.JPDictionary.ContainsKey(tech.name))
                    {
                        tech.name = Main.JPDictionary[tech.name];
                    }
                    if (Main.JPDictionary.ContainsKey(tech.description))
                    {
                        tech.description = Main.JPDictionary[tech.description];
                    }
                }
                LogManager.Logger.LogInfo("MODを翻訳しました");

            }
        }


    }
}
