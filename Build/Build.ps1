cd "$PSScriptRoot" 

"Cleaning up build files..."
del addin.zip

remove-item -recurse -force .\Distribution
md Distribution


"Copying files..."
copy ..\SaveImageToAzureBlob-MarkdownMonster-Addin\bin\Release\*.dll .\Distribution
copy version.json .\Distribution
del .\Distribution\NewtonSoft.Json.dll


"Zipping up setup file..."
7z a -tzip  addin.zip .\Distribution\*.*
