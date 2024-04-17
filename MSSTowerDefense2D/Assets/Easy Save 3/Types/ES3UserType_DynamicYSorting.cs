using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_sortableSprites", "_sortOffsetMarker")]
	public class ES3UserType_DynamicYSorting : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DynamicYSorting() : base(typeof(DynamicYSorting)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DynamicYSorting)obj;
			
			writer.WritePrivateField("_sortableSprites", instance);
			writer.WritePrivateFieldByRef("_sortOffsetMarker", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DynamicYSorting)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_sortableSprites":
					instance = (DynamicYSorting)reader.SetPrivateField("_sortableSprites", reader.Read<DynamicYSorting.SortableSprite[]>(), instance);
					break;
					case "_sortOffsetMarker":
					instance = (DynamicYSorting)reader.SetPrivateField("_sortOffsetMarker", reader.Read<UnityEngine.Transform>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DynamicYSortingArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DynamicYSortingArray() : base(typeof(DynamicYSorting[]), ES3UserType_DynamicYSorting.Instance)
		{
			Instance = this;
		}
	}
}