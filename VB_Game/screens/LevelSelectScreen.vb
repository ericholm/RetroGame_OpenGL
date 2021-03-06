﻿Imports OpenTK

Public Class LevelSelectScreen : Inherits Screen

    Private Shared instance As LevelSelectScreen
    Private backgroundImg As GameObject
    Private backgroundFilter As ShapeTexture
    Private btnFont As New Drawing.Font("Impact", 26 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular)
    Private titleLabel As TextLabel
    Private btnStyle = New ButtonStyle(Drawing.Brushes.White, Drawing.Color.FromArgb(255, 64, 64, 64))

    Private mapPreviewView As Control
    Private mapPreviewSize As New Vector2(290 * Constants.DESIGN_SCALE_FACTOR, 215 * Constants.DESIGN_SCALE_FACTOR)
    Private mapNameLabel As TextLabel
    Private highScoreLabel As TextLabel

    Private currentMapChosenIndex As Integer = 0
    Private currentDisplayedMap As Map

    Private leftBtn As Button
    Private rightBtn As Button
    Private startGameBtn As Button
    Private backBtn As Button
    Private mainFont As New Drawing.Font("Impact", 24 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular)

    'Should show map selection nav buttons
    Private showLeft As Boolean = True
    Private showRight As Boolean = True

    Public Sub New()
        'Background image
        backgroundImg = New GameObject(False)
        backgroundImg.texture = ContentPipe.loadTexture(Constants.TILE_RES_DIR + "startscreen_background.png")
        backgroundImg.pos = Constants.TOP_LEFT_COORD
        backgroundImg.scale = New Vector2(Constants.DESIGN_WIDTH / backgroundImg.texture.width)
        backgroundFilter = New ShapeTexture(Constants.DESIGN_WIDTH, Constants.DESIGN_HEIGHT,
                                            Drawing.Color.FromArgb(68, 0, 0, 0), ShapeTexture.ShapeType.Rectangle)

        'Title
        titleLabel = New TextLabel("Select Level", New Drawing.Font("IMPACT",
                30 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Bold), Drawing.Brushes.White)

        titleLabel.pos = New Vector2(-titleLabel.getWidth() / 2,
                                     -Constants.DESIGN_HEIGHT / 2 + titleLabel.getHeight() / 15)

        mapPreviewView = New Control()
        mapPreviewView.pos = New Vector2(-mapPreviewSize.X / 2, titleLabel.pos.Y + titleLabel.getHeight() * 2)
        mapNameLabel = New TextLabel("Map Name", New Drawing.Font("IMPACT",
                22 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular), Drawing.Brushes.White)
        mapNameLabel.pos = New Vector2(-mapNameLabel.getWidth() / 2,
                                     mapPreviewView.pos.Y - mapNameLabel.getHeight() * 1.05)

        highScoreLabel = New TextLabel("Highscore: ", New Drawing.Font("IMPACT",
                24 * Constants.DESIGN_SCALE_FACTOR, Drawing.FontStyle.Regular), Drawing.Brushes.White)
        highScoreLabel.pos = New Vector2(-highScoreLabel.getWidth() / 2,
                                     mapPreviewView.pos.Y + mapPreviewSize.Y + highScoreLabel.getHeight() / 2)

        Dim btnSize As New Drawing.Size(250 * Constants.DESIGN_SCALE_FACTOR, 45 * Constants.DESIGN_SCALE_FACTOR)

        backBtn = New Button("BACK", New OpenTK.Vector2(-btnSize.Width / 2,
                Constants.DESIGN_HEIGHT / 2 - btnSize.Height * 1.5), mainFont, btnSize, btnStyle)
        backBtn.setOnClickListener(AddressOf onBackClicked)

        startGameBtn = New Button("START GAME", New OpenTK.Vector2(-btnSize.Width / 2,
                backBtn.pos.Y - btnSize.Height * 1.5), mainFont, btnSize, btnStyle)
        startGameBtn.setOnClickListener(AddressOf onStartClicked)
        Dim navBtnSize = New Drawing.Size(30 * Constants.DESIGN_SCALE_FACTOR, 40 * Constants.DESIGN_SCALE_FACTOR)
        Dim navBtnY = mapPreviewView.pos.Y + mapPreviewSize.Y / 2 - navBtnSize.Width / 2
        leftBtn = New Button("<", New Vector2(mapPreviewView.pos.X - navBtnSize.Width * 1.3, navBtnY), navBtnSize, btnStyle)
        rightBtn = New Button(">", New Vector2(mapPreviewView.pos.X + mapPreviewSize.X + navBtnSize.Width * 0.3,
                                               navBtnY), navBtnSize, btnStyle)
        leftBtn.setOnClickListener(AddressOf navLeft)
        rightBtn.setOnClickListener(AddressOf navRight)

        If currentMapChosenIndex = 0 Then
            'Disable left nav button
            showLeft = False
        End If

        If currentMapChosenIndex = TileMapHandler.getInstance().maps.Count - 1 Then
            'Disable right nav button
            showRight = False
        End If

        displayMap()
        'mapPreviewView.texture
    End Sub

    'Show map on the left
    Public Sub navLeft()
        currentMapChosenIndex -= 1
        displayMap()
        If currentMapChosenIndex = 0 Then
            'Disable left nav button
            showLeft = False
        End If
        showRight = True
    End Sub

    'Show map on the right
    Public Sub navRight()
        currentMapChosenIndex += 1
        displayMap()
        If currentMapChosenIndex = TileMapHandler.getInstance().maps.Count - 1 Then
            'Disable right nav button
            showRight = False
        End If
        showLeft = True
    End Sub

    Public Sub displayMap()
        Me.currentDisplayedMap = TileMapHandler.getInstance().maps(currentMapChosenIndex)
        mapPreviewView.texture = currentDisplayedMap.getPreviewImg()
        mapPreviewView.scale = New Vector2(mapPreviewSize.X / currentDisplayedMap.getPreviewImg().width,
                                           mapPreviewSize.Y / currentDisplayedMap.getPreviewImg().height)
        mapNameLabel.Text = currentDisplayedMap.getName()
        mapNameLabel.pos = New Vector2(-mapNameLabel.getWidth() / 2, mapNameLabel.pos.Y)
        highScoreLabel.Text = "Highscore: " + CStr(currentDisplayedMap.Highscore)
        highScoreLabel.pos = New Vector2(-highScoreLabel.getWidth() / 2, highScoreLabel.pos.Y)
    End Sub

    Public Overrides Sub render(delta As Double)
        backgroundImg.render(delta)
        SpriteBatch.drawTexture(backgroundFilter, Constants.TOP_LEFT_COORD)
        titleLabel.render(delta)
        mapPreviewView.render(delta)
        mapNameLabel.render(delta)
        highScoreLabel.render(delta)
        startGameBtn.render(delta)
        backBtn.render(delta)
        If showLeft Then
            leftBtn.render(delta)
        End If

        If showRight Then
            rightBtn.render(delta)
        End If
    End Sub

    Public Overrides Sub update(delta As Double)
    End Sub

    Private Sub onBackClicked()
        Game.getInstance().currentScreen = StartScreen.getInstance()
    End Sub

    Private Sub onStartClicked()
        'If map is classic dont enable enemy fall direction randomisation change
        If currentMapChosenIndex = 0 Then
            Constants.RANDOMISE_ENEMY_DIR = False
        Else
            Constants.RANDOMISE_ENEMY_DIR = True
        End If
        TileMapHandler.getInstance().loadMap(currentMapChosenIndex)
        GameScreen.getInstance().configureNormal()
        Game.getInstance().currentScreen = GameScreen.getInstance()
    End Sub


    Public Overrides Sub dispose()
    End Sub

    Public Overrides Sub onResize()
    End Sub

    Public Shared Function getInstance() As LevelSelectScreen
        If instance Is Nothing Then
            instance = New LevelSelectScreen()
        End If
        Return instance
    End Function

    Public Overrides Sub onShow()
        highScoreLabel.Text = "Highscore: " + CStr(currentDisplayedMap.Highscore)
    End Sub
End Class
