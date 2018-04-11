cd "$PSScriptRoot" 

$src = "$env:appdata\Markdown Monster\Addins\GistIntegration"
"Copying from: $src"

"Cleaning up build files..."
del addin.zip

remove-item -recurse -force .\Distribution
md Distribution

"Copying files..."
copy "$src\GistIntegrationAddin.dll" .\Distribution
copy "$src\version.json" .\Distribution
copy "$src\version.json" .\

"Zipping up setup file..."
.\7z a -tzip  addin.zip .\Distribution\*.*

remove-item Distribution -recurse
