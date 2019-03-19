Imports System.IO
Imports System.IO.Compression

Public Class Form1

    Public Class GV
        Public Shared DownloadVersion
    End Class

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim Path2

        ' Check if the path is already set in settings, otherwise invoke an input box to get the new path. 

        If My.Settings.Path = "" Then
            Path2 = InputBox("Enter the path to your addons folder for WoW. Make sure to include the \ at the end.")

            ' Check if the path contains "addons", as this should be the correct parent folder.

            If Not My.Settings.Path.Contains("addons\") Then
                DialogResult = MsgBox("Path does not contain ...\addons\. Please update the path via the button. If you are sure it does, then press Yes. Else, press no.", MsgBoxStyle.YesNo)

                If DialogResult = DialogResult.Yes Then
                    ' Save the settings
                    My.Settings.Path = Path2
                    My.Settings.CanUpdate = True
                    My.Settings.Save()
                Else
                    Console.WriteLine(Path2)
                    Console.WriteLine(My.Settings.Path)
                End If

            End If
        End If

        ' Create our working directory to store the download. 
        If Not My.Computer.FileSystem.DirectoryExists("C:\Program Files (x86)\SplitSecond\ElvUI_Updater") Then
            My.Computer.FileSystem.CreateDirectory("C:\Program Files (x86)\SplitSecond\ElvUI_Updater")
        End If


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        ' See comments in Form_Load

        Dim Path3

        Path3 = InputBox("Enter the path to your addons folder for WoW. Make sure to include the \ at the end.")

        If Not My.Settings.Path.Contains("addons\") Then
            DialogResult = MsgBox("Path does not contain ...\addons\. Please update the path via the button. If you are sure it does, then press Yes. Else, press no.", MsgBoxStyle.YesNo)

            If DialogResult = DialogResult.Yes Then
                My.Settings.Path = Path3
                My.Settings.Save()
                My.Settings.CanUpdate = True
                My.Settings.Save()
            Else
                Console.WriteLine(Path3)
                Console.WriteLine(My.Settings.Path)
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' Check if the old version zip exists, and if it does, delete it, as well as any possible sub folders it may have created.

        If My.Settings.CanUpdate = True Then
            If My.Computer.FileSystem.FileExists("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip") Then
                My.Computer.FileSystem.DeleteFile("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip")
            End If
            If My.Computer.FileSystem.DirectoryExists("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvUI") Then
                My.Computer.FileSystem.DeleteDirectory("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvUI", FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If
            If My.Computer.FileSystem.DirectoryExists("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvUI_Config") Then
                My.Computer.FileSystem.DeleteDirectory("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvUI_Config", FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If

            ' Set IsDownloading to false for the hack check.
            Dim IsDownloading = False

            ' Invoke the webpage download, to get the source, to get the latest version number.

            ReadWebpage()
            ' If the version string is ever not xx.xx, this will fail. Find a better method in the future. 
            Me.BackColor = Color.Purple
            Threading.Thread.Sleep(100) ' A small sleep to make sure the back color change isn't blocked.
            My.Computer.Network.DownloadFile("https://www.tukui.org" + GV.DownloadVersion, "C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip") ' Begin the download, and save to our location.
            IsDownloading = True

            ' This will continuously try to move the file (Rename), and if the file is downloading, it will be blocked, preventing IsDownloading from becoming false. 
            While IsDownloading = True
                Try
                    My.Computer.FileSystem.MoveFile("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip", "C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvuiMOVE.zip")
                    IsDownloading = False
                Catch ex As Exception

                End Try
            End While

            My.Computer.FileSystem.MoveFile("C:\Program Files (x86)\SplitSecond\ElvUI_Updater\ElvuiMOVE.zip", "C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip") ' Move it back to the original name.

            ' Delete the files from the addon dir
            ' This really should be an if statement, but it was done in a hurry, and a try block does what's needed without fuss.
            Try
                My.Computer.FileSystem.DeleteDirectory(My.Settings.Path + "ElvUI", FileIO.DeleteDirectoryOption.DeleteAllContents)
            Catch ex As Exception

            End Try

            Try
                My.Computer.FileSystem.DeleteDirectory(My.Settings.Path + "ElvUI_Config", FileIO.DeleteDirectoryOption.DeleteAllContents)
            Catch ex As Exception

            End Try


            ' Extract the downloaded zip to the addons directory. 
            Dim zipPath As String = "C:\Program Files (x86)\SplitSecond\ElvUI_Updater\Elvui.zip"
            Dim extractPath As String = My.Settings.Path
            ZipFile.ExtractToDirectory(zipPath, extractPath)
            Me.BackColor = Color.Green
        Else
            MsgBox("Why are you trying to update without a path set? Set your path first.")
        End If






    End Sub

    Public Sub ReadWebpage()
        Dim sourceString As String = New System.Net.WebClient().DownloadString("https://www.tukui.org/download.php?ui=elvui")         ' Download the web page.
        '
        Dim FindStart = (sourceString.IndexOf("/downloads/elvui-"))                                                                   ' Find the start of the version number information, to pass as a download link.
        FindStart = FindStart + 1                                                                                                     ' The first character it finds in the string is ("), so we need to skip over that.
        Dim DownloadHREF = Mid(sourceString, FindStart, 26)                                                                           ' Read the next 26 characters, which ends in .zip. This should be updated to read UNTIL it reads a ("), instead of assuming. 
        GV.DownloadVersion = DownloadHREF                                                                                             ' Sets the download URL
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        ReadWebpage()   ' Debug. Not needed nor usable. 

    End Sub
End Class
