using SyControlsBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BodyPartHP
{
	public class BodyPartHPSettings: ModSettings
	{
		#region PROPERTIES
		public List<ValueSetting<int>> BodyParts { get; } = new List<ValueSetting<int>>();
		#endregion

		#region CONSTRUCTORS
		public BodyPartHPSettings()
		{
			// body part hitpoint settings
			var partOf = new List<string>();
			foreach (var bodyPartDef in DefDatabase<BodyPartDef>.AllDefsListForReading)
			{
				// local variables
				var defName = bodyPartDef.defName;
				var label = bodyPartDef.label.CapitalizeFirst();
				var baseHP = bodyPartDef.hitPoints;

				// create setting
				var setting = new ValueSetting<int>($"{defName}_hitPoints", $"{label} ({defName})", "", baseHP, baseHP, v => bodyPartDef.hitPoints = v);
				// add to list
				BodyParts.Add(setting);

				// find all creatures using this part
				foreach (var def in DefDatabase<ThingDef>.AllDefsListForReading)
				{
					if (def?.race?.body?.AllParts?.Any(p => p?.def?.defName == defName) == true)
						partOf.Add(def.label.CapitalizeFirst());
				}
				partOf.Sort();
				setting.Description = $"{label} ({defName})\n- Base is: {baseHP}\n- Part of:\n{string.Join(", ", partOf)}";
				partOf.Clear();
			}

			// sort list
			BodyParts.SortBy(s => s.Label);
		}
		#endregion

		#region PUBLIC METHODS
		public void DoSettingsWindowContents(Rect inRect)
		{
			var width = inRect.width;
			var offsetY = 0.0f;

			ControlsBuilder.Begin(inRect);
			try
			{
				ControlsBuilder.CreateText(
					ref offsetY, 
					width, 
					"WARNING! Reduction of hitpoints may cause loss of bodyparts and death!", 
					Color.red,
					TextAnchor.MiddleCenter,
					GameFont.Medium);

				foreach (var bodyPart in BodyParts)
				{
					bodyPart.Value = ControlsBuilder.CreateNumeric(
						ref offsetY,
						width,
						bodyPart.Label,
						bodyPart.Description,
						bodyPart.Value,
						bodyPart.DefaultValue,
						bodyPart.Name,
						1,
						int.MaxValue);
				}
			}
			finally
			{
				ControlsBuilder.End(offsetY);
			}
		}
		#endregion

		#region OVERRIDES
		public override void ExposeData()
		{
			base.ExposeData();

			foreach (var bodyPart in BodyParts)
			{
				var intValue = bodyPart.Value;
				Scribe_Values.Look(ref intValue, bodyPart.Name, bodyPart.DefaultValue);
				bodyPart.Value = intValue;
			}
		}
		#endregion
	}
}
