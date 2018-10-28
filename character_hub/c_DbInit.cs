using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace character_hub
{
	class c_DbInit
	{
		//Timelines	/ project
		//			/ story
		//			/ character
		//			/ object (with known history)

		//For character, checkbox if he's alive

		private c_SqlFunctions _sf = null;
		List<SQLTableContainer> tables = new List<SQLTableContainer>() {
			//Which user owns which projects + user data
			new SQLTableContainer(){ name = "users",			columns = new SQLiteColumn[]{
				new SQLiteColumn(){ columnName = "id",					dataType = SQLiteDataType.INT, autoIncrement = true, pimaryKey = true },
				new SQLiteColumn(){ columnName = "name",				dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "nom_de_plume",		dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "born",				dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "bio",					dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "other_works",			dataType = SQLiteDataType.TEXT }
			} },  

			//Every project gets a number + project base data
			new SQLTableContainer(){ name = "projects",			columns = new SQLiteColumn[]{
				new SQLiteColumn(){ columnName = "id",					dataType = SQLiteDataType.INT, autoIncrement = true, pimaryKey = true },
				new SQLiteColumn(){ columnName = "user",				dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "users", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "timeline",			dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "timelines", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "project_name",		dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "project_desc",		dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "additional_data",		dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "project_save_dir",	dataType = SQLiteDataType.TEXT }
			} },  

			//Multiple stories contained in a project
			new SQLTableContainer(){ name = "stories",			columns = new SQLiteColumn[]{
				new SQLiteColumn(){ columnName = "id",					dataType = SQLiteDataType.INT, autoIncrement = true, pimaryKey = true },
				new SQLiteColumn(){ columnName = "project",				dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "projects", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "timeline",			dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "timelines", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "story_name",			dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "story_desc",			dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "dedication",			dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "story_start_date",	dataType = SQLiteDataType.TEXT }
			} },	

			//Characters trhat are alive and have some form of thought ( + pets and plants maybe)
			new SQLTableContainer(){ name = "characters",		columns = new SQLiteColumn[]{
				new SQLiteColumn(){ columnName = "id",					dataType = SQLiteDataType.INT, autoIncrement = true, pimaryKey = true },
				new SQLiteColumn(){ columnName = "project",				dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "projects", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "story",				dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "stories", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "timeline",			dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "timelines", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "name",				dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "short_info",			dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "bio",					dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "alive",				dataType = SQLiteDataType.BOOLEAN }
			} },  

			new SQLTableContainer(){ name = "objects",			columns = new SQLiteColumn[]{ } },  //Inanimate objects

			new SQLTableContainer(){ name = "locations",		columns = new SQLiteColumn[]{ } },  //Places

			new SQLTableContainer(){ name = "relations",		columns = new SQLiteColumn[]{ } },  //Relationships already made

			new SQLTableContainer(){ name = "storylines",		columns = new SQLiteColumn[]{ } },  //Loose connection of assets forming a story

			new SQLTableContainer(){ name = "timelines",		columns = new SQLiteColumn[]{ } },  //Timelines that can be for a whole project, a story, a character or an item with a known background

			new SQLTableContainer(){ name = "relation_types",	columns = new SQLiteColumn[]{ } },  //Types of relationships that can occur

			new SQLTableContainer(){ name = "graphs",			columns = new SQLiteColumn[]{ } },  //Graphs and graph data

			new SQLTableContainer(){ name = "templates",		columns = new SQLiteColumn[]{ } },  //premade templates that can be applied quickly

			new SQLTableContainer(){ name = "content_storage",	columns = new SQLiteColumn[]{ } },  //Table to store useful texts and stuff
			
			new SQLTableContainer(){ name = "references",		columns = new SQLiteColumn[]{ } },  //Categorized reference material

			new SQLTableContainer(){ name = "art",				columns = new SQLiteColumn[]{ } },  //Art to be used, stored in text form (b64?)

			new SQLTableContainer(){ name = "cross_references",	columns = new SQLiteColumn[]{ } },  //Will figure out for sure later

			//Basically a dictionary with word:explanation pairs
			new SQLTableContainer(){ name = "lexicon",			columns = new SQLiteColumn[]{
				new SQLiteColumn(){ columnName = "id",			dataType = SQLiteDataType.INT, autoIncrement = true, pimaryKey = true },
				new SQLiteColumn(){ columnName = "story_id",	dataType = SQLiteDataType.INT, foreignKey = true, foreignTable = "stories", foreignColumn = "id" },
				new SQLiteColumn(){ columnName = "word",		dataType = SQLiteDataType.TEXT },
				new SQLiteColumn(){ columnName = "explanation",	dataType = SQLiteDataType.TEXT }
			} },
			new SQLTableContainer(){ name = "", columns = new SQLiteColumn[]{ } },
		};
		public c_DbInit(c_SqlFunctions sqlfnc = null)
		{
			if(sqlfnc == null) { return; }

			_sf = sqlfnc;
			_sf.createDbFile();
			_sf.createInnerConnection();
			checkCreateTables();
		}

		private SQLTableContainer getTable(string name)
		{
			var v = tables.Where(x => x.name == name).ToList<SQLTableContainer>();
			if (v.Count() > 0)
			{
				return v[0];
			}
			else
			{
				return null;
			}
		}

		public void checkCreateTables()
		{

		}
	}
}
