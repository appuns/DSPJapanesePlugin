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

        //CJKであることの強制判定  Version  0.10.28.21172
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "get_isCJK")]
        public static bool Localization_get_isCJK_Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }

        //KMGであることの強制判定  Version  0.10.28.21172
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "get_isKMG")]
        public static bool Localization_get_isKMG_Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }

        //ZHCNであることの強制判定  Version  0.10.28.21172
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "get_isZHCN")]
        public static bool Localization_get_isZHCN_Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }

        //ENUSでないことの強制判定  Version  0.10.28.21172
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "get_isENUS")]
        public static bool Localization_get_isENUS_Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }

        //FRFRでないことの強制判定  Version  0.10.28.21172
        [HarmonyPrefix, HarmonyPatch(typeof(Localization), "get_isFRFR")]
        public static bool Localization_get_isFRFR_Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }

        //#######################################データ取得テスト
        //[HarmonyPrefix, HarmonyPatch(typeof(Localization), "LoadLanguage")]
        public static void Localization_Load_Prefix(ref bool __result, int index)
        {
			if (!Localization.Loaded)
			{
				__result = false;
				return;
			}
			int num = Localization.Languages.Length;
			int num2 = Localization.resourcePages.Length;
			int namesCount = Localization.NamesCount;
			if ((ulong)index >= (ulong)((long)num))
			{
				__result = false;
				return;
			}
			string[] array = Localization.strings[index] = new string[namesCount];
			float[] array2 = Localization.floats[index] = new float[namesCount];
			StreamReader[] array3 = new StreamReader[num2];
			bool flag = true;
			try
			{
				for (int i = 0; i < num2; i++)
				{
					string text = string.Format("{0}{1}/{2}.txt", Localization.ResourcesPath, Localization.Languages[index].lcId, Localization.resourcePages[i]);
					if (new FileInfo(text).Exists)
					{
						array3[i] = new StreamReader(text, true);
						array3[i].Peek();
					}
				}
			}
			catch (Exception ex)
			{
				flag = false;
				Debug.LogError("[Locale] 访问文件路径失败：" + ex.ToString());
			}
			if (flag)
			{
				try
				{
					(new char[1])[0] = '\t';
					StringBuilder stringBuilder = new StringBuilder();
					StringBuilder stringBuilder2 = new StringBuilder();
					for (int j = 0; j < num2; j++)
					{
						if (array3[j] != null)
						{
							for (; ; )
							{
								string text2 = array3[j].ReadLine();
								if (text2 == null)
								{
									break;
								}
								if (!text2.Equals(string.Empty))
								{
									int length = text2.Length;
									int num3 = 0;
									stringBuilder.Clear();
									for (int k = num3; k < length; k++)
									{
										if (text2[k] == '\t')　　　　//一つ目のタブまで
										{
											stringBuilder.Append(text2, 0, k);　　
											num3 = k + 1;
											break;
										}
									}
									string text3 = Localization.UnescapeString(stringBuilder, stringBuilder2).ToString(); //////////////キーワード
									if (text3.Length != 0 && Localization.namesIndexer.ContainsKey(text3))
									{
										int num4 = Localization.namesIndexer[text3];
										if (array[num4] == null)
										{
											bool flag2 = false;
											bool flag3 = false;
											for (int l = num3; l < length; l++)
											{
												if (text2[l] == '\t') //２つ目のタブまで
												{
													num3 = l + 1;
													break;
												}
												if (text2[l] == '#')
												{
													flag2 = true;
												}
											}
											//---------------------------------------------------
											string text6 = text2[num3].ToString(); //値は取得していない
											//---------------------------------------------------
											for (int m = num3; m < length; m++)//３つ目のタブまで
											{
												if (text2[m] == '\t')
												{
													num3 = m + 1;
													flag3 = true;
													break;
												}
											}
											if (flag3)
											{
												stringBuilder.Clear();
												stringBuilder.Append(text2, num3, length - num3);
												string text4 = Localization.UnescapeString(stringBuilder, stringBuilder2).ToString();　//訳文
												array[num4] = text4;
												//---------------------------------------------------
												LogManager.Logger.LogInfo(index + " : " + text3 + " : " + text6 + " : " + array[num4]);
												//---------------------------------------------------
												if (flag2 && !float.TryParse(text4, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out array2[num4]))
												{
													array2[num4] = 0f;
													Debug.LogWarning("[Locale] 标识名 [" + text3 + "] 参数解析失败");
												}
											}
											else
											{
												Debug.LogWarning("[Locale] 标识名 [" + text3 + "] 文本读取失败");
											}
										}
									}
								}
							}
						}
					}
					stringBuilder.Clear();
					stringBuilder2.Clear();
					int num5 = 0;
					foreach (KeyValuePair<string, int> keyValuePair in Localization.namesIndexer)
					{
						if (array[keyValuePair.Value] == null)
						{
							array[keyValuePair.Value] = keyValuePair.Key;
							num5++;
						}
					}
					if (num5 > 0)
					{
						Debug.LogWarning(string.Format("[Locale] {0} 个词条未找到翻译文本", num5));
					}
				}
				catch (Exception ex2)
				{
					flag = false;
					Debug.LogError("[Locale] 读取过程失败：" + ex2.ToString());
				}
			}
			for (int n = 0; n < num2; n++)
			{
				if (array3[n] != null)
				{
					array3[n].Close();
					array3[n] = null;
				}
			}
			array3 = null;
			__result = flag;
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



		//////////////////////////////////////////////////////////////////////////////////
		//test
		//////////////////////////////////////////////////////////////////////////////////

		//[HarmonyPrefix, HarmonyPatch(typeof(UICursor), "LoadCursors")]
		public static void UICursor_LoadCursors_Prefix()
		{
			LogManager.Logger.LogInfo("------------------------------------------------------------------_UICursor_LoadCursors_Prefix");
			if (!UICursor.loaded)
			{
				UICursor.cursorTexs = new Texture2D[]
				{
				//Resources.Load<Texture2D>("UI/Cursor/cursor"),
				Resources.Load<Texture2D>("Icons/Property/property-icon-6002"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-transfer"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-target-in"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-target-out"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-target-a"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-target-b"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-ban"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-delete"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-reform"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-dyson-node-create"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-painter"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-eyedropper"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-eraser"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-upgrade"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-downgrade"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-blank"),
				Resources.Load<Texture2D>("UI/Cursor/cursor-remove")
				};
				UICursor.cursorHots = new Vector2[]
				{
				new Vector2(5f, 6f),
				new Vector2(16f, 22f),
				new Vector2(16f, 16f),
				new Vector2(16f, 16f),
				new Vector2(9f, 9f),
				new Vector2(9f, 9f),
				new Vector2(16f, 16f),
				new Vector2(16f, 16f),
				new Vector2(4f, 4f),
				new Vector2(4f, 4f),
				new Vector2(4f, 26f),
				new Vector2(4f, 26f),
				new Vector2(6f, 27f),
				new Vector2(16f, 12f),
				new Vector2(16f, 20f),
				new Vector2(16f, 16f),
				new Vector2(4f, 26f)
				};
				UICursor.loaded = true;
				UICursor._cursorIndexApplied = 0;
				///////////////////////////////////////////////////////////////////////////////////
				//MeshRenderer targetMeshRenderer;
		  //      Material targetMaterial;
				//targetMaterial = targetMeshRenderer.material;
				//var resizedTexture = new Texture2D(UICursor.cursorTexs[0].width * 2, UICursor.cursorTexs[0].height * 2);
				//Graphics.ConvertTexture(UICursor.cursorTexs[0], resizedTexture);
				//UICursor.cursorTexs[0] = resizedTexture;
				//targetMaterial.SetTexture("_MainTex", UICursor.cursorTexs[0]);
				
				
				//var rt = RenderTexture.GetTemporary(UICursor.cursorTexs[0].width * 2, UICursor.cursorTexs[0].height * 2);
				//Graphics.Blit(UICursor.cursorTexs[0], rt);
				//var preRT = RenderTexture.active;
				//RenderTexture.active = rt;
				//var ret = new Texture2D(UICursor.cursorTexs[0].width * 2, UICursor.cursorTexs[0].height * 2);
				//ret.ReadPixels(new Rect(0, 0, UICursor.cursorTexs[0].width * 2, UICursor.cursorTexs[0].height * 2), 0, 0);
				//ret.Apply();
				//RenderTexture.active = preRT;
				//RenderTexture.ReleaseTemporary(rt);
				//UICursor.cursorTexs[0] = ret;
				///////////////////////////////////////////////////////////////////////////////////
				Cursor.SetCursor(UICursor.cursorTexs[0], UICursor.cursorHots[0], CursorMode.Auto);
			}
		}
	}
}
