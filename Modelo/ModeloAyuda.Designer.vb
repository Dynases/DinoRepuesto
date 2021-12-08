<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModeloAyuda
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.grJBuscador = New Janus.Windows.GridEX.GridEX()
        Me.GPPanelP = New DevComponents.DotNetBar.Controls.GroupPanel()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ButtonX1 = New DevComponents.DotNetBar.ButtonX()
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GPPanelP.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'grJBuscador
        '
        Me.grJBuscador.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.[False]
        Me.grJBuscador.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grJBuscador.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grJBuscador.HeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.grJBuscador.Location = New System.Drawing.Point(0, 0)
        Me.grJBuscador.Margin = New System.Windows.Forms.Padding(4)
        Me.grJBuscador.Name = "grJBuscador"
        Me.grJBuscador.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Custom
        Me.grJBuscador.Office2007CustomColor = System.Drawing.Color.DodgerBlue
        Me.grJBuscador.Size = New System.Drawing.Size(1020, 397)
        Me.grJBuscador.TabIndex = 0
        Me.grJBuscador.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007
        '
        'GPPanelP
        '
        Me.GPPanelP.BackColor = System.Drawing.Color.Transparent
        Me.GPPanelP.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2010
        Me.GPPanelP.Controls.Add(Me.grJBuscador)
        Me.GPPanelP.DisabledBackColor = System.Drawing.Color.Empty
        Me.GPPanelP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GPPanelP.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GPPanelP.Location = New System.Drawing.Point(0, 80)
        Me.GPPanelP.Margin = New System.Windows.Forms.Padding(4)
        Me.GPPanelP.Name = "GPPanelP"
        Me.GPPanelP.Size = New System.Drawing.Size(1026, 430)
        '
        '
        '
        Me.GPPanelP.Style.BackColor = System.Drawing.Color.DodgerBlue
        Me.GPPanelP.Style.BackColor2 = System.Drawing.Color.DodgerBlue
        Me.GPPanelP.Style.BackColorGradientAngle = 90
        Me.GPPanelP.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GPPanelP.Style.BorderBottomWidth = 1
        Me.GPPanelP.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarCaptionBackground
        Me.GPPanelP.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GPPanelP.Style.BorderLeftWidth = 1
        Me.GPPanelP.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GPPanelP.Style.BorderRightWidth = 1
        Me.GPPanelP.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GPPanelP.Style.BorderTopWidth = 1
        Me.GPPanelP.Style.CornerDiameter = 4
        Me.GPPanelP.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GPPanelP.Style.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GPPanelP.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GPPanelP.Style.TextColor = System.Drawing.Color.White
        Me.GPPanelP.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GPPanelP.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GPPanelP.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.GPPanelP.TabIndex = 1
        Me.GPPanelP.Text = "GroupPanel1"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.DodgerBlue
        Me.Panel1.Controls.Add(Me.ButtonX1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1026, 80)
        Me.Panel1.TabIndex = 2
        '
        'ButtonX1
        '
        Me.ButtonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.ButtonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.Office2007WithBackground
        Me.ButtonX1.Font = New System.Drawing.Font("Calibri", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonX1.Image = Global.Modelo.My.Resources.Resources._error
        Me.ButtonX1.ImageFixedSize = New System.Drawing.Size(35, 35)
        Me.ButtonX1.Location = New System.Drawing.Point(12, 21)
        Me.ButtonX1.Name = "ButtonX1"
        Me.ButtonX1.Size = New System.Drawing.Size(311, 52)
        Me.ButtonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeMobile2014
        Me.ButtonX1.TabIndex = 375
        Me.ButtonX1.Text = "Salir"
        '
        'ModeloAyuda
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(49, Byte), Integer), CType(CType(59, Byte), Integer), CType(CType(66, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1026, 510)
        Me.Controls.Add(Me.GPPanelP)
        Me.Controls.Add(Me.Panel1)
        Me.DoubleBuffered = True
        Me.ForeColor = System.Drawing.Color.Black
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.Name = "ModeloAyuda"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ModeloAyuda"
        CType(Me.grJBuscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GPPanelP.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GPPanelP As DevComponents.DotNetBar.Controls.GroupPanel
    Public WithEvents grJBuscador As Janus.Windows.GridEX.GridEX
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ButtonX1 As DevComponents.DotNetBar.ButtonX
End Class
