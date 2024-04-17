using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("bar", "anim")]
	public class ES3UserType_EmployeeProgressBar : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EmployeeProgressBar() : base(typeof(EmployeeProgressBar)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (EmployeeProgressBar)obj;
			
			writer.WritePrivateFieldByRef("bar", instance);
			writer.WritePrivateFieldByRef("anim", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (EmployeeProgressBar)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "bar":
					instance = (EmployeeProgressBar)reader.SetPrivateField("bar", reader.Read<UnityEngine.UI.Image>(), instance);
					break;
					case "anim":
					instance = (EmployeeProgressBar)reader.SetPrivateField("anim", reader.Read<UnityEngine.Animator>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_EmployeeProgressBarArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EmployeeProgressBarArray() : base(typeof(EmployeeProgressBar[]), ES3UserType_EmployeeProgressBar.Instance)
		{
			Instance = this;
		}
	}
}