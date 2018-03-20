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
		protected CompRefuelable refuelableComp;

		protected CompRefuelableDualConsumption refuelableDualConsumptionComp;

		protected CompBreakdownable breakdownableComp;

		public bool recharging = false;

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
                // if connected to a network...
                if (this.transNet != null)
                {
                    // and batteries are connected
                    if (this.transNet.batteryComps.Count > 0)
                    {
                        if (this.recharging && this.transNet.CurrentStoredEnergy() >= refuelableDualConsumptionComp.Props.highPowerTreshhold)
                        {
                            // turn back off when we fill one battery's worth of charge
                            this.recharging = false;
                        }
                        else if (this.transNet.CurrentStoredEnergy() <= refuelableDualConsumptionComp.Props.lowPowerTreshhold)
                        {
                            // turn on when we drop below 200Wd stored
                            this.recharging = true;
                        }
                        else
                        {
                            // maintain existing value of recharging!
                            // we are somewhere between 200Wd and 1000Wd and charging or discharging
                        }
                    }
                    else
                    {
                        // without batteries connected, always produce power
                        this.recharging = true;
                    }
                }
                else
                {
                    // don't produce power if not connected to a network
                    this.recharging = false;
                }

                return this.recharging;
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			this.refuelableComp = this.parent.GetComp<CompRefuelable>();
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
			    (this.refuelableComp != null && !this.refuelableComp.HasFuel) ||
			    (this.flickableComp != null && !this.flickableComp.SwitchIsOn) ||
			    !this.IsCharging ||
			    !base.PowerOn)
			{
				base.PowerOutput = 0f;
				this.refuelableDualConsumptionComp.LowPowerMode = true;
				this.recharging = false;
			}
			else
			{
				base.PowerOutput = this.DesiredPowerOutput;
				this.refuelableDualConsumptionComp.LowPowerMode = false;
			}
		}
	}
}
