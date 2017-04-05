/*
 * Created by SharpDevelop.
 * User: Mephistopheles
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
		public const string RefueledSignal = "Refueled";

		public const string RanOutOfFuelSignal = "RanOutOfFuel";

		private float fuel;

		private float configuredTargetFuelLevel = -1f;
		
		private bool lowPowerMode = true;

		private CompFlickable flickComp;

		private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel", true);

		private static readonly Vector2 FuelBarSize = new Vector2(1f, 0.2f);

		private static readonly Material FuelBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 0.56f, 0.13f));

		private static readonly Material FuelBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

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
		
		public float TargetFuelLevel
		{
			get
			{
				if (this.configuredTargetFuelLevel >= 0f)
				{
					return this.configuredTargetFuelLevel;
				}
				if (this.Props.targetFuelLevelConfigurable)
				{
					return this.Props.initialConfigurableTargetFuelLevel;
				}
				return this.Props.fuelCapacity;
			}
			set
			{
				this.configuredTargetFuelLevel = Mathf.Clamp(value, 0f, this.Props.fuelCapacity);
			}
		}

		public CompProperties_RefuelableDualConsumption Props
		{
			get
			{
				return (CompProperties_RefuelableDualConsumption)this.props;
			}
		}

		public float Fuel
		{
			get
			{
				return this.fuel;
			}
		}

		public float FuelPercentOfTarget
		{
			get
			{
				return this.fuel / this.TargetFuelLevel;
			}
		}

		public float FuelPercentOfMax
		{
			get
			{
				return this.fuel / this.Props.fuelCapacity;
			}
		}

		public bool IsFull
		{
			get
			{
				return this.TargetFuelLevel - this.fuel < 1f;
			}
		}

		public bool HasFuel
		{
			get
			{
				return this.fuel > 0f;
			}
		}

		private float ConsumptionRatePerTick
		{
			get
			{
				return this.CurrentConsumptionRate / 60000f;
			}
		}
		
		private float CurrentConsumptionRate
		{
			get
			{
				return this.lowPowerMode ? this.Props.fuelLowConsumptionRate : this.Props.fuelHighConsumptionRate;
			}
		}

		public bool ShouldAutoRefuelNow
		{
			get
			{
				return this.FuelPercentOfTarget <= this.Props.autoRefuelPercent && !this.IsFull && this.TargetFuelLevel > 0f && !this.parent.IsBurning() && (this.flickComp == null || this.flickComp.SwitchIsOn) && this.parent.Map.designationManager.DesignationOn(this.parent, DesignationDefOf.Flick) == null && this.parent.Map.designationManager.DesignationOn(this.parent, DesignationDefOf.Deconstruct) == null;
			}
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			this.flickComp = this.parent.GetComp<CompFlickable>();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<float>(ref this.fuel, "fuel", 0f, false);
			Scribe_Values.LookValue<float>(ref this.configuredTargetFuelLevel, "configuredTargetFuelLevel", -1f, false);
		}

		public override void PostDraw()
		{
			base.PostDraw();
			if (!this.HasFuel && this.Props.drawOutOfFuelOverlay)
			{
				this.parent.Map.overlayDrawer.DrawOverlay(this.parent, OverlayTypes.OutOfFuel);
			}
			if (this.Props.drawFuelGaugeInMap)
			{
				GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
				r.center = this.parent.DrawPos + Vector3.up * 0.1f;
				r.size = CompRefuelableDualConsumption.FuelBarSize;
				r.fillPercent = this.FuelPercentOfMax;
				r.filledMat = CompRefuelableDualConsumption.FuelBarFilledMat;
				r.unfilledMat = CompRefuelableDualConsumption.FuelBarUnfilledMat;
				r.margin = 0.15f;
				Rot4 rotation = this.parent.Rotation;
				rotation.Rotate(RotationDirection.Clockwise);
				r.rotation = rotation;
				GenDraw.DrawFillableBar(r);
			}
		}

		public override void PostDestroy(DestroyMode mode, Map previousMap)
		{
			base.PostDestroy(mode, previousMap);
			if (previousMap != null && this.Props.fuelFilter.AllowedDefCount == 1)
			{
				ThingDef thingDef = this.Props.fuelFilter.AllowedThingDefs.First<ThingDef>();
				float num = 1f;
				int i = GenMath.RoundRandom(num * this.fuel);
				while (i > 0)
				{
					Thing thing = ThingMaker.MakeThing(thingDef, null);
					thing.stackCount = Mathf.Min(i, thingDef.stackLimit);
					i -= thing.stackCount;
					GenPlace.TryPlaceThing(thing, this.parent.Position, previousMap, ThingPlaceMode.Near, null);
				}
			}
		}

		public override string CompInspectStringExtra()
		{
			string text = string.Concat(new string[]
			{
				"Fuel".Translate(),
				": ",
				this.fuel.ToStringDecimalIfSmall(),
				" / ",
				this.Props.fuelCapacity.ToStringDecimalIfSmall()
			});
			if (this.HasFuel)
			{
				int numTicks = (int)(this.fuel / this.CurrentConsumptionRate * 60000f);
				text = text + " (" + numTicks.ToStringTicksToPeriod(true) + ")";
			}
			if (this.Props.targetFuelLevelConfigurable)
			{
				text = text + "\n" + "ConfiguredTargetFuelLevel".Translate(new object[]
				{
					this.TargetFuelLevel.ToStringDecimalIfSmall()
				});
			}
			return text;
		}

		public override void CompTick()
		{
			base.CompTick();
			if (this.flickComp == null || this.flickComp.SwitchIsOn)
			{
				this.ConsumeFuel(this.ConsumptionRatePerTick);
			}
		}

		public void ConsumeFuel(float amount)
		{
			if (this.fuel <= 0f)
			{
				return;
			}
			this.fuel -= amount;
			if (this.fuel <= 0f)
			{
				this.fuel = 0f;
				this.parent.BroadcastCompSignal("RanOutOfFuel");
			}
		}

		public void Refuel(Thing fuelThing)
		{
			this.fuel += (float)fuelThing.stackCount;
			if (this.fuel > this.Props.fuelCapacity)
			{
				this.fuel = this.Props.fuelCapacity;
			}
			fuelThing.Destroy(DestroyMode.Vanish);
			this.parent.BroadcastCompSignal("Refueled");
		}

		public void Notify_UsedThisTick()
		{
			this.ConsumeFuel(this.ConsumptionRatePerTick);
		}

		public int GetFuelCountToFullyRefuel()
		{
			float f = this.TargetFuelLevel - this.fuel;
			return Mathf.Max(Mathf.CeilToInt(f), 1);
		}
	}
}
