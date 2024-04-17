using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("shelfTypeNameString", "maxCustomers", "visibility", "purchaseRadius", "customerStayDuration", "loadAmount", "initalLoadAmount", "loadAmountMax", "loadAllowed", "targetObjectName", "costToBuy", "canvas", "itemsCanBeSold", "sellingItem", "thisType", "threeStates", "costToMaintain", "purchasePower", "visibilityIndicator")]
	public class ES3UserType_ShelfScript : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ShelfScript() : base(typeof(ShelfScript)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (ShelfScript)obj;
			
			writer.WriteProperty("shelfTypeNameString", instance.shelfTypeNameString, ES3Type_string.Instance);
			writer.WriteProperty("maxCustomers", instance.maxCustomers, ES3Type_int.Instance);
			writer.WriteProperty("visibility", instance.visibility, ES3Type_float.Instance);
			writer.WriteProperty("purchaseRadius", instance.purchaseRadius, ES3Type_float.Instance);
			writer.WriteProperty("customerStayDuration", instance.customerStayDuration, ES3Type_float.Instance);
			writer.WriteProperty("loadAmount", instance.loadAmount, ES3Type_int.Instance);
			writer.WriteProperty("initalLoadAmount", instance.initalLoadAmount, ES3Type_int.Instance);
			writer.WriteProperty("loadAmountMax", instance.loadAmountMax, ES3Type_int.Instance);
			writer.WriteProperty("loadAllowed", instance.loadAllowed, ES3Type_bool.Instance);
			writer.WriteProperty("targetObjectName", instance.targetObjectName, ES3Type_string.Instance);
			writer.WritePrivateField("costToBuy", instance);
			writer.WritePropertyByRef("canvas", instance.canvas);
			writer.WriteProperty("itemsCanBeSold", instance.itemsCanBeSold, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<Items>)));
			writer.WritePropertyByRef("sellingItem", instance.sellingItem);
			writer.WriteProperty("thisType", instance.thisType, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(shelvesType)));
			writer.WriteProperty("threeStates", instance.threeStates, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<UnityEngine.Sprite[]>)));
			writer.WriteProperty("costToMaintain", instance.costToMaintain, ES3Type_int.Instance);
			writer.WriteProperty("purchasePower", instance.purchasePower, ES3Type_float.Instance);
			writer.WritePropertyByRef("visibilityIndicator", instance.visibilityIndicator);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (ShelfScript)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "shelfTypeNameString":
						instance.shelfTypeNameString = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "maxCustomers":
						instance.maxCustomers = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "visibility":
						instance.visibility = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "purchaseRadius":
						instance.purchaseRadius = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "customerStayDuration":
						instance.customerStayDuration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "loadAmount":
						instance.loadAmount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "initalLoadAmount":
						instance.initalLoadAmount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "loadAmountMax":
						instance.loadAmountMax = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "loadAllowed":
						instance.loadAllowed = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "targetObjectName":
						instance.targetObjectName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "costToBuy":
					instance = (ShelfScript)reader.SetPrivateField("costToBuy", reader.Read<System.Single>(), instance);
					break;
					case "canvas":
						instance.canvas = reader.Read<UnityEngine.Canvas>();
						break;
					case "itemsCanBeSold":
						instance.itemsCanBeSold = reader.Read<System.Collections.Generic.List<Items>>();
						break;
					case "sellingItem":
						instance.sellingItem = reader.Read<Items>();
						break;
					case "thisType":
						instance.thisType = reader.Read<shelvesType>();
						break;
					case "threeStates":
						instance.threeStates = reader.Read<System.Collections.Generic.List<UnityEngine.Sprite[]>>();
						break;
					case "costToMaintain":
						instance.costToMaintain = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "purchasePower":
						instance.purchasePower = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "visibilityIndicator":
						instance.visibilityIndicator = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_ShelfScriptArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ShelfScriptArray() : base(typeof(ShelfScript[]), ES3UserType_ShelfScript.Instance)
		{
			Instance = this;
		}
	}
}