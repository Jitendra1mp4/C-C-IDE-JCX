﻿Imports System
Imports ChikMiki.fileManipulator
Imports ChikMiki.CodeExecuters
Imports ChikMiki.EditMenu
Imports ChikMiki.MyUtilities
Imports ChikMiki.Theme
Imports ChikMiki.selectLangDialog

Public Class Editor

    Public Const appName As String = "ChikMiki"
    Public codeChanged As Boolean = False
    Dim tempCode As String
    Dim codeBeautified As Boolean = False
    Dim tempCodeBueatified As String

    Dim themer As New Theme(Me)
    Dim Mfile As New fileManipulator(Me)

    Public Executer As New CodeExecuters(Me)
    'Public Executer As New CodeExecuters()
    Dim LangDialog As selectLangDialog()
    '*********************
    Dim edtMenu As New EditMenu(Me)
    '*******************
    Public Sub callCodeRunner(ByVal compileOnly As Boolean)
        If Mfile.filePath <> "\0" Then
            Mfile.saver()
        End If
        Executer.codeRunner(CodeBox, Mfile.filePath, compileOnly)
    End Sub

    '**************************Auto-created functions***********************************'

    Private Sub WorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WorToolStripMenuItem.Click
        If CodeBox.WordWrap Then
            CodeBox.WordWrap = False
            EventMessage.Text = "Word Wrap Disabled"
        Else
            CodeBox.WordWrap = True
            EventMessage.Text = "Word Wrap Enabled"
        End If
    End Sub


    Private Sub CodeBox_FontChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeBox.FontChanged
        '****************For line number
        lineNumberBox.Font = CodeBox.Font

    End Sub


    Private Sub CodeBox_SelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeBox.SelectionChanged
        Dim pt As Point = CodeBox.GetPositionFromCharIndex(CodeBox.SelectionStart)
        If (pt.X = 1) Then
            AddLineNumbers()
        End If
    End Sub


    Private Sub CodeBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CodeBox.TextChanged
        If CodeBox.Text = "" Then
            AddLineNumbers()
        End If
        numberOfWords.Text = CStr(CodeBox.Text.Length)
        Status_NumberOfLine.Text = CStr(CodeBox.Lines.Length)
        updateSaveStatus()
        codeBeautified = False
        tempCodeBueatified = CodeBox.Text
    End Sub


    Private Sub SAVEToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SAVEToolStripMenuItem1.Click
        Mfile.saver()
    End Sub

    Private Sub CutToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Cut.Click
        edtMenu.cutText()
    End Sub

    Private Sub RunCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_RunCode.Click
        callCodeRunner(False)
    End Sub

    Private Sub RUNToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RUNToolStripMenuItem.Click
        callCodeRunner(False)
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        callCodeRunner(False)
    End Sub

    Private Sub ToolStripMenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem5.Click
        Executer.programArgs = putInsideDoubleQuouts(InputBox("Enter Arguments", "Cpp IDE", ""))
        'CodeBox.Text = programArgs
        callCodeRunner(False)
    End Sub

    Private Sub ToolStripMenuItem4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem4.Click
        callCodeRunner(True)
    End Sub

    Private Sub CompileCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CompileCodeOption.Click
        callCodeRunner(True)
    End Sub

    Private Sub FontToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FontToolStripMenuItem.Click
        If codeBoxFontDialog.ShowDialog <> DialogResult.Cancel Then
            Me.CodeBox.Font = codeBoxFontDialog.Font
            EventMessage.Text = "Font Changed to " + CodeBox.Font.Name
        End If
    End Sub

    Private Sub StatusBarToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StatusBarToolStripMenuItem.Click
        If DownStatusStrip.Visible = False Then
            DownStatusStrip.Visible = True
        Else
            DownStatusStrip.Visible = False
        End If
    End Sub

    Private Sub PasteToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PasteMenuItem.Click
        edtMenu.pasteText()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyMenuItem.Click
        edtMenu.copyText()

    End Sub

    Private Sub CutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CutMenuItem.Click
        edtMenu.cutText()

    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveAsToolStripMenuItem.Click
        Mfile.callSaveAs()

    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Mfile.saver()
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        If codeChanged Then
            Dim mr As MsgBoxResult
            mr = CType(ShowSaveDialog(), MsgBoxResult)
            If mr = MsgBoxResult.Yes Then
                Mfile.saver()
                If Mfile.Saved <> True Then
                    MsgBox("Unable to save", , appName)
                End If
            ElseIf mr = MsgBoxResult.No Then
                Mfile.openFile()
            End If
        Else
            Mfile.openFile()
            AddLineNumbers()
        End If
    End Sub

    Private Sub NewToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripMenuItem.Click
        createNewForm()
        AddLineNumbers()
    End Sub


    Private Sub Editor_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        'If Mfile.Saved = False Then
        If (codeChanged) Then
            Dim mr As DialogResult
            mr = ShowSaveDialog()
            If mr = DialogResult.Yes Then
                Mfile.saver()
                If Mfile.Saved <> True Then
                    e.Cancel = True
                End If
            ElseIf mr = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub


    Sub InitialSetups()

        'Setting last theme
        themer.SetTheme(My.MySettings.Default.Theme)

        'opening last opened file (if any)
        If (Not (My.MySettings.Default.lastOpenedFileName.Contains("\0"))) Then
            Mfile.setCodeBoxText(My.MySettings.Default.lastOpenedFileName)
        Else
            setLangMode()
        End If

        'Updating status strip
        numberOfWords.Text = CStr(CodeBox.Text.Length)
        Status_NumberOfLine.Text = CStr(CodeBox.Lines.Length)

        'Adding line number
        lineNumberBox.Font = CodeBox.Font
        CodeBox.Select()
        AddLineNumbers()
       codeChanged = False
    End Sub


    Private Sub Editor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = appName + " - Untitled"
        InitialSetups()
    End Sub
  
    Private Sub ToolStripMenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DayNightMenuItem.Click
        themer.manageTheme()

    End Sub

    Private Sub CodeBeatifier()
        '''''Was working here and trying set the postion of cursor after code being beautified
        Dim lstCursPosition As New Point
        lstCursPosition = CodeBox.Cursor.Position() '(CodeBox.GetLineFromCharIndex(CodeBox.GetFirstCharIndexOfCurrentLine))
        tempCode = CodeBox.Text
        'Executer.butifyCode(CodeBox, EventMessage)
        CodeBox.SuspendLayout()
        Executer.butifyCode()
        CodeBox.ResumeLayout()
        CodeBox.Cursor.Position = lstCursPosition
        tempCodeBueatified = CodeBox.Text
        codeBeautified = True
    End Sub


    Private Sub FormateCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FormateCodeToolStripMenuItem.Click
        CodeBeatifier()
    End Sub

    Private Sub ToolStripMenuItem6_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Butify.Click
        CodeBeatifier()
    End Sub

    Private Sub CopyOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Copy.Click
        edtMenu.copyText()

    End Sub

    Private Sub PastPast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Past.Click
        edtMenu.pasteText()
    End Sub

    Private Sub AboutDeveloperToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutDeveloperToolStripMenuItem.Click
        AboutSoftwareBox.Show()
    End Sub

    Private Sub Edit_Undo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Edit_Undo.Click
        If codeBeautified Then
            CodeBox.Text = tempCode
            codeBeautified = False
        Else
            CodeBox.Undo()
        End If
    End Sub

    Private Sub Edit_Redo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Edit_Redo.Click
        CodeBox.Redo()
    End Sub


    Sub resetCodeEditor()
        Mfile.filePath = "\0"
        My.MySettings.Default.lastOpenedFileName = Mfile.filePath
        Me.Text = appName + " - Untitled"
        CodeBox.Text = My.MySettings.Default.preAvalibleCCode
        codeChanged = False
        Mfile.Saved = True
        Executer.languageMD = languageMode.C
        StatusLanguageMode.Text = Executer.languageMD.ToString
        SAVEToolStripMenuItem1.Text = "SAVE"
        EventMessage.Text = "New Editor is ready!"
        setLangMode()
    End Sub

    Private Sub createNewForm()
        'If Not Mfile.Saved Then
        If codeChanged Then
            Dim mr As DialogResult
            mr = ShowSaveDialog()
            If mr = MsgBoxResult.Yes Then
                Mfile.saver()
                If Mfile.Saved <> True Then
                    MsgBox("Unable to save", , appName)
                End If
            ElseIf mr = MsgBoxResult.No Then
                resetCodeEditor()
            End If
        Else
            resetCodeEditor()
        End If
    End Sub



    '*******************Adding line number*****************************


    Private Sub Editor_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        AddLineNumbers()
    End Sub

    Function getWidth() As Integer
        Dim w As Integer = 0
        Dim line As Integer
        'Get total lines of CodeBox
        line = CodeBox.Lines.Length
        If (line <= 99) Then
            w = 60 ' + CInt(CodeBox.Lines.Length)
        ElseIf (line <= 999) Then
            w = 80 ' + CInt(CodeBox.Lines.Length)
        ElseIf (line <= 9999) Then
            w = 100 '+ CInt(CodeBox.Lines.Length)
        Else
            w = 120 '+ CInt(CodeBox.Lines.Length)
        End If
        Return w
    End Function

    Public Sub AddLineNumbers()
        'Create and set pointer to (0,0)
        Dim pt As Point = New Point(0, 0)
        'Get first index and first line number form CodeBox
        Dim firstIndex As Integer
        Dim firstLine As Integer
        Dim lastIndex As Integer
        Dim lastLine As Integer
        firstIndex = CodeBox.GetCharIndexFromPosition(pt)
        firstLine = CodeBox.GetLineFromCharIndex(firstIndex)
        'Set X and Y co-ordinate of point to clientRectangle Width & Height respactively
        pt.X = ClientRectangle.Width
        pt.Y = ClientRectangle.Height
        'Get last index and last line number form CodeBox
        lastIndex = CodeBox.GetCharIndexFromPosition(pt)
        lastLine = CodeBox.GetLineFromCharIndex(lastIndex)
        'Set center alignment to LineNumber textBox
        lineNumberBox.SelectionAlignment = HorizontalAlignment.Center
        'Set lineNumberTextBox to null & width to getWidth function
        lineNumberBox.Text = ""
        lineNumberAndSepraterContainer.Width = getWidth()
        'Now add each line number to lineNumber text box upto last line
        For i As Integer = firstLine To (lastLine + 2) Step 1
            lineNumberBox.Text += CStr(i + 1) + Environment.NewLine
        Next
    End Sub

    Private Sub CodeBox_VScroll(ByVal sender As Object, ByVal e As System.EventArgs) Handles CodeBox.VScroll
        lineNumberBox.Font = CodeBox.Font
        CodeBox.Select()
        AddLineNumbers()
    End Sub

    Private Sub lineNumberBox_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lineNumberBox.MouseDown
        CodeBox.Select()
        lineNumberBox.DeselectAll()
    End Sub

    Private Sub ResetZoomToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetZoomToolStripMenuItem.Click
        CodeBox.ZoomFactor = 1
    End Sub

    Private Sub prepairAppearenceForDragEnter()
        CodeBox.SendToBack()
        CodeBox.Visible = False
        CodeBox.Enabled = False
        DropFilePanel.BringToFront()
        CodeboxSepraterPanel.Visible = False
        lineNumberBox.Visible = False
        lineNumberAndSepraterContainer.Visible = False
    End Sub

    Private Sub resetAppearanceAfterDragAction()
        DropFilePanel.SendToBack()
        CodeBox.BringToFront()
        CodeBox.Visible = True
        CodeBox.Enabled = True
        CodeboxSepraterPanel.Visible = True
        lineNumberBox.Visible = True
        lineNumberAndSepraterContainer.Visible = True
    End Sub

    Private Sub Editor_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        prepairAppearenceForDragEnter()
    End Sub

    Private Sub DropFilePanel_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DropFilePanel.DragEnter
        prepairAppearenceForDragEnter()

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub DropFilePanel_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DropFilePanel.DragDrop
        resetAppearanceAfterDragAction()
        Dim file() As String = CType(e.Data.GetData(DataFormats.FileDrop), String()) 'Returns path
        Dim fileExt As String = file(0).Substring(file(0).LastIndexOf("."))
        Dim allowedExtenstion() As String = {".c", ".C", ".cpp", ".CPP", ".txt"}
        If Array.IndexOf(allowedExtenstion, fileExt) > -1 Then
            'If Not Mfile.Saved Then
            If codeChanged Then
                Dim mr As DialogResult = ShowSaveDialog()
                If mr = MsgBoxResult.Yes Then
                    If Mfile.saver() <> True Then
                        MsgBox("Unable to save", , appName)
                    End If
                ElseIf mr = MsgBoxResult.No Then
                    Mfile.setCodeBoxText(file(0))
                    AddLineNumbers()
                End If
            Else
                Mfile.setCodeBoxText(file(0))
                AddLineNumbers()
            End If
        Else
            MessageBox.Show("Only C, C++ and Text file are allowed", appName, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub DropFilePanel_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropFilePanel.DragLeave
        resetAppearanceAfterDragAction()
    End Sub


    Private Sub Editor_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DragLeave
        resetAppearanceAfterDragAction()
    End Sub


    Private Sub NewWindowToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewWindowToolStripMenuItem.Click
        Dim ed As Editor = New Editor
        ed.WindowState = FormWindowState.Normal
        ed.StartPosition = FormStartPosition.Manual
        ed.Show()
    End Sub

    Private Sub updateSaveStatus()
        Mfile.Saved = False
        codeChanged = True
        If (codeBeautified = False) Then
            SAVEToolStripMenuItem1.Text = "*SAVE"
        End If

        EventMessage.Text = ""
    End Sub

    Private Sub contex_Undo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Undo.Click
        If codeBeautified Then
            CodeBox.Text = tempCode
            codeBeautified = False
        Else
            CodeBox.Undo()
        End If
    End Sub

    Private Sub contex_Redo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles contex_Redo.Click
        CodeBox.Redo()
    End Sub

    Private Sub LanguageModeMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LanguageModeMenuItem2.Click
        Dim lm As DialogResult = selectLangDialog.ShowDialog()
        If lm = DialogResult.OK Then
            Executer.languageMD = languageMode.C
            Executer.languageMD = languageMode.C
            StatusLanguageMode.Text = "C"
        Else
            Executer.languageMD = languageMode.Cpp
            Executer.languageMD = languageMode.Cpp
            StatusLanguageMode.Text = "C++"
        End If
    End Sub

    Private Sub setLangMode()
        Dim lm As DialogResult = selectLangDialog.ShowDialog()
        If lm = DialogResult.OK Then
            Executer.languageMD = languageMode.C
            Executer.languageMD = languageMode.C
            StatusLanguageMode.Text = "C"
            CodeBox.Text = My.MySettings.Default.preAvalibleCCode
        Else
            Executer.languageMD = languageMode.Cpp
            Executer.languageMD = languageMode.Cpp
            StatusLanguageMode.Text = "C++"
            CodeBox.Text = My.MySettings.Default.preAvalibleCppCode
        End If
    End Sub

    Private Sub SetTextColorMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SetTextColorMenuItem.Click
        CodeBoxTextColorDialog.Color = Me.CodeBox.ForeColor
        If CodeBoxTextColorDialog.ShowDialog <> DialogResult.Cancel Then
            Me.CodeBox.ForeColor = CodeBoxTextColorDialog.Color
            EventMessage.Text = "Text Color Changed to " + CodeBox.ForeColor.ToString
        End If
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        If codeBoxFontDialog.ShowDialog <> DialogResult.Cancel Then
            Me.CodeBox.Font = codeBoxFontDialog.Font
            EventMessage.Text = "Font Changed to " + CodeBox.Font.Name
        End If
    End Sub

    Private Sub KeyboardShortcutsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KeyboardShortcutsToolStripMenuItem.Click
        KeyboardShortCuts.Show()
    End Sub

    Private Function ShowSaveDialog() As DialogResult
        Return MessageBox.Show("Do yo want to save current file...?", appName, System.Windows.Forms.MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
    End Function

End Class