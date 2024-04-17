using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("maxAcceleration", "rotationSpeed", "slowdownDistance", "pickNextWaypointDist", "endReachedDistance", "alwaysDrawGizmos", "slowWhenNotFacingTarget", "whenCloseToDestination", "constrainInsideGraph", "radius", "height", "canMove", "maxSpeed", "gravity", "groundMask", "centerOffsetCompatibility", "repathRateCompatibility", "canSearchCompability", "orientation", "enableRotation", "movementPlane", "autoRepath", "targetCompatibility", "version")]
	public class ES3UserType_AIPath : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AIPath() : base(typeof(Pathfinding.AIPath)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.AIPath)obj;
			
			writer.WriteProperty("maxAcceleration", instance.maxAcceleration, ES3Type_float.Instance);
			writer.WriteProperty("rotationSpeed", instance.rotationSpeed, ES3Type_float.Instance);
			writer.WriteProperty("slowdownDistance", instance.slowdownDistance, ES3Type_float.Instance);
			writer.WriteProperty("pickNextWaypointDist", instance.pickNextWaypointDist, ES3Type_float.Instance);
			writer.WriteProperty("endReachedDistance", instance.endReachedDistance, ES3Type_float.Instance);
			writer.WriteProperty("alwaysDrawGizmos", instance.alwaysDrawGizmos, ES3Type_bool.Instance);
			writer.WriteProperty("slowWhenNotFacingTarget", instance.slowWhenNotFacingTarget, ES3Type_bool.Instance);
			writer.WriteProperty("whenCloseToDestination", instance.whenCloseToDestination, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Pathfinding.CloseToDestinationMode)));
			writer.WriteProperty("constrainInsideGraph", instance.constrainInsideGraph, ES3Type_bool.Instance);
			writer.WriteProperty("radius", instance.radius, ES3Type_float.Instance);
			writer.WriteProperty("height", instance.height, ES3Type_float.Instance);
			writer.WriteProperty("canMove", instance.canMove, ES3Type_bool.Instance);
			writer.WriteProperty("maxSpeed", instance.maxSpeed, ES3Type_float.Instance);
			writer.WriteProperty("gravity", instance.gravity, ES3Type_Vector3.Instance);
			writer.WriteProperty("groundMask", instance.groundMask, ES3Type_LayerMask.Instance);
			writer.WritePrivateField("centerOffsetCompatibility", instance);
			writer.WritePrivateField("repathRateCompatibility", instance);
			writer.WritePrivateField("canSearchCompability", instance);
			writer.WriteProperty("orientation", instance.orientation, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Pathfinding.OrientationMode)));
			writer.WriteProperty("enableRotation", instance.enableRotation, ES3Type_bool.Instance);
			writer.WriteProperty("movementPlane", instance.movementPlane, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Pathfinding.Util.IMovementPlane)));
			writer.WriteProperty("autoRepath", instance.autoRepath, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Pathfinding.AutoRepathPolicy)));
			writer.WritePrivateFieldByRef("targetCompatibility", instance);
			writer.WritePrivateField("version", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.AIPath)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "maxAcceleration":
						instance.maxAcceleration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "rotationSpeed":
						instance.rotationSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "slowdownDistance":
						instance.slowdownDistance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "pickNextWaypointDist":
						instance.pickNextWaypointDist = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "endReachedDistance":
						instance.endReachedDistance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "alwaysDrawGizmos":
						instance.alwaysDrawGizmos = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "slowWhenNotFacingTarget":
						instance.slowWhenNotFacingTarget = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "whenCloseToDestination":
						instance.whenCloseToDestination = reader.Read<Pathfinding.CloseToDestinationMode>();
						break;
					case "constrainInsideGraph":
						instance.constrainInsideGraph = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "radius":
						instance.radius = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "height":
						instance.height = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "canMove":
						instance.canMove = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "maxSpeed":
						instance.maxSpeed = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "gravity":
						instance.gravity = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "groundMask":
						instance.groundMask = reader.Read<UnityEngine.LayerMask>(ES3Type_LayerMask.Instance);
						break;
					case "centerOffsetCompatibility":
					instance = (Pathfinding.AIPath)reader.SetPrivateField("centerOffsetCompatibility", reader.Read<System.Single>(), instance);
					break;
					case "repathRateCompatibility":
					instance = (Pathfinding.AIPath)reader.SetPrivateField("repathRateCompatibility", reader.Read<System.Single>(), instance);
					break;
					case "canSearchCompability":
					instance = (Pathfinding.AIPath)reader.SetPrivateField("canSearchCompability", reader.Read<System.Boolean>(), instance);
					break;
					case "orientation":
						instance.orientation = reader.Read<Pathfinding.OrientationMode>();
						break;
					case "enableRotation":
						instance.enableRotation = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "movementPlane":
						instance.movementPlane = reader.Read<Pathfinding.Util.IMovementPlane>();
						break;
					case "autoRepath":
						instance.autoRepath = reader.Read<Pathfinding.AutoRepathPolicy>();
						break;
					case "targetCompatibility":
					instance = (Pathfinding.AIPath)reader.SetPrivateField("targetCompatibility", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "version":
					instance = (Pathfinding.AIPath)reader.SetPrivateField("version", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_AIPathArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AIPathArray() : base(typeof(Pathfinding.AIPath[]), ES3UserType_AIPath.Instance)
		{
			Instance = this;
		}
	}
}