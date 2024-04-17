using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_CircleCollider2D : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CircleCollider2D() : base(typeof(UnityEngine.CircleCollider2D)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.CircleCollider2D)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.CircleCollider2D)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CircleCollider2DArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CircleCollider2DArray() : base(typeof(UnityEngine.CircleCollider2D[]), ES3UserType_CircleCollider2D.Instance)
		{
			Instance = this;
		}
	}
}