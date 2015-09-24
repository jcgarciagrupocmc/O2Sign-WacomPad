Dim Domain, strComputer, strWS, strWSS
Dim dwordZone, regPath, objReg, counter, subkeyPath, arrSubKeys, strSubkey, arrSubKeys2, keySoftware
Dim subkeyValue
Const HKEY_CURRENT_USER = &H80000001
Const HKEY_LOCAL_MACHINE = &H80000002
Const HKEY_USERS 		= &H80000003
keySoftware = "Software"
Domain = "//127.0.0.1/"
strComputer = "."
strWS = "ws"
strWSS = "wss"
dwordZone = "2"
regPath = "SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\"
Set objReg = GetObject("winmgmts:{impersonationLevel = impersonate}!\\" & strComputer & "\root\default:StdRegProv")
subkeyPath = regPath & Domain


objReg.CreateKey HKEY_CURRENT_USER,regPath
objReg.CreateKey HKEY_CURRENT_USER,subkeyPath
objReg.SetDWORDValue HKEY_CURRENT_USER,subkeyPath,strWS,dwordZone
objReg.SetDWORDValue HKEY_CURRENT_USER,subkeyPath,strWSS,dwordZone
