cd "$PSScriptRoot" 

"Cleaning up build files..."
del addin.zip

remove-item -recurse -force .\Distribution
md Distribution


"Copying files..."
copy ..\PasteCodeAsGistAddin\bin\Release\PasteCodeAsGistAddin.dll .\Distribution

copy version.json .\Distribution



"Zipping up setup file..."
7z a -tzip  addin.zip .\Distribution\*.*
