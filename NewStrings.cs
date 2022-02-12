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
            string assetPath = Path.Combine(Paths.GameRootPath, @"DSPGAME_Data\resources.assets");

            using (var fs = new FileStream(assetPath, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                byte[] StringBinary = { 0x53, 0x74, 0x72, 0x69, 0x6e, 0x67, 0x50, 0x72, 0x6f, 0x74, 0x6f, 0x53, 0x65, 0x74, 0x00, 0x00 };
                int matchCount = 0;
                byte[] checker;
                while (matchCount != 15)
                {
                    checker = br.ReadBytes(16);
                    for (int j = 0; j < 16; j++)
                    {
                        if (checker[j] != StringBinary[j])
                        {
                            break;
                        }
                        matchCount = j;
                    }
                }
                if (matchCount == 15)
                {
                    LogManager.Logger.LogInfo("StringProtoSet取得成功");

                    string path = LDB.protoResDir + typeof(StringProtoSet).Name;
                    StringProtoSet strings = (Resources.Load(path) as StringProtoSet);
                    StringProtoSet stringProtoSet = Localization.strings;
                    var tsvText = new StringBuilder();

                    br.ReadBytes(12);
                    br.ReadBytes(4);
                    var stringsCount = br.ReadInt32();

                    for (int j = 0; j < stringsCount; j++)
                    {
                        string keyString = readString(br);

                        if (!Main.JPDictionary.ContainsKey(keyString))
                        {
                            int Index = br.ReadInt32();
                            string numString = readString(br);
                            string ZHCNString = readStringAndReplace(br);
                            string ENUSString = readStringAndReplace(br);
                            string FRFRString = readStringAndReplace(br);

                            //記号を置換
                            string ENUSStringReplaced = ENUSString.Replace("#", "[SHARP]");//.Replace("{0}", "[FORMATTEDNUM]");


                            IEnumerator coroutine = GASAccess.TranslateString(ENUSStringReplaced);
                            coroutine.MoveNext();
                            //記号を戻す
                            string JpString = coroutine.Current.ToString().Replace("<color = ", "<color=").Replace("</ color>", "</color>").Replace("[SHARP] ","#").Replace("[SHARP]", "#").Replace("<size = ", "<size=").Replace("</ size>", "</size>");//.Replace(" [FORMATTEDNUM] ", "{0}").Replace("[FORMATTEDNUM]", "{0}");

                            tsvText.Append($"{Index}\t{numString}\t{keyString}\t{JpString}\tnew\t\t{ENUSString}\t{ZHCNString}\t{FRFRString}\r\n");
                            //LogManager.Logger.LogInfo($" {Index} : {numString} : {keyString} : {JpString}: {ENUSString}  : {FRFRString}");
                            //string Text = $"{Index}\t{numString}\t{keyString}\t{JpString}\tnew\t\t{ENUSString}\t{ZHCNString}\t{FRFRString}\r\n";
                            //File.AppendAllText(Main.newStringsFilePath, Text);

                        }
                        else
                        {
                            int Index = br.ReadInt32();
                            string numString = readString(br);
                            string ZHCNString = readString(br);
                            string ENUSString = readString(br);
                            string FRFRString = readString(br);
                        }
                    }
                    br.Dispose();
                    if (tsvText.Length == 0)
                    {
                        LogManager.Logger.LogInfo("新規文字列はありません");
                    }
                    else
                    {
                        LogManager.Logger.LogInfo($"新規文字列がありましたので、{Main.newStringsFilePath}に書き出しました。");
                    }

                    //File.WriteAllText(Main.newStringsFilePath, tsvText.ToString());
                }
                else
                {
                    LogManager.Logger.LogInfo("StringProtoSet取得失敗");
                }
            }

        }

        public static string readString(BinaryReader br)
        {
            int length = br.ReadInt32();
            if (length > 0)
            {
                byte[] tmpBytes = br.ReadBytes(length);
                string tmp = Encoding.UTF8.GetString(tmpBytes);
                //余りのバイトを読み込む
                if (length % 4 > 0)
                {
                    br.ReadBytes(4 - length % 4);
                }
                return tmp;
            }
            else
            {
                return "";
            }
        }
        public static string readStringAndReplace(BinaryReader br)
        {
            int length = br.ReadInt32();
            if (length > 0)
            {
                byte[] tmpBytes = br.ReadBytes(length);
                byte[] replacedBytes = new byte[length];
                int add = 0;
                for (int i = 0; i < length; i++)
                {
                    if (tmpBytes[i] == 0x0A)
                    {
                        Array.Resize(ref replacedBytes, length + add + 3);
                        replacedBytes[i + add] = 0x5B;
                        replacedBytes[i + add + 1] = 0x4C;
                        replacedBytes[i + add + 2] = 0x46;
                        replacedBytes[i + add + 3] = 0x5D;
                        add += 3;
                   }
                    else
                    {
                        replacedBytes[i + add] = tmpBytes[i];
                    }
                }
                string tmp;
                if (add == 0)
                {
                    tmp = Encoding.UTF8.GetString(tmpBytes);
                }
                else
                {
                    tmp = Encoding.UTF8.GetString(replacedBytes);
                }
                //余りのバイトを読み込む
                if (length % 4 > 0)
                {
                    br.ReadBytes(4 - length % 4);
                }
                return tmp;
            }
            else
            {
                return "";
            }
        }
    }
}
