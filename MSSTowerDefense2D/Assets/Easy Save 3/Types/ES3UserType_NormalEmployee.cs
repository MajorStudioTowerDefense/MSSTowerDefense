using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Walking", "isCarrying", "carryMax", "carryCount", "stayShelfDuration", "myEmployeeAreaPos", "shelfPlacementManager", "eStage", "eAction", "employeeArea", "carriedItemSprite", "carriedShelfSprite", "employeeCanvas", "employeeLoadingImage", "botName", "botBudget", "shoppingEffect", "shoppingSFX", "aiPath", "destinationSetter", "tags")]
	public class ES3UserType_NormalEmployee : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NormalEmployee() : base(typeof(NormalEmployee)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (NormalEmployee)obj;
			
			writer.WritePropertyByRef("Walking", instance.Walking);
			writer.WriteProperty("isCarrying", instance.isCarrying, ES3Type_bool.Instance);
			writer.WriteProperty("carryMax", instance.carryMax, ES3Type_int.Instance);
			writer.WriteProperty("carryCount", instance.carryCount, ES3Type_int.Instance);
			writer.WriteProperty("stayShelfDuration", instance.stayShelfDuration, ES3Type_float.Instance);
			writer.WriteProperty("myEmployeeAreaPos", instance.myEmployeeAreaPos, ES3Type_Vector3.Instance);
			writer.WritePropertyByRef("shelfPlacementManager", instance.shelfPlacementManager);
			writer.WriteProperty("eStage", instance.eStage, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(employeeStage)));
			writer.WriteProperty("eAction", instance.eAction, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(employeeAction)));
			writer.WritePropertyByRef("employeeArea", instance.employeeArea);
			writer.WritePropertyByRef("carriedItemSprite", instance.carriedItemSprite);
			writer.WritePropertyByRef("carriedShelfSprite", instance.carriedShelfSprite);
			writer.WritePropertyByRef("employeeCanvas", instance.employeeCanvas);
			writer.WritePropertyByRef("employeeLoadingImage", instance.employeeLoadingImage);
			writer.WriteProperty("botName", instance.botName, ES3Type_string.Instance);
			writer.WriteProperty("botBudget", instance.botBudget, ES3Type_int.Instance);
			writer.WritePropertyByRef("shoppingEffect", instance.shoppingEffect);
			writer.WritePropertyByRef("shoppingSFX", instance.shoppingSFX);
			writer.WritePropertyByRef("aiPath", instance.aiPath);
			writer.WritePropertyByRef("destinationSetter", instance.destinationSetter);
			writer.WriteProperty("tags", instance.tags, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BotTags)));
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (NormalEmployee)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Walking":
						instance.Walking = reader.Read<UnityEngine.AudioClip>(ES3Type_AudioClip.Instance);
						break;
					case "isCarrying":
						instance.isCarrying = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "carryMax":
						instance.carryMax = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "carryCount":
						instance.carryCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "stayShelfDuration":
						instance.stayShelfDuration = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "myEmployeeAreaPos":
						instance.myEmployeeAreaPos = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "shelfPlacementManager":
						instance.shelfPlacementManager = reader.Read<ShelfPlacementManager>();
						break;
					case "eStage":
						instance.eStage = reader.Read<employeeStage>();
						break;
					case "eAction":
						instance.eAction = reader.Read<employeeAction>();
						break;
					case "employeeArea":
						instance.employeeArea = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "carriedItemSprite":
						instance.carriedItemSprite = reader.Read<UnityEngine.SpriteRenderer>(ES3Type_SpriteRenderer.Instance);
						break;
					case "carriedShelfSprite":
						instance.carriedShelfSprite = reader.Read<UnityEngine.SpriteRenderer>(ES3Type_SpriteRenderer.Instance);
						break;
					case "employeeCanvas":
						instance.employeeCanvas = reader.Read<UnityEngine.Canvas>();
						break;
					case "employeeLoadingImage":
						instance.employeeLoadingImage = reader.Read<UnityEngine.UI.Image>(ES3Type_Image.Instance);
						break;
					case "botName":
						instance.botName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "botBudget":
						instance.botBudget = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "shoppingEffect":
						instance.shoppingEffect = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "shoppingSFX":
						instance.shoppingSFX = reader.Read<UnityEngine.AudioClip>(ES3Type_AudioClip.Instance);
						break;
					case "aiPath":
						instance.aiPath = reader.Read<Pathfinding.AIPath>(ES3UserType_AIPath.Instance);
						break;
					case "destinationSetter":
						instance.destinationSetter = reader.Read<Pathfinding.AIDestinationSetter>(ES3UserType_AIDestinationSetter.Instance);
						break;
					case "tags":
						instance.tags = reader.Read<BotTags>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_NormalEmployeeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NormalEmployeeArray() : base(typeof(NormalEmployee[]), ES3UserType_NormalEmployee.Instance)
		{
			Instance = this;
		}
	}
}