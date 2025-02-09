# Command-line HEIC to JPG converter

Convert HEIC files to JPG in a folder and all it's subfolders.

Require [Imagemagic](https://imagemagick.org) to be installed first. Download from [here.](https://imagemagick.org/script/download.php#windows)

## To install HeicToJpg

1. Clone this repository
2. Compile in Visual Studio

OR

1. Download zip from [here](https://wessman.blob.core.windows.net/blob/HeicToJpg.zip)
2. Unzip ```HeicToJpg.exe``` to folder where HEIC images are located or some other folder that has [PATH](https://www.opentechguides.com/how-to/article/windows-10/113/windows-10-set-path.html) set

## To use HeicToJpg

1. Open command prompt
2. Go to folder where HEIC images are located
3. Enter command ```heictojpg```

### Command-line attributes for converting HEIC files
```
-s      Process subfolders
-o      Overwrite existing JPG files
-d      Delete HEIC files after converting to JPG

Sample: heictojpg -s -d
```
### Patch deletion attributes
```
-dheic  Delete HEIC files if JPG file with same name exists
-dimg   Delete IMG_ files (4:3) if IMG_E file (16:9) with same name exists
-dimge  Delete IMG_E files (16:9) if IMG_ file (4:3) with same name exists
-daae   Delete AAE files

Sample: heictojpg -s -dheic -dimg -daae
```
### Delete 2268 pixel images (dublicates)
```
-d2268  Delete all files that has width or height 2268 pixels and not named as IMG_ or IMG_E
-s      Process subfolders
-daae   Delete AAE files

Sample: heictojpg -d2268 -s -daae
```
### Patch move attributes
```
-move   Move files to folders based on file date
-o      Overwrite existing files

Creates folder if needed and name folder using format yyyy-MM-dd

Sample: heictojpg -move -o