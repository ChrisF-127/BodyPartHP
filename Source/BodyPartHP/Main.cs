using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using HugsLib.Settings;
using HugsLib.Utils;
using UnityEngine;

namespace BodyPartHP
{
	public class Main : HugsLib.ModBase
	{
		public Main()
		{
			//var harmony = new Harmony("syrus.BodyPartHP");
			//harmony.PatchAll();
		}

		private List<SettingHandle> _settingHandles = new List<SettingHandle>();

		public override void DefsLoaded()
		{
			// hitpoint reduction warning label
			var warningHandle = Settings.GetHandle("WarningLabel", "HitPoint-Reduction-Warning", "WARNING! Reduction of hitpoints can cause loss of bodyparts and death!", false);
			warningHandle.Unsaved = true;
			warningHandle.CustomDrawerFullWidth = rect => CustomDrawerFullWidth_Label(rect, warningHandle.Description, Color.red, TextAnchor.MiddleCenter, FontStyle.Bold);

			// body part hitpoint settings
			List<string> partOf = new List<string>();
			foreach (var bodyPartDef in DefDatabase<BodyPartDef>.AllDefsListForReading)
			{
				// local variables
				var defName = bodyPartDef.defName;
				var label = bodyPartDef.label.CapitalizeFirst();
				var baseHP = bodyPartDef.hitPoints;

				// create settings handle
				var handle = Settings.GetHandle(
					$"{defName}_hitPoints",
					$"{label} ({defName})",
					"",
					baseHP,
					Validators.IntRangeValidator(1, int.MaxValue));
				handle.SpinnerIncrement = 1;
				handle.ValueChanged += val => bodyPartDef.hitPoints = (SettingHandle<int>)val;

				// apply loaded settings
				bodyPartDef.hitPoints = handle;

				// add to list
				_settingHandles.Add(handle);

				foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
				{
					if (def?.race?.body?.AllParts?.FirstOrDefault((p) => p?.def?.defName == defName) != null)
						partOf.Add(def.label.CapitalizeFirst());
				}
				partOf.Sort();
				handle.Description = $"{label} ({defName})\n- Base is: {baseHP}\n- Part of:\n{string.Join(", ", partOf)}";
				partOf.Clear();
			}

			// sort list
			_settingHandles.SortBy((s) => s.Title);

			// sort body part list by title
			for (int i = 0; i < _settingHandles.Count; i++)
				_settingHandles[i].DisplayOrder = i;
		}

		public bool CustomDrawerFullWidth_Label(Rect rect, string label, Color color, TextAnchor textAnchor, FontStyle fontStyle)
		{
			// remember previous style
			var prevColor = GUI.color;
			var prevTextAnchor = Text.Anchor;
			var prevFontStyle = Text.CurFontStyle.fontStyle;
			
			// set desired style
			GUI.color = color;
			Text.Anchor = textAnchor;
			Text.CurFontStyle.fontStyle = fontStyle;

			// draw label
			Widgets.Label(rect, label);
			
			// reset to previous style
			Text.CurFontStyle.fontStyle = prevFontStyle;
			Text.Anchor = prevTextAnchor;
			GUI.color = prevColor;

			return false;
		}
	}
}
