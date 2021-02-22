# Modify the README.md file with mark-up that works with GitHub's website 
# and remove HTML elements not supported by NuGet's website

New-Item -Path ./temp_NuGet/ -ItemType Directory -Force
Copy-Item  ../README.md ./temp_NuGet/

# Remove specified html elements
# Remove matching line (first line)
# Truncate (to remove leading and trailing blank lines)

$file = "temp_NuGet/README.md"
(Get-Content $file).Replace("<br>","") | where { $_ -notlike "# Sprocket.Text*dll*API*" } `
    | Out-String `
    | % { $_.Trim() } `
    |  Set-Content $file 

