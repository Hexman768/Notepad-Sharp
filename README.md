<h1 align="center">Notepad-Sharp</h1>
<p align="center">
    A general purpose text editor that supports various programming languages for code editing.
</p>

[![GitHub version](https://img.shields.io/badge/version-1.0.0-ff69b4)](https://github.com/Hexman768/Notepad-Sharp/releases/latest)
[![GitHub issues](https://img.shields.io/badge/open%20issues-12-yellow)](https://github.com/Hexman768/Notepad-Sharp/issues?q=is%3Aopen)
[![GitHub contributors](https://img.shields.io/badge/contributers-2-brightgreen)](https://github.com/Hexman768/Notepad-Sharp/contributors)
[![license](https://img.shields.io/badge/license-GNU--v3.0-orange)](https://github.com/Hexman768/Notepad-Sharp/blob/master/LICENSE)

# How to Build
Notepad# can be cloned into visual studio.
Once inside visual studio, you can just click the run button and everything should build correctly.

![Demo](https://user-images.githubusercontent.com/41409007/87869785-f2c48000-c967-11ea-83cf-bf988ef5665f.png)
# Troubleshooting
If you are having trouble building Notepad# then try the following:
* Delete the reference to TabStrip.dll under Notepad-Sharp -> References and add it back from the Notepad-Sharp/Resources folder in the working directory.
# Contributing
If you would like to contribute, please do one of the following:

* Build the latest version and test for bugs, if any are found then please log a new issue
      describing the reproduction steps and expected results.

* Create a pull request and tag me as a reviewer. 

* Provide feedback/tips and/or tricks. I am and will continue to be a student of software engineering. 
# Components used
Notepad-Sharp utilizes the FastColoredTextBox as it's preferred method for syntax and custom syntax highlighting. Shoutout to PavelTorgashov for creating this amazing component!
