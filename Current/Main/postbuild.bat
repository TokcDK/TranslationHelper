@echo off & setlocal EnableDelayedExpansion

set targetDir=%~1
set targetName=%~2
set projectDir=%~3
set versionNumber=%~4
set configurationName=%~5

::safe check if one of param is empty
if "%targetDir%" == "" (
	echo targetDir is empty!
	goto :exit
)
if "%targetName%" == "" (
	echo targetName is empty!
	goto :exit
)
if "%projectDir%" == "" (
	echo projectDir is empty!
	goto :exit
)
if "%versionNumber%" == "" (
	echo versionNumber is empty!
	goto :exit
)
if "%configurationName%" == "" (
	echo configurationName is empty!
	goto :exit
)

set projectBuildDir=%projectDir%BUILD
set projectResDir=%projectBuildDir%\RES
set targetResDir=%targetDir%RES

echo move libs..
set targetLibDir=%targetResDir%\lib
::lib files
if exist "%targetLibDir%" rd "%targetLibDir%" /s /q
ROBOCOPY "%targetDir% " "%targetLibDir% " *.dll *.pdb *.xml /XF "%targetName%.dll" "%targetName%.pdb" "%targetName%.xml" /LEV:1 /MOV
::lib lang dirs
FOR %%D IN (zh-Hans zh-Hant cs de es fr it ja ko pl pt-BR ru tr) DO (
	if exist "%targetLibDir%\%%D" rd "%targetLibDir%\%%D" /s /q
	if exist "%targetDir%%%D" move "%targetDir%%%D" "%targetLibDir%\"
)

REM set themeDefaultSubPath=theme\default
REM set reportDirSubPath=%themeDefaultSubPath%\report
REM set reportTemplateFileName=ReportTemplate.html
REM set projectReportDir=%projectResDir%\%reportDirSubPath%
REM set projectReportTemplatePath=%projectReportDir%\%reportTemplateFileName%
REM set targetReportDir=%targetResDir%\%reportDirSubPath%
REM set targetReportTemplatePath=%targetReportDir%\%reportTemplateFileName%
REM if exist "%projectReportTemplatePath%" (
	REM echo copy %reportTemplateFileName% and game backgrounds..
	REM robocopy "%projectReportDir% " "%targetReportDir%\ " %reportTemplateFileName% *.jpg /MIR /COPYALL /B /R:3 /W:1
REM )

set projectResLocaleDir=%projectResDir%\locale
set targetResLocaleDir=%targetResDir%\locale
if exist "%projectResLocaleDir%" (
	echo copy localization files
	robocopy "%projectResLocaleDir% " "%targetResLocaleDir%\ " *.po *.mo /MIR /COPYALL /B /R:3 /W:1
)

:: release creation
set releasesDirPath=%targetDir%RELEASES
if not exist "%releasesDirPath%" md "%releasesDirPath%"
set targetReleaseDir=%targetDir%RELEASE
set targetReleaseResDir=%targetReleaseDir%\RES
if "%configurationName%" == "Release" (
	echo Release creation..

	:: recreate release dir
	if exist "%targetReleaseDir%" rd "%targetReleaseDir%" /s /q
	md "%targetReleaseDir%"
	md "%targetReleaseResDir%"
	
	md "%targetReleaseDir%\DB
	md "%targetReleaseDir%\Work

	::make symlinks for dirs and files
	for %%d in (locale,sounds,tools) do (
		if not exist "%targetReleaseResDir%\%%d" (
			if exist "%targetResDir%\%%d" (
				echo make link for %%d dir
				MKLINK "%targetReleaseResDir%\%%d" "%targetResDir%\%%d" /D
			)
		)
	)
	REM if not exist "%targetReleaseResDir%\%reportDirSubPath%" (
		REM echo make link for report dir
		REM :: make parent dir
		REM md "%targetReleaseResDir%\%themeDefaultSubPath%"

		REM MKLINK "%targetReleaseResDir%\%reportDirSubPath%" "%targetReportDir%" /D
	REM )
	if not exist "%targetReleaseDir%\%targetName%.exe" (
		echo make link for the app exe
		MKLINK "%targetReleaseDir%\%targetName%.exe" "%targetDir%\%targetName%.exe"
	)
	set targetReleaseLibDir=%targetReleaseResDir%\lib
	if not exist "!targetReleaseLibDir!" (
		echo make link for the app exe libs dir
		MKLINK "!targetReleaseLibDir!" "%targetLibDir%" /D
		Del "!targetReleaseLibDir!\*.pdb"
		Del "!targetReleaseLibDir!\*.xml"
	)

	:: create archive of the release
	set destinationName=%targetName% %versionNumber%
	set destinationPath=%releasesDirPath%\!destinationName!
	set powershellApp=powershell Compress-Archive -Path '%targetReleaseDir%\*' -DestinationPath '!destinationPath!.zip' -Force
	set sevenZipApp7z="C:\Program Files\7-Zip\7z.exe" a "!destinationPath!.7z" -m0=LZMA2:d=96m:fb=128 -mx=9 -mmt4 "%targetReleaseDir%\*"
	set archiveAppRun=!sevenZipApp7z!	
	echo '!destinationPath!' archive creating..
	echo run archive creation command: !archiveAppRun!
	!archiveAppRun!
)

:exit
echo postbuild script finished!