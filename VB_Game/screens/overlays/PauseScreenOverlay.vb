﻿Public Class PauseScreenOverlay

    Private overlayBackground As ShapeTexture
    Private pos As OpenTK.Vector2
    Private titleLabel As TextLabel
    Private paddingY As Integer = 24 * Constants.DESIGN_SCALE_FACTOR

    Private resumeBtn As Button
    Private settingsBtn As Button
    Private quitBtn As Button
    Private btnStyle As New ButtonStyle(Drawing.Brushes.White, Drawing.Color.FromArgb(255, 64, 64, 64))
    Private btnFont As New Drawing.Font("Impact", 26 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular)
    Private confirmDialog As Dialog = New Dialog({"Are you sure you want to quit?"})
    Private showDialog As Boolean = False

    Public Sub New()
        pos = New OpenTK.Vector2(-Constants.DESIGN_WIDTH / 2, -Constants.DESIGN_HEIGHT / 2)
        overlayBackground = New ShapeTexture(Constants.DESIGN_WIDTH, Constants.DESIGN_HEIGHT,
            Drawing.Color.FromArgb(200, 0, 0, 0), ShapeTexture.ShapeType.Rectangle)

        titleLabel = New TextLabel("Game Paused", New Drawing.Font("Impact", 40 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular),
                                  Drawing.Brushes.White)
        titleLabel.pos = New OpenTK.Vector2(-titleLabel.getWidth() / 2, pos.Y + paddingY)

        Dim btnSize As New Drawing.Size(200 * Constants.DESIGN_SCALE_FACTOR, 50 * Constants.DESIGN_SCALE_FACTOR)

        resumeBtn = New Button("RESUME", New OpenTK.Vector2(-btnSize.Width / 2,
                titleLabel.pos.Y + titleLabel.getHeight() + paddingY * 2), btnFont, btnSize, btnStyle)

        settingsBtn = New Button("SETTINGS", New OpenTK.Vector2(-btnSize.Width / 2,
                resumeBtn.pos.Y + btnSize.Height + paddingY * 2), btnFont, btnSize, btnStyle)

        quitBtn = New Button("QUIT", New OpenTK.Vector2(-btnSize.Width / 2,
                settingsBtn.pos.Y + btnSize.Height + paddingY * 2), btnFont, btnSize, btnStyle)
        resumeBtn.setOnClickListener(AddressOf onResumeClicked)
        settingsBtn.setOnClickListener(AddressOf onSettingsClicked)
        quitBtn.setOnClickListener(AddressOf onQuitClicked)
        confirmDialog.registerResultListeners(AddressOf dialogOk, AddressOf dialogCancel)
    End Sub

    Private Sub onSettingsClicked()
        GameScreen.getInstance().CurrentState = GameScreen.State.SETTINGS
    End Sub

    Private Sub dialogOk()
        showDialog = False
        GameScreen.getInstance().restart()
        Game.getInstance().currentScreen = StartScreen.getInstance()
    End Sub

    Private Sub dialogCancel()
        showDialog = False
    End Sub

    Private Sub onQuitClicked()
        'Dim result = MsgBox("Are you sure you want to quit?", MsgBoxStyle.YesNoCancel)
        'If result = MsgBoxResult.Yes Then
        '    GameScreen.getInstance().restart()
        '    Game.getInstance().currentScreen = StartScreen.getInstance()
        'End If
        confirmDialog.show()
        showDialog = True
    End Sub

    Private Sub onResumeClicked()
        GameScreen.getInstance().CurrentState = GameScreen.State.PLAY
    End Sub

    Public Sub render(delta)
        SpriteBatch.drawTexture(overlayBackground, pos)
        titleLabel.render(delta)
        resumeBtn.render(delta)
        settingsBtn.render(delta)
        quitBtn.render(delta)
        If showDialog Then
            confirmDialog.render(delta)
        End If
    End Sub

    Public Sub tick(delta)
    End Sub


End Class
