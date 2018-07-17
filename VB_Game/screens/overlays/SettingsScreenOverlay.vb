﻿Public Class SettingsScreenOverlay

    Delegate Sub onBackDelegate()
    Private overlayBackground As ShapeTexture
    Private pos As OpenTK.Vector2
    Private titleLabel As TextLabel
    Private Const paddingY As Integer = 32
    Private onBack As onBackDelegate
    Private backBtn As Button
    Private btnStyle As New ButtonStyle(Drawing.Brushes.White, Drawing.Color.FromArgb(255, 64, 64, 64))
    Private mainFont As New Drawing.Font("Impact", 24 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular)
    Private renderOverlay As Boolean = True
    Private volumeSlider As Slider
    Private volumeLabel As TextLabel
    Private volumeTitleLabel As TextLabel
    Dim controlSize As New OpenTK.Vector2(280 * Constants.DESIGN_SCALE_FACTOR, 44 * Constants.DESIGN_SCALE_FACTOR)
    Private graphicsSwitch As Switch
    Private graphicsQualityTitleLabel As TextLabel
    Private applyChangesBtn As Button
    Private windowModeSwitch As Switch
    Private windowModeLabel As TextLabel

    'Currently chosen settings - not necessarily saved yet

    Private initialGraphicsQuality As String
    Private initVolumeLevel As Integer
    Private volumeLevel As Integer
    Private graphicsQuality As String
    Private windowMode As String
    Private initialWindowMode As String

    Private Function hasSettingsChanged() As Boolean
        Return graphicsQuality <> initialGraphicsQuality Or initVolumeLevel <> volumeLevel Or windowMode <> initialWindowMode
    End Function

    Public Sub New()
        pos = New OpenTK.Vector2(-Constants.DESIGN_WIDTH / 2, -Constants.DESIGN_HEIGHT / 2)
        overlayBackground = New ShapeTexture(Constants.DESIGN_WIDTH, Constants.DESIGN_HEIGHT,
            Drawing.Color.FromArgb(200, 0, 0, 0), ShapeTexture.ShapeType.Rectangle)

        titleLabel = New TextLabel("Settings", New Drawing.Font("Impact", 32 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular),
                                  Drawing.Brushes.White)
        titleLabel.pos = New OpenTK.Vector2(-titleLabel.getWidth() / 2, pos.Y + paddingY)

        Dim btnSize As New Drawing.Size(250 * Constants.DESIGN_SCALE_FACTOR, 40 * Constants.DESIGN_SCALE_FACTOR)

        backBtn = New Button("BACK", New OpenTK.Vector2(-btnSize.Width / 2,
                Constants.DESIGN_HEIGHT / 2 - btnSize.Height * 1.5), mainFont, btnSize, btnStyle)
        backBtn.setOnClickListener(AddressOf onBackClicked)

        applyChangesBtn = New Button("APPLY", New OpenTK.Vector2(backBtn.pos.X, backBtn.pos.Y - backBtn.getHeight() * 1.5),
                                     mainFont, btnSize, btnStyle)
        applyChangesBtn.setOnClickListener(AddressOf applySettingsChanges)

        'Volume settings controls
        volumeSlider = New Slider(New OpenTK.Vector2(-controlSize.X / 5,
                                                     titleLabel.pos.Y + titleLabel.getHeight() * 3), controlSize)
        volumeSlider.setOnValueChangeListener(AddressOf onVolumeChanged)
        volumeLabel = New TextLabel("0", mainFont, Drawing.Brushes.White)
        volumeLabel.pos = New OpenTK.Vector2(volumeSlider.pos.X + volumeSlider.getWidth() + volumeLabel.getWidth() / 4,
                                             volumeSlider.pos.Y)
        volumeTitleLabel = New TextLabel("Volume", mainFont, Drawing.Brushes.White)
        volumeTitleLabel.pos = New OpenTK.Vector2(volumeSlider.pos.X - volumeSlider.getWidth() / 8 - volumeTitleLabel.getWidth(),
                                             volumeSlider.pos.Y)

        'Configure Graphics Settings
        graphicsSwitch = New Switch(New OpenTK.Vector2(volumeSlider.pos.X, volumeSlider.pos.Y + controlSize.Y * 1.5),
                                    controlSize, Constants.GRAPHICS_PRESETS.Keys.ToArray())
        graphicsSwitch.setOnValueChangeListener(AddressOf onGraphicsQualityChanged)

        graphicsQualityTitleLabel = New TextLabel("Graphics Quality", mainFont, Drawing.Brushes.White)
        graphicsQualityTitleLabel.pos = New OpenTK.Vector2(graphicsSwitch.pos.X - graphicsSwitch.getWidth() / 8 - graphicsQualityTitleLabel.getWidth(),
                                             graphicsSwitch.pos.Y)

        windowModeSwitch = New Switch(New OpenTK.Vector2(volumeSlider.pos.X, volumeSlider.pos.Y + controlSize.Y * 3),
                                    controlSize, {"FULLSCREEN", "WINDOWED"}, New SwitchStyle(Drawing.Color.FromArgb(255, 64, 64, 64),
                                                  Drawing.Color.FromArgb(255, 112, 112, 112),
                                                  Drawing.Color.FromArgb(32, 255, 255, 255),
                                                  15 * Constants.DESIGN_SCALE_FACTOR))
        windowModeSwitch.setOnValueChangeListener(AddressOf onWindowModeChanged)

        windowModeLabel = New TextLabel("Window Mode", mainFont, Drawing.Brushes.White)
        windowModeLabel.pos = New OpenTK.Vector2(windowModeSwitch.pos.X - windowModeSwitch.getWidth() / 8 - windowModeLabel.getWidth(),
                                             windowModeSwitch.pos.Y)
    End Sub

    Public Sub applySettingsChanges()
        Dim msgResult = Nothing
        If graphicsQuality <> initialGraphicsQuality Then
            msgResult = MsgBox("Are you sure you want to apply changes?
Changing graphic options requires a restart and any ongoing game progress will be lost",
                               MsgBoxStyle.OkCancel, "Apply Changes")
        End If

        AudioMaster.getInstance().setVolume(volumeLevel)
        initVolumeLevel = volumeLevel

        If windowMode.ToLower() = "windowed" Then
            Game.getInstance().WindowState = OpenTK.WindowState.Normal
        ElseIf windowMode.ToLower() = "fullscreen" Then
            Game.getInstance().WindowState = OpenTK.WindowState.Fullscreen
        End If

        initialWindowMode = windowMode.ToLower()

        If msgResult = MsgBoxResult.Ok Then
            Constants.CURRENT_GRAPHICS_PRESET = graphicsQuality
            Constants.saveSettings()
            System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName)
            Environment.Exit(0)
        End If
        Constants.saveSettings()
    End Sub

    Public Sub loadValuesFromSettings()
        Debug.WriteLine("loading")
        volumeSlider.Value = AudioMaster.getInstance().getVolume()
        volumeLevel = volumeSlider.Value
        initVolumeLevel = volumeSlider.Value
        graphicsSwitch.setSelectedIndex(Array.IndexOf(Constants.GRAPHICS_PRESETS.Keys.ToArray(),
                                              Constants.CURRENT_GRAPHICS_PRESET), False)
        initialGraphicsQuality = graphicsSwitch.getSelectedValue()
        graphicsQuality = initialGraphicsQuality

        If Game.getInstance().WindowState = OpenTK.WindowState.Normal Then
            initialWindowMode = "windowed"
            windowModeSwitch.setSelectedIndex(1, False)
        ElseIf Game.getInstance().WindowState = OpenTK.WindowState.Fullscreen Then
            initialWindowMode = "fullscreen"
            windowModeSwitch.setSelectedIndex(0, False)
        End If
        windowMode = initialWindowMode
    End Sub

    Public Sub load()
        loadValuesFromSettings()
    End Sub

    Public Sub onGraphicsQualityChanged(val As Object)
        graphicsQuality = CStr(val)
    End Sub

    Public Sub onWindowModeChanged(val As Object)
        windowMode = CStr(val)
    End Sub


    Public Sub onVolumeChanged(val As Object)
        volumeLabel.Text = CStr(val)
        volumeLevel = CInt(val)
    End Sub

    Public Sub New(renderOverlay As Boolean)
        Me.New()
        Me.renderOverlay = renderOverlay
    End Sub

    Public Sub setOnBackAction(backAction As onBackDelegate)
        onBack = backAction
    End Sub

    Private Sub onBackClicked()
        If Not onBack Is Nothing Then
            onBack.Invoke()
        End If
    End Sub

    Public Sub render(delta)
        If renderOverlay Then
            SpriteBatch.drawTexture(overlayBackground, pos)
        End If
        titleLabel.render(delta)
        backBtn.render(delta)
        volumeSlider.render(delta)
        volumeLabel.render(delta)
        volumeTitleLabel.render(delta)
        graphicsSwitch.render(delta)
        graphicsQualityTitleLabel.render(delta)
        windowModeSwitch.render(delta)
        windowModeLabel.render(delta)
        If hasSettingsChanged() Then
            applyChangesBtn.render(delta)
        End If
    End Sub

    Public Sub tick(delta)
    End Sub

End Class
