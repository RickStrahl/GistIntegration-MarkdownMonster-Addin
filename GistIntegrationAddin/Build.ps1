cd "$PSScriptRoot"

$src = "$env:appdata\Markdown Monster\Addins\GistIntegration"
$tgt = "..\Build"
"Copying from: $src"

"Cleaning up build files..."
remove-item $tgt\Distribution -recurse
del $tgt\addin.zip

"Copying files..."
mkdir $tgt\Distribution
Copy-Item "$src\GistIntegrationAddin.dll" $tgt\Distribution
Copy-Item "$src\version.json" $tgt\Distribution
Copy-Item "$src\version.json" $tgt

"Zipping up setup file..."
.\7z.exe a -tzip  $tgt\addin.zip $tgt\Distribution\*.*