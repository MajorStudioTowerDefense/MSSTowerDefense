using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("updateError", "checkTime", "uniqueID", "version")]
	public class ES3UserType_DynamicGridObstacle : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DynamicGridObstacle() : base(typeof(Pathfinding.DynamicGridObstacle)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.DynamicGridObstacle)obj;
			
			writer.WriteProperty("updateError", instance.updateError, ES3Type_float.Instance);
			writer.WriteProperty("checkTime", instance.checkTime, ES3Type_float.Instance);
			writer.WritePrivateField("uniqueID", instance);
			writer.WritePrivateField("version", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.DynamicGridObstacle)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "updateError":
						instance.updateError = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "checkTime":
						instance.checkTime = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "uniqueID":
					instance = (Pathfinding.DynamicGridObstacle)reader.SetPrivateField("uniqueID", reader.Read<System.UInt64>(), instance);
					break;
					case "version":
					instance = (Pathfinding.DynamicGridObstacle)reader.SetPrivateField("version", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DynamicGridObstacleArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DynamicGridObstacleArray() : base(typeof(Pathfinding.DynamicGridObstacle[]), ES3UserType_DynamicGridObstacle.Instance)
		{
			Instance = this;
		}
	}
}