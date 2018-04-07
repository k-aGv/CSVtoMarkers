namespace GoogleMarkers {
    partial class Notes {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tb_notes = new System.Windows.Forms.TextBox();
            this.btn_apply = new System.Windows.Forms.Button();
            this.btn_discard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_notes
            // 
            this.tb_notes.Location = new System.Drawing.Point(13, 13);
            this.tb_notes.Multiline = true;
            this.tb_notes.Name = "tb_notes";
            this.tb_notes.Size = new System.Drawing.Size(259, 203);
            this.tb_notes.TabIndex = 0;
            this.tb_notes.Text = "Add your notes here";
            // 
            // btn_apply
            // 
            this.btn_apply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_apply.Location = new System.Drawing.Point(13, 223);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(75, 23);
            this.btn_apply.TabIndex = 1;
            this.btn_apply.Text = "Apply notes";
            this.btn_apply.UseVisualStyleBackColor = true;
            this.btn_apply.Click += new System.EventHandler(this.btn_apply_Click);
            // 
            // btn_discard
            // 
            this.btn_discard.Location = new System.Drawing.Point(197, 226);
            this.btn_discard.Name = "btn_discard";
            this.btn_discard.Size = new System.Drawing.Size(75, 23);
            this.btn_discard.TabIndex = 2;
            this.btn_discard.Text = "Discard";
            this.btn_discard.UseVisualStyleBackColor = true;
            // 
            // Notes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btn_discard);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.tb_notes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Notes";
            this.Text = "Notes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_notes;
        private System.Windows.Forms.Button btn_apply;
        private System.Windows.Forms.Button btn_discard;
    }
}