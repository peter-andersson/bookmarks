# Bookmarks
A bookmark manager

## Build release
In folder **src/Bookmarks.ClientApp** run
```
npm run build-only
```

Copy files from **src/Bookmarks.ClientApp/dist** to **src/Bookmarks.Backend/wwwroot**

In folder **src/Bookmarks.Backend/** run
```
dotnet publish --runtime win-x64 --self-contained -p:PublishProfile=FolderWindows
```
For Windows version or 
```
dotnet publish --runtime linux-x64 --self-contained -p:PublishProfile=FolderLinux
```
for Linux version.

Publish is done to **src/Bookmarks.Backend/bin/publish**

## Installation
### Database
Manually create database

### Linux
Copy the files from publish folder to somewhere on the linux machine (/opt/bookmarks)

Copy installation/bookmarks.service to /etc/systemd/system

Edit appsettings.json and enter correct database inforamtion.

sudo systemctl daemon-reload
sudo systemctl start bookmarks.service
sudo systemctl status bookmarks.service
sudo systemctl enable bookmarks.service