# Notepad-Sharp
A general purpose text editor that supports various programming languages for code editing.
___
# How to Build
Notepad# can be cloned into visual studio.
Once inside visual studio, you can just click the run button and everything shoud build correctly.

![Demo](https://user-images.githubusercontent.com/41409007/87869785-f2c48000-c967-11ea-83cf-bf988ef5665f.png)
___
# Troubleshooting
If you are having trouble building Notepad# then try the following:
* Delete the reference to TabStrip.dll under Essay_Analysis_Tool -> References and add it back from the Essay_Analysis_Tool/Resources folder in the working directory.
___
# Contributing
If you would like to contribute, please do one of the following:

* Build the latest version and test for bugs, please email Zachary.Pedigo1@gmail.com with details and log a new issue
      describing the reproduction steps and expected results.

* Create a pull request and tag me as a reviewer. Although you DO have the freedom to commit directly to the master branch, I do
      ask that you please refrain. If anyone makes an unapproved commit to the master branch, I will manually revert the commit(s).

* Provide feedback/tips and/or tricks. I am and will continue to be a student of software engineering, please contact me by my email
      listed above with any helpful advice you may have.
___
# Components used

Notepad-Sharp utilizes the FastColoredTextBox as it's preferred method for syntax and custom syntax highlighting. Shoutout to PavelTorgashov for creating this amazing component!
