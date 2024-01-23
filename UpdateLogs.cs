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
    internal class UpdateLogs
    {
        public static bool UpdateLogCreated = false;

        //アップデートログを書き出し
        //[HarmonyPostfix, HarmonyPatch(typeof(UIMainMenu), "_OnCreate")]
        public static void UIMainMenu_UpdateLogText_Postfix(UIMainMenu __instance)
        {
            if(UpdateLogCreated)
            {
                return;
            }
            for (int j = GlobalObject.versionList.Count - 1; j > 0; j--)
            {
                string text2 = "<b><color=\"#ffee00\">[Version " + GlobalObject.versionList[j].ToFullString() + "]</color></b>\r\n";
                string text3 = "";

                for (UpdateLogType updateLogType = UpdateLogType.Features; updateLogType <= UpdateLogType.Bugfix; updateLogType++)
                {

                    text3 = text3 + "   <b><color=\"#ffee00\">" + updateLogType.ToString() + ":</color></b>\r\n";

                    for (int k = 0; k < __instance.updateLogs.Length; k++)
                    {
                        UpdateLog updateLog = __instance.updateLogs[k];
                        for (int l = 0; l < updateLog.logs.Length; l++)
                        {
                            text3 += updateLog.logs[l].logEn;
                            text3 += "\r\n";
                        }
                    }
                }
                LogManager.Logger.LogInfo(text2);
                LogManager.Logger.LogInfo(text3);
                LogManager.Logger.LogInfo("\r\n");
            }
            UpdateLogCreated = true;
        }
    }
}
