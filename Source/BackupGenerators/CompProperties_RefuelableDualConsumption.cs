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

		public CompProperties_RefuelableDualConsumption()
		{
			this.compClass = typeof(CompRefuelableDualConsumption);
		}
	}
}
