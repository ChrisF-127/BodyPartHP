using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace BodyPartHP
{
	public class BodyPartHP : Mod
	{
		#region PROPERTIES
		public static BodyPartHP Instance { get; private set; }
		public static BodyPartHPSettings Settings { get; private set; }
		#endregion

		#region CONSTRUCTORS
		public BodyPartHP(ModContentPack content) : base(content)
		{
			Instance = this;

			LongEventHandler.ExecuteWhenFinished(Initialize);
		}
		#endregion

		#region OVERRIDES
		public override string SettingsCategory() =>
			"Body Part HP";

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);

			Settings.DoSettingsWindowContents(inRect);
		}
		#endregion

		#region PRIVATE METHODS
		private void Initialize()
		{
			Settings = GetSettings<BodyPartHPSettings>();
		}
		#endregion
	}
}
