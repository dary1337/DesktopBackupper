# DesktopBackupper - simplify local backups! 
- Double click and Your important folder is already saved in another location or drive

# Screenshots:

<img src="https://github.com/dary1337/DesktopBackupper/blob/master/screenshots/1.png" width=100% height=100%>

# `config.ini`
- When you run the program for the first time, it will create a config.ini with this content:
```
# If you want, you can use any other folder
backupFrom=%UserDesktop%
backupTo=
# Compress backup to .zip instead of copying folder
compressToArchive=true
# See every skipped folder path
skippedEcho=true
# See every copied file and folder
echo=false
# Close the programm if there were no errors during backup?
closeOnFinish=true
```

# `excludes.txt`
- You can also specify the names of folders you want to exclude from the backup (The program will also ask about it for the first time)
- For example:
```
node_modules
```
- Now, all node_modules folders will be ignored by the program
