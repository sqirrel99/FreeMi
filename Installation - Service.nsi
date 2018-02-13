;NSIS Modern User Interface

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"
  !include "FileFunc.nsh"
  !include "x64.nsh"
  
;--------------------------------
;General

  XPStyle on 
  ShowInstDetails show
  
  ;Name and file
  Name "FreeMi UPnP $3 - Mode Service"
  OutFile ".\Setup Service\Setup.exe"
  BrandingText "Stéphane Mitermite" 
   
;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING
  !define MUI_WELCOMEPAGE_TITLE_3LINES
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_RIGHT
  !define MUI_HEADERIMAGE_BITMAP_NOSTRETCH
  !define MUI_HEADERIMAGE_BITMAP "GenericMovieClip.bmp"
  !define MUI_HEADERIMAGE_BITMAP_RTL_NOSTRETCH
  !define MUI_HEADERIMAGE_BITMAP_RTL "GenericMovieClip.bmp"
  !define MUI_COMPONENTSPAGE_NODESC
  !define MUI_FINISHPAGE_RUN "$INSTDIR\FreeMi UPnP Media Server.exe"
  !define MUI_FINISHPAGE_LINK "FreeMi UPnP Media Server (http://freemiupnp.fr/)"
  !define MUI_FINISHPAGE_LINK_LOCATION "http://freemiupnp.fr/"
  !define MUI_ICON ".\FreeMi.WindowsService\FreeMi.ico"
;--------------------------------
;Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
 
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "French"
  !insertmacro MUI_LANGUAGE "English"
  
  LangString desktopShortcut ${LANG_ENGLISH} "Desktop shortcut"
  LangString desktopShortcut ${LANG_FRENCH} "Raccourci sur le bureau"
  LangString startMenuShortcut ${LANG_ENGLISH} "Shortcut into the start menu"
  LangString startMenuShortcut ${LANG_FRENCH} "Raccourci dans le menu démarrer"
  LangString serviceInstall ${LANG_ENGLISH} "Windows service installation"
  LangString serviceInstall ${LANG_FRENCH} "Installation du service Windows"
  LangString serviceUninstall ${LANG_ENGLISH} "Windows service uninstallation"
  LangString serviceUninstall ${LANG_FRENCH} "Désinstallation du service Windows"
  LangString message ${LANG_ENGLISH} "A newer version of FreeMi UPnP Media Server ($1) already exists in the directory $INSTDIR.$\r$\n$\r$\nDo you want to replace this version?"
  LangString message ${LANG_FRENCH} "Une version plus récente de FreeMi UPnP Media Server ($1) existe déjà dans le répertoire $INSTDIR.$\r$\n$\r$\nVoulez-vous écraser cette version ?"
  LangString frameworkMessage ${LANG_ENGLISH} ".NET Framework v4.0 is not installed on this computer. The software needs this framework to be installed in order to work. Do you want to download it now ? (Thanks to restart FreeMi installation after the installation of the .NET Framework)"
  LangString frameworkMessage ${LANG_FRENCH} "Le Framework .NET v4.0 n'est pas installé sur cet ordinateur. Le logiciel a besoin que ce framework soit installé pour fonctionner. Voulez-vous le télécharger maintenant ? (Merci de relancer l'installation de FreeMi après l'installation du Framework .NET)"
     
;--------------------------------
;Installer Sections

Function RelGotoPage
  IntCmp $R9 0 0 Move Move
  StrCmp $R9 "X" 0 Move
  StrCpy $R9 "120"
 
  Move:
  SendMessage $HWNDPARENT "0x408" "$R9" ""
FunctionEnd

Function .onInit
  
  ClearErrors
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}
  ReadRegStr $INSTDIR HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" InstallLocation
  
  ${If} ${RunningX64}
    ${If} $INSTDIR == ""
      strcpy $INSTDIR "$PROGRAMFILES64\FreeMi UPnP Media Server\"
	  ${EndIf}
  ${Else}
    ${If} $INSTDIR == ""
      strcpy $INSTDIR "$PROGRAMFILES\FreeMi UPnP Media Server\"
	  ${EndIf}
	${EndIf}
  
  GetDLLVersionLocal "./bin/Service/FreeMi UPnP Media Server.exe" $R0 $R1
  IntOp $R2 $R0 >> 16
  IntOp $R2 $R2 & 0x0000FFFF ; $R2 now contains major version
  IntOp $R3 $R0 & 0x0000FFFF ; $R3 now contains minor version
  IntOp $R4 $R1 >> 16
  IntOp $R4 $R4 & 0x0000FFFF ; $R4 now contains release
  IntOp $R5 $R1 & 0x0000FFFF ; $R5 now contains build
  StrCpy $3 "$R2.$R3.$R4" ; $3 now contains string like "1.2.0"
  
FunctionEnd

