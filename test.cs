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
	class Test
	{


		//////////////////////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////テスト//////////////////////////////////////////////
		//////////////////////////////////////////////////////////////////////////////////////////////

		//[HarmonyPostfix, HarmonyPatch(typeof(PlanetTransport), "GameTick")]
		public static void PlanetTransport_GameTick_PrePatch(PlanetTransport __instance)
		{
			for (int k = 1; k < __instance.dispenserCursor; k++)
			{
				if (__instance.dispenserPool[k] != null && __instance.dispenserPool[k].id == k)
				{
					StorageComponent storageComponent = __instance.dispenserPool[k].storage;





				}
			}
		}
	}
}