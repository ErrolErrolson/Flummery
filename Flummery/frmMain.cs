﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using WeifenLuo.WinFormsUI.Docking;
using ToxicRagers.Carmageddon2.Formats;
using Flummery.ContentPipeline.Stainless;

namespace Flummery
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        static bool bPublicRelease = false;

        SceneManager scene;

        public DockPanel DockPanel { get { return dockPanel; } }

        private void frmMain_Load(object sender, EventArgs e)
        {
            var overview = new pnlOverview();
            var viewport = new pnlViewport();
            var materials = new pnlMaterialList();
            var settings = new widgetTransform();

            overview.Show(dockPanel, DockState.DockLeft);
            settings.Show(dockPanel, DockState.DockRight);
            materials.Show(dockPanel, DockState.DockBottom);
            viewport.Show(dockPanel, DockState.Document);

            var extensions = new List<string>(GL.GetString(StringName.Extensions).Split(' '));
            this.Text += " v" + Flummery.Version;

            scene = new SceneManager(extensions.Contains("GL_ARB_vertex_buffer_object"));
            overview.RegisterEventHandlers();
            materials.RegisterEventHandlers();
            settings.RegisterEventHandlers();

            BuildMenu();

            ToxicRagers.Helpers.Logger.ResetLog();

            SetActionScalingText("Action Scaling: 1.000");

            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(frmMain_KeyPress);

            SceneManager.Current.OnProgress += scene_OnProgress;

            //flpMaterials.Tag = new SortedList<string, string>();

            //var bundle = ToxicRagers.BurnoutParadise.Formats.BUNDLE.Load(@"D:\Steam\steamapps\common\Burnout(TM) Paradise The Ultimate Box\VEHICLES\VEH_PUSMBG13_GR.BIN");
            //foreach (var entry in bundle.Contents)
            //{
            //    //Console.WriteLine("{0} @ {1}: {2} {3}", entry.Name, entry.Offset, entry.Size, entry.Unknown);
            //    bundle.Extract(entry, @"D:\Unbundle\VEH_PUSMBG13_GR\");
            //}

            Flummery.UI = this;
        }

        void scene_OnProgress(object sender, ProgressEventArgs e)
        {
            tsslProgress.Text = e.Status;
            tsslProgress.Owner.Refresh();
            Application.DoEvents();
        }

        void frmMain_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = ViewportManager.Current.KeyPress(sender, e);
        }

        private void BuildMenu()
        {
            MainMenu menu = new MainMenu();
            menu.MenuItems.Add("&File");
            menu.MenuItems[0].MenuItems.Add("&New", menuClick);
            menu.MenuItems[0].MenuItems[0].Shortcut = Shortcut.CtrlN;
            menu.MenuItems[0].MenuItems.Add("&Open...");
            menu.MenuItems[0].MenuItems[1].MenuItems.Add("Carmageddon 2");
            menu.MenuItems[0].MenuItems[1].MenuItems[0].MenuItems.Add("Actor", menuCarmageddon2Click);

            menu.MenuItems[0].MenuItems[1].MenuItems.Add("Carmageddon Mobile");
            menu.MenuItems[0].MenuItems[1].MenuItems[1].MenuItems.Add("Vehicle", menuCarmageddonMobileClick);

            menu.MenuItems[0].MenuItems[1].MenuItems.Add("Carmageddon Reincarnation");
            menu.MenuItems[0].MenuItems[1].MenuItems[2].MenuItems.Add("Accessory", menuCarmageddonReincarnationClick);
            menu.MenuItems[0].MenuItems[1].MenuItems[2].MenuItems.Add("Environment", menuCarmageddonReincarnationClick);
            menu.MenuItems[0].MenuItems[1].MenuItems[2].MenuItems.Add("Pedestrian", menuCarmageddonReincarnationClick);
            menu.MenuItems[0].MenuItems[1].MenuItems[2].MenuItems.Add("Vehicle", menuCarmageddonReincarnationClick);

            menu.MenuItems[0].MenuItems[1].MenuItems.Add("Novadrome");
            menu.MenuItems[0].MenuItems[1].MenuItems[3].MenuItems.Add("Environment", menuNovadromeClick);
            menu.MenuItems[0].MenuItems[1].MenuItems[3].MenuItems.Add("Vehicle", menuNovadromeClick);

            menu.MenuItems[0].MenuItems[1].MenuItems.Add("TDR2000");
            menu.MenuItems[0].MenuItems[1].MenuItems[4].MenuItems.Add("Hierarchy", menuTDR2000Click);

            menu.MenuItems[0].MenuItems.Add("&Import");
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("Autodesk FBX File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("Blender OBJ File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("BRender ACT File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("BRender DAT File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("Stainless CNT File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("Stainless MDL File...", menuImportClick);
            menu.MenuItems[0].MenuItems[2].MenuItems.Add("Torus MSHS File...", menuImportClick);

            menu.MenuItems[0].MenuItems.Add("-");
            menu.MenuItems[0].MenuItems.Add("Save", menuClick);

            menu.MenuItems[0].MenuItems.Add("Save As...");
            menu.MenuItems[0].MenuItems[5].MenuItems.Add("Carmageddon Reincarnation");
            menu.MenuItems[0].MenuItems[5].MenuItems[0].MenuItems.Add("Environment", menuSaveAsClick);
            menu.MenuItems[0].MenuItems[5].MenuItems[0].MenuItems.Add("Vehicle", menuSaveAsClick);

            menu.MenuItems[0].MenuItems.Add("&Export");
            menu.MenuItems[0].MenuItems[6].MenuItems.Add("Autodesk FBX File...", menuExportClick);
            menu.MenuItems[0].MenuItems[6].MenuItems.Add("Stainless CNT File...", menuExportClick);

            menu.MenuItems[0].MenuItems.Add("-");
            menu.MenuItems[0].MenuItems.Add("E&xit", menuClick);

            menu.MenuItems.Add("&View");
            menu.MenuItems[1].MenuItems.Add("Preferences", menuViewClick);

            menu.MenuItems.Add("&Object");
            menu.MenuItems[2].MenuItems.Add("New...", menuObjectClick);
            menu.MenuItems[2].MenuItems.Add("Remove...", menuObjectClick);
            menu.MenuItems[2].MenuItems.Add("-");
            menu.MenuItems[2].MenuItems.Add("Modify model...");
            menu.MenuItems[2].MenuItems[3].MenuItems.Add("Modify geometry...", menuObjectClick);
            menu.MenuItems[2].MenuItems.Add("Modify actor...", menuObjectClick);
            menu.MenuItems[2].MenuItems.Add("-");
            menu.MenuItems[2].MenuItems.Add("Rename", menuObjectClick);
            menu.MenuItems[2].MenuItems.Add("-");
            menu.MenuItems[2].MenuItems.Add("Munge Mesh with Bone", menuObjectClick);

            menu.MenuItems.Add("&Tools");
            menu.MenuItems[3].MenuItems.Add("General");
            menu.MenuItems[3].MenuItems[0].MenuItems.Add("TDX Convertor", menuToolsClick);
            menu.MenuItems[3].MenuItems[0].MenuItems.Add("Process all...");
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("CNT files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("LIGHT files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("MDL files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("MTL files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("TDX files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("Accessory.txt files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("Routes.txt files", menuCarmageddonReincarnationClick);
            menu.MenuItems[3].MenuItems[0].MenuItems[1].MenuItems.Add("XT2 files", menuNovadromeClick);
            menu.MenuItems[3].MenuItems.Add("Carma 2");
            menu.MenuItems[3].MenuItems[1].MenuItems.Add("Convert &&Actors to Entities", menuCarmageddon2Click);
            menu.MenuItems[3].MenuItems.Add("Reincarnation");
            menu.MenuItems[3].MenuItems[2].MenuItems.Add("Wheel Preview", menuCarmageddonReincarnationClick);

            menu.MenuItems.Add("&Help");
            menu.MenuItems[4].MenuItems.Add("About Flummery");

            this.Menu = menu;
        }

        private void menuClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "&New":
                    scene.Reset();
                    break;

                case "Save":
                    if (bPublicRelease) { MessageBox.Show("Coming Soon!"); return; }

                    sfdBrowse.Filter = "Stainless CNT files (*.cnt)|*.cnt";
                    if (sfdBrowse.ShowDialog() == DialogResult.OK)
                    {
                        var cx = new CNTExporter();
                        cx.ExportSettings.AddSetting("Scale", new Vector3(1.0f, 1.0f, -1.0f));
                        cx.Export(scene.Models[0], sfdBrowse.FileName);

                        var mx = new MDLExporter();
                        mx.ExportSettings.AddSetting("Transform", Matrix4.CreateScale(1.0f, 1.0f, -1.0f));
                        mx.ExportSettings.AddSetting("Handed", Model.CoordinateSystem.RightHanded);
                        mx.Export(scene.Models[0], Path.GetDirectoryName(sfdBrowse.FileName) + "\\");
                    }
                    break;

                case "E&xit":
                    Application.Exit();
                    break;
            }
        }

        private void menuImportClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Autodesk FBX File...":
                    ofdBrowse.Filter = "Autodesk FBX files (*.fbx)|*.fbx";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, ContentPipeline.Core.FBXImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "Blender OBJ File...":
                    ofdBrowse.Filter = "Blender OBJ files (*.obj)|*.obj";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, ContentPipeline.Core.OBJImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "BRender ACT File...":
                    ofdBrowse.Filter = "BRender ACT files (*.act)|*.act";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, ACTImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "BRender DAT File...":
                    ofdBrowse.Filter = "BRender DAT files (*.dat)|*.dat";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, DATImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "Stainless CNT File...":
                    ofdBrowse.Filter = "Stainless CNT files (*.cnt)|*.cnt";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, CNTImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "Stainless MDL File...":
                    ofdBrowse.Filter = "Stainless MDL files (*.mdl)|*.mdl";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, MDLImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "Torus MSHS File...":
                    ofdBrowse.Filter = "Torus MSHS files (*.mshs)|*.mshs";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, ContentPipeline.TDR2000.MSHSImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;
            }
        }

        private void menuExportClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Autodesk FBX File...":
                    sfdBrowse.Filter = "Autodesk FBX files (*.fbx)|*.fbx";
                    if (sfdBrowse.ShowDialog() == DialogResult.OK)
                    {
                        var fx = new ContentPipeline.Core.FBXExporter();
                        fx.ExportSettings.AddSetting("Scale", new Vector3(1.0f, 1.0f, -1.0f));
                        fx.Export(scene.Models[0], sfdBrowse.FileName);
                        SceneManager.Current.UpdateProgress(string.Format("Saved {0}", Path.GetFileName(sfdBrowse.FileName)));
                    }
                    break;

                case "Stainless CNT File...":
                    sfdBrowse.Filter = "Stainless CNT files (*.cnt)|*.cnt";
                    if (sfdBrowse.ShowDialog() == DialogResult.OK)
                    {
                        var cx = new CNTExporter();
                        cx.ExportSettings.AddSetting("Scale", new Vector3(1.0f, 1.0f, -1.0f));
                        cx.Export(scene.Models[0], sfdBrowse.FileName);
                    }
                    break;
            }
        }

        private void menuCarmageddon2Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Actor":
                    ofdBrowse.Filter = "Carmageddon 2 ACTOR (*.act)|*.act";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Content.Load<Model, ACTImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;

                case "Convert &&Actors to Entities":
                    if (scene.Models.Count == 0) { return; }

                    for (int i = scene.Models[0].Bones.Count - 1; i >= 0; i--)
                    {
                        var bone = scene.Models[0].Bones[i];

                        if (bone.Name.StartsWith("&"))
                        {
                            var entity = new Entity();

                            if (bone.Name.StartsWith("&£"))
                            {
                                string key = bone.Name.Substring(2, 2);

                                entity.UniqueIdentifier = "errol_B00BIE" + key + "_" + i.ToString("000");
                                entity.EntityType = EntityType.Powerup;

                                var pup = ToxicRagers.Carmageddon2.Powerups.LookupID(int.Parse(key));

                                if (pup.InCR)
                                {
                                    entity.Name = "pup_" + pup.Name;
                                    entity.Tag = pup.Model;
                                }
                                else
                                {
                                    entity.Name = "pup_Pinball";
                                    entity.Tag = pup.Model;
                                }
                            }
                            else
                            {
                                // accessory
                                entity.UniqueIdentifier = "errol_HEAD00" + bone.Name.Substring(1, 2) + "_" + i.ToString("000");
                                entity.EntityType = EntityType.Accessory;
                                entity.Name = "C2_" + ((ModelMesh)bone.Tag).Name.Substring(3);
                            }

                            entity.Transform = bone.CombinedTransform;
                            entity.AssetType = AssetType.Sprite;
                            scene.Entities.Add(entity);

                            scene.Models[0].RemoveBone(bone.Index);
                        }
                    }

                    SceneManager.Current.Change();
                    break;
            }
        }

        private void menuCarmageddonMobileClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Vehicle":
                    openContent("Carmageddon Mobile Vehicles (carbody.cnt)|carbody.cnt");
                    break;
            }
        }
        

        private void openContent(string filter)
        {
            ofdBrowse.Filter = filter;

            if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
            {
                scene.Reset();
                scene.Content.Load<Model, CNTImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
            }
        }

        private void menuCarmageddonReincarnationClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Accessory":
                    ofdBrowse.Filter = "Carmageddon ReinCARnation Accessory files (accessory.cnt)|accessory.cnt";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Reset();
                        var accessory = scene.Add(scene.Content.Load<Model, CNTImporter>(Path.GetFileName(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName)));

                        string accessorytxt = ofdBrowse.FileName.Replace(".cnt", ".txt", StringComparison.OrdinalIgnoreCase);

                        if (File.Exists(accessorytxt))
                        {
                            accessory.Tag = ToxicRagers.CarmageddonReincarnation.Formats.Accessory.Load(accessorytxt);
                        }
                    }
                    break;

                case "Environment":
                    openContent("Carmageddon ReinCARnation Environment files (level.cnt)|level.cnt");
                    break;

                case "Pedestrian":
                    openContent("Carmageddon ReinCARnation Pedestrians (bodyform.cnt)|bodyform.cnt");
                    break;

                case "Vehicle":
                    ofdBrowse.Filter = "Carmageddon ReinCARnation Vehicles (car.cnt)|car.cnt";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Reset();
                        var vehicle = (Model)scene.Add(scene.Content.Load<Model, CNTImporter>(Path.GetFileName(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName)));

                        foreach (var bone in vehicle.Bones)
                        {
                            string boneName = bone.Name.ToLower();

                            if (boneName.StartsWith("wheel_") || bone.Name.StartsWith("driver"))
                            {
                                var entity = new Entity { 
                                    Name = bone.Name, 
                                    EntityType = (boneName.StartsWith("driver") ? EntityType.Driver : EntityType.Wheel), 
                                    AssetType = AssetType.Sprite 
                                };
                                entity.LinkWith(bone);

                                scene.Entities.Add(entity);
                            }
                        }
                    }
                    break;

                case "CNT files":
                case "MDL files":
                case "MTL files":
                case "TDX files":
                case "LIGHT files":
                case "Accessory.txt files":
                case "Routes.txt files":
                    processAll(mi.Text);
                    break;

                case "Wheel Preview":
                    var preview = new frmReincarnationWheelPreview();

                    var result = preview.ShowDialog();

                    switch (result)
                    {
                        case DialogResult.OK:
                        case DialogResult.Abort:
                            foreach (var entity in SceneManager.Current.Entities)
                            {
                                if (entity.EntityType == EntityType.Wheel)
                                {
                                    entity.Asset = (result == DialogResult.OK ? preview.Wheel : null);
                                    entity.AssetType = (result == DialogResult.OK ? AssetType.Model : AssetType.Sprite);
                                }
                            }
                            break;
                    }
                    break;
            }
        }

        private void menuNovadromeClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Environment":
                    openContent("Novadrome Environments (level-*.cnt)|level-*.cnt");
                    break;

                case "Vehicle":
                    openContent("Novadrome Vehicles (carbody.cnt)|carbody.cnt");
                    break;

                case "XT2 files":
                    processAll(mi.Text);
                    break;
            }
        }

        private void processAll(string fileType)
        {
            string extension = fileType.Substring(0, fileType.IndexOf(' ')).ToLower();
            fbdBrowse.SelectedPath = (Properties.Settings.Default.LastBrowsedFolder != null ? Properties.Settings.Default.LastBrowsedFolder : Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));

            int success = 0;
            int fail = 0;

            if (fbdBrowse.ShowDialog() == DialogResult.OK && Directory.Exists(fbdBrowse.SelectedPath))
            {
                Properties.Settings.Default.LastBrowsedFolder = fbdBrowse.SelectedPath;
                Properties.Settings.Default.Save();

                ToxicRagers.Helpers.IO.LoopDirectoriesIn(fbdBrowse.SelectedPath, (d) =>
                {
                    foreach (FileInfo fi in d.GetFiles((extension.Contains(".") ? extension : "*." + extension)))
                    {
                        object result = null;

                        switch (extension)
                        {
                            case "cnt":
                                result = ToxicRagers.Stainless.Formats.CNT.Load(fi.FullName);
                                break;

                            case "mdl":
                                result = ToxicRagers.Stainless.Formats.MDL.Load(fi.FullName);
                                break;

                            case "mtl":
                                result = ToxicRagers.Stainless.Formats.MTL.Load(fi.FullName);
                                break;

                            case "tdx":
                                result = ToxicRagers.CarmageddonReincarnation.Formats.TDX.Load(fi.FullName);
                                break;

                            case "light":
                                result = ToxicRagers.CarmageddonReincarnation.Formats.LIGHT.Load(fi.FullName);
                                break;

                            case "xt2":
                                result = ToxicRagers.Novadrome.Formats.XT2.Load(fi.FullName);
                                break;

                            case "accessory.txt":
                                result = ToxicRagers.CarmageddonReincarnation.Formats.Accessory.Load(fi.FullName);
                                break;

                            case "routes.txt":
                                result = ToxicRagers.CarmageddonReincarnation.Formats.Routes.Load(fi.FullName);
                                break;
                        }

                        if (result != null) { success++; } else { fail++; }

                        tsslProgress.Text = string.Format("[{0}/{1}] {2}", success, fail, fi.FullName.Replace(fbdBrowse.SelectedPath, ""));
                        Application.DoEvents();
                    }
                }
                );

                tsslProgress.Text = string.Format("{0} processing complete. {1} success {2} fail", extension, success, fail);
            }
        }

        private void menuTDR2000Click(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Hierarchy":
                    ofdBrowse.Filter = "TDR2000 Hierarchy (*.hie)|*.hie";

                    if (ofdBrowse.ShowDialog() == DialogResult.OK && File.Exists(ofdBrowse.FileName))
                    {
                        scene.Reset();
                        scene.Content.Load<Model, ContentPipeline.TDR2000.HIEImporter>(Path.GetFileNameWithoutExtension(ofdBrowse.FileName), Path.GetDirectoryName(ofdBrowse.FileName), true);
                    }
                    break;
            }
        }

        private void menuSaveAsClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            if (bPublicRelease) { MessageBox.Show("Coming Soon!"); return; }

            switch (mi.Text)
            {
                case "Environment":
                    var fmSaveAsEnvironment = new frmSaveAsEnvironment();
                    fmSaveAsEnvironment.Show(this);
                    break;

                case "Vehicle":
                    var fmSaveAsVehicle = new frmSaveAsVehicle();
                    fmSaveAsVehicle.Show(this);
                    break;
            }
        }

        private void menuViewClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "Preferences":
                    var prefs = new frmPreferences();
                    prefs.Show();
                    break;
            }
        }

        private void menuObjectClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "New...":
                    var addNew = new frmNewObject();
                    addNew.SetParentNode(SceneManager.Current.SelectedBoneIndex);

                    if (addNew.ShowDialog(this) == DialogResult.OK)
                    {
                        SceneManager.Current.Change();
                    }
                    break;

                case "Remove...":
                    var removeObject = new frmRemoveObject();
                    removeObject.SetParentNode(SceneManager.Current.SelectedBoneIndex);

                    if (removeObject.ShowDialog(this) == DialogResult.OK)
                    {
                        SceneManager.Current.Change();
                    }
                    break;

                case "Modify geometry...":
                    var geometry = new frmModifyModel();
                    geometry.SetParentNode(SceneManager.Current.SelectedBoneIndex);

                    if (geometry.ShowDialog(this) == DialogResult.OK)
                    {
                        SceneManager.Current.Change();
                    }
                    break;

                case "Modify actor...":
                    var transform = new frmModifyActor();
                    transform.SetParentNode(SceneManager.Current.SelectedBoneIndex);

                    if (transform.ShowDialog(this) == DialogResult.OK)
                    {
                        SceneManager.Current.Change();
                    }
                    break;

                case "Rename":
                    var rename = new frmRename();
                    rename.SetParentNode(SceneManager.Current.SelectedBoneIndex);

                    if (rename.ShowDialog(this) == DialogResult.OK)
                    {
                        SceneManager.Current.Change();
                    }
                    break;

                case "Munge Mesh with Bone":
                    if (SceneManager.Current.SelectedBoneIndex > -1 && SceneManager.Current.Models[0].Bones[SceneManager.Current.SelectedBoneIndex].Tag != null)
                    {
                        var bone = SceneManager.Current.Models[0].Bones[SceneManager.Current.SelectedBoneIndex];
                        var mesh = (ModelMesh)bone.Tag;
                        var offset = mesh.BoundingBox.Centre;

                        foreach (var meshpart in mesh.MeshParts) { 
                            for (int i = 0; i < meshpart.VertexCount; i++) { meshpart.VertexBuffer.ModifyVertexPosition(i, meshpart.VertexBuffer.Data[i].Position - offset); }
                            meshpart.VertexBuffer.Initialise();
                        }
                        mesh.BoundingBox.Calculate(mesh);

                        var m = bone.Transform;
                        m.M41 += offset.X;
                        m.M42 += offset.Y;
                        m.M43 += offset.Z;
                        bone.Transform = m;

                        SceneManager.Current.Change();
                    }
                    break;
            }
        }

        private void menuToolsClick(object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            switch (mi.Text)
            {
                case "TDX Convertor":
                    var tdx = new frmTDXConvert();
                    tdx.Show(this);
                    break;
            }
        }

        public void SetActionScalingText(string scale)
        {
            tsslActionScaling.Text = scale;
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            Flummery.Active = true;
        }

        private void frmMain_Deactivate(object sender, EventArgs e)
        {
            Flummery.Active = false;
        }
    }
}
 