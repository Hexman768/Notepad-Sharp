<h1 align="center">:spiral_notepad: Notepad# :spiral_notepad:</h1>
<div align="center">
    <img src="https://user-images.githubusercontent.com/41409007/87869785-f2c48000-c967-11ea-83cf-bf988ef5665f.png" />
    <a href="https://github.com/Hexman768/Notepad-Sharp/releases/latest" target="_blank">
        <img src="https://img.shields.io/badge/version-1.0.0-ff69b4" />
    </a>
    <a href="https://github.com/Hexman768/Notepad-Sharp/issues?q=is%3Aopen" target="_blank">
        <img src="https://img.shields.io/github/issues/Hexman768/Notepad-Sharp" />
    </a>
    <a href="https://github.com/Hexman768/Notepad-Sharp/contributors" target="_blank">
        <img src="https://img.shields.io/github/contributors/Hexman768/Notepad-Sharp?color=green" />
    </a>
    <a href="https://github.com/Hexman768/Notepad-Sharp/blob/master/LICENSE" target="_blank">
        <img src="https://img.shields.io/github/license/Hexman768/Notepad-Sharp" />
    </a>
</div>

<div align="center">
    <a href="#computer-About">About</a>&nbsp;&nbsp;|&nbsp;&nbsp;
    <a href="#hammer_and_wrench-Compilation">Compilation</a>&nbsp;&nbsp;|&nbsp;&nbsp;
    <a href="desktop_computer-System Requirements>System Requirements</a>&nbsp;&nbsp;|&nbsp;&nbsp;
    <a href="#toolbox-Troubleshooting">Troubleshooting</a>&nbsp;&nbsp;|&nbsp;&nbsp;
    <a href="#octocat-Contributing">Contributing</a>
</div>

## :computer: About:
Notepad# (Short for Notepad-Sharp) is a general purpose text editor that supports various programming languages for code editing.

## :hammer_and_wrench: Compilation:
<ol>
    <li>Execute the init.bat script in the project root directory.</li>
    <li>Build the project in Visual Studio IDE, or with the dotnet CLI tool.</li>
</ol>

## :desktop_computer: System Requirements:
Notepad# is only compatible with Windows machines (XP and up) due to a heavy reliance on the Win32 API. There are currently no plans to make the app cross-platform as it is intended to be a Notepad++ clone, and thus, has very similar requirements. 
.NET 4.8.2 is required to launch. 

## :toolbox: Troubleshooting:
If you are having trouble building Notepad# then try the following:
<ul>
    <li>Make sure to run the init.bat script in the project directory.</li>
    <li>Open the Nuget package manager and update/install all required packages.</li>
    <li>Delete the reference to TabStrip.dll under Notepad-Sharp -> References and add it back from the Notepad-Sharp/Resources folder in the working directory.</li>
</ul>

## :octocat: Contributing:
If you would like to contribute, please do one of the following:
<ol>
    <li align="left">If any issues are found then please log a new issue describing the reproduction steps and expected results.</li>
    <li align="left">Create a pull request and tag me as a reviewer.</li>
    <li align="left">Provide feedback/tips and/or tricks. I am and will continue to be a student of software engineering.</li>
</ol>

## :gear: Dependency:
Here is a list of the great dependencies that Notepad# consumes:
<ol>
    <li><a href="https://www.nuget.org/packages/FCTB/">FastColoredTextBox</a> - Shoutout to PavelTorgashov for creating this amazing component!</li>
    <li><a href="https://www.nuget.org/packages/DockPanelSuite/">DockPanelSuite</a> - Provides docking behavior.</li>
    <li><a href="https://www.nuget.org/packages/DockPanelSuite.ThemeVS2005">DockPanelSuite.ThemeVS2005</a> - Provides themes for DockPanelSuite.</li>
</ol>
