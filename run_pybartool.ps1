# activate python environment
.env/Scripts/Activate.ps1

# run script
Start-Process -Wait -NoNewWindow -FilePath python -ArgumentList $PSScriptRoot\pybartool.py