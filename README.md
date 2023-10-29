# Bookmarks
A bookmark manager

## Installation
### Database
Manually create database

### Linux
Create folder for installation (/opt/bookmarks) and change to that folder
```
cd /opt/bookmarks
```

If upgrading remove old files and stop service
```
rm -rf wwwroot

systemctl stop bookmarks.service
```

Download the version you want to install
```
Replace <version> with the latest release version
wget https://github.com/peter-andersson/bookmarks/releases/download/<version>/linux-x64-<version>.zip
```

Unzip the folder, overwrite all files
```
unzip linux-x64-<version>.zip
```

First installation
```
Copy installation/bookmarks.service to /etc/systemd/system
Copy appsettings.json to appsettings.Production.json and enter correct database information.
systemctl daemon-reload
systemctl enable bookmarks.service
```

Start service
```
chmod +x Bookmarks
systemctl start bookmarks.service
```

Check that service is running
```
systemctl status bookmarks.service
```