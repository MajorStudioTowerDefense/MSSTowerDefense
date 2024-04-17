using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("drawGizmos", "detailedGizmos")]
	public class ES3UserType_Seeker : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Seeker() : base(typeof(Pathfinding.Seeker)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Pathfinding.Seeker)obj;
			
			writer.WriteProperty("drawGizmos", instance.drawGizmos, ES3Type_bool.Instance);
			writer.WriteProperty("detailedGizmos", instance.detailedGizmos, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Pathfinding.Seeker)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "drawGizmos":
						instance.drawGizmos = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "detailedGizmos":
						instance.detailedGizmos = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SeekerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SeekerArray() : base(typeof(Pathfinding.Seeker[]), ES3UserType_Seeker.Instance)
		{
			Instance = this;
		}
	}
}