cd "$PSScriptRoot" 

$src = "$env:appdata\Markdown Monster\Addins\PasteCodeAsGist"
"Copying from: $src"

"Cleaning up build files..."
del addin.zip

remove-item -recurse -force .\Distribution
md Distribution

"Copying files..."
copy "$src\PasteCodeAsGistAddin.dll" .\Distribution
copy "$src\version.json" .\Distribution
copy "$src\version.json" .\

"Zipping up setup file..."
7z a -tzip  addin.zip .\Distribution\*.*
