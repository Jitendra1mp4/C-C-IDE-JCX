﻿Public Class fileManipulator

    Public Saved As Boolean = False
    Public filePath As String = My.MySettings.Default.lastOpenedFileName
    Public fileName As String
    Private Const fileListFilter As String = "C source files (*.c)|*.c|C++ source files (*.cpp)|*.cpp|All files (*.*)|*.*"
    Private _editor As Editor

    Sub New(ByRef editor As Editor)
        ' TODO: Complete member initialization 
        _editor = editor
    End Sub

    Public Function getFileName() As String
        Return filePath.Substring(filePath.LastIndexOf("\") + 1)
    End Function

    Public Function getFileExtention() As String
        Return filePath.Substring(filePath.LastIndexOf(".") + 1)
    End Function

    Private Function saveFile() As Boolean
        _editor.CodeBox.SaveFile(filePath, RichTextBoxStreamType.PlainText)
        saveFile = True
    End Function

    Public Sub callSaveAs()
        _editor.SaveFileDialog1.Filter = fileListFilter
        If _editor.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK _
            Then
            filePath = _editor.SaveFileDialog1.FileName
        End If
        Saved = saveFile()
        My.MySettings.Default.lastOpenedFileName = filePath
        _editor.Text = _editor.appName + " - " + CStr(getFileName())

        _editor.SAVEToolStripMenuItem1.Text = "SAVE"
        _editor.EventMessage.Text = "Saved as " + CStr(getFileName())

    End Sub

    Public Function saver() As Boolean
        If (filePath = "\0") Then 'if file path is not set then open save file dailog
            _editor.SaveFileDialog1.Filter = fileListFilter
            If _editor.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK _
                Then
                filePath = _editor.SaveFileDialog1.FileName
            Else
                Saved = False
                Return False
            End If
        End If
        Saved = saveFile()
        If (Saved) Then
            fileName = CStr(getFileName())
            _editor.Text = _editor.appName + " - " + fileName
            My.MySettings.Default.lastOpenedFileName = filePath

            _editor.SAVEToolStripMenuItem1.Text = "SAVE"
            _editor.EventMessage.Text = fileName + " Saved!"
            _editor.codeChanged = False
            Return True
        End If
        Return False
    End Function

    Public Function setCodeBoxText(ByVal path As String) As Boolean
        If (My.Computer.FileSystem.FileExists(path)) Then
            _editor.CodeBox.LoadFile(path, RichTextBoxStreamType.PlainText) 'Load text to codeBox
            filePath = path
            Saved = True                       'set saved = true
            fileName = CStr(getFileName())    'set file name
            _editor.Text = _editor.appName + " - " + fileName
            _editor.SAVEToolStripMenuItem1.Text = "SAVE"
            _editor.EventMessage.Text = "Sucessfully Opened file " + fileName
            _editor.codeChanged = False
            setCodeBoxText = True
            If getFileExtention() = "c" Or getFileExtention() = "C" Then
                _editor.Executer.languageMD = CodeExecuters.languageMode.C
            Else
                _editor.Executer.languageMD = CodeExecuters.languageMode.Cpp
            End If

        Else
            filePath = "\0"
            _editor.EventMessage.Text = "Failed to open last opened file"
            setCodeBoxText = False
        End If
        My.MySettings.Default.lastOpenedFileName = path 'save file path for next time
    End Function

    Public Function openFile() As Boolean
        _editor.OpenFileDialog1.Filter = fileListFilter
        If _editor.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK _
            Then
            filePath = _editor.OpenFileDialog1.FileName 'get file path
            Return setCodeBoxText(filePath)
        Else
            Return False
        End If
    End Function

End Class
