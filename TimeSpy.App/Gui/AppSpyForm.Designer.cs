namespace TimeSpy
{
    partial class TimeSpyForm
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
            this.components = new System.ComponentModel.Container();
            this.TreeView = new BrightIdeasSoftware.TreeListView();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.TabActivities = new System.Windows.Forms.TabPage();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.TreeView)).BeginInit();
            this.ToolStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.TabActivities.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeView
            // 
            this.TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeView.Location = new System.Drawing.Point(0, 0);
            this.TreeView.Name = "TreeView";
            this.TreeView.OwnerDraw = true;
            this.TreeView.ShowGroups = false;
            this.TreeView.Size = new System.Drawing.Size(510, 353);
            this.TreeView.TabIndex = 0;
            this.TreeView.Text = "treeListView1";
            this.TreeView.UseCompatibleStateImageBehavior = false;
            this.TreeView.View = System.Windows.Forms.View.Details;
            this.TreeView.VirtualMode = true;
            // 
            // ToolStrip
            // 
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.ToolStrip.Size = new System.Drawing.Size(518, 25);
            this.ToolStrip.TabIndex = 1;
            this.ToolStrip.Text = "ToolStrip";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.TabActivities);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(518, 379);
            this.tabControl1.TabIndex = 2;
            // 
            // TabActivities
            // 
            this.TabActivities.Controls.Add(this.TreeView);
            this.TabActivities.Location = new System.Drawing.Point(4, 22);
            this.TabActivities.Name = "TabActivities";
            this.TabActivities.Size = new System.Drawing.Size(510, 353);
            this.TabActivities.TabIndex = 0;
            this.TabActivities.Text = "Activities";
            this.TabActivities.UseVisualStyleBackColor = true;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(35, 22);
            this.toolStripButton1.Text = "Start";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // TimeSpyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 404);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ToolStrip);
            this.Name = "TimeSpyForm";
            this.Text = "TimeSpy";
            this.Load += new System.EventHandler(this.TimeSpyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TreeView)).EndInit();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.TabActivities.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal BrightIdeasSoftware.TreeListView TreeView;
        internal System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage TabActivities;
        private System.Windows.Forms.ToolStripButton toolStripButton1;

    }
}

