/*
 * Created by SharpDevelop.
 * User: DamnationLtd
 * Date: 4/4/2017
 * Time: 10:10 AM
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
	/// Description of BackupGenerator.
	/// </summary>
	public class BackupGenerator
	{

	}
		
	public class CompPowerRangeTrigger : CompPowerPlant
	{
		private bool generating;
		private CompRefuelableDualConsumption refuelableDualConsumptionComp;
		
		protected override float DesiredPowerOutput
		{
			get
			{
				if (this.generating) {
					return -base.Props.basePowerConsumption;
				} else {
					return 0;
				}
			}
		}
		
		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			this.generating = false;
			this.refuelableDualConsumptionComp = this.parent.GetComp<CompRefuelableDualConsumption>();	
		}

		public override void CompTick()
		{
			base.CompTick();

			if (this.transNet != null && this.transNet.batteryComps.Count > 0)
			{
				if (this.transNet.CurrentStoredEnergy() <= 200)
				{
					this.generating = true;
				}
				else if (this.transNet.CurrentStoredEnergy() >= 1000)
				{
					this.generating = false;
				}

			}

			base.PowerOutput = this.DesiredPowerOutput;
			this.refuelableDualConsumptionComp.LowPowerMode = this.generating;
		}
		
		
	}

	public class CompProperties_RefuelableDualConsumption : CompProperties_Refuelable
	{
		public float lowFuelConsumptionRate = 1f;
		
		public float highFuelConsumptionRate = 1f;
	}
	
	
	[StaticConstructorOnStartup]
	public class CompRefuelableDualConsumption : CompRefuelable
	{
		private float fuelConsumptionHighRate;
		private float fuelConsumptionLowRate;
		public bool LowPowerMode;

		private float ConsumptionRatePerTick
		{
			get
			{
				if (this.LowPowerMode) {
					return this.fuelConsumptionLowRate / 60000f;
				}
				else
				{
					return this.fuelConsumptionHighRate / 60000f;
				}
			}
		}
		
		public override void CompTick()
		{
			base.parent.CompTick();
			if (!this.Props.consumeFuelOnlyWhenUsed && (this.flickComp == null || this.flickComp.SwitchIsOn))
			{
				this.ConsumeFuel(this.ConsumptionRatePerTick);
			}
		}
		
		public CompProperties_RefuelableDualConsumption Props
		{
			get
			{
				return (CompProperties_RefuelableDualConsumption)this.props;
			}
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			this.LowPowerMode = false;
			
			this.fuelConsumptionHighRate = this.Props.highFuelConsumptionRate;
			this.fuelConsumptionLowRate = this.Props.lowFuelConsumptionRate;
		}
		
	}
}