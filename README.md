# Command-line HEIC to JPG converter

Convert HEIC files to JPG in a folder and all it's subfolders.

Require [Imagemagic](https://imagemagick.org) to be installed first. Download from [here.](https://imagemagick.org/script/download.php#windows)

## To install HeicToJpg

1. Clone this repository
2. Compile in Visual Studio

OR

1. Download zip from [here](https://skepsisblob.blob.core.windows.net/heic-to-jpg/HeicToJpg.zip)
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
-dheic  Delete HEIC files if JPG file exists
-dimg   Delete IMG_ files (4:3) if IMG_E file (16:9) exists
-dimge  Delete IMG_E files (16:9) files if IMG_ file (4:3) exists
-daae   Delete AAE files

Sample: heictojpg -s -dheic -dimg -daae
```
### Patch move JPG files
```
-mbcd   Move JPG files to folders by file creation date. Create folder if needed

Sample: heictojpg -mbcd