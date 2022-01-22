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
    class UIPatches
    {

        public static bool BeltCheckSignUpdated = false;


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////表示の修正//////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////


        //アイテムチップの増産剤効果表示の調整
        [HarmonyPatch(typeof(UIItemTip), "SetTip")]
        public static class UIItemTip_SetTip_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UIItemTip __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    GameObject incEffectName1 = __instance.transform.Find("inc-effect-name-1").gameObject;
                    incEffectName1.transform.localPosition = new Vector3(25f, incEffectName1.transform.localPosition.y, 0f);
                    GameObject incEffectName2 = incEffectName1.transform.Find("inc-effect-name-2").gameObject;
                    incEffectName2.transform.localPosition = new Vector3(153f, 0f, 0f);
                    GameObject incEffectName3 = incEffectName1.transform.Find("inc-effect-name-3").gameObject;
                    incEffectName3.transform.localPosition = new Vector3(253f, 0f, 0f);
                    incEffectName3.transform.localScale = new Vector3(0.8f, 1f, 1f);
                    incEffectName3.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 16f);
                }
            }
        }

        //メカエディタ表示の調整
        [HarmonyPatch(typeof(UIMechaEditor), "_OnInit")]
        public static class UIMechaEditorl_OnInit_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UIMechaEditor __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    GameObject Text1 = __instance.transform.Find("Left Panel/scroll-view/Viewport/Left Panel Content/part-group/disable-all-button/Text").gameObject;
                    Text1.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject Text2 = __instance.transform.Find("Left Panel/scroll-view/Viewport/Left Panel Content/part-group/enable-all-button/Text").gameObject;
                    Text2.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject Text3 = __instance.transform.Find("Left Panel/scroll-view/Viewport/Left Panel Content/bone-group/disable-all-button/Text").gameObject;
                    Text3.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject Text4 = __instance.transform.Find("Left Panel/scroll-view/Viewport/Left Panel Content/bone-group/enable-all-button/Text").gameObject;
                    Text4.transform.localScale = new Vector3(0.7f, 1f, 1f);
                }
            }
        }



        //ダイソンスフィアエディタ表示の調整１
        [HarmonyPatch(typeof(UIDESwarmPanel), "_OnInit")]
        public static class UIDESwarmPanel_OnInit_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UIDESwarmPanel __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    GameObject inEditorText1 = __instance.transform.Find("display-group/display-toggle-1/checkbox-editor/in-editor-text").gameObject;
                    inEditorText1.transform.localPosition = new Vector3(35f, 0f, 0f);
                    inEditorText1.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject inGameText1 = __instance.transform.Find("display-group/display-toggle-1/checkbox-game/in-game-text").gameObject;
                    inGameText1.transform.localPosition = new Vector3(33f, 0f, 0f);
                    inGameText1.transform.localScale = new Vector3(0.8f, 1f, 1f);
                    GameObject displayText = __instance.transform.Find("display-group/display-toggle-2/display-text").gameObject;
                    displayText.transform.localScale = new Vector3(0.66f, 1f, 1f);
                    GameObject inEditorText2 = __instance.transform.Find("display-group/display-toggle-2/checkbox-editor/in-editor-text").gameObject;
                    inEditorText2.transform.localPosition = new Vector3(35f, 0f, 0f);
                    inEditorText2.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject inGameText2 = __instance.transform.Find("display-group/display-toggle-2/checkbox-game/in-game-text").gameObject;
                    inGameText2.transform.localPosition = new Vector3(33f, 0f, 0f);
                    inGameText2.transform.localScale = new Vector3(0.8f, 1f, 1f);
                }
            }
        }

        //ダイソンスフィアエディタ表示の調整２
        [HarmonyPatch(typeof(UIDELayerPanel), "_OnInit")]
        public static class UIDELayerPanel_OnInit_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UIDELayerPanel __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    GameObject inEditorText3 = __instance.transform.Find("display-group/display-toggle-1/checkbox-editor/in-editor-text").gameObject;
                    inEditorText3.transform.localScale = new Vector3(0.7f, 1f, 1f);
                    GameObject inEditorText4 = __instance.transform.Find("display-group/display-toggle-2/checkbox-editor/in-editor-text").gameObject;
                    inEditorText4.transform.localScale = new Vector3(0.7f, 1f, 1f);
                }
            }
        }



        //組み立て機等のアラームボタンの調整
        [HarmonyPatch]
        public static class alarmSwitchButton_Patch
        {
            [HarmonyTargetMethods]
            static IEnumerable<MethodBase> TargetMethods()
            {
                yield return AccessTools.Method(typeof(UIAssemblerWindow), "_OnInit");
                yield return AccessTools.Method(typeof(UIMinerWindow), "_OnInit");
                yield return AccessTools.Method(typeof(UIPowerGeneratorWindow), "_OnInit");
                yield return AccessTools.Method(typeof(UISiloWindow), "_OnInit");
                yield return AccessTools.Method(typeof(UILabWindow), "_OnInit");
                yield return AccessTools.Method(typeof(UIVeinCollectorPanel), "_OnInit");
            }
            [HarmonyPostfix]
            public static void Postfix(ref UIButton ___alarmSwitchButton)
            {
                if (Main.EnableFixUI.Value)
                {
                    ___alarmSwitchButton.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 22);
                    ___alarmSwitchButton.transform.Find("alarm-state-text").transform.localPosition = new Vector3(44, 9, 0);
                }
            }
        }

        //マイルストーンの説明文で、惑星名のスペースで改行されてしまう問題の解消
        [HarmonyPatch(typeof(CommonUtils), "ToNonBreakingString")]
        public static class CommonUtils_ToNonBreakingString_PrePatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref string __result, string str)
            {
                if (str.Contains(" "))
                {
                    __result = str.Replace(" ", "\u00a0");
                    return false;
                }
                if (str.Contains("_"))
                {
                    __result = str.Replace("_", " ");
                    return false;
                }
                return true;
            }
        }

        //SailIndicatorの日本語化
        [HarmonyPatch(typeof(UISailIndicator), "_OnInit")]
        public static class UISailIndicator_OnInit_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UISailIndicator __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    GameObject SailIndicator = GameObject.Find("UI Root/Auxes/Sail Indicator/group");
                    SailIndicator.transform.Find("labels").GetComponent<TextMesh>().text = "\n\n\n\n\n到着まで\n偏角\n方位角                                   仰俯角";
                    SailIndicator.transform.Find("labels").GetComponent<TextMesh>().transform.localScale = new Vector3(0.7f, 1, 1);
                    SailIndicator.transform.Find("dist").transform.position = new Vector3(0.71f, -0.565f, 0);
                    SailIndicator.transform.Find("eta").transform.position = new Vector3(1.1f, -1.723f, 0);
                    SailIndicator.transform.Find("bias").transform.position = new Vector3(1.1f, -2.067f, 0);
                    SailIndicator.transform.Find("yaw").transform.position = new Vector3(1.1f, -2.411f, 0);
                    SailIndicator.transform.Find("yaw-sign").transform.position = new Vector3(1f, -2.44f, 0);
                    SailIndicator.transform.Find("pitch").transform.position = new Vector3(3.5f, -2.411f, 0);
                    SailIndicator.transform.Find("pitch-sign").transform.position = new Vector3(3.4f, -2.44f, 0);
                }
            }
        }
        //ブループリント保存画面のUI修正１
        [HarmonyPatch(typeof(UIBlueprintBrowser), "_OnOpen")]
        public static class UIBlueprintBrowser_OnOpen_Harmony
        {
            [HarmonyPostfix]
            public static void Postfix(UIBlueprintBrowser __instance)
            {
                if (Main.EnableFixUI.Value)
                {

                    __instance.transform.Find("inspector-group/delete-button").GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30);
                    __instance.transform.Find("inspector-group/group-1/thumbnail-image/layout-combo/label").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                    __instance.transform.Find("folder-info-group/delete-button").GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30);
                }
            }
        }
        //ブループリント保存画面のUI修正１
        [HarmonyPatch(typeof(UIBlueprintInspector), "_OnOpen")]
        public static class UIBlueprintInspector_OnOpen_Harmony
        {
            [HarmonyPostfix]
            public static void Postfix(UIBlueprintInspector __instance)
            {
                if (Main.EnableFixUI.Value)
                {

                    __instance.transform.Find("Blueprint Copy Inspector/group-1/thumbnail-image/layout-combo/label").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                    __instance.transform.Find("Blueprint Copy Inspector/group-1/save-state-text").transform.localPosition = new Vector3(80, -30, 0);
                }
            }
        }

        //トラフィックモニタの表示調整
        [HarmonyPatch(typeof(UIMonitorWindow), "_OnInit")]
        public static class UIMonitorWindow_OnInit_PostPatch
        {
            [HarmonyPostfix]
            public static void Postfix(UIMonitorWindow __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    __instance.transform.Find("speaker-panel/volume/label").transform.localScale = new Vector3(0.8f, 1, 1);
                    __instance.transform.Find("alarm-settings/system-mode/system-label").transform.localScale = new Vector3(0.7f, 1, 1);
                    __instance.transform.Find("alarm-settings/system-mode/system-label").GetComponent<RectTransform>().sizeDelta = new Vector2(120, 30);
                    __instance.transform.Find("alarm-settings/system-mode/system-mode-box/Main Button").GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
                    __instance.transform.Find("alarm-settings/speaker-mode/speaker-label").transform.localScale = new Vector3(0.7f, 1, 1);
                    __instance.transform.Find("alarm-settings/speaker-mode/speaker-label").GetComponent<RectTransform>().sizeDelta = new Vector2(120, 30);
                    __instance.transform.Find("alarm-settings/speaker-mode/speaker-mode-box/Main Button").GetComponent<RectTransform>().sizeDelta = new Vector2(10, 0);
                    __instance.transform.Find("alarm-settings/signal/icon-tag-label").transform.localScale = new Vector3(0.7f, 1, 1);
                    __instance.transform.Find("alarm-settings/signal/icon-tag-label").GetComponent<RectTransform>().sizeDelta = new Vector2(120, 24);
                    __instance.transform.Find("monitor-settings/cargo-filter/icon-tag-label").transform.localScale = new Vector3(0.7f, 1, 1);
                    __instance.transform.Find("monitor-settings/cargo-filter/icon-tag-label").GetComponent<RectTransform>().sizeDelta = new Vector2(120, 24);
                }
            }
        }

        //建築メニューの[☑コンベアベルト]の位置調整
        [HarmonyPatch(typeof(UIBuildMenu), "UpdateUXPanel")]
        public static class UIBuildMenu_UpdateUXPanels_PrePatch
        {
            [HarmonyPrefix]

            public static void Prefix(Image ___uxBeltCheckSign, UIButton ___uxBeltCheck)
            {
                if (!BeltCheckSignUpdated)
                {
                    ___uxBeltCheck.transform.Translate(-0.4f, 0, 0);
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
                if (Main.EnableFixUI.Value)
                {
                    var texts = GameObject.Find("UI Root/Overlay Canvas/DialogGroup/MessageBox VE(Clone)/Window/Body").GetComponentsInChildren<Text>();
                    //var texts = Resources.FindObjectsOfTypeAll(typeof(Text)) as Text[];
                    foreach (var text in texts)
                    {
                        text.font = Main.newFont;
                    }
                }
            }
        }

        //UIRandomTipのフック：バルーンチップのサイズ調整
        [HarmonyPatch(typeof(UIRandomTip), "_OnOpen")]
        static class UIRandomTip_OnOpen_Postfix
        {
            [HarmonyPostfix]
            static void Postfix(RectTransform ___balloonTrans)
            {
                if (Main.EnableFixUI.Value)
                {
                    ___balloonTrans.sizeDelta = new Vector2(290.0f, ___balloonTrans.sizeDelta.y - 18f);
                    //___balloonTrans.gameObject.GetComponentInParent<Text>().text = HyphenationJpn.GetFormatedText(___balloonTrans.gameObject.GetComponentInParent<Text>(), ___balloonTrans.gameObject.GetComponentInParent<Text>().text);
                }
            }
        }

        // UITechNodeのフック：テックツリーの技術名の位置調整 by aki9284
        [HarmonyPatch(typeof(UITechNode), "UpdateLayoutDynamic")]
        static class UITechNodePatch
        {
            [HarmonyPostfix]
            static void Postfix(Text ___titleText2, Text ___techDescText)
            {
                if (Main.EnableFixUI.Value)
                {
                    ___titleText2.rectTransform.anchoredPosition = new Vector2(0, 10.0f);
                }
            }
        }

        //新規開始画面の恒星タイプ名の文字位置調整
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
        static class UpdateUIDisplayPatch
        {
            [HarmonyPostfix]
            static void Postfix(UIGalaxySelect __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    __instance.transform.Find("right-group/m-star").GetComponent<Text>().text = "　　　　　　　" + "M型恒星".Translate();
                    __instance.transform.Find("right-group/k-star").GetComponent<Text>().text = "　　　　　　　" + "K型恒星".Translate();
                    __instance.transform.Find("right-group/g-star").GetComponent<Text>().text = "　　　　　　　" + "G型恒星".Translate();
                    __instance.transform.Find("right-group/f-star").GetComponent<Text>().text = "　　　　　　　" + "A型恒星".Translate();
                    __instance.transform.Find("right-group/a-star").GetComponent<Text>().text = "　　　　　　　" + "B型恒星".Translate();
                    __instance.transform.Find("right-group/b-star").GetComponent<Text>().text = "　　　　　　　" + "O型恒星".Translate();
                    __instance.transform.Find("right-group/o-star").GetComponent<Text>().text = "　　　　　　　" + "M型恒星".Translate();
                    __instance.transform.Find("right-group/n-star").GetComponent<Text>().text = "　　　　　　　" + "空格中子星".Translate();
                    __instance.transform.Find("right-group/wd-star").GetComponent<Text>().text = "　　　　　　　" + "空格白矮星".Translate();
                    __instance.transform.Find("right-group/bh-star").GetComponent<Text>().text = "　　　　　　　" + "空格黑洞".Translate();
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
                if (Main.EnableFixUI.Value)
                {

                    //LogManager.Logger.LogInfo("copyButton");
                    Text copyText = ___copyButton.GetComponent<Text>();
                    if (copyText != null)
                    {
                        float width = copyText.preferredWidth;
                        float height = copyText.preferredHeight;

                        RectTransform trs = (RectTransform)___copyButton.button.transform;

                        trs.offsetMin = new Vector2(-35, trs.offsetMin.y);
                        trs.offsetMax = new Vector2(35, trs.offsetMax.y);
                    }
                    // LogManager.Logger.LogInfo("pasteButton");
                    Text pasteText = ___pasteButton.GetComponent<Text>();
                    if (pasteText != null)
                    {
                        RectTransform trs = (RectTransform)___pasteButton.button.transform;
                        trs.offsetMin = new Vector2(10, trs.offsetMin.y);
                        trs.offsetMax = new Vector2(80, trs.offsetMax.y);
                    }
                }
            }
        }

    }
}
