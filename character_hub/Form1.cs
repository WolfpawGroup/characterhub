using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace character_hub
{
	public partial class Form1 : Form
	{
		public c_SqlFunctions sqlFunctions = new c_SqlFunctions();

		public Form1()
		{
			InitializeComponent();
			Load += Form1_Load;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			loadAllData();
		}

		public void loadAllData()
		{
			loadFiles();
			loadDB();
			loadSettings();
		}

		public void loadFiles()
		{
			if (!File.Exists("rel.txt"))
			{
				File.WriteAllText("rel.txt", Properties.Resources.rel);
			}
			else
			{
				misc.buildEnum();
			}
		}

		public void loadDB()
		{
			c_DbInit cd = new c_DbInit(sqlFunctions);
		}

		public void loadSettings()
		{

		}
	}
}
