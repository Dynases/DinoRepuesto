<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F1_Traspaso
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim cbSucOrigen_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F1_Traspaso))
        Dim cbSucDestino_DesignTimeLayout As Janus.Windows.GridEX.GridEXLayout = New Janus.Windows.GridEX.GridEXLayout()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ReflectionLabel1 = New DevComponents.DotNetBar.Controls.ReflectionLabel()
        Me.btnAgregar = New DevComponents.DotNetBar.ButtonX()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        Me.cbSucOrigen = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX7 = New DevComponents.DotNetBar.LabelX()
        Me.cbSucDestino = New Janus.Windows.GridEX.EditControls.MultiColumnCombo()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.Panel1.SuspendLayout()
        CType(Me.cbSucOrigen, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.cbSucDestino, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = Global.DinoM.My.Resources.Resources.fondo1
        Me.Panel1.Controls.Add(Me.ReflectionLabel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(519, 55)
        Me.Panel1.TabIndex = 10
        '
        'ReflectionLabel1
        '
        Me.ReflectionLabel1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.ReflectionLabel1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ReflectionLabel1.Font = New System.Drawing.Font("Calibri", 16.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReflectionLabel1.ForeColor = System.Drawing.Color.White
        Me.ReflectionLabel1.Location = New System.Drawing.Point(9, 10)
        Me.ReflectionLabel1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.ReflectionLabel1.Name = "ReflectionLabel1"
        Me.ReflectionLabel1.Size = New System.Drawing.Size(278, 43)
        Me.ReflectionLabel1.TabIndex = 5
        Me.ReflectionLabel1.Text = "Traspaso"
        '
        'btnAgregar
        '
        Me.btnAgregar.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnAgregar.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.btnAgregar.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAgregar.Image = Global.DinoM.My.Resources.Resources.checked
        Me.btnAgregar.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.btnAgregar.Location = New System.Drawing.Point(154, 190)
        Me.btnAgregar.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnAgregar.Name = "btnAgregar"
        Me.btnAgregar.Size = New System.Drawing.Size(96, 42)
        Me.btnAgregar.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.btnAgregar.TabIndex = 373
        Me.btnAgregar.Text = "Confirmar"
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.DinoM.My.Resources.Resources.cancel
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(30, 30)
        Me.ButtonX1.Location = New System.Drawing.Point(281, 190)
        Me.ButtonX1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(96, 42)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX1.TabIndex = 374
        Me.ButtonX1.Text = "Salir"
        '
        'cbSucOrigen
        '
        cbSucOrigen_DesignTimeLayout.LayoutString = resources.GetString("cbSucOrigen_DesignTimeLayout.LayoutString")
        Me.cbSucOrigen.DesignTimeLayout = cbSucOrigen_DesignTimeLayout
        Me.cbSucOrigen.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbSucOrigen.Location = New System.Drawing.Point(232, 99)
        Me.cbSucOrigen.Name = "cbSucOrigen"
        Me.cbSucOrigen.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbSucOrigen.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.cbSucOrigen.ReadOnly = True
        Me.cbSucOrigen.SelectedIndex = -1
        Me.cbSucOrigen.SelectedItem = Nothing
        Me.cbSucOrigen.Size = New System.Drawing.Size(202, 22)
        Me.cbSucOrigen.TabIndex = 659
        Me.cbSucOrigen.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX7
        '
        Me.LabelX7.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX7.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX7.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX7.Location = New System.Drawing.Point(97, 98)
        Me.LabelX7.Name = "LabelX7"
        Me.LabelX7.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX7.Size = New System.Drawing.Size(111, 23)
        Me.LabelX7.TabIndex = 658
        Me.LabelX7.Text = "Sucursal Origen:"
        '
        'cbSucDestino
        '
        cbSucDestino_DesignTimeLayout.LayoutString = resources.GetString("cbSucDestino_DesignTimeLayout.LayoutString")
        Me.cbSucDestino.DesignTimeLayout = cbSucDestino_DesignTimeLayout
        Me.cbSucDestino.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cbSucDestino.Location = New System.Drawing.Point(232, 138)
        Me.cbSucDestino.Name = "cbSucDestino"
        Me.cbSucDestino.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.cbSucDestino.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.cbSucDestino.SelectedIndex = -1
        Me.cbSucDestino.SelectedItem = Nothing
        Me.cbSucDestino.Size = New System.Drawing.Size(202, 22)
        Me.cbSucDestino.TabIndex = 661
        Me.cbSucDestino.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'LabelX1
        '
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(97, 137)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(111, 23)
        Me.LabelX1.TabIndex = 660
        Me.LabelX1.Text = "Sucursal Destino:"
        '
        'F1_Traspaso
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(519, 286)
        Me.Controls.Add(Me.cbSucDestino)
        Me.Controls.Add(Me.LabelX1)
        Me.Controls.Add(Me.cbSucOrigen)
        Me.Controls.Add(Me.LabelX7)
        Me.Controls.Add(Me.ButtonX1)
        Me.Controls.Add(Me.btnAgregar)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "F1_Traspaso"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F1_Cantidad"
        Me.Panel1.ResumeLayout(False)
        CType(Me.cbSucOrigen, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.cbSucDestino, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents ReflectionLabel1 As DevComponents.DotNetBar.Controls.ReflectionLabel
    Friend WithEvents btnAgregar As DevComponents.DotNetBar.ButtonX
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
    Friend WithEvents cbSucOrigen As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX7 As DevComponents.DotNetBar.LabelX
    Friend WithEvents cbSucDestino As Janus.Windows.GridEX.EditControls.MultiColumnCombo
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
End Class
