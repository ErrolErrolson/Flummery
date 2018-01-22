﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Flummery.ContentPipeline.Stainless;

using ToxicRagers.CarmageddonReincarnation.Formats;

using OpenTK;

namespace Flummery
{
    public partial class frmSaveAsVehicle : Form
    {
        FlumpFile flump;
        string car;

        Label lblInfo = null;
        Label lblProgress = null;
        System.Timers.Timer timer = new System.Timers.Timer(200);
        string[] frames = new string[] { "◐", "◓", "◑", "◒" };
        int progressMax = 0;

        public frmSaveAsVehicle()
        {
            InitializeComponent();

            timer.AutoReset = true;
            timer.SynchronizingObject = this;
            timer.Elapsed += timer_Elapsed;

            txtPath.Text = Properties.Settings.Default.SaveAsVehiclePath;
            setCar();
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            fbdBrowse.SelectedPath = txtPath.Text;

            if (fbdBrowse.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(fbdBrowse.SelectedPath)) { Directory.CreateDirectory(fbdBrowse.SelectedPath); }
                txtPath.Text = fbdBrowse.SelectedPath + "\\";

                setCar();

                Properties.Settings.Default.SaveAsVehiclePath = txtPath.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void setCar()
        {
            if (txtPath.Text.Length == 0)
            {
                btnOK.Enabled = false;
                return;
            }
            else
            {
                btnOK.Enabled = true;
            }

            flump = FlumpFile.Load(txtPath.Text + "car.flump");
            if (flump.Settings.ContainsKey("pretty.name")) { txtPrettyCarName.Text = flump.Settings["pretty.name"]; }

            car = Path.GetFileName(Path.GetDirectoryName(txtPath.Text));
            txtCarName.Text = car;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SceneManager.Current.OnProgress += scene_OnProgress;

            btnOK.Visible = false;
            btnCancel.Visible = false;

            gbProgress.Visible = true;
            pbProgress.Visible = true;

            Application.DoEvents();
            timer.Start();

            if (!Directory.Exists(txtPath.Text)) { Directory.CreateDirectory(txtPath.Text); }

            flump.Settings["car"] = car;
            flump.Settings["pretty.name"] = txtPrettyCarName.Text;

            lblInfo = lblInfoMeshes;
            lblProgress = lblProgressMeshes;
            lblProgress.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            progressMax = 30;

            new CNTExporter().Export(SceneManager.Current.Models[0], txtPath.Text + "car.cnt");
            new MDLExporter().Export(SceneManager.Current.Models[0], txtPath.Text);

            lblProgress.Text = "✓";
            lblProgress.ForeColor = Color.Green;
            lblInfo.Text = "Meshes";
            pbProgress.Value = progressMax;

            Application.DoEvents();

            lblInfo = lblInfoTextures;
            lblProgress = lblProgressTextures;
            lblProgress.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            progressMax = 50;

            var textures = new List<string>();

            foreach (var material in SceneManager.Current.Materials)
            {
                string fileName = txtPath.Text + "\\" + material.Texture.Name;

                if (!textures.Contains(material.Texture.Name))
                {
                    if (!File.Exists(fileName + ".tdx"))
                    {
                        var tx = new TDXExporter();
                        tx.ExportSettings.AddSetting("Format", ToxicRagers.Helpers.D3DFormat.DXT5);
                        tx.Export(material.Texture, txtPath.Text);
                    }

                    textures.Add(material.Texture.Name);
                }
            }

            lblProgress.Text = "✓";
            lblProgress.ForeColor = Color.Green;
            lblInfo.Text = "Textures";
            pbProgress.Value = progressMax;

            Application.DoEvents();

            lblInfo = lblInfoMaterials;
            lblProgress = lblProgressMaterials;
            lblProgress.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            progressMax = 60;

            foreach (var material in SceneManager.Current.Materials)
            {
                string fileName = Path.Combine(txtPath.Text, material.Name + ".mt2");

                if (!File.Exists(fileName))
                {
                    var simple = new ToxicRagers.CarmageddonReincarnation.Formats.Materials.simple_base();
                    simple.DiffuseColour = material.Texture.Name;
                    simple.Save(fileName);
                }
            }

            lblProgress.Text = "✓";
            lblProgress.ForeColor = Color.Green;
            lblInfo.Text = "Materials";
            pbProgress.Value = progressMax;

            Application.DoEvents();

            lblInfo = lblInfoPaperwork;
            lblProgress = lblProgressPaperwork;
            lblProgress.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            progressMax = 75;

            if (!File.Exists(txtPath.Text + "setup.lol"))
            {
                Setup setup = new Setup(SetupContext.Vehicle);

                setup.Settings.SetParameterForMethod("PowerMultiplier", "Value", 1.5f);
                setup.Settings.SetParameterForMethod("TractionFactor", "Factor", 1.2f);
                setup.Settings.SetParameterForMethod("RearGrip", "Value", 1.6f);
                setup.Settings.SetParameterForMethod("FrontGrip", "Value", 1.7f);
                setup.Settings.SetParameterForMethod("FrontRoll", "Value", 0.4f);
                setup.Settings.SetParameterForMethod("RearRoll", "Value", 0.3f);
                setup.Settings.SetParameterForMethod("FrontSuspGive", "Value", 0.1f);
                setup.Settings.SetParameterForMethod("RearSuspGive", "Value", 0.08f);
                setup.Settings.SetParameterForMethod("SteerCentreMultiplier", "Value", 2);
                setup.Settings.SetParameterForMethod("DragCoefficient", "Value", 0.4f);
                setup.Settings.SetParameterForMethod("Mass", "Value", 1300);
                setup.Settings.SetParameterForMethod("TorqueCurve", "1", 150);
                setup.Settings.SetParameterForMethod("TorqueCurve", "2", 232);

                SceneManager.Current.Models[0].SupportingDocuments["Setup"] = setup;

                var sx = new SetupLOLExporter();
                sx.ExportSettings.AddSetting("Context", SetupContext.Vehicle);
                sx.Export(SceneManager.Current.Models[0], txtPath.Text);
            }

            if (!File.Exists(txtPath.Text + "Structure.xml"))
            {
                new StructureXMLExporter().Export(SceneManager.Current.Models[0], txtPath.Text);
            }

            if (!File.Exists(txtPath.Text + "SystemsDamage.xml"))
            {
                new SystemsDamageXMLExporter().Export(SceneManager.Current.Models[0], txtPath.Text);
            }

            if (!File.Exists(txtPath.Text + "vehicle_setup.cfg"))
            {
                var cfgx = new VehicleSetupCFGExporter();
                cfgx.ExportSettings.AddSetting("VehicleName", txtCarName.Text);
                cfgx.Export(SceneManager.Current.Models[0], txtPath.Text);
            }

            lblProgress.Text = "✓";
            lblProgress.ForeColor = Color.Green;
            lblInfo.Text = "Paperwork";
            pbProgress.Value = progressMax;

            Application.DoEvents();

            lblInfo = lblInfoZAD;
            lblProgress = lblProgressZAD;
            lblProgress.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            progressMax = 100;

            var minge = new ToxicRagers.CarmageddonReincarnation.Formats.MINGE();
            minge.Name = txtPrettyCarName.Text;
            minge.Author = Properties.Settings.Default.PersonalAuthor;
            minge.Website = Properties.Settings.Default.PersonalWebsite;
            minge.Type = MINGE.ModType.Vehicle;
            minge.Save(Path.Combine(txtPath.Text, txtCarName.Text + ".minge"));

            var zad = ToxicRagers.Stainless.Formats.ZAD.Create(Path.Combine(txtPath.Text, txtCarName.Text + ".zip"));
            zad.AddDirectory(Path.GetDirectoryName(txtPath.Text));

            lblProgress.Text = "✓";
            lblProgress.ForeColor = Color.Green;
            lblInfo.Text = "CarMODgeddon ZIP file";
            pbProgress.Value = progressMax;

            flump.Save(txtPath.Text + "car.flump");

            timer.Stop();
            SceneManager.Current.OnProgress -= scene_OnProgress;

            btnClose.Visible = true;

            Application.DoEvents();

            SceneManager.Current.UpdateProgress(string.Format("Vehicle '{0}' saved successfully!", car));
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (lblProgress != null && lblProgress.Text != "✓")
            {
                int frame = (lblProgress.Tag == null ? 0 : int.Parse(lblProgress.Tag.ToString()));

                lblProgress.Text = frames[frame++];

                if (frame == 4) { frame = 0; }

                lblProgress.Tag = frame;
            }

            if (pbProgress.Value < progressMax) { pbProgress.Value += Math.Max(1, (int)((progressMax - pbProgress.Value) * 0.01f)); }
        }

        void scene_OnProgress(object sender, ProgressEventArgs e)
        {
            if (lblInfo != null) { lblInfo.Text = e.Status; }
        }
    }
}
