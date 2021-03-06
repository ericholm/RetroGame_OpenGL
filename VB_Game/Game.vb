﻿Imports System.ComponentModel
Imports System.Drawing
Imports System.Threading
Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
'''
''' Base game class which runs the whole game managing all screen and loading of game
'''
Public Class Game : Inherits GameWindow : Implements KeyListener

    Private Shared instance As Game
    Private camera As Camera
    Private maxDelta As Double = 0
    Public gameTime As Double = 0

    Private _currentScreen As Screen
    Public Property currentScreen() As Screen
        Get
            Return _currentScreen
        End Get
        Set(ByVal value As Screen)
            _currentScreen = value
            _currentScreen.onShow()
        End Set
    End Property

    Public Function getCamera() As Camera
        Return camera
    End Function

    Private Sub New()
        MyBase.New(Constants.WINDOW_START_WIDTH, Constants.WINDOW_START_HEIGHT,
                   New GraphicsMode(32, 0, 0, Constants.NUM_FSAA_SAMPLES))
        'Initialise OpenGL Settings
        GL.Enable(EnableCap.Texture2D)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(CType(BlendingFactorSrc.SrcAlpha, BlendingFactor), CType(BlendingFactorSrc.OneMinusSrcAlpha, BlendingFactor))
        InputHandler.init(Me)
        DebugHandler.init()
        InputHandler.keyListeners.Add(Me)
        PhysicsHandler.init()
        TileMapHandler.getInstance()
        camera = New Camera(New Vector2(0.5, 0.5), 0, 1)
        VSync = True
        currentScreen = StartScreen.getInstance()
        Me.Title = "Bounce"
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        MyBase.OnResize(e)
        _currentScreen.onResize()
    End Sub

    Protected Overrides Sub OnUpdateFrame(ByVal e As FrameEventArgs)
        If e.Time < Constants.MAX_FRAME_DELTA_TIME Then
            camera.update()
            _currentScreen.update(e.Time)
            DebugHandler.update(e.Time)
            gameTime += e.Time
            'maxDelta = Math.Max(e.Time, maxDelta)
            'Debug.WriteLine(maxDelta)
        End If
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As FrameEventArgs)
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.ClearColor(Color.Black)
        SpriteBatch.begin(Me.ClientSize.Width, Me.ClientSize.Height)
        camera.applyTransform()
        _currentScreen.render(e.Time)
        DebugHandler.render(e.Time)
        Me.SwapBuffers()
    End Sub

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)
    End Sub


    Public Overloads Sub Dispose()
        MyBase.Dispose()
        _currentScreen.dispose()
        AudioMaster.getInstance().Dispose()
    End Sub

    Public Shared Function getInstance() As Game
        If instance Is Nothing Then
            instance = New Game()
        End If
        Return instance
    End Function

    Private Sub KeyListener_KeyUp(e As KeyboardKeyEventArgs) Implements KeyListener.KeyUp
    End Sub

    Private Sub KeyListener_KeyDown(e As KeyboardKeyEventArgs) Implements KeyListener.KeyDown
        If Not Constants.RELEASE Then
            If e.Key = Key.Q Then
                Me.Close()
            End If

            If e.Key = Key.F Then
                If WindowState = OpenTK.WindowState.Fullscreen Then
                    WindowState = OpenTK.WindowState.Normal
                Else
                    WindowState = OpenTK.WindowState.Fullscreen
                End If
            End If

            If e.Key = Key.End Then

            End If
        End If
    End Sub
End Class
