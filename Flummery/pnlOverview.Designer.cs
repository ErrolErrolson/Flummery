﻿namespace Flummery
{
    partial class pnlOverview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tvNodes = new System.Windows.Forms.TreeView();
            this.lblCoords = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tvNodes
            // 
            this.tvNodes.AllowDrop = true;
            this.tvNodes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvNodes.Location = new System.Drawing.Point(12, 12);
            this.tvNodes.Name = "tvNodes";
            this.tvNodes.Size = new System.Drawing.Size(133, 214);
            this.tvNodes.TabIndex = 0;
            this.tvNodes.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvNodes_ItemDrag);
            this.tvNodes.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvNodes_NodeMouseClick);
            this.tvNodes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvNodes_NodeMouseDoubleClick);
            this.tvNodes.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvNodes_DragDrop);
            this.tvNodes.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvNodes_DragEnter);
            // 
            // lblCoords
            // 
            this.lblCoords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCoords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCoords.Location = new System.Drawing.Point(12, 229);
            this.lblCoords.Name = "lblCoords";
            this.lblCoords.Size = new System.Drawing.Size(133, 23);
            this.lblCoords.TabIndex = 1;
            this.lblCoords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCoords.Click += new System.EventHandler(this.lblCoords_Click);
            // 
            // pnlOverview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(157, 261);
            this.Controls.Add(this.lblCoords);
            this.Controls.Add(this.tvNodes);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "pnlOverview";
            this.Text = "pnlOverview";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvNodes;
        private System.Windows.Forms.Label lblCoords;
    }
}