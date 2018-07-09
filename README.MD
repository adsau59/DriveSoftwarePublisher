# DriveSoftwarePublisher

DriveSoftwarePublisher (DriveSP) is a software publishing software that uses Google Drive.

Ever wanted to send your software your client but you can't afford a to hire a server? with DriveSP you can use google drive to version your software.

This project might help programmers and software developers who have very low budget and this will also help open source developers to publish their software without any investement at all.

### Demo video
<center>[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/TdtkTxmOgw8/0.jpg)](https://www.youtube.com/watch?v=TdtkTxmOgw8)</center>

### Feature
By using DriveSP you can,
- Upload your project to google drive by using just one command.
- Create updates that clients can install by just one double click.
- Ignore the files and folders you don't want to include in the update using .pubignore file.

### I just want to use the software.
You can download an alpha release from [here]()

### Technology Stack
DriveSP is made using
- C#
- Drive REST API
- Docopt (Command Line API)
- Newtonsoft Json

### How does it work?
DriveSP solution contains 3 projects,
- Publisher, it is used to create projects, create updates, create updater for clients.
- Updater, it is used by clients to install updates, and even install selected version of the software.
- CommonScripts, contains all the command classes used by both the projects.

### How to use it?
- Go to [Google Cloud Platform Console](https://console.cloud.google.com/), and create a project.
- Enable Drive API and download client_secret.json.
- Include the client_secret.json file inside the CommonScripts project and change its properties to copy it to output directory.
- For release build make sure that the projects are build in this format
```
\out
|	SP.exe
|	\out\updater
|	|	update.bat
|	|	\out\updater\.update
|	|	|	SU.exe
```
Where, 
- ```SP.exe``` represents all the binaries of Publisher project.
- ```SU.exe``` represents al the binaries of Updater project.
- ```update.bat``` contains
```
@echo off
.updater\SU.exe install
```
### Roadmap
Development plans of DriveSP can be viewed on the [Trello Board](https://trello.com/b/G1UKrXN6).

Some of the plans include,
- Delete updates.
- Error report system.
- Check for update and print Json response (which can be used in 3rd party application to check for updates and install them, inside the 3rd party application itself).

### How to contribute?
You can contribute this project by,
- Using the the software and creating issue when any bug is encountered.
- Helping me in the development by bug squashing or developing new features. (If you want to do this, contact me so that we can collaborate.)
- Let me know if you have any good feature ideas.

Before creating issues for bugs, or suggesting new feature ideas, check out the [Trello Board](https://trello.com/b/G1UKrXN6) if it is already been found/suggested.

### Contact
If you have any problems or you want to contact me for feature ideas or want to collaborate in development you can contact me on [DefineX Community discord server](https://discord.gg/V6e2fpc)

### Feeling generous?
You can donate me on [PayPal](https://www.paypal.me/AdamSaudagar).
