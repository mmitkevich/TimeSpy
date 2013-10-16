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
            this.MainTree = new BrightIdeasSoftware.TreeListView();
            ((System.ComponentModel.ISupportInitialize)(this.MainTree)).BeginInit();
            this.SuspendLayout();
            // 
            // MainTree
            // 
            this.MainTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTree.Location = new System.Drawing.Point(0, 0);
            this.MainTree.Name = "MainTree";
            this.MainTree.Size = new System.Drawing.Size(518, 404);
            this.MainTree.TabIndex = 0;
            this.MainTree.Text = "treeListView1";
            // 
            // TimeSpyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 404);
            this.Controls.Add(this.MainTree);
            this.Name = "TimeSpyForm";
            this.Text = "TimeSpy";
            this.Load += new System.EventHandler(this.TimeSpyForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainTree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.TreeListView MainTree;

    }
}

