using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DbSharper.CodeGenerator
{
	internal static class VsHelper
	{
		#region Methods

		public static ProjectItem FindProjectItem(Project project, string file)
		{
			return FindProjectItem(project.ProjectItems, file);
		}

		public static ProjectItem FindProjectItem(ProjectItems items, string file)
		{
			string atom = file.Substring(0, file.IndexOf("\\") + 1);

			foreach (ProjectItem item in items)
			{
				if (atom.StartsWith(item.Name))
				{
					// Then step in
					ProjectItem ritem = FindProjectItem(item.ProjectItems, file.Substring(file.IndexOf("\\") + 1));

					if (ritem != null)
					{
						return ritem;
					}
				}

				if (Regex.IsMatch(item.Name, file))
				{
					return item;
				}

				if (item.ProjectItems.Count > 0)
				{
					ProjectItem ritem = FindProjectItem(item.ProjectItems, file.Substring(file.IndexOf("\\") + 1));

					if (ritem != null)
					{
						return ritem;
					}
				}
			}

			return null;
		}

		public static List<ProjectItem> FindProjectItems(ProjectItems items, string match)
		{
			List<ProjectItem> values = new List<ProjectItem>();

			foreach (ProjectItem item in items)
			{
				if (Regex.IsMatch(item.Name, match))
				{
					values.Add(item);
				}

				if (item.ProjectItems.Count > 0)
				{
					values.AddRange(FindProjectItems(item.ProjectItems, match));
				}
			}

			return values;
		}

		public static IVsHierarchy GetCurrentHierarchy(IServiceProvider provider)
		{
			DTE vs = (DTE)provider.GetService(typeof(DTE));

			if (vs == null)
			{
				throw new InvalidOperationException("DTE not found.");
			}

			return ToHierarchy(vs.SelectedItems.Item(1).ProjectItem.ContainingProject);
		}

		public static Project ToDteProject(IVsHierarchy hierarchy)
		{
			if (hierarchy == null)
			{
				throw new ArgumentNullException("hierarchy");
			}

			object prjObject = null;

			if (hierarchy.GetProperty(0xfffffffe, -2027, out prjObject) >= 0)
			{
				return (EnvDTE.Project)prjObject;
			}
			else
			{
				throw new ArgumentException("Hierarchy is not a project.");
			}
		}

		public static Project ToDteProject(IVsProject project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}

			return ToDteProject(project as IVsHierarchy);
		}

		public static IVsHierarchy ToHierarchy(Project project)
		{
			if (project == null)
			{
				// DTE does not expose the project GUID that exists at in the msbuild project file.
				// Cannot use MSBuild object model because it uses a static instance of the Engine,
				// and using the Project will cause it to be unloaded from the engine when the
				// GC collects the variable that we declare.
				throw new ArgumentNullException("project");
			}

			string projectGuid = null;

			using (XmlReader projectReader = XmlReader.Create(project.FileName))
			{
				projectReader.MoveToContent();
				object nodeName = projectReader.NameTable.Add("ProjectGuid");

				while (projectReader.Read())
				{
					if (Object.Equals(projectReader.LocalName, nodeName))
					{
						projectGuid = (String)projectReader.ReadElementContentAsString(); break;
					}
				}
			}

			Debug.Assert(!String.IsNullOrEmpty(projectGuid));

			IServiceProvider serviceProvider = new ServiceProvider(project.DTE as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

			return VsShellUtilities.GetHierarchy(serviceProvider, new Guid(projectGuid));
		}

		public static IVsProject ToVsProject(Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}

			IVsProject vsProject = ToHierarchy(project) as IVsProject;

			if (vsProject == null)
			{
				throw new ArgumentException("Project is not a VS project.");
			}

			return vsProject;
		}

		#endregion Methods
	}
}