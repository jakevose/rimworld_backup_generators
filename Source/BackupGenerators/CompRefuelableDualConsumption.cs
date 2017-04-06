/*
 * Created by SharpDevelop.
 * User: DamnationLtd
 * Date: 4/4/2017
 * Time: 9:15 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace DLTD
{
	/// <summary>
	/// Description of CompRefuelableDualConsumption.
	/// </summary>
	[StaticConstructorOnStartup]
	public class CompRefuelableDualConsumption : ThingComp
	{
		protected CompRefuelable refuelableComp;
		
		private bool lowPowerMode = false;

		public bool LowPowerMode
		{
			get
			{
				return this.lowPowerMode;
			}
			set
			{
				this.lowPowerMode = value;
			}
		}
		
		public CompProperties_RefuelableDualConsumption Props
		{
			get
			{
				return (CompProperties_RefuelableDualConsumption)this.props;
			}
		}

		private float CurrentConsumptionRate
		{
			get
			{
				return this.lowPowerMode ? this.Props.fuelLowConsumptionRate : this.Props.fuelHighConsumptionRate;
			}
		}

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			this.refuelableComp = this.parent.GetComp<CompRefuelable>();
		}
		
		public override void CompTick()
		{
			this.refuelableComp.Props.fuelConsumptionRate = this.CurrentConsumptionRate;
			base.CompTick();
		}
	}
}
