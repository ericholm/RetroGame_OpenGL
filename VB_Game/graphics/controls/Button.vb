﻿Imports OpenTK
Imports OpenTK.Input
Imports VB_Game

''' <summary>
''' Represents a single clickable button on screen
''' </summary>
Public Class Button : Inherits Control : Implements MouseListener

    Private clickListener As OnClick
    Private label As TextLabel
    Private border As ShapeTexture
    Private highlightOverlay As ShapeTexture
    Private hoveredOver As Boolean = False
    Private size As Vector2
    Private style As ButtonStyle
    Private backPos As Vector2

    ''' <summary>
    ''' Creates a new button
    ''' </summary>
    ''' <param name="text">Button display text</param>
    ''' <param name="pos">Initial position</param>
    ''' <param name="size">Overall button dimensions</param>
    ''' <param name="style">Button Appearance style</param>
    Public Sub New(text As String, pos As Vector2, size As Drawing.Size, style As ButtonStyle)
        Me.style = style
        Me.size = New Vector2(size.Width, size.Height)
        Me.pos = pos
        Me.backPos = New Vector2(pos.X + style.borderSize, pos.Y + style.borderSize)
        Dim compatSize = New Drawing.Size(size.Width - (style.borderSize * 2), size.Height - (style.borderSize * 2))
        label = New TextLabel(text, compatSize, style.textColor)
        label.pos = New Vector2(pos.X + size.Width / 2 - label.getWidth() / 2, pos.Y + size.Height / 2 - label.getHeight() / 2)
        highlightOverlay = New ShapeTexture(compatSize.Width, compatSize.Height,
            Drawing.Color.FromArgb(50, 255, 255, 255), ShapeTexture.ShapeType.Rectangle)

        Me.texture = New ShapeTexture(size.Width - (style.borderSize * 2), size.Height - (style.borderSize * 2),
            style.backColor, ShapeTexture.ShapeType.Rectangle)
        'label.pos = New Vector2(pos.X + style.borderSize + (size.Width - label.texture.width) / 2,
        '                        pos.Y + style.borderSize + (size.Height - label.texture.height) / 2)
        InputHandler.mouseListeners.Add(Me)

        'Handle borders
        If Not IsNothing(style.borderColor) Then
            border = New ShapeTexture(size.Width, size.Height,
                   style.borderColor, ShapeTexture.ShapeType.Rectangle)
        End If
        Me.customRender = True
    End Sub

    ''' <summary>
    ''' Applies padding in case text gets cut off
    ''' </summary>
    Public Sub applyTextPadding(padding As Integer)
        Me.label.applyTextPadding(padding)
    End Sub

    Public Sub New(text As String, pos As Vector2, font As Drawing.Font, size As Drawing.Size, style As ButtonStyle)
        Me.style = style
        Me.size = New Vector2(size.Width, size.Height)
        Me.pos = pos
        Me.backPos = New Vector2(pos.X + style.borderSize, pos.Y + style.borderSize)
        Dim compatSize = New Drawing.Size(size.Width - (style.borderSize * 2), size.Height - (style.borderSize * 2))
        label = New TextLabel(text, font, style.textColor)
        label.pos = New Vector2(pos.X + size.Width / 2 - label.getWidth() / 2, pos.Y + size.Height / 2 - label.getHeight() / 2)
        highlightOverlay = New ShapeTexture(compatSize.Width, compatSize.Height,
            Drawing.Color.FromArgb(50, 255, 255, 255), ShapeTexture.ShapeType.Rectangle)

        Me.texture = New ShapeTexture(size.Width - (style.borderSize * 2), size.Height - (style.borderSize * 2),
            style.backColor, ShapeTexture.ShapeType.Rectangle)
        'label.pos = New Vector2(pos.X + style.borderSize + (size.Width - label.texture.width) / 2,
        '                        pos.Y + style.borderSize + (size.Height - label.texture.height) / 2)
        InputHandler.mouseListeners.Add(Me)

        'Handle borders
        If Not IsNothing(style.borderColor) Then
            border = New ShapeTexture(size.Width, size.Height,
                   style.borderColor, ShapeTexture.ShapeType.Rectangle)
        End If
    End Sub

    Public Overrides Sub render(delta As Double)
        MyBase.render(delta)
        If Not border Is Nothing Then
            SpriteBatch.drawTexture(border, pos)
        End If
        SpriteBatch.drawTexture(Me.texture, backPos)
        If hoveredOver Then
            SpriteBatch.drawTexture(highlightOverlay, backPos)
        End If
        label.render(delta)
    End Sub

    Delegate Sub OnClick()

    ''' <summary>
    ''' Sets a click listener which is called when the button is clicked
    ''' </summary>
    ''' <param name="listener"></param>
    Public Sub setOnClickListener(listener As OnClick)
        Me.clickListener = listener
    End Sub

    Public Sub MouseButtonUp(e As MouseEventArgs) Implements MouseListener.MouseButtonUp
    End Sub

    Public Sub MouseScroll(e As MouseWheelEventArgs) Implements MouseListener.MouseScroll
    End Sub

    Public Sub MouseButtonDown(e As MouseEventArgs) Implements MouseListener.MouseButtonDown
        If hoveredOver And Visible And Not clickListener Is Nothing Then
            SoundEffects.menu_navigate.play()
            clickListener.Invoke()
        End If
    End Sub

    Public Sub MouseMove(e As MouseMoveEventArgs) Implements MouseListener.MouseMove
        hoveredOver = PhysicUtils.pointWithin(SpriteBatch.normaliseScreenCoords(e.X, e.Y),
                                   New BoundingRect(Me.size, Me.pos))
    End Sub
End Class
