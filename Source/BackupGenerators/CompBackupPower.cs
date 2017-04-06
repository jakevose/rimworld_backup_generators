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
	public class CompBackupPower : CompPowerTrader
	{
		protected CompRefuelableDualConsumption refuelableDualConsumptionComp;

		protected CompBreakdownable breakdownableComp;
		
		private bool recharging = false;

		protected virtual float DesiredPowerOutput
		{
			get
			{
				return -base.Props.basePowerConsumption;
			}
		}
		
		protected virtual bool IsCharging
		{
			get
			{
				if (this.transNet != null && this.transNet.batteryComps.Count > 0)
				{
					if (this.recharging)
					{
						if (this.transNet.CurrentStoredEnergy() >= 1000)
						{
							this.recharging = false;
						}
					}
					else
					{
						if (this.transNet.CurrentStoredEnergy() <= 200)
						{
							this.recharging = true;
						}
					}
				}
				else
				{
					this.recharging = false;
				}
				
				return this.recharging;
			}
		}

		public override void PostSpawnSetup()
		{
			base.PostSpawnSetup();
			this.refuelableDualConsumptionComp = this.parent.GetComp<CompRefuelableDualConsumption>();
			this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
			if (base.Props.basePowerConsumption < 0f && !this.parent.IsBrokenDown())
			{
				base.PowerOn = true;
			}
		}

		public override void CompTick()
		{
			base.CompTick();
			if ((this.breakdownableComp != null && this.breakdownableComp.BrokenDown) ||
			    (this.refuelableDualConsumptionComp != null && !this.refuelableDualConsumptionComp.HasFuel) ||
			    (this.flickableComp != null && !this.flickableComp.SwitchIsOn) ||
			    !this.IsCharging ||
			    !base.PowerOn)
			{
				base.PowerOutput = 0f;
				this.refuelableDualConsumptionComp.LowPowerMode = true;
			}
			else
			{
				base.PowerOutput = this.DesiredPowerOutput;
				this.refuelableDualConsumptionComp.LowPowerMode = false;
			}
		}
	}
}