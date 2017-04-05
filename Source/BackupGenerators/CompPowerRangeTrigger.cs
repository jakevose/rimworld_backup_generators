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
		private CompRefuelableDualConsumption refuelableDualConsumptionComp;
		
		protected override float DesiredPowerOutput
		{
			get
			{
				return this.refuelableDualConsumptionComp.LowPowerMode ? 0 : -base.Props.basePowerConsumption;
			}
		}
		
		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			this.refuelableDualConsumptionComp = this.parent.GetComp<CompRefuelableDualConsumption>();
		}

		public override void CompTick()
		{
			base.CompTick();

			if (this.transNet != null && this.transNet.batteryComps.Count > 0)
			{
				if (this.refuelableDualConsumptionComp.LowPowerMode &&
				    this.transNet.CurrentStoredEnergy() <= 200)
				{
					this.refuelableDualConsumptionComp.LowPowerMode = false;
				}
				else if (!this.refuelableDualConsumptionComp.LowPowerMode &&
				         this.transNet.CurrentStoredEnergy() >= 1000)
				{
					this.refuelableDualConsumptionComp.LowPowerMode = true;
				}

			}
			else
			{
				this.refuelableDualConsumptionComp.LowPowerMode = true;
			}
		}
	}
}