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
dotnet publish -p:PublishProfile=FolderWindows
```
For Windows version or 
```
dotnet publish -p:PublishProfile=FolderLinux
```
for Linux version.

Publish is done to **src/Bookmarks.Backend/bin/publish**

## Installation
Copy the files from publish folder for the platform you want to install on.

TODO: Add more instructions