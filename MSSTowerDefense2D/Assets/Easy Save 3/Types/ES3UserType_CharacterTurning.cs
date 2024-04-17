using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("front", "back", "forward", "path")]
	public class ES3UserType_CharacterTurning : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CharacterTurning() : base(typeof(CharacterTurning)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (CharacterTurning)obj;
			
			writer.WritePropertyByRef("front", instance.front);
			writer.WritePropertyByRef("back", instance.back);
			writer.WritePropertyByRef("forward", instance.forward);
			writer.WriteProperty("path", instance.path, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Pathfinding.IAstarAI)));
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (CharacterTurning)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "front":
						instance.front = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "back":
						instance.back = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "forward":
						instance.forward = reader.Read<UnityEngine.Sprite>(ES3Type_Sprite.Instance);
						break;
					case "path":
						instance.path = reader.Read<Pathfinding.IAstarAI>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CharacterTurningArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CharacterTurningArray() : base(typeof(CharacterTurning[]), ES3UserType_CharacterTurning.Instance)
		{
			Instance = this;
		}
	}
}