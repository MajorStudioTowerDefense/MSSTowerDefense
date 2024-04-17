using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("target", "targetPosition")]
	public class ES3UserType_AIDestinationSetter : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AIDestinationSetter() : base(typeof(Pathfinding.AIDestinationSetter)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.AIDestinationSetter)obj;
			
			writer.WritePropertyByRef("target", instance.target);
			writer.WriteProperty("targetPosition", instance.targetPosition, ES3Type_Vector3.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.AIDestinationSetter)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "target":
						instance.target = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "targetPosition":
						instance.targetPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_AIDestinationSetterArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AIDestinationSetterArray() : base(typeof(Pathfinding.AIDestinationSetter[]), ES3UserType_AIDestinationSetter.Instance)
		{
			Instance = this;
		}
	}
}