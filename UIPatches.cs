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


        //////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////表示の修正//////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////

        public static bool BeltCheckSignUpdated = false;

        //建築メニューの[☑コンベアベルト]の位置調整
        [HarmonyPrefix, HarmonyPatch(typeof(UIBuildMenu), "UpdateUXPanel")]
        public static void PrUIBuildMenu_UpdateUXPanels_PrePatchefix(Image ___uxBeltCheckSign, UIButton ___uxBeltCheck)
        {
            if (!BeltCheckSignUpdated)
            {
                ___uxBeltCheck.transform.Translate(-0.4f, 0, 0);
                BeltCheckSignUpdated = true;
            }
        }

        //戦場分析基地ウインドウの修正 Version 0.10.28.20856
        [HarmonyPostfix, HarmonyPatch(typeof(UIBattleBaseWindow), "_OnCreate")]
        public static void UIBattleBaseWindow_OnCreate_Patch(UIBattleBaseWindow __instance)
        {
            GameObject droneTitle = __instance.transform.Find("module-group/drone/drone-title").gameObject;
            droneTitle.GetComponent<RectTransform>().sizeDelta = new Vector2(80f, 20f);
        }
        
        //タレットウインドウの修正 Version 0.10.28.20856
        [HarmonyPostfix, HarmonyPatch(typeof(UITurretWindow), "_OnCreate")]
        public static void UITurretWindow_OnCreate_Patch(UITurretWindow __instance)
        {
            __instance.activeGroupBtn.gameObject.transform.localScale = new Vector3(0.8f, 1, 1);
            GameObject currentRof = __instance.transform.Find("supernova-desc/effect-desc/current-rof").gameObject;
            currentRof.transform.localScale = new Vector3(0.8f, 1f, 1f);
            GameObject currentPower = __instance.transform.Find("supernova-desc/effect-desc/current-power").gameObject;
            currentPower.transform.localScale = new Vector3(0.8f, 1f, 1f);
            GameObject burstPower = __instance.transform.Find("supernova-desc/effect-desc/burst-power").gameObject;
            burstPower.transform.localScale = new Vector3(0.6f, 1f, 1f);

            //GameObject select1Btn = __instance.select1Btn.gameObject;
            GameObject select1Btn = __instance.transform.Find("supernova-desc/switch-button-1").gameObject;
            select1Btn.transform.localPosition = new Vector3(-56f, -24f, 0f); //-56 -24 0
            select1Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 24f); //107 24
            GameObject select2Btn = __instance.transform.Find("supernova-desc/switch-button-2").gameObject;
            select2Btn.transform.localPosition = new Vector3(-56f, -48f, 0f); //-56 -48 0
            select2Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 24f); //107 24
            GameObject select3Btn = __instance.transform.Find("supernova-desc/switch-button-3").gameObject;
            select3Btn.transform.localPosition = new Vector3(-56f, -72f, 0f); //-56 -72 0
            select3Btn.GetComponent<RectTransform>().sizeDelta = new Vector2(130f, 24f); //107 24

            GameObject Text = __instance.transform.Find("ammo-desc/group-selection/group/Text").gameObject;
            Text.transform.localPosition = new Vector3(-18f, -7f, 0f); //-15 -7 0
        }

        //ロード画面の「サンドボックスツールを有効にする」の場所を調整　0.9.26.12891
        [HarmonyPostfix, HarmonyPatch(typeof(UILoadGameWindow), "_OnOpen")]
        public static void UILoadGameWindow_OnOpen_Patch(UILoadGameWindow __instance)
        {
            LogManager.Logger.LogInfo("UILoadGameWindow_OnOpen_Patch");

            __instance.loadSandboxGroup.transform.localPosition = new Vector3(450f, __instance.loadSandboxGroup.transform.localPosition.y, 0f);


        }



        ////サンドボックスツールの表示を調整　0.9.26.12891
        //[HarmonyPostfix, HarmonyPatch(typeof(UISandboxMenu), "_OnOpen")]
        //public static void UISandboxMenu_OnOpen_Patch()
        //{
        //    GameObject tip = GameObject.Find("UI Root/Overlay Canvas/In Game/Function Panel/Sandbox Menu/general-group/tools-group/fast-build-batch-size/tip").gameObject;
        //    tip.GetComponent<Text>().alignment = TextAnchor.UpperRight;
        //    tip.transform.localPosition = new Vector3(tip.transform.localPosition.x, -25f, 0f);
        //}

        //合成機ウインドウの無料アイテム関係の表示を調整　0.9.26.12891
        [HarmonyPostfix, HarmonyPatch(typeof(UIReplicatorWindow), "_OnOpen")]
        public static void UIReplicatorWindow_OnOpen_Patch()
        {
            GameObject label = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Replicator Window/recipe-group/instant-switch/label").gameObject;
            label.GetComponent<RectTransform>().sizeDelta = new Vector2(80f, 36f);
            GameObject buttonText = GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Replicator Window/recipe-group/sandbox-button-2/button-text").gameObject;
            buttonText.GetComponent<RectTransform>().sizeDelta = new Vector2(40f, 0f);
            buttonText.transform.localScale = new Vector3(0.75f, 1f, 1f);
        }

        //    //セーブ画面のクラウド保存の警告の場所を調整 →　不要　
        //    [HarmonyPostfix, HarmonyPatch(typeof(UISaveGameWindow), "_OnOpen")]
        //    public static void VFPreload_Start2_Patch()
        //    {
        //        GameObject tipText = GameObject.Find("UI Root/Overlay Canvas/Top Windows/Save Game Window/tip-text").gameObject;
        //        tipText.transform.localPosition = new Vector3(-666f, tipText.transform.localPosition.y, 0f);
        //    }

        //    //初回起動時の言語選択画面で「日本語」を表示する →　不要
        //    [HarmonyPostfix,HarmonyPatch(typeof(VFPreload), "Start")]
        //    public static void VFPreload_Start_Patch()
        //    {
        //        GameObject btnZh = GameObject.Find("UI Root/Overlay Canvas/Splash/Launch Game/language-init/btn-zh").gameObject;
        //        btnZh.transform.localPosition = new Vector3(-170f, 0f, 0f);
        //        GameObject btnEn = GameObject.Find("UI Root/Overlay Canvas/Splash/Launch Game/language-init/btn-en").gameObject;
        //        btnEn.transform.localPosition = new Vector3(0f, 0f, 0f);
        //        GameObject btnFr = GameObject.Find("UI Root/Overlay Canvas/Splash/Launch Game/language-init/btn-fr").gameObject;
        //        btnFr.transform.localPosition = new Vector3(170f, 0f, 0f);
        //        btnFr.GetComponent<RectTransform>().sizeDelta = new Vector3(150f, 36f, 0f);
        //        GameObject btnFrText = GameObject.Find("UI Root/Overlay Canvas/Splash/Launch Game/language-init/btn-fr/text").gameObject;
        //        btnFrText.GetComponent<Text>().text = "日本語";
        //        btnFr.SetActive(true);
        //        GameObject tipText = GameObject.Find("UI Root/Overlay Canvas/Splash/Launch Game/language-init/tip-text").gameObject;
        //        tipText.transform.localPosition = new Vector3(0f, 65f, 0f);
        //        tipText.GetComponent<Text>().text = "请选择语言\r\nPlease select your language\r\n言語を選択してください";
        //    }


        //ロード画面の画像でメタ寄与数が表示されない問題の修正
        [HarmonyPrefix, HarmonyPatch(typeof(UILoadGameWindow), "_OnCreate")]
        public static void UILoadGameWindowt__OnCreate(UILoadGameWindow __instance)
        {
            //LogManager.Logger.LogInfo("ロード画面の画像でメタ寄与数が表示されない問題の修正");

            __instance.propertyItemPrefab.countText.GetComponent<RectTransform>().sizeDelta = new Vector2(60f, 20f);

        }


        //レイレシーバーUIの修正
        [HarmonyPostfix, HarmonyPatch(typeof(UIPowerGeneratorWindow), "_OnInit")]
        public static void UIAssemblerWindow_OnInit_PostPatch()
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject.Find("UI Root/Overlay Canvas/In Game/Windows/Power Generator Window/ray-receiver/label-2").transform.localScale = new Vector3(0.75f, 1f, 1f);
            }
        }



        //    //アイテムチップの増産剤効果表示の調整
        //    //[HarmonyPostfix, HarmonyPatch(typeof(UIItemTip), "SetTip")]
        //    public static void UIItemTip_SetTip_PostPatch(UIItemTip __instance)
        //    {
        //        if (Main.EnableFixUI.Value)
        //        {
        //            __instance.incNameText1.rectTransform.anchoredPosition = new Vector2(40f, __instance.incNameText1.rectTransform.anchoredPosition.y);
        //            __instance.incNameText2.transform.localPosition = new Vector3(138f, 0f, 0f);
        //            __instance.incNameText3.transform.localPosition = new Vector3(238f, 0f, 0f);
        //            __instance.incNameText3.transform.localScale = new Vector3(0.8f, 1f, 1f);
        //            __instance.incNameText3.rectTransform.sizeDelta = new Vector2(110f, 16f);

        //            __instance.incDescText1[0].rectTransform.anchoredPosition = new Vector2(0f, __instance.incDescText1[0].rectTransform.anchoredPosition.y);
        //            __instance.incDescText2[0].transform.localPosition = new Vector3(178f, 0f, 0f);
        //            __instance.incDescText3[0].transform.localPosition = new Vector3(278f, 0f, 0f);
        //            __instance.incDescText1[1].rectTransform.anchoredPosition = new Vector2(0f, __instance.incDescText1[1].rectTransform.anchoredPosition.y);
        //            __instance.incDescText2[1].transform.localPosition = new Vector3(178f, 0f, 0f);
        //            __instance.incDescText3[1].transform.localPosition = new Vector3(278f, 0f, 0f);
        //            //__instance.incDescText1[0].text = "          " + __instance.incDescText1[0].text;
        //            //__instance.incDescText1[1].text = "          " + __instance.incDescText1[1].text;

        //            //GameObject incEffectName1 = __instance.transform.Find("inc-effect-name-1").gameObject;
        //            //incEffectName1.transform.localPosition = new Vector3(25f, incEffectName1.transform.localPosition.y, 0f);
        //            //GameObject incEffectName2 = incEffectName1.transform.Find("inc-effect-name-2").gameObject;
        //            //incEffectName2.transform.localPosition = new Vector3(153f, 0f, 0f);
        //            //GameObject incEffectName3 = incEffectName1.transform.Find("inc-effect-name-3").gameObject;
        //            //incEffectName3.transform.localPosition = new Vector3(253f, 0f, 0f);
        //            //incEffectName3.transform.localScale = new Vector3(0.8f, 1f, 1f);
        //            //incEffectName3.GetComponent<RectTransform>().sizeDelta = new Vector2(110f, 16f);
        //        }
        //    }





        //メカエディタ表示の調整１:パーツグループ
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMechaPartGroup), "_OnInit")]
        [HarmonyPatch(typeof(UIMechaPartGroup), "_OnOpen")]
        public static void UIMechaPartGroup_OnInit_PostPatch(UIMechaPartGroup __instance)
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject disableAllText = __instance.transform.Find("disable-all-button/Text").gameObject;
                disableAllText.transform.localScale = new Vector3(0.7f, 1f, 1f);
                GameObject enableAllText = __instance.transform.Find("enable-all-button/Text").gameObject;
                enableAllText.transform.localScale = new Vector3(0.7f, 1f, 1f);

            }
        }

        //メカエディタ表示の調整２:ボーングループ
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIMechaBoneGroup), "_OnInit")]
        [HarmonyPatch(typeof(UIMechaBoneGroup), "_OnOpen")]
        public static void UIMechaBoneGroup_OnInit_PostPatch(UIMechaBoneGroup __instance)
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject disableAllText = __instance.transform.Find("disable-all-button/Text").gameObject;
                disableAllText.transform.localScale = new Vector3(0.7f, 1f, 1f);
                GameObject enableAllText = __instance.transform.Find("enable-all-button/Text").gameObject;
                enableAllText.transform.localScale = new Vector3(0.7f, 1f, 1f);
                __instance.addButtonTip.transform.localScale = new Vector3(0.85f, 1f, 1f);

            }
        }

        //メカエディタ表示の調整３：マテリアルグループ
        [HarmonyPostfix, HarmonyPatch(typeof(UIMechaMatsGroup), "_OnInit")]
        public static void UIMechaMatsGroup_OnInit_PostPatch(UIMechaMatsGroup __instance)
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject titleText1 = __instance.transform.Find("fold-group-0/transfer-all-button/text").gameObject;
                titleText1.transform.localScale = new Vector3(0.8f, 1f, 1f);
                titleText1.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 0);
                GameObject titleText2 = __instance.transform.Find("fold-group-0/transfer-half-button/text").gameObject;
                titleText2.transform.localScale = new Vector3(0.8f, 1f, 1f);
                titleText2.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 0);
            }
        }

        //メカエディタ表示の調整４：propグループ
        [HarmonyPostfix, HarmonyPatch(typeof(UIMechaPropGroup), "_OnInit")]
        public static void UIMechaPropGroup_OnInit_PostPatch(UIMechaPropGroup __instance)
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject titleText = __instance.transform.Find("title-text").gameObject;
                titleText.transform.localScale = new Vector3(0.9f, 1f, 1f);
            }
        }

        //ダイソンスフィアエディタ表示の調整１
        [HarmonyPostfix, HarmonyPatch(typeof(UIDESwarmPanel), "_OnInit")]
        public static void UIDESwarmPanel_OnInit_PostPatch(UIDESwarmPanel __instance)
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

        //ダイソンスフィアエディタ表示の調整２
        [HarmonyPostfix, HarmonyPatch(typeof(UIDELayerPanel), "_OnInit")]
        public static void UIDELayerPanel_OnInit_PostPatch(UIDELayerPanel __instance)
        {
            if (Main.EnableFixUI.Value)
            {
                GameObject inEditorText3 = __instance.transform.Find("display-group/display-toggle-1/checkbox-editor/in-editor-text").gameObject;
                inEditorText3.transform.localScale = new Vector3(0.7f, 1f, 1f);
                GameObject inEditorText4 = __instance.transform.Find("display-group/display-toggle-2/checkbox-editor/in-editor-text").gameObject;
                inEditorText4.transform.localScale = new Vector3(0.7f, 1f, 1f);
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
                yield return AccessTools.Method(typeof(UITurretWindow), "_OnInit");
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
        [HarmonyPostfix, HarmonyPatch(typeof(UIMilestoneTip), "SetMilestoneTip")]
        public static void UIMilestoneTip_SetMilestoneTip_PostPatch(UIMilestoneTip __instance)
        {

            if (__instance.subTextComp.text.Contains(" "))
            {
                __instance.subTextComp.text = __instance.subTextComp.text.Replace(" ", "\u00a0");

            }
            if (__instance.subTextComp.text.Contains("_"))
            {
                __instance.subTextComp.text = __instance.subTextComp.text.Replace("_", " ");
            }

        }

        //SailIndicatorの日本語化
        [HarmonyPostfix, HarmonyPatch(typeof(UISailIndicator), "_OnInit")]
        public static void UISailIndicator_OnInit_PostPatch(UISailIndicator __instance)
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
        //ブループリント保存画面のUI修正１
        //[HarmonyPostfix, HarmonyPatch(typeof(UIBlueprintBrowser), "_OnCreate")]
        //public static void UIBlueprintBrowser_OnOpen_Harmony(UIBlueprintBrowser __instance)
        //{
        //    if (Main.EnableFixUI.Value)
        //    {



        //        }
        //    }

        //ブループリント保存画面のUI修正２
        [HarmonyPatch(typeof(UIBlueprintInspector), "_OnCreate")]
        public static class UIBlueprintInspector_OnOpen_Harmony
        {
            [HarmonyPostfix]
            public static void Postfix(UIBlueprintInspector __instance)
            {
                if (Main.EnableFixUI.Value)
                {
                    __instance.deleteButton.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30);
                    __instance.group1.gameObject.transform.Find("thumbnail-image/layout-combo/label").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
                    __instance.pasteButton.gameObject.transform.Find("text").transform.localScale = new Vector3(0.8f, 1, 1);
                    __instance.pasteButton.gameObject.transform.Find("text").GetComponent<RectTransform>().sizeDelta = new Vector2(10, 2);
                    __instance.savePathStateText.gameObject.transform.localScale = new Vector3(0.9f, 1, 1);
                }
            }
        }




        //    //ブループリント保存画面のUI修正３
        //    //[HarmonyPatch(typeof(UIBlueprintInspector), "_OnCreate")]
        //    //public static class UIBlueprintInspector_OnOpen_Harmony
        //    //{
        //    //    [HarmonyPostfix]
        //    //    public static void Postfix(UIBlueprintInspector __instance)
        //    //    {
        //    //        if (Main.EnableFixUI.Value)
        //    //        {
        //    //            __instance.group1.gameObject.transform.Find("thumbnail-image/layout-combo/label").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        //    //            //__instance.transform.Find("Blueprint Copy Inspector/group-1/thumbnail-image/layout-combo/label").GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        //    //            //__instance.transform.Find("Blueprint Copy Inspector/group-1/save-state-text").transform.localPosition = new Vector3(80, -30, 0);
        //    //        }
        //    //    }
        //    //}



        //トラフィックモニタの表示調整
        [HarmonyPostfix, HarmonyPatch(typeof(UIMonitorWindow), "_OnInit")]
        public static void UIMonitorWindow_OnInit_PostPatch(UIMonitorWindow __instance)
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

        //セーブ＆ロード確認MessageBoxのフォント変更
        //新しく作られるのでフォントの変更
        [HarmonyPostfix, HarmonyPatch(typeof(UIDialog), "CreateDialog")]
                public static void UIMessageBox_Show_Patch() //UIDialog __result)
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

        //    //UIRandomTipのフック：バルーンチップのサイズ調整
        //    [HarmonyPostfix, HarmonyPatch(typeof(UIRandomTip), "_OnOpen")]
        //    static void UIRandomTip_OnOpen_Postfix(RectTransform ___balloonTrans)
        //    {
        //        if (Main.EnableFixUI.Value)
        //        {
        //            ___balloonTrans.sizeDelta = new Vector2(290.0f, ___balloonTrans.sizeDelta.y - 18f);
        //            //___balloonTrans.gameObject.GetComponentInParent<Text>().text = HyphenationJpn.GetFormatedText(___balloonTrans.gameObject.GetComponentInParent<Text>(), ___balloonTrans.gameObject.GetComponentInParent<Text>().text);
        //        }
        //    }

        // UITechNodeのフック：テックツリーの技術名の位置調整 by aki9284
        [HarmonyPostfix, HarmonyPatch(typeof(UITechNode), "UpdateLayoutDynamic")]
        static void UITechNode_UpdateLayoutDynamic(Text ___titleText2, Text ___techDescText)
        {
            if (Main.EnableFixUI.Value)
            {
                ___titleText2.rectTransform.anchoredPosition = new Vector2(0, 10.0f);
            }
        }

        //新規開始画面の恒星タイプ名の文字位置調整
        [HarmonyPostfix, HarmonyPatch(typeof(UIGalaxySelect), "_OnOpen")]
                static void UpdateUIDisplayPatch(UIGalaxySelect __instance)
                {
                    if (Main.EnableFixUI.Value)
                    {
                        MoveStarCount("m-star");
                        MoveStarCount("k-star");
                        MoveStarCount("g-star");
                        MoveStarCount("f-star");
                        MoveStarCount("a-star");
                        MoveStarCount("b-star");
                        MoveStarCount("o-star");
                        MoveStarCount("n-star");
                        MoveStarCount("wd-star");
                        MoveStarCount("bh-star");
                    }
                }

                static void MoveStarCount(string starType)
                {
                    GameObject starSet = GameObject.Find("UI Root/Overlay Canvas/Galaxy Select/right-group/" + starType);
                    float y = starSet.transform.localPosition.y;
                    starSet.transform.localPosition = new Vector3(-15f, y, 0);
                    starSet.transform.Find("count").transform.localPosition = new Vector3(-220f, 0, 0);
                    starSet.transform.Find("Image").transform.localPosition = new Vector3(-212f, -16f, 0);
                }


        //    //UIAssemblerWindowのフック：コピー＆ペーストボタンのサイズ拡大
    //    [HarmonyPostfix, HarmonyPatch(typeof(UIAssemblerWindow), "_OnOpen")]
    //    //static void UIAssemblerWindow_OnOpen_Patch(UIButton ___resetButton, UIButton ___copyButton, UIButton ___pasteButton)
    //    //{
    //    //    if (Main.EnableFixUI.Value)
    //    //    {

    //    //        //LogManager.Logger.LogInfo("copyButton");
    //    //        Text copyText = ___copyButton.GetComponent<Text>();
    //    //        if (copyText != null)
    //    //        {
    //    //            float width = copyText.preferredWidth;
    //    //            float height = copyText.preferredHeight;

    //    //            RectTransform trs = (RectTransform)___copyButton.button.transform;

    //    //            trs.offsetMin = new Vector2(-35, trs.offsetMin.y);
    //    //            trs.offsetMax = new Vector2(35, trs.offsetMax.y);
    //    //        }
    //    //        // LogManager.Logger.LogInfo("pasteButton");
    //    //        Text pasteText = ___pasteButton.GetComponent<Text>();
    //    //        if (pasteText != null)
    //    //        {
    //    //            RectTransform trs = (RectTransform)___pasteButton.button.transform;
    //    //            trs.offsetMin = new Vector2(10, trs.offsetMin.y);
    //    //            trs.offsetMax = new Vector2(80, trs.offsetMax.y);
    //    //        }
    //    //    }
    //    //}

    }
}
