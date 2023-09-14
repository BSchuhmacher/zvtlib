using System;
using System.Reflection;
using System.Windows.Forms;

namespace ZvtEcrInterfaceTester {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Main());
		}

		public static string CreateVersionString(bool includeBuild) {
			Version ver = Assembly.GetExecutingAssembly().GetName().Version;
			string versionText = $@"v{ver.Major}.{ver.Minor}";

			if (includeBuild) {
				versionText += $@".{ver.Build}";
			}

#if BETA
				versionText += @" BETA";
			#elif DEBUG
				versionText += @" DEBUG";
			#endif

			return versionText;
		}

	}
}