Section "FreeMi UPnP Media Server $3"  
SectionIn RO

  ;Framework .Net 4.0
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" Install
  ${If} $0 == ""
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" Install
    ${If} $0 == ""
      MessageBox MB_YESNO|MB_ICONEXCLAMATION "$(frameworkMessage)" IDNO ContinueInstall   
      ExecShell open "http://www.microsoft.com/fr-fr/download/details.aspx?id=17113"
      Quit
    ${EndIf}
  ${EndIf}

  ContinueInstall:
  ; Test if the file already exists
  IfFileExists "$INSTDIR\FreeMi UPnP Media Server.exe" FileExists RunInstall
  
  FileExists:
  ; Check the version of the file
  GetDLLVersion "$INSTDIR\FreeMi UPnP Media Server.exe" $R6 $R7
  IntOp $R8 $R6 >> 16
  IntOp $R8 $R8 & 0x0000FFFF ; $R8 now contains major version
  IntOp $R9 $R6 & 0x0000FFFF ; $R9 now contains minor version
  IntOp $6 $R7 >> 16
  IntOp $6 $6 & 0x0000FFFF ; $6 now contains release
  IntOp $7 $R7 & 0x0000FFFF ; $7 now contains build
  StrCpy $1 "$R8.$R9.$6.$7" ; $1 now contains string like "1.2.0.192"

  IntCmp $R2 $R8 TestMinor ShowMessageBox UninstallBeforeInstall
  TestMinor:
  IntCmp $R3 $R9 TestRelease ShowMessageBox UninstallBeforeInstall
  TestRelease:
  IntCmp $R4 $6 TestBuild ShowMessageBox UninstallBeforeInstall
  TestBuild:
  IntCmp $R5 $7 UninstallBeforeInstall ShowMessageBox UninstallBeforeInstall
  ShowMessageBox:
  MessageBox MB_YESNO "$(message)" IDYES UninstallBeforeInstall
  StrCpy $R9 -1
  Call RelGotoPage
  Abort

  UninstallBeforeInstall:
  nsExec::Exec 'TaskKill /F /IM "FreeMi UPnP Media Server.exe"'
  nsExec::Exec 'TaskKill /F /IM "FreeMi.WindowsService.exe"'
  DetailPrint "$(serviceUninstall)"
  nsExec::Exec '$INSTDIR\FreeMi.WindowsService.exe /S /U' 
	
  RunInstall:  
  SetOutPath "$INSTDIR"
  SetOverwrite on  	
  File /x *.pdb /x *.vshost.* /x *.manifest ".\bin\Service\*.*"
  
  DetailPrint "$(serviceInstall)"
  nsExec::Exec '$INSTDIR\FreeMi.WindowsService.exe /S /I'
   
  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
  WriteRegExpandStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegExpandStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "Publisher" "Stéphane Mitermite"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "DisplayName" "FreeMi UPnP Media Server"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "DisplayVersion" "$3"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "DisplayIcon" "$INSTDIR\FreeMi UPnP Media Server.exe,0"
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  IntFmt $0 "0x%08X" $0
  WriteRegDword HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server" "EstimatedSize" "$0"
  
  ;http://wiz0u.free.fr/prog/nsisFirewall/
  nsisFirewall::AddAuthorizedApplication "$INSTDIR\FreeMi.WindowsService.exe" "FreeMi UPnP Media Server"
   
SectionEnd

Section "$(desktopShortcut)"

  ;Create shortcuts
  SetShellVarContext All
  CreateShortCut "$DESKTOP\FreeMi UPnP Media Server.lnk" "$INSTDIR\FreeMi UPnP Media Server.exe"  

SectionEnd

Section "$(startMenuShortcut)"

  ;Create shortcuts
  SetShellVarContext All
  CreateDirectory "$SMPROGRAMS\FreeMi UPnP Media Server\"
  CreateShortCut "$SMPROGRAMS\FreeMi UPnP Media Server\FreeMi UPnP Media Server.lnk" "$INSTDIR\FreeMi UPnP Media Server.exe"
 
SectionEnd

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  nsExec::Exec 'TaskKill /F /IM "FreeMi UPnP Media Server.exe"'
  nsExec::Exec 'TaskKill /F /IM "FreeMi.WindowsService.exe"'
  
  DetailPrint "$(serviceUninstall)"
  nsExec::Exec '$INSTDIR\FreeMi.WindowsService.exe /S /U'  
  
  Delete "$INSTDIR\FreeMi UPnP Media Server.exe"
  Delete "$INSTDIR\FreeMi.config"
  Delete "$INSTDIR\FreeMi.Core.dll"
  Delete "$INSTDIR\FreeMi.UI.dll"
  Delete "$INSTDIR\FreeMi.WindowsService.exe"
  Delete "$INSTDIR\UPnP.dll"
  Delete "$INSTDIR\user.config"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  ${GetParent} "$INSTDIR" $R0
  RMDir $R0
  
  ${If} ${RunningX64}
    SetRegView 64
  ${EndIf}
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Run\FreeMi UPnP Media Server"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\FreeMi UPnP Media Server"
  SetShellVarContext All
  Delete "$SMPROGRAMS\FreeMi UPnP Media Server\FreeMi UPnP Media Server.lnk"
  RMDir "$SMPROGRAMS\FreeMi UPnP Media Server" 
  Delete "$DESKTOP\FreeMi UPnP Media Server.lnk"
  
  nsisFirewall::RemoveAuthorizedApplication "$INSTDIR\FreeMi.WindowsService.exe"
  
SectionEnd