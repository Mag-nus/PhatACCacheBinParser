﻿using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

using PhatACCacheBinParser.Properties;

namespace PhatACCacheBinParser
{
	public partial class ParserControl : UserControl
	{
		public ParserControl()
		{
			InitializeComponent();
		}


		private string propertyName;

		public string ProperyName
		{
			private get
			{
				return propertyName;
			}
			set
			{
				if (propertyName == value)
					return;

				propertyName = value;

				chkWriteJSON.CheckedChanged -= chkWriteJSON_CheckedChanged;

				if (!String.IsNullOrEmpty(ProperyName))
				{
					var property = new SettingsProperty(Settings.Default.Properties["SourceBin"]);
					property.Name = ProperyName + "SourceBin";
					property.PropertyType = typeof(string);
					Settings.Default.Properties.Add(property);

					property = new SettingsProperty(Settings.Default.Properties["WriteJSON"]);
					property.Name = ProperyName + "WriteJSON";
					property.PropertyType = typeof(bool);
					Settings.Default.Properties.Add(property);

					property = new SettingsProperty(Settings.Default.Properties["WriteSQL"]);
					property.Name = ProperyName + "WriteSQL";
					property.PropertyType = typeof(bool);
					Settings.Default.Properties.Add(property);


					SourceBin = (string)Settings.Default[ProperyName + "SourceBin"];
					WriteJSON = (bool) Settings.Default[ProperyName + "WriteJSON"];

					chkWriteJSON.CheckedChanged += chkWriteJSON_CheckedChanged;
				}
				else
				{
					SourceBin = null;
					WriteJSON = false;
				}
			}
		}

		public string Label
		{
			get => label1.Text;
			set => label1.Text = value;
		}


		public string SourceBin
		{
			get => lblSourceBin.Text;
			set => lblSourceBin.Text = value;
		}

		public bool WriteJSON
		{
			get => chkWriteJSON.Checked;
			set => chkWriteJSON.Checked = value;
		}

		public int ParseInputProgress
		{
			get => progressParseSource.Value;
			set
			{
				if (value == 0 || value == 100)
					progressParseSource.Style = ProgressBarStyle.Continuous;
				else
					progressParseSource.Style = ProgressBarStyle.Marquee;

				progressParseSource.Value = value;
			}
		}

		public int WriteJSONOutputProgress
		{
			get => progressWriteJSONOutput.Value;
			set
			{
				if (value == 0 || value == 100)
					progressWriteJSONOutput.Style = ProgressBarStyle.Continuous;
				else
					progressWriteJSONOutput.Style = ProgressBarStyle.Marquee;

				progressWriteJSONOutput.Value = value;
			}
		}


		public event Action<ParserControl> DoParse;


		private void cmdChooseSource_Click(object sender, EventArgs e)
		{
			using (var dialog = new OpenFileDialog())
			{
				if (!String.IsNullOrEmpty(SourceBin) && File.Exists(SourceBin))
					dialog.InitialDirectory = Path.GetDirectoryName(SourceBin);

				SourceBin = null;

				dialog.Filter = "Raw Chunk Files (raw)|*.raw";

				if (dialog.ShowDialog() == DialogResult.OK)
					SourceBin = dialog.FileName;
			}

			Settings.Default[ProperyName + "SourceBin"] = SourceBin;
			Settings.Default.Save();
		}

		private void chkWriteJSON_CheckedChanged(object sender, EventArgs e)
		{
			Settings.Default[ProperyName + "WriteJSON"] = WriteJSON;
			Settings.Default.Save();
		}

		private void cmdDoParse_Click(object sender, EventArgs e)
		{
			if (DoParse != null)
				DoParse(this);
		}
	}
}
