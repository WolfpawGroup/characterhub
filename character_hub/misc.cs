using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace character_hub
{
	static class misc
	{
		/// <summary>
		/// Creates the enum using the rel.txt file filling it with the relations used by the user
		/// </summary>
		public static void buildEnum()
		{
			AppDomain currentDomain	= AppDomain.CurrentDomain;
			AssemblyName aName		= new AssemblyName("TempAssembly");
			AssemblyBuilder ab		= currentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder mb		= ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");
			EnumBuilder eb			= mb.DefineEnum("relations", TypeAttributes.Public, typeof(int));

			if (File.Exists("rel.txt"))
			{
				foreach(string s in File.ReadAllLines("rel.txt"))
				{
					if (!s.StartsWith("#"))
					{
						var v = getValueFromLine(s);
						if(v.value > -1) { eb.DefineLiteral(v.name, v.value); }
					}
				}
			}
		}

		/// <summary>
		/// Gets property name and value for a line from rel.txt
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public static enumval getValueFromLine(string line)
		{
			enumval ev = new enumval();

			Regex r = new Regex(@"^[a-zA-Z_]+\:\d+$");

			if (r.IsMatch(line) && line.Split(':').Length == 2)
			{
				ev.name = line.Split(':')[0];
				int i = -1;
				int.TryParse(line.Split(':')[1], out i);
				ev.value = i;
			}

			return ev;
		}
	}

	public class enumval
	{
		public string name { get; set; } = "";
		public int value { get; set; } = -1;
	}

	public enum relations
	{

	}
}
