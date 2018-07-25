# DriveSoftwarePublisher

DriveSoftwarePublisher (DriveSP) is a software publishing software that uses Google Drive.

Ever wanted to send your software your client but you can't afford a to hire a server? with DriveSP you can use google drive to version your software.

This project might help programmers and software developers who have very low budget and this will also help open source developers to publish their software without any investement at all.

### Demo video
<div align="center">
  <a href="https://www.youtube.com/watch?v=TdtkTxmOgw8"><img src="https://img.youtube.com/vi/TdtkTxmOgw8/0.jpg" alt="IMAGE ALT TEXT"></a>
</div>

### Feature
By using DriveSP you can,
- Upload your project to google drive by using just one command.
- Create updates that clients can install by just one double click.
- Ignore the files and folders you don't want to include in the update using .pubignore file.

### I just want to use the software.
You can download an alpha release from [here](https://github.com/adsau59/DriveSoftwarePublisher/releases), put the SP.exe inside environment variables file path for ease of use.

### Technology Stack
DriveSP is made using
- C#
- .NET Core
- Drive REST API
- Docopt (Command Line API)
- Newtonsoft Json

### How does it work?
DriveSP solution contains 3 VSProjects,
- Publisher, it is used to create projects, create updates, create updater for clients.
- Updater, it is used by clients to install updates, and even install selected version of the software.
- CommonScripts, contains all the command classes used by both the projects.

Definitions:
- Project is an encapsulation of for a software that has to be versioned using DriveSP, it has attributes as follows,
	- Software name, the name of the folder created inside ```DefineXSoftwarePublisher``` folder.
	- Folder ID, which is the folder id in the drive.
	- Versions, List of version that or on drive.
- Version encapsulates the information of versions (update files) from drive, it has attributes as follows
	- VersionCode, Integer defining the version number, of an update file. VersionCode starts with 1 and increments with each new updated.
	- VersionName, String naming the update with a name, could be any string without "--" substring or space, and should be unique for each version.

Basic commands of Publisher, 

- When ```Publisher init SOFTWARENAME``` is called,
	- A project is created on drive inside ```DefineXSoftwarePublisher``` folder inside ```root```.
	- It also initializes files it needs under .publish folder.
- When ```Publisher push``` is called, 
	- It searches for .pubignore file in the root directory of your project folder and then ignore all the files which returns true on regex check for the patterns on each line.
	- Then it creates a zip with all the files and uploads it to drive with the file name ```versioncode--softwarename--versionname.zip``` inside the folder ```SOFTWARENAME``` in the ```DefineXSoftwarePublisher``` folder inside ```root```.
- When ```Publisher upversion VERSIONNAME``` is use,
	- It increases the version code of the project and changes its version name inside the version.json file.
	- It should be called before pushing a new version.
- When ```Publisher getupdater``` is called, it returns a link to ```updater.zip``` file which is to be given to the clients to download your software.

Basic commands of Updater,

- When ```Updater install [--code=<vc> | --name=<vn>]``` is called,
	- Install the software with specified version, or the latest version if not specified

### How to use it?
- Go to [Google Cloud Platform Console](https://console.cloud.google.com/), and create a project.
- Enable Drive API and download client_secret.json.
- Include the client_secret.json file inside the CommonScripts project and change its properties to copy it to output directory.
- For release build make sure that the projects are build in this format
```
\out
|	Publisher
|	\out\updater
|	|	install.bat
|	|	\out\updater\.update
|	|	|	Updater
```
Where, 
- ```Publisher``` represents all the binaries of Publisher project.
- ```Updater``` represents al the binaries of Updater project.
- ```install.bat``` contains
```
@echo off
.updater\SU.exe install
```
### Roadmap
Development plans of DriveSP can be viewed on the [Trello Board](https://trello.com/b/G1UKrXN6).

Phase 1: (COMPLETED)
- Create DriveSP Prototype with basic Publisher and Updater commands

Phase 2: (COMPLETED)
- Port to .NET Core
- Add Features:
  - pushnow commnd in Publisher, increase version and push with one command
  - drop command in Publisher, delete a version from the drive
  - showversions command, show versions on the drive
  - target install version with either version code or version name
  - checkupdate command, check if new version is available

Phase 3:
- Error reporting system, report errors using google drive.
- Add Feature:
	- Add ability for 3rd party application to check updated and perform update automatically.

### How to contribute?
You can contribute this project by,
- Using the the software and creating issue when any bug is encountered.
- Helping me in the development by bug squashing or developing new features. (If you want to do this, contact me so that we can collaborate.)
- Let me know if you have any good feature ideas.

Before creating issues for bugs, or suggesting new feature ideas, check out the [Trello Board](https://trello.com/b/G1UKrXN6) if it is already been found/suggested.

### Whats new
Check out [changes](https://github.com/adsau59/DriveSoftwarePublisher/blob/master/changes) for each commits.

### Contact
If you have any problems or you want to contact me for feature ideas or want to collaborate in development you can contact me on [DefineX Community discord server](https://discord.gg/V6e2fpc)

### Feeling generous?
You can donate me on [PayPal](https://www.paypal.me/AdamSaudagar).

### Licencse
This project is licence to the MIT License, check out the full licence over [here](https://github.com/adsau59/DriveSoftwarePublisher/blob/master/LICENSE).