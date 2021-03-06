﻿Imports System.IO
Imports OpenTK.Audio
Imports OpenTK.Audio.OpenAL
Imports OpenTK.Input
Imports VB_Game

''' <summary>
''' Manages all game audio and the audio context
''' </summary>
Public Class AudioMaster : Implements IDisposable

    Private buffers As List(Of Integer)
    Private context As AudioContext
    Private sources As New List(Of Source)
    Private Shared instance As AudioMaster
    Private enabled As Boolean
    Private volumeLevel As Double

    Public Function isEnabled() As Boolean
        'Return enabled
        Return True
    End Function

    ''' <summary>
    ''' Sets volume as a range between 0 and 100
    ''' </summary>
    ''' <param name="level"></param>
    Public Sub setVolume(level As Integer)
        volumeLevel = (level / 100) 'gain expects value from 0 to 1
        'For i = 0 To sources.Count - 1
        '    sources(i).gain = volumeLevel
        'Next
        'Pass volume over to naudio
        SoundEffects.setVolume(volumeLevel)
    End Sub

    ''' <summary>
    ''' Gets volume as a range between 0 and 100
    ''' </summary>
    ''' <returns></returns>
    Public Function getVolume() As Integer
        Return CInt(volumeLevel * 100)
    End Function

    Public Sub New()
        Try
            context = New AudioContext()
            context.MakeCurrent()
            setListenerData()
            enabled = True
        Catch ex As Exception
            enabled = False
            'Audio disabled
        End Try

        'Dim buffer = ContentPipe.loadWave(".\res\test.wav")
        'Dim buffer2 = ContentPipe.loadWave(".\res\test2.wav")
        'source = New Source()
        'source.shouldLoop = True
        'Dim source2 As New Source()
        'source2.shouldLoop = True

        'source.play(buffer)
        'source2.play(buffer2)
    End Sub

    Public Shared Function getInstance() As AudioMaster
        If instance Is Nothing Then
            instance = New AudioMaster()
        End If
        Return instance
    End Function

    Public Sub setListenerData()
        AL.Listener(ALListener3f.Position, 0, 0, 0)
        AL.Listener(ALListener3f.Velocity, 0, 0, 0)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        context.Dispose()
    End Sub
End Class
