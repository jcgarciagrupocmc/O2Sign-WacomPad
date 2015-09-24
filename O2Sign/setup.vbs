Dim found
found = False
Const ForReading = 1, ForWriting = 2, ForAppending = 8, ReadOnl = 1
Set fso = CreateObject("Scripting.FileSystemObject")
Set WshShell=CreateObject("WScript.Shell")
WinDir =WshShell.ExpandEnvironmentStrings("%WinDir%")
HostsFile = WinDir & "\System32\Drivers\etc\Hosts"

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile(HostsFile, ForReading)

Do Until (found OR objFile.AtEndOfStream)
	If InStr (objFile.ReadLine, "127.0.0.1 galeonsoftware.com") <> 0 Then
		found = true
	End If
	i = i + 1
Loop
objFile.Close
If Not found Then
	Set objFSO = CreateObject("Scripting.FileSystemObject")
	Set objFile = objFSO.GetFile(HostsFile)
	If objFile.Attributes AND ReadOnl Then
		objFile.Attributes = objFile.Attributes XOR ReadOnl
	End If

	Set filetxt = fso.OpenTextFile(HostsFile, ForAppending, True)
	filetxt.WriteLine(vbNewLine & "127.0.0.1 galeonsoftware.com")
	filetxt.Close
End If