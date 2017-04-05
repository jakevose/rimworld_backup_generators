/*
 * Created by SharpDevelop.
 * User: Mephistopheles
 * Date: 4/4/2017
 * Time: 9:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace DLTD
{
	/// <summary>
	/// Description of CompProperties_RefuelableDualConsumption.
	/// </summary>
	public class CompProperties_RefuelableDualConsumption : CompProperties
	{
		public float fuelHighConsumptionRate = 1f;
		public float fuelLowConsumptionRate = 1f;

		public float fuelCapacity = 2f;

		public float autoRefuelPercent = 0.3f;

		public ThingFilter fuelFilter;

		public bool showFuelGizmo;

		public bool targetFuelLevelConfigurable;

		public float initialConfigurableTargetFuelLevel;

		public bool drawOutOfFuelOverlay = true;

		public bool drawFuelGaugeInMap;
		
		public CompProperties_RefuelableDualConsumption()
		{
			this.compClass = typeof(CompRefuelableDualConsumption);
		}

		public override void ResolveReferences(ThingDef parentDef)
		{
			base.ResolveReferences(parentDef);
			this.fuelFilter.ResolveReferences();
		}
	}
}
